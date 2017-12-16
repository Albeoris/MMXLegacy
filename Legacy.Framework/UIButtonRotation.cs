using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Rotation")]
public class UIButtonRotation : MonoBehaviour
{
	public Transform tweenTarget;

	public Vector3 hover = Vector3.zero;

	public Vector3 pressed = Vector3.zero;

	public Single duration = 0.2f;

	private Quaternion mRot;

	private Boolean mStarted;

	private Boolean mHighlighted;

	private void Start()
	{
		if (!mStarted)
		{
			mStarted = true;
			if (tweenTarget == null)
			{
				tweenTarget = transform;
			}
			mRot = tweenTarget.localRotation;
		}
	}

	private void OnEnable()
	{
		if (mStarted && mHighlighted)
		{
			OnHover(UICamera.IsHighlighted(gameObject));
		}
	}

	private void OnDisable()
	{
		if (mStarted && tweenTarget != null)
		{
			TweenRotation component = tweenTarget.GetComponent<TweenRotation>();
			if (component != null)
			{
				component.rotation = mRot;
				component.enabled = false;
			}
		}
	}

	private void OnPress(Boolean isPressed)
	{
		if (enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenRotation.Begin(tweenTarget.gameObject, duration, (!isPressed) ? ((!UICamera.IsHighlighted(gameObject)) ? mRot : (mRot * Quaternion.Euler(hover))) : (mRot * Quaternion.Euler(pressed))).method = UITweener.Method.EaseInOut;
		}
	}

	private void OnHover(Boolean isOver)
	{
		if (enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenRotation.Begin(tweenTarget.gameObject, duration, (!isOver) ? mRot : (mRot * Quaternion.Euler(hover))).method = UITweener.Method.EaseInOut;
			mHighlighted = isOver;
		}
	}
}
