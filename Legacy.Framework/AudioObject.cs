using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("ClockStone/Audio/AudioObject")]
[RequireComponent(typeof(AudioSource))]
public class AudioObject : RegisteredComponent
{
	private const Single VOLUME_TRANSFORM_POWER = 1.6f;

	private AudioSubItem _subItemPrimary;

	private AudioSubItem _subItemSecondary;

	private AudioEventDelegate _completelyPlayedDelegate;

	private Int32 _pauseCoroutineCounter;

	internal Single _volumeExcludingCategory = 1f;

	private Single _volumeFromPrimaryFade = 1f;

	private Single _volumeFromSecondaryFade = 1f;

	internal Single _volumeFromScriptCall = 1f;

	private Boolean _paused;

	private Boolean _applicationPaused;

	private AudioFader _primaryFader;

	private AudioFader _secondaryFader;

	private Double _playTime = -1.0;

	private Double _playStartTimeLocal = -1.0;

	private Double _playStartTimeSystem = -1.0;

	private Double _playScheduledTimeDsp = -1.0;

	private Double _audioObjectTime;

	private Boolean _IsInactive = true;

	private Boolean _stopRequested;

	private Int32 _loopSequenceCount;

	private Boolean _stopAfterFadeoutUserSetting;

	private Boolean _pauseWithFadeOutRequested;

	private Double _dspTimeRemainingAtPause;

	private AudioController _audioController;

	internal Boolean _isCurrentPlaylistTrack;

	internal Single _audioSource_MinDistance_Saved = 1f;

	internal Single _audioSource_MaxDistance_Saved = 500f;

	internal Int32 _lastChosenSubItemIndex = -1;

	private AudioSource _audioSource1;

	private AudioSource _audioSource2;

	private Boolean _primaryAudioSourcePaused;

	private Boolean _secondaryAudioSourcePaused;

	public String audioID { get; internal set; }

	public AudioCategory category { get; internal set; }

	public AudioSubItem subItem
	{
		get => _subItemPrimary;
	    internal set => _subItemPrimary = value;
	}

	public AudioItem audioItem
	{
		get
		{
			if (subItem != null)
			{
				return subItem.item;
			}
			return null;
		}
	}

	public AudioEventDelegate completelyPlayedDelegate
	{
		get => _completelyPlayedDelegate;
	    set => _completelyPlayedDelegate = value;
	}

	public Single volume
	{
		get => _volumeWithCategory;
	    set
		{
			Single volumeFromCategory = _volumeFromCategory;
			if (volumeFromCategory > 0f)
			{
				_volumeExcludingCategory = value / volumeFromCategory;
			}
			else
			{
				_volumeExcludingCategory = value;
			}
			_ApplyVolumeBoth();
		}
	}

	public Single volumeItem
	{
		get
		{
			if (_volumeFromScriptCall > 0f)
			{
				return _volumeExcludingCategory / _volumeFromScriptCall;
			}
			return _volumeExcludingCategory;
		}
		set
		{
			_volumeExcludingCategory = value * _volumeFromScriptCall;
			_ApplyVolumeBoth();
		}
	}

	public Single volumeTotal => volumeTotalWithoutFade * _volumeFromPrimaryFade;

    public Single volumeTotalWithoutFade
	{
		get
		{
			Single num = _volumeWithCategory;
			AudioController audioController;
			if (category != null)
			{
				audioController = category.audioController;
			}
			else
			{
				audioController = _audioController;
			}
			if (audioController != null)
			{
				num *= audioController.Volume;
			}
			return num;
		}
	}

	public Double playCalledAtTime => _playTime;

    public Double startedPlayingAtTime => _playStartTimeSystem;

    public Single timeUntilEnd => clipLength - audioTime;

    public Double scheduledPlayingAtDspTime
	{
		get => _playScheduledTimeDsp;
        set
		{
			_playScheduledTimeDsp = value;
			primaryAudioSource.SetScheduledStartTime(_playScheduledTimeDsp);
		}
	}

	public Single clipLength
	{
		get
		{
			if (_stopClipAtTime > 0f)
			{
				return _stopClipAtTime - _startClipAtTime;
			}
			if (primaryAudioSource.clip != null)
			{
				return primaryAudioSource.clip.length - _startClipAtTime;
			}
			return 0f;
		}
	}

	public Single audioTime
	{
		get => primaryAudioSource.time - _startClipAtTime;
	    set => primaryAudioSource.time = value + _startClipAtTime;
	}

