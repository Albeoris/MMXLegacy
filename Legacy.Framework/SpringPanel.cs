using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Spring Panel")]
[RequireComponent(typeof(UIPanel))]
public class SpringPanel : IgnoreTimeScale
{
	public Vector3 target = Vector3.zero;

	public Single strength = 10f;

	public OnFinished onFinished;

	private UIPanel mPanel;

	private Transform mTrans;

	private Single mThreshold;

	private UIDraggablePanel mDrag;

	private void Start()
	{
		mPanel = GetComponent<UIPanel>();
		mDrag = GetComponent<UIDraggablePanel>();
		mTrans = transform;
	}

	private void Update()
	{
		Single deltaTime = UpdateRealTimeDelta();
		if (mThreshold == 0f)
		{
			mThreshold = (target - mTrans.localPosition).magnitude * 0.005f;
		}
		Boolean flag = false;
		Vector3 localPosition = mTrans.localPosition;
		Vector3 vector = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, deltaTime);
		if (mThreshold >= Vector3.Magnitude(vector - target))
		{
			vector = target;
			enabled = false;
			flag = true;
		}
		mTrans.localPosition = vector;
		Vector3 vector2 = vector - localPosition;
		Vector4 clipRange = mPanel.clipRange;
		clipRange.x -= vector2.x;
		clipRange.y -= vector2.y;
		mPanel.clipRange = clipRange;
		if (mDrag != null)
		{
			mDrag.UpdateScrollbars(false);
		}
		if (flag && onFinished != null)
		{
			onFinished();
		}
	}

	public static SpringPanel Begin(GameObject go, Vector3 pos, Single strength)
	{
		SpringPanel springPanel = go.GetComponent<SpringPanel>();
		if (springPanel == null)
		{
			springPanel = go.AddComponent<SpringPanel>();
		}
		springPanel.target = pos;
		springPanel.strength = strength;
		springPanel.onFinished = null;
		if (!springPanel.enabled)
		{
			springPanel.mThreshold = 0f;
			springPanel.enabled = true;
		}
		return springPanel;
	}

	public delegate void OnFinished();
}
