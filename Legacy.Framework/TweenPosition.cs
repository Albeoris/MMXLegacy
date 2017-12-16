using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Position")]
public class TweenPosition : UITweener
{
	public Vector3 from;

	public Vector3 to;

	private Transform mTrans;

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = transform;
			}
			return mTrans;
		}
	}

	public Vector3 position
	{
		get => cachedTransform.localPosition;
	    set => cachedTransform.localPosition = value;
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		cachedTransform.localPosition = from * (1f - factor) + to * factor;
	}

	public static TweenPosition Begin(GameObject go, Single duration, Vector3 pos)
	{
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
		tweenPosition.from = tweenPosition.position;
		tweenPosition.to = pos;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, true);
			tweenPosition.enabled = false;
		}
		return tweenPosition;
	}
}
