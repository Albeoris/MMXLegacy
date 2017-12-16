using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	public Transform tweenTarget;

	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);

	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);

	public Single duration = 0.2f;

	private Vector3 mScale;

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
			mScale = tweenTarget.localScale;
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
			TweenScale component = tweenTarget.GetComponent<TweenScale>();
			if (component != null)
			{
				component.scale = mScale;
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
			TweenScale.Begin(tweenTarget.gameObject, duration, (!isPressed) ? ((!UICamera.IsHighlighted(gameObject)) ? mScale : Vector3.Scale(mScale, hover)) : Vector3.Scale(mScale, pressed)).method = UITweener.Method.EaseInOut;
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
			TweenScale.Begin(tweenTarget.gameObject, duration, (!isOver) ? mScale : Vector3.Scale(mScale, hover)).method = UITweener.Method.EaseInOut;
			mHighlighted = isOver;
		}
	}
}
