using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Loading;
using Legacy.NpcInteraction;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Audio
{
	public class MusicController : MonoBehaviour
	{
		private const Int32 LOAD_MUSIC_PRIO = 5;

		private static MusicController s_instance;

		private Mode m_currentMode;

		private Single m_volume = 1f;

		private String m_currentAudioID;

		private AudioRequest m_currentAudioRequest;

		private AudioObject m_currentAudioObj;

		private Boolean m_isIndoorSceneActivated;

		private IndoorSceneRoot m_indoorSceneRoot;

		private Single m_indoorMusicNotPlayedTime;

		private Int32 m_indoorMusicFrameCounter;

		private List<MusicZone> m_activeMusicZones = new List<MusicZone>();

		[SerializeField]
		private IndoorSceneLoader m_Loader;

		[SerializeField]
		private Single m_defaultMusicFadeInTime;

		[SerializeField]
		private Boolean m_isDay = true;

		private String m_protectedAudioControllerName;

		public static MusicController Instance => s_instance;

	    public void SetMode(Mode pMode)
		{
			if (m_currentMode != pMode)
			{
				m_currentMode = pMode;
				HandleEnterMode(m_currentMode);
			}
		}

		public void SetVolume(Single p_value)
		{
			p_value = Mathf.Clamp01(p_value);
			if (p_value < 0.075)
			{
				p_value = 0f;
			}
			if (m_volume != p_value)
			{
				Single volume = m_volume;
				m_volume = p_value;
				if (m_volume == 0f)
				{
					if (m_currentAudioObj != null)
					{
						m_currentAudioObj.Stop();
						m_currentAudioObj = null;
					}
				}
				else if (volume == 0f)
				{
					if (m_currentAudioID != null)
					{
						PlayMusic(m_currentAudioID, 0f);
					}
				}
				else if (m_currentAudioObj != null)
				{
					m_currentAudioObj.volume = m_currentAudioObj.volume * (m_volume / volume);
				}
			}
		}

		public void RegisterMusicZone(Object owner, String musicAudioID, Int32 priority, Single fadeIn)
		{
			if (owner == null)
			{
				throw new ArgumentNullException();
			}
			if (musicAudioID == null)
			{
				throw new ArgumentNullException();
			}
			Int32 num = m_activeMusicZones.FindIndex((MusicZone a) => a.Owner == owner);
			if (num != -1)
			{
				throw new ArgumentNullException();
			}
			m_activeMusicZones.Add(new MusicZone(owner, musicAudioID, priority, fadeIn));
			m_activeMusicZones.Sort((MusicZone a, MusicZone b) => a.Priority.CompareTo(b.Priority));
		}

		public void DeregisterMusicZone(Object owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException();
			}
			m_activeMusicZones.RemoveAll((MusicZone a) => a.Owner == owner);
		}

		private void Awake()
		{
			if (s_instance == null || s_instance == this)
			{
				s_instance = this;
				return;
			}
			Debug.LogError("There are multiple MusicControllers in the scene!", gameObject);
			enabled = false;
			Destroy(gameObject);
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnFinishSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnGameTimeDayStateChanged));
			if (m_Loader != null)
			{
				m_Loader.FinishLoadIndoorScene += OnIndoorSceneLoaded;
			}
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_LOAD_VIEWS, new EventHandler(OnFinishSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_DAYSTATE_CHANGED, new EventHandler(OnGameTimeDayStateChanged));
			if (m_Loader != null)
			{
				m_Loader.FinishLoadIndoorScene -= OnIndoorSceneLoaded;
			}
		}

		private void OnGameTimeDayStateChanged(Object p_sender, EventArgs p_args)
		{
			m_isDay = (LegacyLogic.Instance.GameTime.DayState == EDayState.DAY);
		}

		private void OnFinishSceneLoad(Object p_sender, EventArgs p_args)
		{
			SetMode(Mode.GAME);
		}

		private void OnIndoorSceneLoaded(Object p_sender, FinishLoadIndoorSceneEventArgs p_args)
		{
			m_indoorSceneRoot = p_args.Root;
			SetMode(Mode.INDOOR);
		}

		private void HandleEnterMode(Mode p_mode)
		{
			switch (p_mode)
			{
			case Mode.MUTE:
				StopMusic(true);
				break;
			case Mode.MENU:
				PlayMusic("MainTheme", 0f);
				break;
			case Mode.GAME:
				HandleMusicInMode_GAME();
				break;
			case Mode.INDOOR:
				m_isIndoorSceneActivated = false;
				m_indoorMusicFrameCounter = 0;
				m_indoorMusicNotPlayedTime = 0f;
				if (!String.IsNullOrEmpty(m_indoorSceneRoot.MusicAudioID))
				{
					AudioManager.Instance.RequestByAudioID(m_indoorSceneRoot.MusicAudioID);
				}
				break;
			case Mode.GAME_OVER:
				PlayMusic("DungeonFacelessTemple", 5f);
				break;
			case Mode.CREDITS:
				PlayMusic("EndingTheme", 0f);
				break;
			case Mode.ENDING_SLIDES:
				PlayMusic("AtmosphereCityDay", 0f);
				break;
			default:
				StopMusic(true);
				Debug.LogError("MusicController: no instructions defined for mode '" + p_mode + "'! Will stop playing!");
				break;
			}
		}

		private void Update()
		{
			Mode currentMode = m_currentMode;
			if (currentMode != Mode.GAME)
			{
				if (currentMode == Mode.INDOOR)
				{
					HandleMusicInMode_INDOOR();
				}
			}
			else
			{
				HandleMusicInMode_GAME();
			}
		}

		private void HandleMusicInMode_GAME()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			String text = GetFightMusicAudioID(grid);
			if (text != null)
			{
				if (!String.IsNullOrEmpty(m_currentAudioID))
				{
					m_protectedAudioControllerName = AudioManager.Instance.FindCategoryNameByAudioID(m_currentAudioID);
				}
				AudioController.Instance.musicCrossFadeTime_In = 0f;
			}
			else if (m_activeMusicZones.Count > 0)
			{
				MusicZone musicZone = m_activeMusicZones[m_activeMusicZones.Count - 1];
				text = musicZone.MusicAudioID;
				AudioController.Instance.musicCrossFadeTime_In = musicZone.FadeIn;
			}
			else
			{
				if (m_isDay)
				{
					text = grid.MusicAudioIDDay;
				}
				else
				{
					text = grid.MusicAudioIDNight;
				}
				AudioController.Instance.musicCrossFadeTime_In = m_defaultMusicFadeInTime;
			}
			if (!String.IsNullOrEmpty(text))
			{
				if (m_currentAudioID != text || m_currentAudioObj == null || m_currentAudioObj.audioID != text)
				{
					PlayMusic(text, 0f);
				}
			}
			else
			{
				StopMusic(true);
			}
			m_protectedAudioControllerName = null;
		}

		private void HandleMusicInMode_INDOOR()
		{
			if (m_indoorSceneRoot != null && m_indoorSceneRoot.gameObject.activeInHierarchy)
			{
				if (!String.IsNullOrEmpty(m_indoorSceneRoot.MusicAudioID))
				{
					if (!m_isIndoorSceneActivated)
					{
						if (IsReadyToPlayIndoorMusic())
						{
							PlayMusic(m_indoorSceneRoot.MusicAudioID, 0f);
							m_isIndoorSceneActivated = true;
						}
					}
					else if (m_indoorSceneRoot.IsMusicLooped && (m_currentAudioObj == null || !m_currentAudioObj.IsPlaying()))
					{
						if (m_indoorMusicNotPlayedTime > m_indoorSceneRoot.MusicLoopDelay)
						{
							m_indoorMusicNotPlayedTime = 0f;
							PlayMusic(m_indoorSceneRoot.MusicAudioID, 0f);
						}
						else
						{
							m_indoorMusicNotPlayedTime += Time.deltaTime;
						}
					}
				}
				else
				{
					m_isIndoorSceneActivated = true;
				}
			}
			else if (m_isIndoorSceneActivated)
			{
				SetMode(Mode.GAME);
			}
		}

		private void PlayMusic(String musicAudioID, Single p_delay)
		{
			if (m_volume <= 0f)
			{
				if (m_currentAudioObj != null)
				{
					m_currentAudioObj.Stop();
					m_currentAudioObj = null;
				}
				return;
			}
			if (!AudioManager.Instance.IsValidAudioID(musicAudioID))
			{
				if (m_currentAudioID != musicAudioID)
				{
					Debug.LogError("PlayMusic unknow audioID '" + musicAudioID + "'");
				}
				m_currentAudioID = musicAudioID;
				return;
			}
			if (m_currentAudioRequest != null && m_currentAudioRequest.IsLoading)
			{
				String a2 = AudioManager.Instance.FindCategoryNameByAudioID(musicAudioID);
				if (!(a2 != m_currentAudioRequest.CategoryName))
				{
					return;
				}
				m_currentAudioRequest.AbortLoad();
				m_currentAudioRequest = null;
			}
			if (AudioManager.Instance.IsAudioIDLoaded(musicAudioID))
			{
				String currentAudioID = m_currentAudioID;
				m_currentAudioID = musicAudioID;
				m_currentAudioObj = AudioController.PlayMusic(musicAudioID, m_volume, p_delay, 0f);
				UnloadMusic(p_delay + AudioController.Instance.musicCrossFadeTime_Out, currentAudioID, m_protectedAudioControllerName);
				m_currentAudioRequest = null;
			}
			else if (m_currentAudioRequest == null || m_currentAudioRequest.IsDone)
			{
				m_currentAudioRequest = AudioManager.Instance.RequestByAudioID(musicAudioID, 5, true, delegate(AudioRequest a)
				{
					if (a.Controller != null)
					{
						OnMusicLoaded(a);
						String currentAudioID2 = m_currentAudioID;
						m_currentAudioID = musicAudioID;
						m_currentAudioObj = AudioController.PlayMusic(musicAudioID, m_volume, p_delay, 0f);
						UnloadMusic(p_delay + AudioController.Instance.musicCrossFadeTime_Out, currentAudioID2, m_protectedAudioControllerName);
						m_currentAudioRequest = null;
					}
				});
			}
		}

		private void StopMusic(Boolean p_doFadeOut)
		{
			AudioController.StopMusic((!p_doFadeOut) ? -1f : AudioController.Instance.musicCrossFadeTime_Out);
			if (m_currentAudioRequest != null && m_currentAudioRequest.IsLoading)
			{
				m_currentAudioRequest.AbortLoad();
			}
			m_currentAudioRequest = null;
			AudioManager.Instance.UnloadByAudioID(m_currentAudioID);
			m_currentAudioID = null;
			m_currentAudioObj = null;
		}

		private void OnMusicLoaded(AudioRequest request)
		{
			if (request.Controller != null)
			{
				AudioCategory category = AudioController.GetCategory("Music");
				foreach (AudioCategory audioCategory in request.Controller.AudioCategories)
				{
					if (audioCategory != category && audioCategory.parentCategory == null)
					{
						audioCategory.parentCategory = category;
					}
				}
			}
		}

		private void UnloadMusic(Single delay, String musicAudioID, String p_protectedAudioControllerName)
		{
			if (delay > 0f)
			{
				StartCoroutine(LateUnloadMusic(delay, musicAudioID, p_protectedAudioControllerName));
			}
			else
			{
				String a = AudioManager.Instance.FindCategoryNameByAudioID(m_currentAudioID);
				String b = AudioManager.Instance.FindCategoryNameByAudioID(musicAudioID);
				if (a != b && p_protectedAudioControllerName != b)
				{
					AudioManager.Instance.UnloadByAudioID(musicAudioID);
				}
			}
		}

		private IEnumerator LateUnloadMusic(Single delay, String musicAudioID, String p_protectedAudioControllerName)
		{
			yield return new WaitForSeconds(delay);
			UnloadMusic(0f, musicAudioID, p_protectedAudioControllerName);
			yield break;
		}

		private Boolean IsFightMusicAlreadyPlaying(String p_musicID)
		{
			if (m_currentAudioID != null && m_currentAudioID.StartsWith("CombatMusic"))
			{
				if (m_currentAudioID == "CombatMusicBoss")
				{
					return true;
				}
				if (m_currentAudioID == "CombatMusicChampion" && p_musicID == "CombatMusicDefault")
				{
					return true;
				}
			}
			return false;
		}

		private static String GetFightMusicAudioID(Grid p_grid)
		{
			String result = null;
			EMonsterGrade emonsterGrade = EMonsterGrade.CORE;
			foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
			{
				if (monster.IsAggro)
				{
					if (monster.StaticData.Grade == EMonsterGrade.BOSS)
					{
						result = "CombatMusicBoss";
						break;
					}
					if ((p_grid.IsWithFightMusic || monster.IsFightMusicForced) && emonsterGrade <= monster.StaticData.Grade)
					{
						emonsterGrade = monster.StaticData.Grade;
						if (emonsterGrade >= EMonsterGrade.CHAMPION)
						{
							result = "CombatMusicChampion";
						}
						else
						{
							result = "CombatMusicDefault";
						}
					}
				}
			}
			return result;
		}

		private Boolean IsReadyToPlayIndoorMusic()
		{
			AudioListener currentAudioListener = AudioController.GetCurrentAudioListener();
			Boolean flag = currentAudioListener != null && currentAudioListener == m_indoorSceneRoot.SceneCamera.GetComponent<AudioListener>();
			if (flag)
			{
				m_indoorMusicFrameCounter++;
			}
			else
			{
				m_indoorMusicFrameCounter = 0;
			}
			return m_indoorMusicFrameCounter > 10;
		}

		public enum Mode
		{
			MUTE,
			MENU,
			GAME,
			INDOOR,
			GAME_OVER,
			CREDITS,
			ENDING_SLIDES
		}

		private struct MusicZone
		{
			public Object Owner;

			public String MusicAudioID;

			public Int32 Priority;

			public Single FadeIn;

			public MusicZone(Object owner, String musicAudioID, Int32 priority, Single fadeIn)
			{
				Owner = owner;
				MusicAudioID = musicAudioID;
				Priority = priority;
				FadeIn = fadeIn;
			}
		}
	}
}
