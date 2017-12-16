using System;
using Legacy.Configuration;
using Legacy.MMInput;
using UnityEngine;

namespace Legacy.Game.MMGUI.Tooltip
{
	[AddComponentMenu("MM Legacy/MMGUI/Tooltip/ActionTooltipOnHover")]
	public class ActionTooltipOnHover : TextTooltipOnHover
	{
		[SerializeField]
		private Color m_actionColor = new Color(0f, 0.75f, 1f);

		[SerializeField]
		private EHotkeyType m_actionHotKey;

		private String m_actionColorHex;

		private void Start()
		{
			m_actionColorHex = "[" + NGUITools.EncodeColor(m_actionColor) + "]";
		}

		protected override void OnTooltip(Boolean p_isHovered)
		{
			if (p_isHovered)
			{
				String text = String.Empty;
				String p_tooltipText = String.Empty;
				if (m_caption != String.Empty)
				{
					text = LocaManager.GetText(m_caption);
					if (m_actionColor.a > 0.001f)
					{
						Hotkey hotkey = KeyConfigManager.KeyBindings[m_actionHotKey];
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
				}
				if (m_locaKey != String.Empty)
				{
					p_tooltipText = LocaManager.GetText(m_locaKey);
				}
				TooltipManager.Instance.Show(this, text, p_tooltipText, m_size, transform.position, m_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
