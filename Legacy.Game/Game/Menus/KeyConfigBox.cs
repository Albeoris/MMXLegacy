using System;
using System.Runtime.CompilerServices;
using Legacy.Configuration;
using Legacy.MMInput;
using UnityEngine;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/KeyConfigBox")]
	public class KeyConfigBox : MonoBehaviour
	{
		private static readonly KeyCode[] FORBIDDEN_KEYS;

		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private KeyConfigView m_keyConfigView;

		[SerializeField]
		private Color m_selectedColor;

		private Color m_normalColor;

		private EHotkeyType m_hotkeyType;

		private Boolean m_selected;

		private Boolean m_requested;

		private KeyType m_type;

		private KeyCode m_pressedKey;

		static KeyConfigBox()
		{
		    FORBIDDEN_KEYS = new KeyCode[7]
		    {
		        KeyCode.None,
		        KeyCode.LeftCommand,
		        KeyCode.RightCommand,
		        KeyCode.LeftWindows,
		        KeyCode.RightWindows,
		        KeyCode.LeftCommand,
		        KeyCode.RightCommand
		    };
        }

		public KeyType Type => m_type;

	    public void Init(KeyType p_type, EHotkeyType p_hotkeyType)
		{
			m_type = p_type;
			m_hotkeyType = p_hotkeyType;
			m_normalColor = m_label.color;
			UpdateHotkey();
		}

		private void OnSelect(Boolean p_selected)
		{
			m_label.color = ((!p_selected) ? m_normalColor : m_selectedColor);
			m_selected = p_selected;
			if (p_selected)
			{
				m_keyConfigView.StartKeyInput(this);
				m_label.text = "...";
			}
			else if (!m_requested)
			{
				m_keyConfigView.EndKeyInput();
				UpdateHotkey();
			}
		}

		private void OnGUI()
		{
			if (!m_selected || m_requested)
			{
				return;
			}
			if (Event.current.type == EventType.KeyDown)
			{
				m_pressedKey = Event.current.keyCode;
				foreach (KeyCode keyCode in FORBIDDEN_KEYS)
				{
					if (m_pressedKey == keyCode)
					{
						return;
					}
				}
				m_requested = true;
				m_keyConfigView.RequestKeyBinding(m_pressedKey);
			}
		}

		public void ConfirmKeyBinding()
		{
			m_requested = false;
			Hotkey value = KeyConfigManager.KeyBindings[m_hotkeyType];
			if (m_type == KeyType.PRIMARY)
			{
				value.Key1 = m_pressedKey;
				value.KeyCount = 1;
			}
			else
			{
				value.AltKey1 = m_pressedKey;
				value.AltKeyCount = 1;
			}
			KeyConfigManager.KeyBindings[m_hotkeyType] = value;
			InputManager.SetHotkeyData(KeyConfigManager.KeyBindings[m_hotkeyType]);
			m_keyConfigView.EndKeyInput();
			UpdateHotkey();
			UICamera.selectedObject = null;
		}

		public void CancelKeyBinding()
		{
			m_requested = false;
			UICamera.selectedObject = null;
			m_keyConfigView.EndKeyInput();
			UpdateHotkey();
		}

		public void DeleteKeyBinding()
		{
			Hotkey value = KeyConfigManager.KeyBindings[m_hotkeyType];
			if (m_type == KeyType.PRIMARY)
			{
				value.Key1 = KeyCode.None;
				value.KeyCount = 0;
			}
			else
			{
				value.AltKey1 = KeyCode.None;
				value.AltKeyCount = 0;
			}
			KeyConfigManager.KeyBindings[m_hotkeyType] = value;
			InputManager.SetHotkeyData(KeyConfigManager.KeyBindings[m_hotkeyType]);
			UpdateHotkey();
		}

		public void UpdateHotkey()
		{
			KeyCode keyCode;
			if (m_type == KeyType.PRIMARY)
			{
				keyCode = KeyConfigManager.KeyBindings[m_hotkeyType].Key1;
			}
			else
			{
				keyCode = KeyConfigManager.KeyBindings[m_hotkeyType].AltKey1;
			}
			if (keyCode == KeyCode.None)
			{
				m_label.text = "-";
			}
			else
			{
				m_label.text = LocaManager.GetText("OPTIONS_INPUT_KEYNAME_" + keyCode.ToString().ToUpper());
			}
		}

		public enum KeyType
		{
			PRIMARY,
			ALTERNATIVE
		}
	}
}
