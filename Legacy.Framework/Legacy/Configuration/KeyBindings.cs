using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Core.Utilities.Configuration;
using Legacy.MMInput;
using UnityEngine;

namespace Legacy.Configuration
{
	public class KeyBindings : IEnumerable<Hotkey>, IEnumerable
	{
		private Hotkey[] m_hotkeys;

		private Hotkey[] m_currentSavedHotkeys;

		public KeyBindings()
		{
			m_hotkeys = new Hotkey[EnumUtil<EHotkeyType>.Length];
			m_currentSavedHotkeys = new Hotkey[EnumUtil<EHotkeyType>.Length];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_hotkeys.GetEnumerator();
		}

		public Hotkey this[EHotkeyType type]
		{
			get => m_hotkeys[(Int32)type];
		    set => m_hotkeys[(Int32)type] = value;
		}

		public void Load(ConfigReader p_reader)
		{
			EHotkeyType[] values = EnumUtil<EHotkeyType>.Values;
			Dictionary<String, ConfigSetting> settings = p_reader.Groups["Keys"].Settings;
			for (Int32 i = 0; i < values.Length; i++)
			{
				String text = values[i].ToString().ToLowerInvariant();
				ConfigSetting configSetting = settings[text];
				String @string = configSetting.GetString();
				Hotkey hotkey;
				if (!Hotkey.TryParse(values[i], @string, out hotkey))
				{
					Debug.LogError("Cannot parse HotkeyData... " + text + " - " + @string);
				}
				m_hotkeys[(Int32)values[i]] = hotkey;
				m_currentSavedHotkeys[(Int32)values[i]] = hotkey;
			}
		}

		public void Save(ConfigReader p_reader)
		{
			Dictionary<String, ConfigSetting> settings = p_reader.Groups["Keys"].Settings;
			for (Int32 i = 0; i < m_hotkeys.Length; i++)
			{
				settings[((EHotkeyType)i).ToString().ToLowerInvariant()].SetValue(m_hotkeys[i].ToString());
				m_currentSavedHotkeys[i] = m_hotkeys[i];
			}
		}

		public Boolean HasUnsavedChanges()
		{
			for (Int32 i = 0; i < m_hotkeys.Length; i++)
			{
				if (!m_hotkeys[i].Equals(m_currentSavedHotkeys[i]))
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerator<Hotkey> GetEnumerator()
		{
			for (Int32 i = 0; i < m_hotkeys.Length; i++)
			{
				yield return m_hotkeys[i];
			}
			yield break;
		}
	}
}
