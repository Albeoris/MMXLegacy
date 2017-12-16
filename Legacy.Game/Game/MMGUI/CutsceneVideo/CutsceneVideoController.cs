using System;
using System.Collections;
using AssetBundles.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using Legacy.Game.Context;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI.Videos;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.CutsceneVideo
{
	public class CutsceneVideoController : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private UITexture m_MovieView;

		[SerializeField]
		private BinkVideoController m_Movie;

		private MovieTexture m_movieTexture;

		[SerializeField]
		private MovieSubtitleController m_Subtitle;

		[SerializeField]
		private GameObject m_MainGUIRoot;

		[SerializeField]
		private Video[] m_Cutscenes = Arrays<Video>.Empty;

		[SerializeField]
		private Boolean m_Fullscreen;

		[SerializeField]
		private ScaleMode m_ScaleMode;

		private Int32 m_ScreenWidth;

		private Int32 m_ScreenHeight;

		private Video m_currentVideo;

		private AudioSource m_macAudio;

		private AudioListener m_macListener;

		private Single m_movieTime;

		private EContext m_changeContext = EContext.None;

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.ConversationManager.EndConversation += OnEndConversation;
		}

		private void Start()
		{
			m_Movie.gameObject.SetActive(false);
			m_Subtitle.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.ConversationManager.EndConversation -= OnEndConversation;
		}

		private void OnTokenAdded(Object sender, EventArgs e)
		{
			TokenEventArgs tokenEventArgs = (TokenEventArgs)e;
			if (tokenEventArgs.TokenID == 699)
			{
				LegacyLogic.Instance.WorldManager.SaveCurrentMapData();
				m_changeContext = EContext.End;
				LegacyLogic.Instance.WorldManager.SaveGameName = String.Empty;
				LegacyLogic.Instance.WorldManager.IsSaveGame = false;
				LegacyLogic.Instance.WorldManager.IsShowingEndingSequences = true;
				PlayCutsceneByID("Killed_Erebos");
			}
			if (tokenEventArgs.TokenID == 329)
			{
				InteractiveObject interactiveObject = LegacyLogic.Instance.MapLoader.Grid.FindInteractiveObject<InteractiveObject>(123);
				if (interactiveObject != null)
				{
					interactiveObject.ClearInteractions();
					interactiveObject.Execute(LegacyLogic.Instance.MapLoader.Grid);
					interactiveObject.Update();
				}
				interactiveObject = LegacyLogic.Instance.MapLoader.Grid.FindInteractiveObject<InteractiveObject>(88);
				if (interactiveObject != null)
				{
					interactiveObject.ClearInteractions();
					interactiveObject.Execute(LegacyLogic.Instance.MapLoader.Grid);
					interactiveObject.Update();
				}
			}
		}

		private void OnEndConversation(Object sender, EventArgs e)
		{
			String lastCutsceneVideoID = LegacyLogic.Instance.ConversationManager.LastCutsceneVideoID;
			if (!String.IsNullOrEmpty(lastCutsceneVideoID))
			{
				PlayCutsceneByID(lastCutsceneVideoID);
			}
		}

		private void PlayCutsceneByID(String cutsceneID)
		{
			EVideoDecoder videoDecoder = ConfigManager.Instance.Options.VideoDecoder;
			if (videoDecoder == EVideoDecoder.System)
			{
				Video video = FindCutscene(cutsceneID);
				if (video != null)
				{
					m_Movie.FilePath = video.MovieFile;
					if (m_Movie.LoadMovie())
					{
						m_Subtitle.SubtitleFilePath = video.SubtitleFile;
						m_Subtitle.LoadSubtitle();
						StopAllCoroutines();
						StartCoroutine(PlayMovie());
						return;
					}
				}
				else
				{
					Debug.Log("Video not found " + cutsceneID);
				}
			}
			CheckChangeContext();
		}

		private void OnMacVideoLoaded(AssetRequest request)
		{
			if (request.Asset != null)
			{
				m_movieTexture = (MovieTexture)request.Asset;
				m_Subtitle.SubtitleFilePath = m_currentVideo.SubtitleFile;
				m_Subtitle.LoadSubtitle();
				StopAllCoroutines();
				StartCoroutine(PlayMovie());
				return;
			}
			CheckChangeContext();
		}

		private Video FindCutscene(String cutsceneID)
		{
			foreach (Video video in m_Cutscenes)
			{
				if (video.ID == cutsceneID)
				{
					return video;
				}
			}
			return null;
		}

		public void Activate()
		{
		}

		public void Deactivate()
		{
		}

		private IEnumerator PlayMovie()
		{
			IngameController.Instance.ChangeIngameContext(this);
			IngameController.Instance.LockIngameContext();
			Material mat = m_MovieView.material;
			Single alpha = -1f;
			mat.SetFloat("_Alpha", alpha);
			m_MovieView.mainTexture = null;
			m_Movie.gameObject.SetActive(true);
			m_Subtitle.gameObject.SetActive(true);
			m_Subtitle.ShowSubtitle = ConfigManager.Instance.Options.SubTitles;
			while (alpha <= 1f)
			{
				mat.SetFloat("_Alpha", alpha);
				alpha += Time.deltaTime;
				yield return null;
			}
			m_MainGUIRoot.SetActive(false);
			if (FXMainCamera.Instance.CurrentCamera != null)
			{
				FXMainCamera.Instance.CurrentCamera.camera.enabled = false;
			}
			m_MovieView.mainTexture = m_Movie.Media.OutputTexture;
			m_Movie.Play();
			yield return StartCoroutine(FadeAudioListener(0f));
			while (m_Movie.Media != null && !m_Movie.IsFinishedPlaying)
			{
				if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.End) || Input.GetKeyUp(KeyCode.KeypadEnter))
				{
					m_Movie.Pause();
					break;
				}
				yield return null;
			}
			if (CheckChangeContext())
			{
				IngameController.Instance.ReleaseIngameContextLock();
				IngameController.Instance.ChangeIngameContext(IngameController.Instance.IngameInput);
				yield break;
			}
			yield return StartCoroutine(FadeAudioListener(1f));
			m_movieTime = 0f;
			m_MainGUIRoot.SetActive(true);
			if (FXMainCamera.Instance.CurrentCamera != null)
			{
				FXMainCamera.Instance.CurrentCamera.camera.enabled = true;
			}
			while (alpha >= -1f)
			{
				mat.SetFloat("_Alpha", Math.Max(-1f, alpha));
				alpha -= Time.deltaTime;
				yield return null;
			}
			m_Movie.UnloadMovie();
			m_Movie.gameObject.SetActive(false);
			m_Subtitle.gameObject.SetActive(false);
			IngameController.Instance.ReleaseIngameContextLock();
			IngameController.Instance.ChangeIngameContext(IngameController.Instance.IngameInput);
			yield break;
		}

		private IEnumerator FadeAudioListener(Single targetVolume)
		{
			Single volume = AudioListener.volume;
			while (targetVolume != volume)
			{
				volume = Mathf.MoveTowards(volume, targetVolume, Time.deltaTime * 2f);
				AudioListener.volume = volume;
				yield return null;
			}
			yield break;
		}

		private void EnableAllAudioListener(Boolean p_enable)
		{
			AudioListener[] array = (AudioListener[])FindObjectsOfType(typeof(AudioListener));
			Debug.Log("Count: " + array.Length);
			for (Int32 i = 0; i < array.Length; i++)
			{
				array[i].enabled = p_enable;
			}
			AudioController[] array2 = (AudioController[])FindObjectsOfType(typeof(AudioController));
			Debug.Log("Count: " + array2.Length);
			for (Int32 j = 0; j < array2.Length; j++)
			{
				array2[j].enabled = p_enable;
				array2[j].Volume = (!p_enable) ? 0 : 1;
			}
		}

		private void Update()
		{
			if (m_movieTexture != null && m_movieTexture.isPlaying && m_Subtitle != null)
			{
				m_movieTime += Time.deltaTime;
				m_Subtitle.UpdateSubtitle(m_movieTime);
			}
		}

		private Boolean CheckChangeContext()
		{
			if (m_changeContext != EContext.None)
			{
				AudioListener.volume = 1f;
				AudioController.StopMusic();
				ContextManager.ChangeContext(m_changeContext);
				return true;
			}
			return false;
		}

		[Serializable]
		private class Video
		{
			public String ID;

			public String MovieFile;

			public String MacMovieFile;

			public String SubtitleFile;
		}
	}
}
