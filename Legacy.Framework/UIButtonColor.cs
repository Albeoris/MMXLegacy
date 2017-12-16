using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : MonoBehaviour
{
	public GameObject tweenTarget;

	public Color hover = new Color(0.6f, 1f, 0.2f, 1f);

	public Color pressed = Color.grey;

	public Single duration = 0.2f;

	protected Color mColor;

	protected Boolean mStarted;

	protected Boolean mHighlighted;

	public Color defaultColor
	{
		get
		{
			if (!mStarted)
			{
				Init();
			}
			return mColor;
		}
		set => mColor = value;
	}

	private void Start()
	{
		if (!mStarted)
		{
			Init();
			mStarted = true;
		}
	}

	protected virtual void OnEnable()
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
			TweenColor component = tweenTarget.GetComponent<TweenColor>();
			if (component != null)
			{
				component.color = mColor;
				component.enabled = false;
			}
		}
	}

	protected void Init()
	{
		if (tweenTarget == null)
		{
			tweenTarget = gameObject;
		}
		UIWidget component = tweenTarget.GetComponent<UIWidget>();
		if (component != null)
		{
			mColor = component.color;
		}
		else
		{
			Renderer renderer = tweenTarget.renderer;
			if (renderer != null)
			{
				mColor = renderer.material.color;
			}
			else
			{
				Light light = tweenTarget.light;
				if (light != null)
				{
					mColor = light.color;
				}
				else
				{
					Debug.LogWarning(NGUITools.GetHierarchy(gameObject) + " has nothing for UIButtonColor to color", this);
					enabled = false;
				}
			}
		}
		OnEnable();
	}

	public virtual void OnPress(Boolean isPressed)
	{
		if (enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenColor.Begin(tweenTarget, duration, (!isPressed) ? ((!UICamera.IsHighlighted(gameObject)) ? mColor : hover) : pressed);
		}
	}

	public virtual void OnHover(Boolean isOver)
	{
		if (enabled)
		{
			if (!mStarted)
			{
				Start();
			}
			TweenColor.Begin(tweenTarget, duration, (!isOver) ? mColor : hover);
			mHighlighted = isOver;
		}
	}
}
