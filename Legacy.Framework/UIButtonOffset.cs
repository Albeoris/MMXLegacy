using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Offset")]
public class UIButtonOffset : MonoBehaviour
{
	public Transform tweenTarget;

	public Vector3 hover = Vector3.zero;

	public Vector3 pressed = new Vector3(2f, -2f);

	public Single duration = 0.2f;

	private Vector3 mPos;

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
			mPos = tweenTarget.localPosition;
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
			TweenPosition component = tweenTarget.GetComponent<TweenPosition>();
			if (component != null)
			{
				component.position = mPos;
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
			TweenPosition.Begin(tweenTarget.gameObject, duration, (!isPressed) ? ((!UICamera.IsHighlighted(gameObject)) ? mPos : (mPos + hover)) : (mPos + pressed)).method = UITweener.Method.EaseInOut;
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
			TweenPosition.Begin(tweenTarget.gameObject, duration, (!isOver) ? mPos : (mPos + hover)).method = UITweener.Method.EaseInOut;
			mHighlighted = isOver;
		}
	}
}
