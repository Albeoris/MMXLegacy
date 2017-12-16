using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	public Color disabledColor = Color.grey;

	protected override void OnEnable()
	{
		if (isEnabled)
		{
			base.OnEnable();
		}
		else
		{
			UpdateColor(false, true);
		}
	}

	public override void OnHover(Boolean isOver)
	{
		if (isEnabled)
		{
			base.OnHover(isOver);
		}
	}

	public override void OnPress(Boolean isPressed)
	{
		if (isEnabled)
		{
			base.OnPress(isPressed);
		}
	}

	public Boolean isEnabled
	{
		get
		{
			Collider collider = this.collider;
			return collider && collider.enabled;
		}
		set
		{
			Collider collider = this.collider;
			if (!collider)
			{
				return;
			}
			if (collider.enabled != value)
			{
				collider.enabled = value;
				UpdateColor(value, false);
			}
		}
	}

	public void UpdateColor(Boolean shouldBeEnabled, Boolean immediate)
	{
		if (tweenTarget)
		{
			if (!mStarted)
			{
				mStarted = true;
				Init();
			}
			Color color = (!shouldBeEnabled) ? disabledColor : defaultColor;
			TweenColor tweenColor = TweenColor.Begin(tweenTarget, 0.15f, color);
			if (immediate)
			{
				tweenColor.color = color;
				tweenColor.enabled = false;
			}
		}
	}
}
