using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	[AddComponentMenu("MM Legacy/GUI Misc/GUIMultiColorSpriteButton")]
	public class GUIMultiColorSpriteButton : MonoBehaviour
	{
		[SerializeField]
		private ColorSprite[] m_tweenTargets;

		[SerializeField]
		private Single m_duration = 0.05f;

		[SerializeField]
		private UILabel m_label;

		protected Color[] m_defaultColors;

		protected Boolean m_started;

		protected Boolean m_highlighted;

		public Color[] DefaultColors
		{
			get
			{
				if (!m_started)
				{
					Init();
				}
				return m_defaultColors;
			}
		}

		public Boolean IsEnabled
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
					UpdateColor(value, true);
				}
			}
		}

		private void Start()
		{
			if (!m_started)
			{
				Init();
				m_started = true;
			}
		}

		protected virtual void OnEnable()
		{
			if (IsEnabled)
			{
				if (m_started && m_highlighted)
				{
					OnHover(UICamera.IsHighlighted(gameObject));
				}
			}
			else
			{
				UpdateColor(false, true);
			}
		}

		private void OnDisable()
		{
			for (Int32 i = 0; i < m_tweenTargets.Length; i++)
			{
				ColorSprite colorSprite = m_tweenTargets[i];
				if (colorSprite.TweenTarget != null)
				{
					TweenColor component = colorSprite.TweenTarget.GetComponent<TweenColor>();
					if (component != null)
					{
						component.color = m_defaultColors[i];
						component.enabled = false;
					}
				}
			}
		}

		protected void Init()
		{
			m_defaultColors = new Color[m_tweenTargets.Length];
			for (Int32 i = 0; i < m_tweenTargets.Length; i++)
			{
				ColorSprite colorSprite = m_tweenTargets[i];
				if (colorSprite.TweenTarget == null)
				{
					colorSprite.TweenTarget = gameObject;
				}
				UIWidget component = colorSprite.TweenTarget.GetComponent<UIWidget>();
				if (component != null)
				{
					m_defaultColors[i] = component.color;
				}
				else
				{
					Renderer renderer = colorSprite.TweenTarget.renderer;
					if (renderer != null)
					{
						m_defaultColors[i] = renderer.material.color;
					}
					else
					{
						Light light = colorSprite.TweenTarget.light;
						if (light != null)
						{
							m_defaultColors[i] = light.color;
						}
						else
						{
							Debug.LogWarning(NGUITools.GetHierarchy(gameObject) + " has nothing for UIButtonColor to color", this);
							enabled = false;
						}
					}
				}
			}
			OnEnable();
		}

		public virtual void OnPress(Boolean isPressed)
		{
			if (enabled && IsEnabled)
			{
				if (!m_started)
				{
					Start();
				}
				for (Int32 i = 0; i < m_tweenTargets.Length; i++)
				{
					ColorSprite colorSprite = m_tweenTargets[i];
					TweenColor.Begin(colorSprite.TweenTarget, m_duration, (!isPressed) ? ((!UICamera.IsHighlighted(gameObject)) ? m_defaultColors[i] : colorSprite.HoverColor) : colorSprite.PressedColor);
				}
			}
		}

		public virtual void OnHover(Boolean isOver)
		{
			if (enabled && IsEnabled)
			{
				if (!m_started)
				{
					Start();
				}
				for (Int32 i = 0; i < m_tweenTargets.Length; i++)
				{
					ColorSprite colorSprite = m_tweenTargets[i];
					TweenColor.Begin(colorSprite.TweenTarget, m_duration, (!isOver) ? m_defaultColors[i] : colorSprite.HoverColor);
				}
				m_highlighted = isOver;
			}
		}

		public void UpdateColor(Boolean shouldBeEnabled, Boolean immediate)
		{
			for (Int32 i = 0; i < m_tweenTargets.Length; i++)
			{
				ColorSprite colorSprite = m_tweenTargets[i];
				if (colorSprite.TweenTarget)
				{
					if (!m_started)
					{
						m_started = true;
						Init();
					}
					Color color = (!shouldBeEnabled) ? colorSprite.DisabledColor : m_defaultColors[i];
					TweenColor tweenColor = TweenColor.Begin(colorSprite.TweenTarget, 0.15f, color);
					if (immediate)
					{
						tweenColor.color = color;
						tweenColor.enabled = false;
					}
				}
			}
		}

		public void SetLabelText(String p_text)
		{
			if (m_label != null)
			{
				m_label.text = p_text;
			}
		}

		[Serializable]
		public class ColorSprite
		{
			public GameObject TweenTarget;

			public Color HoverColor;

			public Color PressedColor;

			public Color DisabledColor;
		}
	}
}
