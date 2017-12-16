using System;
using Legacy.Configuration;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.MMInput;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/MovementActionButton")]
	public class MovementActionButton : MonoBehaviour
	{
		[SerializeField]
		private String m_toolTipLocaKey;

		[SerializeField]
		private Vector3 m_tooltipOffset = Vector3.zero;

		[SerializeField]
		private UISprite m_background;

		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private Color m_buttonDisabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);

		[SerializeField]
		private Color m_buttonEnabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

		[SerializeField]
		private Color m_buttonHoveredColor = new Color(1f, 1f, 1f, 0.5f);

		[SerializeField]
		private Color m_actionColor = new Color(0.75f, 1f, 1f);

		private Boolean m_enabled = true;

		private Boolean m_isHovered;

		private Int32 m_hotkey = -1;

		private String m_actionColorHex;

		private void Start()
		{
			m_actionColorHex = "[" + NGUITools.EncodeColor(m_actionColor) + "]";
		}

		public Int32 HotKey
		{
			get => m_hotkey;
		    set => m_hotkey = value;
		}

		private void OnTooltip(Boolean p_show)
		{
			if (m_toolTipLocaKey == null || m_toolTipLocaKey == String.Empty)
			{
				return;
			}
			if (p_show)
			{
				String text = LocaManager.GetText(m_toolTipLocaKey);
				if (m_hotkey >= 0)
				{
					Hotkey hotkey = KeyConfigManager.KeyBindings[(EHotkeyType)m_hotkey];
					KeyCode keyCode = hotkey.Key1;
					if (keyCode == KeyCode.None)
					{
						keyCode = hotkey.AltKey1;
					}
					if (keyCode != KeyCode.None)
					{
						String text2 = "[" + LocaManager.GetText("OPTIONS_INPUT_KEYNAME_" + keyCode.ToString().ToUpper()) + "]";
						String text3 = text;
						text = String.Concat(new String[]
						{
							text3,
							" ",
							m_actionColorHex,
							text2,
							"[-]"
						});
					}
				}
				TooltipManager.Instance.Show(this, text, transform.position, m_tooltipOffset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnHover(Boolean p_isOver)
		{
			m_isHovered = p_isOver;
			if (m_enabled)
			{
				TweenColor.Begin(m_background.gameObject, 0.1f, (!p_isOver) ? m_buttonEnabledColor : m_buttonHoveredColor);
			}
		}

		private void OnPress(Boolean p_isPressed)
		{
			if (!p_isPressed && m_isHovered && m_enabled)
			{
				TweenColor.Begin(m_background.gameObject, 0.1f, m_buttonHoveredColor);
			}
			else
			{
				TweenColor.Begin(m_background.gameObject, 0.1f, (!m_enabled) ? m_buttonDisabledColor : m_buttonEnabledColor);
			}
		}

		public void SetEnabled(Boolean p_enabled)
		{
			if (m_enabled != p_enabled)
			{
				m_enabled = p_enabled;
				if (p_enabled)
				{
					TweenColor.Begin(m_background.gameObject, 0.2f, (!m_isHovered) ? m_buttonEnabledColor : m_buttonHoveredColor);
					TweenColor.Begin(m_icon.gameObject, 0.2f, m_buttonEnabledColor);
				}
				else
				{
					TweenColor.Begin(m_background.gameObject, 0.2f, m_buttonDisabledColor);
					TweenColor.Begin(m_icon.gameObject, 0.2f, m_buttonDisabledColor);
				}
			}
		}
	}
}
