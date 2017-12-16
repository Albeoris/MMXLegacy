using System;
using UnityEngine;

public class AudioFader
{
	private Single _fadeOutTotalTime = -1f;

	private Double _fadeOutStartTime = -1.0;

	private Single _fadeInTotalTime = -1f;

	private Double _fadeInStartTime = -1.0;

	public Double time { get; set; }

	public Boolean isFadingOut => _fadeOutTotalTime >= 0f && time > _fadeOutStartTime;

    public Boolean isFadingIn => _fadeInTotalTime > 0f;

    public void Set0()
	{
		time = 0.0;
		_fadeOutTotalTime = -1f;
		_fadeOutStartTime = -1.0;
		_fadeInTotalTime = -1f;
		_fadeInStartTime = -1.0;
	}

	public void FadeIn(Single fadeInTime, Boolean stopCurrentFadeOut = false)
	{
		FadeIn(fadeInTime, time, stopCurrentFadeOut);
	}

	public void FadeIn(Single fadeInTime, Double startToFadeTime, Boolean stopCurrentFadeOut = false)
	{
		if (isFadingOut && stopCurrentFadeOut)
		{
			Single num = _GetFadeOutValue();
			_fadeOutTotalTime = -1f;
			_fadeOutStartTime = -1.0;
			_fadeInTotalTime = fadeInTime;
			_fadeInStartTime = startToFadeTime - fadeInTime * num;
		}
		else
		{
			_fadeInTotalTime = fadeInTime;
			_fadeInStartTime = startToFadeTime;
		}
	}

	public void FadeOut(Single fadeOutLength, Single startToFadeTime)
	{
		if (isFadingOut)
		{
			Double num = time + startToFadeTime + fadeOutLength;
			Double num2 = _fadeOutStartTime + _fadeOutTotalTime;
			if (num2 < num)
			{
				return;
			}
			Double num3 = time - _fadeOutStartTime;
			Double num4 = startToFadeTime + fadeOutLength;
			Double num5 = num2 - time;
			if (num5 != 0.0)
			{
				Double num6 = num3 * num4 / num5;
				_fadeOutStartTime = time - num6;
				_fadeOutTotalTime = (Single)(num4 + num6);
			}
		}
		else
		{
			_fadeOutTotalTime = fadeOutLength;
			_fadeOutStartTime = time + startToFadeTime;
		}
	}

	public Single Get(out Boolean finishedFadeOut)
	{
		Single num = 1f;
		finishedFadeOut = false;
		if (isFadingOut)
		{
			num *= _GetFadeOutValue();
			if (num == 0f)
			{
				finishedFadeOut = true;
				return 0f;
			}
		}
		if (isFadingIn)
		{
			num *= _GetFadeInValue();
		}
		return num;
	}

	private Single _GetFadeOutValue()
	{
		return 1f - _GetFadeValue((Single)(time - _fadeOutStartTime), _fadeOutTotalTime);
	}

	private Single _GetFadeInValue()
	{
		return _GetFadeValue((Single)(time - _fadeInStartTime), _fadeInTotalTime);
	}

	private Single _GetFadeValue(Single t, Single dt)
	{
		if (dt <= 0f)
		{
			return (t <= 0f) ? 0f : 1f;
		}
		return Mathf.Clamp(t / dt, 0f, 1f);
	}
}