	public Boolean isFadingOut => _primaryFader.isFadingOut;

    public Boolean isFadingIn => _primaryFader.isFadingIn;

    public Single pitch
	{
		get => primaryAudioSource.pitch;
        set => primaryAudioSource.pitch = value;
    }

	public Single pan
	{
		get => primaryAudioSource.pan;
	    set => primaryAudioSource.pan = value;
	}

	public Double audioObjectTime => _audioObjectTime;

    public Boolean stopAfterFadeOut
	{
		get => _stopAfterFadeoutUserSetting;
        set => _stopAfterFadeoutUserSetting = value;
    }

	public void FadeIn(Single fadeInTime)
	{
		if (_playStartTimeLocal > 0.0)
		{
			Double num = _playStartTimeLocal - audioObjectTime;
			if (num > 0.0)
			{
				_primaryFader.FadeIn(fadeInTime, _playStartTimeLocal, false);
				_UpdateFadeVolume();
				return;
			}
		}
		_primaryFader.FadeIn(fadeInTime, audioObjectTime, !_shouldStopIfPrimaryFadedOut);
		_UpdateFadeVolume();
	}

	public void PlayScheduled(Double dspTime)
	{
		_PlayScheduled(dspTime);
	}

	public void PlayAfter(String audioID, Double deltaDspTime = 0.0, Single volume = 1f, Single startTime = 0f)
	{
		AudioController.PlayAfter(audioID, this, deltaDspTime, volume, startTime);
	}

	public void PlayNow(String audioID, Single delay = 0f, Single volume = 1f, Single startTime = 0f)
	{
		AudioItem audioItem = AudioController.GetAudioItem(audioID);
		if (audioItem == null)
		{
			Debug.LogWarning("Audio item with name '" + audioID + "' does not exist");
			return;
		}
		_audioController.PlayAudioItem(audioItem, volume, transform.position, transform.parent, delay, startTime, false, this, 0.0);
	}

	public void Play(Single delay = 0f)
	{
		_PlayDelayed(delay);
	}

	public void Stop()
	{
		Stop(-1f);
	}

	public void Stop(Single fadeOutLength)
	{
		Stop(fadeOutLength, 0f);
	}

	public void Stop(Single fadeOutLength, Single startToFadeTime)
	{
		if (IsPaused(false))
		{
			fadeOutLength = 0f;
			startToFadeTime = 0f;
		}
		if (startToFadeTime > 0f)
		{
			StartCoroutine(_WaitForSecondsThenStop(startToFadeTime, fadeOutLength));
			return;
		}
		_stopRequested = true;
		if (fadeOutLength < 0f)
		{
			fadeOutLength = subItem.FadeOut;
		}
		if (fadeOutLength == 0f && startToFadeTime == 0f)
		{
			_Stop();
			return;
		}
		FadeOut(fadeOutLength, startToFadeTime);
		if (IsSecondaryPlaying())
		{
			SwitchAudioSources();
			FadeOut(fadeOutLength, startToFadeTime);
			SwitchAudioSources();
		}
	}

	private IEnumerator _WaitForSecondsThenStop(Single startToFadeTime, Single fadeOutLength)
	{
		yield return new WaitForSeconds(startToFadeTime);
		if (!_IsInactive)
		{
			Stop(fadeOutLength);
		}
		yield break;
	}

	public void FadeOut(Single fadeOutLength)
	{
		FadeOut(fadeOutLength, 0f);
	}

	public void FadeOut(Single fadeOutLength, Single startToFadeTime)
	{
		if (fadeOutLength < 0f)
		{
			fadeOutLength = subItem.FadeOut;
		}
		if (fadeOutLength > 0f || startToFadeTime > 0f)
		{
			_primaryFader.FadeOut(fadeOutLength, startToFadeTime);
		}
		else if (fadeOutLength == 0f)
		{
			if (_shouldStopIfPrimaryFadedOut)
			{
				_Stop();
			}
			else
			{
				_primaryFader.FadeOut(0f, startToFadeTime);
			}
		}
	}

	public void Pause()
	{
		Pause(0f);
	}

	public void Pause(Single fadeOutTime)
	{
		if (_paused)
		{
			return;
		}
		_paused = true;
		if (fadeOutTime > 0f)
		{
			_pauseWithFadeOutRequested = true;
			FadeOut(fadeOutTime);
			StartCoroutine(_WaitThenPause(fadeOutTime, ++_pauseCoroutineCounter));
			return;
		}
		_PauseNow();
	}

