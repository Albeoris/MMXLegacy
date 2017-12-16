using System;
using UnityEngine;

namespace Legacy.MMGUI
{
	[AddComponentMenu("MM Legacy/GUI Misc/GUIMultiSpriteButton")]
	public class GUIMultiSpriteButton : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] m_tweenTargets;

		[SerializeField]
		private Color m_hoverColor = new Color(1f, 1f, 1f, 0.5f);

		[SerializeField]
		private Color m_pressedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Color m_disabledColor = new Color(0.25f, 0.25f, 0.25f, 0.125f);

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
				collider.enabled = value;
				UpdateColor(value, false);
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
				GameObject gameObject = m_tweenTargets[i];
				if (gameObject != null)
				{
					TweenColor component = gameObject.GetComponent<TweenColor>();
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
				GameObject gameObject = m_tweenTargets[i];
				if (gameObject == null)
				{
					gameObject = this.gameObject;
				}
				UIWidget component = gameObject.GetComponent<UIWidget>();
				if (component != null)
				{
					m_defaultColors[i] = component.color;
				}
				else
				{
					Renderer renderer = gameObject.renderer;
					if (renderer != null)
					{
						m_defaultColors[i] = renderer.material.color;
					}
					else
					{
						Light light = gameObject.light;
						if (light != null)
						{
							m_defaultColors[i] = light.color;
						}
						else
						{
							Debug.LogWarning(NGUITools.GetHierarchy(this.gameObject) + " has nothing for UIButtonColor to color", this);
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
					GameObject go = m_tweenTargets[i];
					TweenColor.Begin(go, m_duration, (!isPressed) ? ((!UICamera.IsHighlighted(gameObject)) ? m_defaultColors[i] : m_hoverColor) : m_pressedColor);
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
					GameObject go = m_tweenTargets[i];
					TweenColor.Begin(go, m_duration, (!isOver) ? m_defaultColors[i] : m_hoverColor);
				}
				m_highlighted = isOver;
			}
		}

		public void UpdateColor(Boolean shouldBeEnabled, Boolean immediate)
		{
			for (Int32 i = 0; i < m_tweenTargets.Length; i++)
			{
				GameObject gameObject = m_tweenTargets[i];
				if (gameObject)
				{
					if (!m_started)
					{
						m_started = true;
						Init();
					}
					Color color = (!shouldBeEnabled) ? m_disabledColor : m_defaultColors[i];
					TweenColor tweenColor = TweenColor.Begin(gameObject, 0.15f, color);
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
	}
}
