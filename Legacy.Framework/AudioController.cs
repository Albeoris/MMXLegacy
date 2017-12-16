using System;
using System.Collections.Generic;
using Legacy;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("ClockStone/Audio/AudioController")]
public class AudioController : MonoBehaviour, ISingletonMonoBehaviour
{
	public const String AUDIO_TOOLKIT_VERSION = "6.4";

	public GameObject AudioObjectPrefab;

	public Boolean Persistent;

	public Boolean UsePooledAudioObjects = true;

	public Boolean PlayWithZeroVolume;

	public Boolean EqualPowerCrossfade;

	public Single musicCrossFadeTime;

	public Boolean specifyCrossFadeInAndOutSeperately;

	[SerializeField]
	private Single _musicCrossFadeTime_In;

	[SerializeField]
	private Single _musicCrossFadeTime_Out;

	public AudioCategory[] AudioCategories = Arrays<AudioCategory>.Empty;

	public String[] musicPlaylist = Arrays<String>.Empty;

	public Boolean loopPlaylist;

	public Boolean shufflePlaylist;

	public Boolean crossfadePlaylist;

	public Single delayBetweenPlaylistTracks = 1f;

	protected static PoolableReference<AudioObject> _currentMusicReference = new PoolableReference<AudioObject>();

	private GameObject _currentAudioListenerGo;

	private Transform _currentAudioListenerTrans;

	private AudioListener _currentAudioListener;

	private Boolean _musicEnabled = true;

	private Boolean _categoriesValidated;

	[SerializeField]
	private Boolean _isAdditionalAudioController;

	[SerializeField]
	private Boolean _audioDisabled;

	private Dictionary<String, AudioItem> _audioItems;

	private static List<Int32> _playlistPlayed;

	private static Boolean _isPlaylistPlaying = false;

	[SerializeField]
	private Single _volume = 1f;

	private static Double _systemTime;

	private static Double _lastSystemTime = -1.0;

	private static Double _systemDeltaTime = -1.0;

	private List<AudioController> _additionalAudioControllers;

	public AudioController()
	{
		SetSingletonType(typeof(AudioController));
	}

	public static AudioController Instance => UnitySingleton<AudioController>.GetSingleton(true, false);

    public static Boolean DoesInstanceExist()
	{
		AudioController singleton = UnitySingleton<AudioController>.GetSingleton(false, false);
		return !Object.Equals(singleton, null);
	}

	public static void SetSingletonType(Type type)
	{
		UnitySingleton<AudioController>._myType = type;
	}

	public Boolean DisableAudio
	{
		get => _audioDisabled;
	    set
		{
			if (value != _audioDisabled)
			{
				if (value)
				{
				}
				_audioDisabled = value;
			}
		}
	}

	public Boolean isAdditionalAudioController
	{
		get => _isAdditionalAudioController;
	    set => _isAdditionalAudioController = value;
	}

	public Single Volume
	{
		get => _volume;
	    set
		{
			if (value != _volume)
			{
				_volume = value;
				_ApplyVolumeChange();
			}
		}
	}

	public Boolean musicEnabled
	{
		get => _musicEnabled;
	    set
		{
			if (_musicEnabled == value)
			{
				return;
			}
			_musicEnabled = value;
			if (_currentMusic)
			{
				if (value)
				{
					if (_currentMusic.IsPaused(true))
					{
						_currentMusic.Play(0f);
					}
				}
				else
				{
					_currentMusic.Pause();
				}
			}
		}
	}

	public Single musicCrossFadeTime_In
	{
		get
		{
			if (specifyCrossFadeInAndOutSeperately)
			{
				return _musicCrossFadeTime_In;
			}
			return musicCrossFadeTime;
		}
		set => _musicCrossFadeTime_In = value;
	}

	public Single musicCrossFadeTime_Out
	{
		get
		{
			if (specifyCrossFadeInAndOutSeperately)
			{
				return _musicCrossFadeTime_Out;
			}
			return musicCrossFadeTime;
		}
		set => _musicCrossFadeTime_Out = value;
	}

	public static Double systemTime => _systemTime;

    public static Double systemDeltaTime => _systemDeltaTime;

    public static AudioObject PlayMusic(String audioID, Single volume, Single delay, Single startTime = 0f)
	{
		_isPlaylistPlaying = false;
		return Instance._PlayMusic(audioID, volume, delay, startTime);
	}

	public static AudioObject PlayMusic(String audioID)
	{
		return PlayMusic(audioID, 1f, 0f, 0f);
	}

	public static AudioObject PlayMusic(String audioID, Single volume)
	{
		return PlayMusic(audioID, volume, 0f, 0f);
	}

	public static AudioObject PlayMusic(String audioID, Vector3 worldPosition, Transform parentObj = null, Single volume = 1f, Single delay = 0f, Single startTime = 0f)
	{
		_isPlaylistPlaying = false;
		return Instance._PlayMusic(audioID, worldPosition, parentObj, volume, delay, startTime);
	}

	public static AudioObject PlayMusic(String audioID, Transform parentObj, Single volume = 1f, Single delay = 0f, Single startTime = 0f)
	{
		_isPlaylistPlaying = false;
		return Instance._PlayMusic(audioID, parentObj.position, parentObj, volume, delay, startTime);
	}

	public static Boolean StopMusic()
	{
		return Instance._StopMusic(0f);
	}

	public static Boolean StopMusic(Single fadeOut)
	{
		return Instance._StopMusic(fadeOut);
	}

	public static Boolean PauseMusic(Single fadeOut = 0f)
	{
		return Instance._PauseMusic(fadeOut);
	}

	public static Boolean IsMusicPaused()
	{
		return _currentMusic != null && _currentMusic.IsPaused(true);
	}