	private void _PauseNow()
	{
		if (_playScheduledTimeDsp > 0.0)
		{
			_dspTimeRemainingAtPause = _playScheduledTimeDsp - AudioSettings.dspTime;
			scheduledPlayingAtDspTime = 9000000000.0;
		}
		_PauseAudioSources();
		if (_pauseWithFadeOutRequested)
		{
			_pauseWithFadeOutRequested = false;
			_primaryFader.Set0();
		}
	}

	public void Unpause()
	{
		Unpause(0f);
	}

	public void Unpause(Single fadeInTime)
	{
		if (!_paused)
		{
			return;
		}
		_UnpauseNow();
		if (fadeInTime > 0f)
		{
			FadeIn(fadeInTime);
		}
		_pauseWithFadeOutRequested = false;
	}

	private void _UnpauseNow()
	{
		_paused = false;
		if (secondaryAudioSource && _secondaryAudioSourcePaused)
		{
			secondaryAudioSource.Play();
		}
		if (_dspTimeRemainingAtPause > 0.0 && _primaryAudioSourcePaused)
		{
			Double num = AudioSettings.dspTime + _dspTimeRemainingAtPause;
			_playStartTimeSystem = AudioController.systemTime + _dspTimeRemainingAtPause;
			primaryAudioSource.PlayScheduled(num);
			scheduledPlayingAtDspTime = num;
			_dspTimeRemainingAtPause = -1.0;
		}
		else if (_primaryAudioSourcePaused)
		{
			primaryAudioSource.Play();
		}
	}

	private IEnumerator _WaitThenPause(Single waitTime, Int32 counter)
	{
		yield return new WaitForSeconds(waitTime);
		if (_pauseWithFadeOutRequested && counter == _pauseCoroutineCounter)
		{
			_PauseNow();
		}
		yield break;
	}

	private void _PauseAudioSources()
	{
		if (primaryAudioSource.isPlaying)
		{
			_primaryAudioSourcePaused = true;
			primaryAudioSource.Pause();
		}
		else
		{
			_primaryAudioSourcePaused = false;
		}
		if (secondaryAudioSource && secondaryAudioSource.isPlaying)
		{
			_secondaryAudioSourcePaused = true;
			secondaryAudioSource.Pause();
		}
		else
		{
			_secondaryAudioSourcePaused = false;
		}
	}

	public Boolean IsPaused(Boolean returnTrueIfStillFadingOut = true)
	{
		if (!returnTrueIfStillFadingOut)
		{
			return !_pauseWithFadeOutRequested && _paused;
		}
		return _paused;
	}

	public Boolean IsPlaying()
	{
		return IsPrimaryPlaying() || IsSecondaryPlaying();
	}

	public Boolean IsPrimaryPlaying()
	{
		return primaryAudioSource.isPlaying;
	}

	public Boolean IsSecondaryPlaying()
	{
		return secondaryAudioSource != null && secondaryAudioSource.isPlaying;
	}

	public AudioSource primaryAudioSource => _audioSource1;

    public AudioSource secondaryAudioSource => _audioSource2;

    public void SwitchAudioSources()
	{
		if (_audioSource2 == null)
		{
			_CreateSecondAudioSource();
		}
		_SwitchValues<AudioSource>(ref _audioSource1, ref _audioSource2);
		_SwitchValues<AudioFader>(ref _primaryFader, ref _secondaryFader);
		_SwitchValues<AudioSubItem>(ref _subItemPrimary, ref _subItemSecondary);
		_SwitchValues<Single>(ref _volumeFromPrimaryFade, ref _volumeFromSecondaryFade);
	}

	private void _SwitchValues<T>(ref T v1, ref T v2)
	{
		T t = v1;
		v1 = v2;
		v2 = t;
	}

	internal Single _volumeFromCategory
	{
		get
		{
			if (category != null)
			{
				return category.VolumeTotal;
			}
			return 1f;
		}
	}

	internal Single _volumeWithCategory => _volumeFromCategory * _volumeExcludingCategory;

    private Single _stopClipAtTime => (subItem == null) ? 0f : subItem.ClipStopTime;

    private Single _startClipAtTime => (subItem == null) ? 0f : subItem.ClipStartTime;

