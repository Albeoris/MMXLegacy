using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public UISprite target;

	public String normalSprite;

	public String hoverSprite;

	public String pressedSprite;

	public String disabledSprite;

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
				UpdateImage();
			}
		}
	}

	private void Awake()
	{
		if (target == null)
		{
			target = GetComponentInChildren<UISprite>();
		}
	}

	private void OnEnable()
	{
		UpdateImage();
	}

	private void UpdateImage()
	{
		if (target != null)
		{
			if (isEnabled)
			{
				target.spriteName = ((!UICamera.IsHighlighted(gameObject)) ? normalSprite : hoverSprite);
			}
			else
			{
				target.spriteName = disabledSprite;
			}
			target.MakePixelPerfect();
		}
	}

	private void OnHover(Boolean isOver)
	{
		if (isEnabled && target != null)
		{
			target.spriteName = ((!isOver) ? normalSprite : hoverSprite);
			target.MakePixelPerfect();
		}
	}

	private void OnPress(Boolean pressed)
	{
		if (pressed)
		{
			target.spriteName = pressedSprite;
			target.MakePixelPerfect();
		}
		else
		{
			UpdateImage();
		}
	}
}