	public static Boolean UnpauseMusic(Single fadeIn = 0f)
	{
		if (!Instance._musicEnabled)
		{
			return false;
		}
		if (_currentMusic != null && _currentMusic.IsPaused(true))
		{
			_currentMusic.Unpause(fadeIn);
			return true;
		}
		return false;
	}

	public static Int32 EnqueueMusic(String audioID)
	{
		return Instance._EnqueueMusic(audioID);
	}

	public static String[] GetMusicPlaylist()
	{
		String[] array = new String[(Instance.musicPlaylist == null) ? 0 : Instance.musicPlaylist.Length];
		if (array.Length > 0)
		{
			Array.Copy(Instance.musicPlaylist, array, array.Length);
		}
		return array;
	}

	public static void SetMusicPlaylist(String[] playlist)
	{
		String[] array = new String[(playlist == null) ? 0 : playlist.Length];
		if (array.Length > 0)
		{
			Array.Copy(playlist, array, array.Length);
		}
		Instance.musicPlaylist = array;
	}

	public static AudioObject PlayMusicPlaylist()
	{
		return Instance._PlayMusicPlaylist();
	}

	public static AudioObject PlayNextMusicOnPlaylist()
	{
		if (IsPlaylistPlaying())
		{
			return Instance._PlayNextMusicOnPlaylist(0f);
		}
		return null;
	}

	public static AudioObject PlayPreviousMusicOnPlaylist()
	{
		if (IsPlaylistPlaying())
		{
			return Instance._PlayPreviousMusicOnPlaylist(0f);
		}
		return null;
	}

	public static Boolean IsPlaylistPlaying()
	{
		return _isPlaylistPlaying;
	}

	public static void ClearPlaylist()
	{
		Instance.musicPlaylist = null;
	}

	public static AudioObject Play(String audioID)
	{
		AudioListener currentAudioListener = GetCurrentAudioListener();
		if (currentAudioListener == null)
		{
			Debug.LogWarning("No AudioListener found in the scene");
			return null;
		}
		return Play(audioID, currentAudioListener.transform.position + currentAudioListener.transform.forward, null, 1f, 0f, 0f);
	}

	public static AudioObject Play(String audioID, Single volume, Single delay = 0f, Single startTime = 0f)
	{
		AudioListener currentAudioListener = GetCurrentAudioListener();
		if (currentAudioListener == null)
		{
			Debug.LogWarning("No AudioListener found in the scene");
			return null;
		}
		return Play(audioID, currentAudioListener.transform.position + currentAudioListener.transform.forward, null, volume, delay, startTime);
	}

	public static AudioObject Play(String audioID, Transform parentObj)
	{
		return Play(audioID, parentObj.position, parentObj, 1f, 0f, 0f);
	}

	public static AudioObject Play(String audioID, Transform parentObj, Single volume, Single delay = 0f, Single startTime = 0f)
	{
		return Play(audioID, parentObj.position, parentObj, volume, delay, startTime);
	}

	public static AudioObject Play(String audioID, Vector3 worldPosition, Transform parentObj = null)
	{
		return Instance._Play(audioID, 1f, worldPosition, parentObj, 0f, 0f, false, 0.0, null);
	}

	public static AudioObject Play(String audioID, Vector3 worldPosition, Transform parentObj, Single volume, Single delay = 0f, Single startTime = 0f)
	{
		return Instance._Play(audioID, volume, worldPosition, parentObj, delay, startTime, false, 0.0, null);
	}

	public static AudioObject PlayScheduled(String audioID, Double dspTime, Vector3 worldPosition, Transform parentObj = null, Single volume = 1f, Single startTime = 0f)
	{
		return Instance._Play(audioID, volume, worldPosition, parentObj, 0f, startTime, false, dspTime, null);
	}

	public static AudioObject PlayAfter(String audioID, AudioObject playingAudio, Double deltaDspTime = 0.0, Single volume = 1f, Single startTime = 0f)
	{
		Double num = AudioSettings.dspTime;
		if (playingAudio.IsPlaying())
		{
			num += playingAudio.timeUntilEnd;
		}
		num += deltaDspTime;
		return PlayScheduled(audioID, num, playingAudio.transform.position, playingAudio.transform.parent, volume, startTime);
	}