    protected override void Awake()
	{
		base.Awake();
		if (_primaryFader == null)
		{
			_primaryFader = new AudioFader();
		}
		else
		{
			_primaryFader.Set0();
		}
		if (_secondaryFader == null)
		{
			_secondaryFader = new AudioFader();
		}
		else
		{
			_secondaryFader.Set0();
		}
		if (_audioSource1 == null)
		{
			_audioSource1 = audio;
		}
		else if (_audioSource2 && _audioSource1 != audio)
		{
			SwitchAudioSources();
		}
		_Set0();
		_audioController = AudioController.Instance;
	}

	private void _CreateSecondAudioSource()
	{
		_audioSource2 = gameObject.AddComponent<AudioSource>();
		_audioSource2.rolloffMode = _audioSource1.rolloffMode;
		_audioSource2.minDistance = _audioSource1.minDistance;
		_audioSource2.maxDistance = _audioSource1.maxDistance;
		_audioSource2.dopplerLevel = _audioSource1.dopplerLevel;
		_audioSource2.spread = _audioSource1.spread;
		_audioSource2.panLevel = _audioSource1.panLevel;
		_audioSource2.velocityUpdateMode = _audioSource1.velocityUpdateMode;
		_audioSource2.ignoreListenerVolume = _audioSource1.ignoreListenerVolume;
		_audioSource2.playOnAwake = false;
		_audioSource2.priority = _audioSource1.priority;
		_audioSource2.bypassEffects = _audioSource1.bypassEffects;
		_audioSource2.ignoreListenerPause = _audioSource1.ignoreListenerPause;
	}

	private void _Set0()
	{
		_SetReferences0();
		_audioObjectTime = 0.0;
		primaryAudioSource.playOnAwake = false;
		if (secondaryAudioSource)
		{
			secondaryAudioSource.playOnAwake = false;
		}
		_lastChosenSubItemIndex = -1;
		_primaryFader.Set0();
		_secondaryFader.Set0();
		_playTime = -1.0;
		_playStartTimeLocal = -1.0;
		_playStartTimeSystem = -1.0;
		_playScheduledTimeDsp = -1.0;
		_volumeFromPrimaryFade = 1f;
		_volumeFromSecondaryFade = 1f;
		_volumeFromScriptCall = 1f;
		_IsInactive = true;
		_stopRequested = false;
		_volumeExcludingCategory = 1f;
		_paused = false;
		_applicationPaused = false;
		_isCurrentPlaylistTrack = false;
		_loopSequenceCount = 0;
		_stopAfterFadeoutUserSetting = true;
		_pauseWithFadeOutRequested = false;
		_dspTimeRemainingAtPause = -1.0;
		_primaryAudioSourcePaused = false;
		_secondaryAudioSourcePaused = false;
	}

	private void _SetReferences0()
	{
		_audioController = null;
		primaryAudioSource.clip = null;
		if (secondaryAudioSource != null)
		{
			secondaryAudioSource.playOnAwake = false;
			secondaryAudioSource.clip = null;
		}
		subItem = null;
		category = null;
		_completelyPlayedDelegate = null;
	}

	private void _PlayScheduled(Double dspTime)
	{
		if (!primaryAudioSource.clip)
		{
			Debug.LogError("audio.clip == null in " + gameObject.name);
			return;
		}
		_playScheduledTimeDsp = dspTime;
		Double num = dspTime - AudioSettings.dspTime;
		_playStartTimeLocal = num + audioObjectTime;
		_playStartTimeSystem = num + AudioController.systemTime;
		primaryAudioSource.PlayScheduled(dspTime);
		_OnPlay();
	}

	private void _PlayDelayed(Single delay)
	{
		if (!primaryAudioSource.clip)
		{
			Debug.LogError("audio.clip == null in " + gameObject.name);
			return;
		}
		primaryAudioSource.PlayDelayed(delay);
		_playScheduledTimeDsp = -1.0;
		_playStartTimeLocal = audioObjectTime + delay;
		_playStartTimeSystem = AudioController.systemTime + delay;
		_OnPlay();
	}

	private void _OnPlay()
	{
		_IsInactive = false;
		_playTime = audioObjectTime;
		_paused = false;
		_primaryAudioSourcePaused = false;
		_secondaryAudioSourcePaused = false;
		_primaryFader.Set0();
	}

