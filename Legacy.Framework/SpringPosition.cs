using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Spring Position")]
public class SpringPosition : IgnoreTimeScale
{
	public Vector3 target = Vector3.zero;

	public Single strength = 10f;

	public Boolean worldSpace;

	public Boolean ignoreTimeScale;

	public GameObject eventReceiver;

	public String callWhenFinished;

	public OnFinished onFinished;

	private Transform mTrans;

	private Single mThreshold;

	private void Start()
	{
		mTrans = transform;
	}

	private void Update()
	{
		Single deltaTime = (!ignoreTimeScale) ? Time.deltaTime : UpdateRealTimeDelta();
		if (worldSpace)
		{
			if (mThreshold == 0f)
			{
				mThreshold = (target - mTrans.position).magnitude * 0.001f;
			}
			mTrans.position = NGUIMath.SpringLerp(mTrans.position, target, strength, deltaTime);
			if (mThreshold >= (target - mTrans.position).magnitude)
			{
				mTrans.position = target;
				if (onFinished != null)
				{
					onFinished(this);
				}
				if (eventReceiver != null && !String.IsNullOrEmpty(callWhenFinished))
				{
					eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
				}
				enabled = false;
			}
		}
		else
		{
			if (mThreshold == 0f)
			{
				mThreshold = (target - mTrans.localPosition).magnitude * 0.001f;
			}
			mTrans.localPosition = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, deltaTime);
			if (mThreshold >= (target - mTrans.localPosition).magnitude)
			{
				mTrans.localPosition = target;
				if (onFinished != null)
				{
					onFinished(this);
				}
				if (eventReceiver != null && !String.IsNullOrEmpty(callWhenFinished))
				{
					eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
				}
				enabled = false;
			}
		}
	}

	public static SpringPosition Begin(GameObject go, Vector3 pos, Single strength)
	{
		SpringPosition springPosition = go.GetComponent<SpringPosition>();
		if (springPosition == null)
		{
			springPosition = go.AddComponent<SpringPosition>();
		}
		springPosition.target = pos;
		springPosition.strength = strength;
		springPosition.onFinished = null;
		if (!springPosition.enabled)
		{
			springPosition.mThreshold = 0f;
			springPosition.enabled = true;
		}
		return springPosition;
	}

	public delegate void OnFinished(SpringPosition spring);
}
