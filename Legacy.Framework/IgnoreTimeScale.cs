using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Ignore TimeScale Behaviour")]
public class IgnoreTimeScale : MonoBehaviour
{
	private Single mRt;

	private Single mTimeStart;

	private Single mTimeDelta;

	private Single mActual;

	private Boolean mTimeStarted;

	public Single realTime => mRt;

    public Single realTimeDelta => mTimeDelta;

    protected virtual void OnEnable()
	{
		mTimeStarted = true;
		mTimeDelta = 0f;
		mTimeStart = Time.realtimeSinceStartup;
	}

	protected Single UpdateRealTimeDelta()
	{
		mRt = Time.realtimeSinceStartup;
		if (mTimeStarted)
		{
			Single b = mRt - mTimeStart;
			mActual += Mathf.Max(0f, b);
			mTimeDelta = 0.001f * Mathf.Round(mActual * 1000f);
			mActual -= mTimeDelta;
			if (mTimeDelta > 1f)
			{
				mTimeDelta = 1f;
			}
			mTimeStart = mRt;
		}
		else
		{
			mTimeStarted = true;
			mTimeStart = mRt;
			mTimeDelta = 0f;
		}
		return mTimeDelta;
	}
}