	private void _Stop()
	{
		_primaryFader.Set0();
		_secondaryFader.Set0();
		primaryAudioSource.Stop();
		if (secondaryAudioSource)
		{
			secondaryAudioSource.Stop();
		}
		_paused = false;
		_primaryAudioSourcePaused = false;
		_secondaryAudioSourcePaused = false;
	}

	private void Update()
	{
		if (_IsInactive)
		{
			return;
		}
		if (!IsPaused(false))
		{
			_audioObjectTime += AudioController.systemDeltaTime;
			_primaryFader.time = _audioObjectTime;
			_secondaryFader.time = _audioObjectTime;
		}
		if (_playScheduledTimeDsp > 0.0 && _audioObjectTime > _playStartTimeLocal)
		{
			_playScheduledTimeDsp = -1.0;
		}
		if (!_paused && !_applicationPaused)
		{
			Boolean flag = IsPrimaryPlaying();
			Boolean flag2 = IsSecondaryPlaying();
			if (!flag && !flag2)
			{
				Boolean flag3 = true;
				if (!_stopRequested && flag3 && completelyPlayedDelegate != null)
				{
					completelyPlayedDelegate(this);
					flag3 = !IsPlaying();
				}
				if (_isCurrentPlaylistTrack && AudioController.DoesInstanceExist())
				{
					AudioController.Instance._NotifyPlaylistTrackCompleteleyPlayed(this);
				}
				if (flag3)
				{
					DestroyAudioObject();
					return;
				}
			}
			else
			{
				if (!_stopRequested && _IsAudioLoopSequenceMode() && !IsSecondaryPlaying() && timeUntilEnd < 1f + Mathf.Max(0f, audioItem.loopSequenceOverlap) && _playScheduledTimeDsp < 0.0)
				{
					_ScheduleNextInLoopSequence();
				}
				if (!primaryAudioSource.loop)
				{
					if (_isCurrentPlaylistTrack && _audioController && _audioController.crossfadePlaylist && audioTime > clipLength - _audioController.musicCrossFadeTime_Out)
					{
						if (AudioController.DoesInstanceExist())
						{
							AudioController.Instance._NotifyPlaylistTrackCompleteleyPlayed(this);
						}
					}
					else
					{
						_StartFadeOutIfNecessary();
						if (flag2)
						{
							SwitchAudioSources();
							_StartFadeOutIfNecessary();
							SwitchAudioSources();
						}
					}
				}
			}
		}
		_UpdateFadeVolume();
	}

	private void _StartFadeOutIfNecessary()
	{
		if (subItem == null)
		{
			Debug.LogWarning("subItem == null");
			return;
		}
		Single audioTime = this.audioTime;
		if (!isFadingOut && subItem.FadeOut > 0f && audioTime > clipLength - subItem.FadeOut)
		{
			FadeOut(subItem.FadeOut);
		}
	}

	private Boolean _IsAudioLoopSequenceMode()
	{
		AudioItem audioItem = this.audioItem;
		if (audioItem != null)
		{
			switch (audioItem.Loop)
			{
			case AudioItem.LoopMode.LoopSequence:
			case (AudioItem.LoopMode)3:
				return true;
			case AudioItem.LoopMode.PlaySequenceAndLoopLast:
				return !primaryAudioSource.loop;
			}
		}
		return false;
	}

	private Boolean _ScheduleNextInLoopSequence()
	{
		if (this.audioItem.loopSequenceCount > 0 && this.audioItem.loopSequenceCount <= _loopSequenceCount + 1)
		{
			return false;
		}
		Double dspTime = AudioSettings.dspTime + timeUntilEnd + _GetRandomLoopSequenceDelay(this.audioItem);
		AudioItem audioItem = this.audioItem;
		SwitchAudioSources();
		_audioController.PlayAudioItem(audioItem, _volumeFromScriptCall, Vector3.zero, null, 0f, 0f, false, this, dspTime);
		_loopSequenceCount++;
		if (this.audioItem.Loop == AudioItem.LoopMode.PlaySequenceAndLoopLast)
		{
			Int32 num;
			if (this.audioItem.loopSequenceCount > 0)
			{
				num = this.audioItem.loopSequenceCount;
			}
			else
			{
				num = this.audioItem.subItems.Length;
			}
			if (num <= _loopSequenceCount + 1)
			{
				primaryAudioSource.loop = true;
			}
		}
		return true;
	}

