using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Volume")]
public class TweenVolume : UITweener
{
	public Single from;

	public Single to = 1f;

	private AudioSource mSource;

	public AudioSource audioSource
	{
		get
		{
			if (mSource == null)
			{
				mSource = audio;
				if (mSource == null)
				{
					mSource = GetComponentInChildren<AudioSource>();
					if (mSource == null)
					{
						Debug.LogError("TweenVolume needs an AudioSource to work with", this);
						enabled = false;
					}
				}
			}
			return mSource;
		}
	}

	public Single volume
	{
		get => audioSource.volume;
	    set => audioSource.volume = value;
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		volume = from * (1f - factor) + to * factor;
		mSource.enabled = (mSource.volume > 0.01f);
	}

	public static TweenVolume Begin(GameObject go, Single duration, Single targetVolume)
	{
		TweenVolume tweenVolume = UITweener.Begin<TweenVolume>(go, duration);
		tweenVolume.from = tweenVolume.volume;
		tweenVolume.to = targetVolume;
		if (duration <= 0f)
		{
			tweenVolume.Sample(1f, true);
			tweenVolume.enabled = false;
		}
		return tweenVolume;
	}
}