	public static Boolean Stop(String audioID, Single fadeOutLength)
	{
		if (Instance._GetAudioItem(audioID) == null)
		{
			return false;
		}
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(audioID, false);
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			if (fadeOutLength < 0f)
			{
				audioObject.Stop();
			}
			else
			{
				audioObject.Stop(fadeOutLength);
			}
		}
		return playingAudioObjects.Length > 0;
	}

	public static Boolean Stop(String audioID)
	{
		return Stop(audioID, -1f);
	}

	public static void StopAll(Single fadeOutLength)
	{
		Instance._StopMusic(fadeOutLength);
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(false);
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			audioObject.Stop(fadeOutLength);
		}
	}

	public static void StopAll()
	{
		StopAll(-1f);
	}

	public static void PauseAll(Single fadeOutLength = 0f)
	{
		Instance._PauseMusic(fadeOutLength);
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(false);
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			audioObject.Pause(fadeOutLength);
		}
	}

	public static void UnpauseAll(Single fadeInLength = 0f)
	{
		UnpauseMusic(fadeInLength);
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(true);
		AudioController instance = Instance;
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			if (audioObject.IsPaused(true))
			{
				if (instance.musicEnabled || !(_currentMusic == audioObject))
				{
					audioObject.Unpause(fadeInLength);
				}
			}
		}
	}

	public static void PauseCategory(String categoryName, Single fadeOutLength = 0f)
	{
		if (_currentMusic != null && _currentMusic.category.Name == categoryName)
		{
			PauseMusic(fadeOutLength);
		}
		AudioObject[] playingAudioObjectsInCategory = GetPlayingAudioObjectsInCategory(categoryName, false);
		foreach (AudioObject audioObject in playingAudioObjectsInCategory)
		{
			audioObject.Pause(fadeOutLength);
		}
	}

	public static void UnpauseCategory(String categoryName, Single fadeInLength = 0f)
	{
		if (_currentMusic != null && _currentMusic.category.Name == categoryName)
		{
			UnpauseMusic(fadeInLength);
		}
		AudioObject[] playingAudioObjectsInCategory = GetPlayingAudioObjectsInCategory(categoryName, true);
		foreach (AudioObject audioObject in playingAudioObjectsInCategory)
		{
			if (audioObject.IsPaused(true))
			{
				audioObject.Unpause(fadeInLength);
			}
		}
	}

	public static Boolean IsPlaying(String audioID)
	{
		return GetPlayingAudioObjects(audioID, false).Length > 0;
	}

	public static AudioObject[] GetPlayingAudioObjects(String audioID, Boolean includePausedAudio = false)
	{
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(includePausedAudio);
		List<AudioObject> list = new List<AudioObject>();
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			if (audioObject.audioID == audioID)
			{
				list.Add(audioObject);
			}
		}
		return list.ToArray();
	}

	public static AudioObject[] GetPlayingAudioObjectsInCategory(String categoryName, Boolean includePausedAudio = false)
	{
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(includePausedAudio);
		List<AudioObject> list = new List<AudioObject>();
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			if (audioObject.DoesBelongToCategory(categoryName))
			{
				list.Add(audioObject);
			}
		}
		return list.ToArray();
	}

	public static AudioObject[] GetPlayingAudioObjects(Boolean includePausedAudio = false)
	{
		List<AudioObject> list = new List<AudioObject>();
		foreach (AudioObject audioObject in RegisteredComponentController.GetAllOfType(typeof(AudioObject)))
		{
			if (audioObject.IsPlaying() || (includePausedAudio && audioObject.IsPaused(true)))
			{
				list.Add(audioObject);
			}
		}
		return list.ToArray();
	}

	public static Int32 GetPlayingAudioObjectsCount(String audioID, Boolean includePausedAudio = false)
	{
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(includePausedAudio);
		Int32 num = 0;
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			if (audioObject.audioID == audioID)
			{
				num++;
			}
		}
		return num;
	}

	public static void EnableMusic(Boolean b)
	{
		Instance.musicEnabled = b;
	}

	public static Boolean IsMusicEnabled()
	{
		return Instance.musicEnabled;
	}

	public static AudioListener GetCurrentAudioListener()
	{
		AudioController instance = Instance;
		GameObject gameObject = instance._currentAudioListenerGo;
		AudioListener audioListener = instance._currentAudioListener;
		if (audioListener == null || !audioListener.enabled || gameObject == null || !gameObject.activeInHierarchy)
		{
			gameObject = null;
			audioListener = null;
		}
		if (audioListener == null || gameObject == null)
		{
			audioListener = (instance._currentAudioListener = (AudioListener)FindObjectOfType(typeof(AudioListener)));
			if (audioListener != null)
			{
				instance._currentAudioListenerGo = audioListener.gameObject;
				instance._currentAudioListenerTrans = audioListener.transform;
			}
		}
		return audioListener;
	}

	public static Transform GetCurrentAudioListenerTransform()
	{
		GetCurrentAudioListener();
		return Instance._currentAudioListenerTrans;
	}

	public static AudioObject GetCurrentMusic()
	{
		return _currentMusic;
	}

	public static AudioCategory GetCategory(String name)
	{
		AudioController instance = Instance;
		AudioCategory audioCategory = instance._GetCategory(name);
		if (audioCategory != null)
		{
			return audioCategory;
		}
		if (instance._additionalAudioControllers != null)
		{
			foreach (AudioController audioController in instance._additionalAudioControllers)
			{
				audioCategory = audioController._GetCategory(name);
				if (audioCategory != null)
				{
					return audioCategory;
				}
			}
		}
		return null;
	}

	public static void SetCategoryVolume(String name, Single volume)
	{
		Boolean flag = false;
		AudioController instance = Instance;
		AudioCategory audioCategory = instance._GetCategory(name);
		if (audioCategory != null)
		{
			audioCategory.Volume = volume;
			flag = true;
		}
		if (instance._additionalAudioControllers != null)
		{
			foreach (AudioController audioController in instance._additionalAudioControllers)
			{
				audioCategory = audioController._GetCategory(name);
				if (audioCategory != null)
				{
					audioCategory.Volume = volume;
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning("No audio category with name " + name);
		}
	}

	public static Single GetCategoryVolume(String name)
	{
		AudioCategory category = GetCategory(name);
		if (category != null)
		{
			return category.Volume;
		}
		Debug.LogWarning("No audio category with name " + name);
		return 0f;
	}

	public static void SetGlobalVolume(Single volume)
	{
		AudioController instance = Instance;
		instance.Volume = volume;
		if (instance._additionalAudioControllers != null)
		{
			foreach (AudioController audioController in instance._additionalAudioControllers)
			{
				audioController.Volume = volume;
			}
		}
	}

	public static Single GetGlobalVolume()
	{
		return Instance.Volume;
	}

	public static AudioCategory NewCategory(String categoryName)
	{
		Int32 num = (Instance.AudioCategories == null) ? 0 : Instance.AudioCategories.Length;
		AudioCategory[] audioCategories = Instance.AudioCategories;
		Instance.AudioCategories = new AudioCategory[num + 1];
		if (num > 0)
		{
			audioCategories.CopyTo(Instance.AudioCategories, 0);
		}
		AudioCategory audioCategory = new AudioCategory(Instance);
		audioCategory.Name = categoryName;
		Instance.AudioCategories[num] = audioCategory;
		Instance._InvalidateCategories();
		return audioCategory;
	}

	public static void RemoveCategory(String categoryName)
	{
		Int32 num = -1;
		Int32 num2;
		if (Instance.AudioCategories != null)
		{
			num2 = Instance.AudioCategories.Length;
		}
		else
		{
			num2 = 0;
		}
		for (Int32 i = 0; i < num2; i++)
		{
			if (Instance.AudioCategories[i].Name == categoryName)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			Debug.LogError("AudioCategory does not exist: " + categoryName);
			return;
		}
		AudioCategory[] array = new AudioCategory[Instance.AudioCategories.Length - 1];
		for (Int32 i = 0; i < num; i++)
		{
			array[i] = Instance.AudioCategories[i];
		}
		for (Int32 i = num + 1; i < Instance.AudioCategories.Length; i++)
		{
			array[i - 1] = Instance.AudioCategories[i];
		}
		Instance.AudioCategories = array;
		Instance._InvalidateCategories();
	}

	public static void AddToCategory(AudioCategory category, AudioItem audioItem)
	{
		Int32 num = (category.AudioItems == null) ? 0 : category.AudioItems.Length;
		AudioItem[] audioItems = category.AudioItems;
		category.AudioItems = new AudioItem[num + 1];
		if (num > 0)
		{
			audioItems.CopyTo(category.AudioItems, 0);
		}
		category.AudioItems[num] = audioItem;
		Instance._InvalidateCategories();
	}

	public static AudioItem AddToCategory(AudioCategory category, AudioClip audioClip, String audioID)
	{
		AudioItem audioItem = new AudioItem();
		audioItem.Name = audioID;
		audioItem.subItems = new AudioSubItem[1];
		AudioSubItem audioSubItem = new AudioSubItem();
		audioSubItem.Clip = audioClip;
		audioItem.subItems[0] = audioSubItem;
		AddToCategory(category, audioItem);
		return audioItem;
	}

	public static Boolean RemoveAudioItem(String audioID)
	{
		AudioItem audioItem = Instance._GetAudioItem(audioID);
		if (audioItem == null)
		{
			return false;
		}
		Int32 num = audioItem.category._GetIndexOf(audioItem);
		if (num < 0)
		{
			return false;
		}
		AudioItem[] audioItems = audioItem.category.AudioItems;
		AudioItem[] array = new AudioItem[audioItems.Length - 1];
		for (Int32 i = 0; i < num; i++)
		{
			array[i] = audioItems[i];
		}
		for (Int32 i = num + 1; i < audioItems.Length; i++)
		{
			array[i - 1] = audioItems[i];
		}
		audioItem.category.AudioItems = array;
		if (Instance._categoriesValidated)
		{
			Instance._audioItems.Remove(audioID);
		}
		return true;
	}

	public static Boolean IsValidAudioID(String audioID)
	{
		return Instance._GetAudioItem(audioID) != null;
	}

	public static AudioItem GetAudioItem(String audioID)
	{
		return Instance._GetAudioItem(audioID);
	}

	public static void DetachAllAudios(GameObject gameObjectWithAudios)
	{
		AudioObject[] componentsInChildren = gameObjectWithAudios.GetComponentsInChildren<AudioObject>(true);
		foreach (AudioObject audioObject in componentsInChildren)
		{
			audioObject.transform.parent = null;
		}
	}

	private static AudioObject _currentMusic
	{
		get => _currentMusicReference.Get();
	    set => _currentMusicReference.Set(value, true);
	}

	private void _ApplyVolumeChange()
	{
		AudioObject[] playingAudioObjects = GetPlayingAudioObjects(true);
		foreach (AudioObject audioObject in playingAudioObjects)
		{
			audioObject._ApplyVolumeBoth();
		}
	}

	internal AudioItem _GetAudioItem(String audioID)
	{
		_ValidateCategories();
		AudioItem result;
		if (_audioItems.TryGetValue(audioID, out result))
		{
			return result;
		}
		return null;
	}

	protected AudioObject _PlayMusic(String audioID, Single volume, Single delay, Single startTime)
	{
		AudioListener currentAudioListener = GetCurrentAudioListener();
		if (currentAudioListener == null)
		{
			Debug.LogWarning("No AudioListener found in the scene");
			return null;
		}
		return _PlayMusic(audioID, currentAudioListener.transform.position + currentAudioListener.transform.forward, null, volume, delay, startTime);
	}

	protected Boolean _StopMusic(Single fadeOutLength)
	{
		if (_currentMusic != null)
		{
			_currentMusic.Stop(fadeOutLength);
			_currentMusic = null;
			return true;
		}
		return false;
	}

	protected Boolean _PauseMusic(Single fadeOut)
	{
		if (_currentMusic != null)
		{
			_currentMusic.Pause(fadeOut);
			return true;
		}
		return false;
	}

	protected AudioObject _PlayMusic(String audioID, Vector3 position, Transform parentObj, Single volume, Single delay, Single startTime)
	{
		if (!IsMusicEnabled())
		{
			return null;
		}
		Boolean flag;
		if (_currentMusic != null && _currentMusic.IsPlaying())
		{
			flag = true;
			_currentMusic.Stop(musicCrossFadeTime_Out);
		}
		else
		{
			flag = false;
		}
		_currentMusic = _Play(audioID, volume, position, parentObj, delay, startTime, false, 0.0, null);
		if (_currentMusic && flag && musicCrossFadeTime_In > 0f)
		{
			_currentMusic.FadeIn(musicCrossFadeTime_In);
		}
		return _currentMusic;
	}

	protected Int32 _EnqueueMusic(String audioID)
	{
		Int32 num;
		if (musicPlaylist == null)
		{
			num = 1;
		}
		else
		{
			num = musicPlaylist.Length + 1;
		}
		String[] array = new String[num];
		if (musicPlaylist != null)
		{
			musicPlaylist.CopyTo(array, 0);
		}
		array[num - 1] = audioID;
		musicPlaylist = array;
		return num;
	}

	protected AudioObject _PlayMusicPlaylist()
	{
		_ResetLastPlayedList();
		return _PlayNextMusicOnPlaylist(0f);
	}

	private AudioObject _PlayMusicTrackWithID(Int32 nextTrack, Single delay, Boolean addToPlayedList)
	{
		if (nextTrack < 0)
		{
			return null;
		}
		_playlistPlayed.Add(nextTrack);
		_isPlaylistPlaying = true;
		AudioObject audioObject = _PlayMusic(musicPlaylist[nextTrack], 1f, delay, 0f);
		if (audioObject != null)
		{
			audioObject._isCurrentPlaylistTrack = true;
			audioObject.primaryAudioSource.loop = false;
		}
		return audioObject;
	}

	internal AudioObject _PlayNextMusicOnPlaylist(Single delay)
	{
		Int32 nextTrack = _GetNextMusicTrack();
		return _PlayMusicTrackWithID(nextTrack, delay, true);
	}

	internal AudioObject _PlayPreviousMusicOnPlaylist(Single delay)
	{
		Int32 nextTrack = _GetPreviousMusicTrack();
		return _PlayMusicTrackWithID(nextTrack, delay, false);
	}

	private void _ResetLastPlayedList()
	{
		_playlistPlayed.Clear();
	}

	protected Int32 _GetNextMusicTrack()
	{
		if (musicPlaylist == null || musicPlaylist.Length == 0)
		{
			return -1;
		}
		if (musicPlaylist.Length == 1)
		{
			return 0;
		}
		if (shufflePlaylist)
		{
			return _GetNextMusicTrackShuffled();
		}
		return _GetNextMusicTrackInOrder();
	}

	protected Int32 _GetPreviousMusicTrack()
	{
		if (musicPlaylist == null || musicPlaylist.Length == 0)
		{
			return -1;
		}
		if (musicPlaylist.Length == 1)
		{
			return 0;
		}
		if (shufflePlaylist)
		{
			return _GetPreviousMusicTrackShuffled();
		}
		return _GetPreviousMusicTrackInOrder();
	}

	private Int32 _GetPreviousMusicTrackShuffled()
	{
		if (_playlistPlayed.Count >= 2)
		{
			Int32 result = _playlistPlayed[_playlistPlayed.Count - 2];
			_RemoveLastPlayedOnList();
			_RemoveLastPlayedOnList();
			return result;
		}
		return -1;
	}

	private void _RemoveLastPlayedOnList()
	{
		_playlistPlayed.RemoveAt(_playlistPlayed.Count - 1);
	}

	private Int32 _GetNextMusicTrackShuffled()
	{
		HashSet_Flash<Int32> hashSet_Flash = new HashSet_Flash<Int32>();
		Int32 num = _playlistPlayed.Count;
		if (loopPlaylist)
		{
			Int32 num2 = Mathf.Clamp(musicPlaylist.Length / 4, 2, 10);
			if (num > musicPlaylist.Length - num2)
			{
				num = musicPlaylist.Length - num2;
				if (num < 1)
				{
					num = 1;
				}
			}
		}
		else if (num >= musicPlaylist.Length)
		{
			return -1;
		}
		for (Int32 i = 0; i < num; i++)
		{
			hashSet_Flash.Add(_playlistPlayed[_playlistPlayed.Count - 1 - i]);
		}
		List<Int32> list = new List<Int32>();
		for (Int32 j = 0; j < musicPlaylist.Length; j++)
		{
			if (!hashSet_Flash.Contains(j))
			{
				list.Add(j);
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private Int32 _GetNextMusicTrackInOrder()
	{
		if (_playlistPlayed.Count == 0)
		{
			return 0;
		}
		Int32 num = _playlistPlayed[_playlistPlayed.Count - 1] + 1;
		if (num >= musicPlaylist.Length)
		{
			if (!loopPlaylist)
			{
				return -1;
			}
			num = 0;
		}
		return num;
	}

	private Int32 _GetPreviousMusicTrackInOrder()
	{
		if (_playlistPlayed.Count >= 2)
		{
			Int32 num = _playlistPlayed[_playlistPlayed.Count - 1] - 1;
			_RemoveLastPlayedOnList();
			_RemoveLastPlayedOnList();
			if (num < 0)
			{
				if (!loopPlaylist)
				{
					return -1;
				}
				num = musicPlaylist.Length - 1;
			}
			return num;
		}
		if (loopPlaylist)
		{
			return musicPlaylist.Length - 1;
		}
		return -1;
	}

	protected AudioObject _Play(String audioID, Single volume, Vector3 worldPosition, Transform parentObj, Single delay, Single startTime, Boolean playWithoutAudioObject, Double dspTime = 0.0, AudioObject useExistingAudioObject = null)
	{
		if (_audioDisabled)
		{
			return null;
		}
		AudioItem audioItem = _GetAudioItem(audioID);
		if (audioItem == null)
		{
			Debug.LogWarning("Audio item with name '" + audioID + "' does not exist");
			return null;
		}
		if (audioItem._lastPlayedTime > 0.0 && dspTime == 0.0 && systemTime < audioItem._lastPlayedTime + audioItem.MinTimeBetweenPlayCalls)
		{
			return null;
		}
		if (audioItem.MaxInstanceCount > 0)
		{
			AudioObject[] playingAudioObjects = GetPlayingAudioObjects(audioID, false);
			Boolean flag = playingAudioObjects.Length >= audioItem.MaxInstanceCount;
			if (flag)
			{
				Boolean flag2 = playingAudioObjects.Length > audioItem.MaxInstanceCount;
				AudioObject audioObject = null;
				for (Int32 i = 0; i < playingAudioObjects.Length; i++)
				{
					if (flag2 || !playingAudioObjects[i].isFadingOut)
					{
						if (audioObject == null || playingAudioObjects[i].startedPlayingAtTime < audioObject.startedPlayingAtTime)
						{
							audioObject = playingAudioObjects[i];
						}
					}
				}
				if (audioObject != null)
				{
					audioObject.Stop((!flag2) ? 0.2f : 0f);
				}
			}
		}
		return PlayAudioItem(audioItem, volume, worldPosition, parentObj, delay, startTime, playWithoutAudioObject, useExistingAudioObject, dspTime);
	}

	public AudioObject PlayAudioItem(AudioItem sndItem, Single volume, Vector3 worldPosition, Transform parentObj = null, Single delay = 0f, Single startTime = 0f, Boolean playWithoutAudioObject = false, AudioObject useExistingAudioObj = null, Double dspTime = 0.0)
	{
		AudioObject audioObject = null;
		sndItem._lastPlayedTime = systemTime;
		AudioSubItem[] array = _ChooseSubItems(sndItem, useExistingAudioObj);
		if (array == null || array.Length == 0)
		{
			return null;
		}
		foreach (AudioSubItem audioSubItem in array)
		{
			if (audioSubItem != null)
			{
				AudioObject audioObject2 = PlayAudioSubItem(audioSubItem, volume, worldPosition, parentObj, delay, startTime, playWithoutAudioObject, useExistingAudioObj, dspTime);
				if (audioObject2)
				{
					audioObject = audioObject2;
					audioObject.audioID = sndItem.Name;
					if (sndItem.overrideAudioSourceSettings)
					{
						audioObject2._audioSource_MinDistance_Saved = audioObject2.primaryAudioSource.minDistance;
						audioObject2._audioSource_MaxDistance_Saved = audioObject2.primaryAudioSource.maxDistance;
						audioObject2.primaryAudioSource.minDistance = sndItem.audioSource_MinDistance;
						audioObject2.primaryAudioSource.maxDistance = sndItem.audioSource_MaxDistance;
						if (audioObject2.secondaryAudioSource != null)
						{
							audioObject2.secondaryAudioSource.minDistance = sndItem.audioSource_MinDistance;
							audioObject2.secondaryAudioSource.maxDistance = sndItem.audioSource_MaxDistance;
						}
					}
				}
			}
		}
		return audioObject;
	}

	internal AudioCategory _GetCategory(String name)
	{
		foreach (AudioCategory audioCategory in AudioCategories)
		{
			if (audioCategory.Name == name)
			{
				return audioCategory;
			}
		}
		return null;
	}

	private void Update()
	{
		if (!_isAdditionalAudioController)
		{
			_UpdateSystemTime();
		}
	}

	private static void _UpdateSystemTime()
	{
		Double timeSinceLaunch = SystemTime.timeSinceLaunch;
		if (_lastSystemTime >= 0.0)
		{
			_systemDeltaTime = timeSinceLaunch - _lastSystemTime;
			if (_systemDeltaTime <= Time.maximumDeltaTime + 0.01f)
			{
				_systemTime += _systemDeltaTime;
			}
			else
			{
				_systemDeltaTime = 0.0;
			}
		}
		else
		{
			_systemDeltaTime = 0.0;
			_systemTime = 0.0;
		}
		_lastSystemTime = timeSinceLaunch;
	}

	public virtual Boolean isSingletonObject => !_isAdditionalAudioController;

    protected virtual void Awake()
	{
		if (Persistent)
		{
			DontDestroyOnLoad(gameObject);
		}
		if (isAdditionalAudioController)
		{
			Instance._RegisterAdditionalAudioController(this);
		}
		else
		{
			AwakeSingleton();
		}
	}

	protected virtual void OnDestroy()
	{
		if (isAdditionalAudioController && DoesInstanceExist())
		{
			Instance._UnregisterAdditionalAudioController(this);
		}
		foreach (AudioCategory audioCategory in AudioCategories)
		{
			if (audioCategory != null)
			{
				foreach (AudioItem audioItem in audioCategory.AudioItems)
				{
					if (audioItem != null)
					{
						foreach (AudioSubItem audioSubItem in audioItem.subItems)
						{
							if (audioSubItem != null && audioSubItem.Clip != null)
							{
								Resources.UnloadAsset(audioSubItem.Clip);
							}
						}
					}
				}
			}
		}
	}

	private void AwakeSingleton()
	{
		_UpdateSystemTime();
		if (AudioObjectPrefab == null)
		{
			Debug.LogError("No AudioObject prefab specified in AudioController.");
		}
		else
		{
			_ValidateAudioObjectPrefab(AudioObjectPrefab);
		}
		_ValidateCategories();
		if (_playlistPlayed == null)
		{
			_playlistPlayed = new List<Int32>();
			_isPlaylistPlaying = false;
		}
	}

	protected void _ValidateCategories()
	{
		if (!_categoriesValidated)
		{
			InitializeAudioItems();
			_categoriesValidated = true;
		}
	}

	protected void _InvalidateCategories()
	{
		_categoriesValidated = false;
	}

	public void InitializeAudioItems()
	{
		if (isAdditionalAudioController)
		{
			return;
		}
		_audioItems = new Dictionary<String, AudioItem>();
		_InitializeAudioItems(this);
		if (_additionalAudioControllers != null)
		{
			foreach (AudioController audioController in _additionalAudioControllers)
			{
				if (audioController != null)
				{
					_InitializeAudioItems(audioController);
				}
			}
		}
	}

	private void _InitializeAudioItems(AudioController audioController)
	{
		foreach (AudioCategory audioCategory in audioController.AudioCategories)
		{
			audioCategory.audioController = audioController;
			String text = audioCategory._AnalyseAudioItems(_audioItems);
			if (text != null)
			{
				Debug.LogError(text, audioController);
			}
			if (audioCategory.AudioObjectPrefab)
			{
				_ValidateAudioObjectPrefab(audioCategory.AudioObjectPrefab);
			}
		}
	}

	private void _RegisterAdditionalAudioController(AudioController ac)
	{
		if (_additionalAudioControllers == null)
		{
			_additionalAudioControllers = new List<AudioController>();
		}
		_additionalAudioControllers.Add(ac);
		_InvalidateCategories();
	}

	private void _UnregisterAdditionalAudioController(AudioController ac)
	{
		if (_additionalAudioControllers != null)
		{
			for (Int32 i = 0; i < _additionalAudioControllers.Count; i++)
			{
				if (_additionalAudioControllers[i] == ac)
				{
					_additionalAudioControllers.RemoveAt(i);
					_InvalidateCategories();
					return;
				}
			}
		}
		else
		{
			Debug.LogWarning("_UnregisterAdditionalAudioController: AudioController " + ac.name + " not found");
		}
	}

	protected static AudioSubItem[] _ChooseSubItems(AudioItem audioItem, AudioObject useExistingAudioObj)
	{
		return _ChooseSubItems(audioItem, audioItem.SubItemPickMode, useExistingAudioObj);
	}

	internal static AudioSubItem _ChooseSingleSubItem(AudioItem audioItem, AudioPickSubItemMode pickMode, AudioObject useExistingAudioObj)
	{
		return _ChooseSubItems(audioItem, pickMode, useExistingAudioObj)[0];
	}

	protected static AudioSubItem[] _ChooseSubItems(AudioItem audioItem, AudioPickSubItemMode pickMode, AudioObject useExistingAudioObj)
	{
		if (audioItem.subItems == null)
		{
			return null;
		}
		Int32 num = audioItem.subItems.Length;
		if (num == 0)
		{
			return null;
		}
		Int32 num2 = 0;
		Boolean flag = !ReferenceEquals(useExistingAudioObj, null);
		Int32 num3;
		if (flag)
		{
			num3 = useExistingAudioObj._lastChosenSubItemIndex;
		}
		else
		{
			num3 = audioItem._lastChosen;
		}
		if (num > 1)
		{
			switch (pickMode)
			{
			case AudioPickSubItemMode.Disabled:
				return null;
			case AudioPickSubItemMode.Random:
				num2 = _ChooseRandomSubitem(audioItem, true, num3);
				break;
			case AudioPickSubItemMode.RandomNotSameTwice:
				num2 = _ChooseRandomSubitem(audioItem, false, num3);
				break;
			case AudioPickSubItemMode.Sequence:
				num2 = (num3 + 1) % num;
				break;
			case AudioPickSubItemMode.SequenceWithRandomStart:
				if (num3 == -1)
				{
					num2 = UnityEngine.Random.Range(0, num);
				}
				else
				{
					num2 = (num3 + 1) % num;
				}
				break;
			case AudioPickSubItemMode.AllSimultaneously:
			{
				AudioSubItem[] array = new AudioSubItem[num];
				for (Int32 i = 0; i < num; i++)
				{
					array[i] = audioItem.subItems[i];
				}
				return array;
			}
			case AudioPickSubItemMode.TwoSimultaneously:
				return new AudioSubItem[]
				{
					_ChooseSingleSubItem(audioItem, AudioPickSubItemMode.RandomNotSameTwice, useExistingAudioObj),
					_ChooseSingleSubItem(audioItem, AudioPickSubItemMode.RandomNotSameTwice, useExistingAudioObj)
				};
			case AudioPickSubItemMode.StartLoopSequenceWithFirst:
				if (flag)
				{
					num2 = (num3 + 1) % num;
				}
				else
				{
					num2 = 0;
				}
				break;
			}
		}
		if (flag)
		{
			useExistingAudioObj._lastChosenSubItemIndex = num2;
		}
		else
		{
			audioItem._lastChosen = num2;
		}
		return new AudioSubItem[]
		{
			audioItem.subItems[num2]
		};
	}

	private static Int32 _ChooseRandomSubitem(AudioItem audioItem, Boolean allowSameElementTwiceInRow, Int32 lastChosen)
	{
		Int32 num = audioItem.subItems.Length;
		Int32 result = 0;
		Single num2 = 0f;
		Single max;
		if (!allowSameElementTwiceInRow)
		{
			if (lastChosen >= 0)
			{
				num2 = audioItem.subItems[lastChosen]._SummedProbability;
				if (lastChosen >= 1)
				{
					num2 -= audioItem.subItems[lastChosen - 1]._SummedProbability;
				}
			}
			else
			{
				num2 = 0f;
			}
			max = 1f - num2;
		}
		else
		{
			max = 1f;
		}
		Single num3 = UnityEngine.Random.Range(0f, max);
		Int32 i = 0;
		while (i < num - 1)
		{
			Single num4 = audioItem.subItems[i]._SummedProbability;
			if (allowSameElementTwiceInRow)
			{
				goto IL_A9;
			}
			if (i != lastChosen)
			{
				if (i > lastChosen)
				{
					num4 -= num2;
					goto IL_A9;
				}
				goto IL_A9;
			}
			IL_BA:
			i++;
			continue;
			IL_A9:
			if (num4 > num3)
			{
				result = i;
				break;
			}
			goto IL_BA;
		}
		if (i == num - 1)
		{
			result = num - 1;
		}
		return result;
	}

	public AudioObject PlayAudioSubItem(AudioSubItem subItem, Single volume, Vector3 worldPosition, Transform parentObj, Single delay, Single startTime, Boolean playWithoutAudioObject, AudioObject useExistingAudioObj, Double dspTime = 0.0)
	{
		AudioItem item = subItem.item;
		AudioSubItemType subItemType = subItem.SubItemType;
		if (subItemType != AudioSubItemType.Clip)
		{
			if (subItemType == AudioSubItemType.Item)
			{
				if (subItem.ItemModeAudioID.Length == 0)
				{
					Debug.LogWarning("No item specified in audio sub-item with ITEM mode (audio item: '" + item.Name + "')");
					return null;
				}
				return _Play(subItem.ItemModeAudioID, volume, worldPosition, parentObj, delay, startTime, playWithoutAudioObject, dspTime, useExistingAudioObj);
			}
		}
		if (subItem.Clip == null)
		{
			return null;
		}
		AudioCategory category = item.category;
		Single num = subItem.Volume * item.Volume * volume;
		if (subItem.RandomVolume != 0f)
		{
			num += UnityEngine.Random.Range(-subItem.RandomVolume, subItem.RandomVolume);
			num = Mathf.Clamp01(num);
		}
		Single num2 = num * category.VolumeTotal;
		AudioController audioController = _GetAudioController(subItem);
		if (!audioController.PlayWithZeroVolume && (num2 <= 0f || Volume <= 0f))
		{
			return null;
		}
		GameObject audioObjectPrefab;
		if (category.AudioObjectPrefab != null)
		{
			audioObjectPrefab = category.AudioObjectPrefab;
		}
		else if (audioController.AudioObjectPrefab != null)
		{
			audioObjectPrefab = audioController.AudioObjectPrefab;
		}
		else
		{
			audioObjectPrefab = AudioObjectPrefab;
		}
		if (playWithoutAudioObject)
		{
			audioObjectPrefab.audio.PlayOneShot(subItem.Clip, AudioObject.TransformVolume(num2));
			return null;
		}
		GameObject gameObject;
		AudioObject audioObject;
		if (useExistingAudioObj == null)
		{
			if (item.DestroyOnLoad)
			{
				if (audioController.UsePooledAudioObjects)
				{
					gameObject = ObjectPoolController.Instantiate(audioObjectPrefab, worldPosition, Quaternion.identity);
				}
				else
				{
					gameObject = ObjectPoolController.InstantiateWithoutPool(audioObjectPrefab, worldPosition, Quaternion.identity);
				}
			}
			else
			{
				gameObject = ObjectPoolController.InstantiateWithoutPool(audioObjectPrefab, worldPosition, Quaternion.identity);
				DontDestroyOnLoad(gameObject);
			}
			if (parentObj)
			{
				gameObject.transform.parent = parentObj;
			}
			audioObject = gameObject.gameObject.GetComponent<AudioObject>();
		}
		else
		{
			gameObject = useExistingAudioObj.gameObject;
			audioObject = useExistingAudioObj;
		}
		audioObject.subItem = subItem;
		if (ReferenceEquals(useExistingAudioObj, null))
		{
			audioObject._lastChosenSubItemIndex = item._lastChosen;
		}
		audioObject.primaryAudioSource.clip = subItem.Clip;
		gameObject.name = "AudioObject:" + audioObject.primaryAudioSource.clip.name;
		audioObject.primaryAudioSource.pitch = AudioObject.TransformPitch(subItem.PitchShift);
		audioObject.primaryAudioSource.pan = subItem.Pan2D;
		if (subItem.RandomStartPosition)
		{
			startTime = UnityEngine.Random.Range(0f, audioObject.clipLength);
		}
		audioObject.primaryAudioSource.time = startTime + subItem.ClipStartTime;
		audioObject.primaryAudioSource.loop = (item.Loop == AudioItem.LoopMode.LoopSubitem || item.Loop == (AudioItem.LoopMode)3);
		audioObject._volumeExcludingCategory = num;
		audioObject._volumeFromScriptCall = volume;
		audioObject.category = category;
		audioObject._ApplyVolumePrimary();
		if (subItem.RandomPitch != 0f)
		{
			audioObject.primaryAudioSource.pitch *= AudioObject.TransformPitch(UnityEngine.Random.Range(-subItem.RandomPitch, subItem.RandomPitch));
		}
		if (subItem.RandomDelay > 0f)
		{
			delay += UnityEngine.Random.Range(0f, subItem.RandomDelay);
		}
		if (dspTime > 0.0)
		{
			audioObject.PlayScheduled(dspTime + delay + subItem.Delay + item.Delay);
		}
		else
		{
			audioObject.Play(delay + subItem.Delay + item.Delay);
		}
		if (subItem.FadeIn > 0f)
		{
			audioObject.FadeIn(subItem.FadeIn);
		}
		return audioObject;
	}

	private AudioController _GetAudioController(AudioSubItem subItem)
	{
		if (subItem.item != null && subItem.item.category != null)
		{
			return subItem.item.category.audioController;
		}
		return this;
	}

	internal void _NotifyPlaylistTrackCompleteleyPlayed(AudioObject audioObject)
	{
		audioObject._isCurrentPlaylistTrack = false;
		if (IsPlaylistPlaying() && _currentMusic == audioObject && _PlayNextMusicOnPlaylist(delayBetweenPlaylistTracks) == null)
		{
			_isPlaylistPlaying = false;
		}
	}

	private void _ValidateAudioObjectPrefab(GameObject audioPrefab)
	{
		if (UsePooledAudioObjects)
		{
			if (audioPrefab.GetComponent<PoolableObject>() == null)
			{
				Debug.LogWarning("AudioObject prefab does not have the PoolableObject component. Pooling will not work.", this);
			}
			else
			{
				ObjectPoolController.Preload(audioPrefab);
			}
		}
		if (audioPrefab.GetComponent<AudioObject>() == null)
		{
			Debug.LogError("AudioObject prefab must have the AudioObject script component!", this);
		}
	}
}