	private void _UpdateFadeVolume()
	{
		Boolean flag;
		Single num = _EqualizePowerForCrossfading(_primaryFader.Get(out flag));
		if (flag)
		{
			if (_stopRequested)
			{
				_Stop();
				return;
			}
			if (!_IsAudioLoopSequenceMode())
			{
				if (_shouldStopIfPrimaryFadedOut)
				{
					_Stop();
				}
				return;
			}
		}
		if (num != _volumeFromPrimaryFade)
		{
			_volumeFromPrimaryFade = num;
			_ApplyVolumePrimary();
		}
		if (_audioSource2 != null)
		{
			Single num2 = _EqualizePowerForCrossfading(_secondaryFader.Get(out flag));
			if (flag)
			{
				_audioSource2.Stop();
			}
			else if (num2 != _volumeFromSecondaryFade)
			{
				_volumeFromSecondaryFade = num2;
				_ApplyVolumeSecondary();
			}
		}
	}

	private Single _EqualizePowerForCrossfading(Single v)
	{
		if (!_audioController.EqualPowerCrossfade)
		{
			return v;
		}
		return InverseTransformVolume(Mathf.Sin(v * 3.14159274f * 0.5f));
	}

	private Boolean _shouldStopIfPrimaryFadedOut => _stopAfterFadeoutUserSetting && !_pauseWithFadeOutRequested;

    private void OnApplicationPause(Boolean b)
	{
		SetApplicationPaused(b);
	}

	private void SetApplicationPaused(Boolean isPaused)
	{
		_applicationPaused = isPaused;
	}

	public void DestroyAudioObject()
	{
		if (IsPlaying())
		{
			_Stop();
		}
		ObjectPoolController.Destroy(gameObject);
		_IsInactive = true;
	}

	public static Single TransformVolume(Single volume)
	{
		return Mathf.Pow(volume, 1.6f);
	}

	public static Single InverseTransformVolume(Single volume)
	{
		return Mathf.Pow(volume, 0.625f);
	}

	public static Single TransformPitch(Single pitchSemiTones)
	{
		return Mathf.Pow(2f, pitchSemiTones / 12f);
	}

	public static Single InverseTransformPitch(Single pitch)
	{
		return Mathf.Log(pitch) / Mathf.Log(2f) * 12f;
	}

	internal void _ApplyVolumeBoth()
	{
		Single volumeTotalWithoutFade = this.volumeTotalWithoutFade;
		Single volume = TransformVolume(volumeTotalWithoutFade * _volumeFromPrimaryFade);
		primaryAudioSource.volume = volume;
		if (secondaryAudioSource)
		{
			volume = TransformVolume(volumeTotalWithoutFade * _volumeFromSecondaryFade);
			secondaryAudioSource.volume = volume;
		}
	}

	internal void _ApplyVolumePrimary()
	{
		Single volume = TransformVolume(volumeTotalWithoutFade * _volumeFromPrimaryFade);
		primaryAudioSource.volume = volume;
	}

	internal void _ApplyVolumeSecondary()
	{
		if (secondaryAudioSource)
		{
			Single volume = TransformVolume(volumeTotalWithoutFade * _volumeFromSecondaryFade);
			secondaryAudioSource.volume = volume;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		AudioItem audioItem = this.audioItem;
		if (audioItem != null && audioItem.overrideAudioSourceSettings)
		{
			_RestoreAudioSourceSettings();
		}
		_SetReferences0();
	}

	private void _RestoreAudioSourceSettings()
	{
		primaryAudioSource.minDistance = _audioSource_MinDistance_Saved;
		primaryAudioSource.maxDistance = _audioSource_MaxDistance_Saved;
		if (secondaryAudioSource != null)
		{
			secondaryAudioSource.minDistance = _audioSource_MinDistance_Saved;
			secondaryAudioSource.maxDistance = _audioSource_MaxDistance_Saved;
		}
	}

	public Boolean DoesBelongToCategory(String categoryName)
	{
		for (AudioCategory audioCategory = category; audioCategory != null; audioCategory = audioCategory.parentCategory)
		{
			if (audioCategory.Name == categoryName)
			{
				return true;
			}
		}
		return false;
	}

	private Single _GetRandomLoopSequenceDelay(AudioItem audioItem)
	{
		Single num = -audioItem.loopSequenceOverlap;
		if (audioItem.loopSequenceRandomDelay > 0f)
		{
			num += UnityEngine.Random.Range(0f, audioItem.loopSequenceRandomDelay);
		}
		return num;
	}

	public delegate void AudioEventDelegate(AudioObject audioObject);
}
