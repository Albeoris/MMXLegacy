﻿using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Transform")]
public class TweenTransform : UITweener
{
	public Transform from;

	public Transform to;

	public Boolean parentWhenFinished;

	private Transform mTrans;

	private Vector3 mPos;

	private Quaternion mRot;

	private Vector3 mScale;

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		if (to != null)
		{
			if (mTrans == null)
			{
				mTrans = transform;
				mPos = mTrans.position;
				mRot = mTrans.rotation;
				mScale = mTrans.localScale;
			}
			if (from != null)
			{
				mTrans.position = from.position * (1f - factor) + to.position * factor;
				mTrans.localScale = from.localScale * (1f - factor) + to.localScale * factor;
				mTrans.rotation = Quaternion.Slerp(from.rotation, to.rotation, factor);
			}
			else
			{
				mTrans.position = mPos * (1f - factor) + to.position * factor;
				mTrans.localScale = mScale * (1f - factor) + to.localScale * factor;
				mTrans.rotation = Quaternion.Slerp(mRot, to.rotation, factor);
			}
			if (parentWhenFinished && isFinished)
			{
				mTrans.parent = to;
			}
		}
	}

	public static TweenTransform Begin(GameObject go, Single duration, Transform to)
	{
		return Begin(go, duration, null, to);
	}

	public static TweenTransform Begin(GameObject go, Single duration, Transform from, Transform to)
	{
		TweenTransform tweenTransform = UITweener.Begin<TweenTransform>(go, duration);
		tweenTransform.from = from;
		tweenTransform.to = to;
		if (duration <= 0f)
		{
			tweenTransform.Sample(1f, true);
			tweenTransform.enabled = false;
		}
		return tweenTransform;
	}
}
