﻿using System;
using UnityEngine;
using Object = System.Object;

[AddComponentMenu("NGUI/Interaction/Center On Child")]
public class UICenterOnChild : MonoBehaviour
{
	public Single springStrength = 8f;

	public SpringPanel.OnFinished onFinished;

	private UIDraggablePanel mDrag;

	private GameObject mCenteredObject;

	public GameObject centeredObject => mCenteredObject;

    private void OnEnable()
	{
		Recenter();
	}

	private void OnDragFinished()
	{
		if (enabled)
		{
			Recenter();
		}
	}

	public void Recenter()
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(String.Concat(new Object[]
				{
					GetType(),
					" requires ",
					typeof(UIDraggablePanel),
					" on a parent object in order to work"
				}), this);
				enabled = false;
				return;
			}
			mDrag.onDragFinished = new UIDraggablePanel.OnDragFinished(OnDragFinished);
			if (mDrag.horizontalScrollBar != null)
			{
				mDrag.horizontalScrollBar.onDragFinished = new UIScrollBar.OnDragFinished(OnDragFinished);
			}
			if (mDrag.verticalScrollBar != null)
			{
				mDrag.verticalScrollBar.onDragFinished = new UIScrollBar.OnDragFinished(OnDragFinished);
			}
		}
		if (mDrag.panel == null)
		{
			return;
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 vector = cachedTransform.localPosition;
		vector.x += clipRange.x;
		vector.y += clipRange.y;
		vector = cachedTransform.parent.TransformPoint(vector);
		Vector3 b = vector - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		Single num = Single.MaxValue;
		Transform transform = null;
		Transform transform2 = this.transform;
		Int32 i = 0;
		Int32 childCount = transform2.childCount;
		while (i < childCount)
		{
			Transform child = transform2.GetChild(i);
			Single num2 = Vector3.SqrMagnitude(child.position - b);
			if (num2 < num)
			{
				num = num2;
				transform = child;
			}
			i++;
		}
		if (transform != null)
		{
			mCenteredObject = transform.gameObject;
			Vector3 a = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 b2 = cachedTransform.InverseTransformPoint(vector);
			Vector3 b3 = a - b2;
			if (mDrag.scale.x == 0f)
			{
				b3.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				b3.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				b3.z = 0f;
			}
			SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - b3, springStrength).onFinished = onFinished;
		}
		else
		{
			mCenteredObject = null;
		}
	}
}
