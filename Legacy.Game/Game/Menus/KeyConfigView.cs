using System;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/KeyConfigView")]
	public class KeyConfigView : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private KeyConfigBox m_keyPrimary;

		[SerializeField]
		private KeyConfigBox m_keyAlternative;

		[SerializeField]
		private Color m_selectedColor;

		private Color m_normalColor;

		private OptionsInput m_optionsInput;

		private KeyConfigBox m_currentInputKeyBox;

		private EHotkeyType m_hotkeyType;

		public EHotkeyType HotkeyType => m_hotkeyType;

	    public void Init(EHotkeyType p_hotkeyType, OptionsInput p_optionsInput)
		{
			m_normalColor = m_label.color;
			m_optionsInput = p_optionsInput;
			m_hotkeyType = p_hotkeyType;
			m_keyPrimary.Init(KeyConfigBox.KeyType.PRIMARY, p_hotkeyType);
			m_keyAlternative.Init(KeyConfigBox.KeyType.ALTERNATIVE, p_hotkeyType);
			m_label.text = LocaManager.GetText("OPTIONS_INPUT_KEY_" + p_hotkeyType);
		}

		public void StartKeyInput(KeyConfigBox p_keyBox)
		{
			m_currentInputKeyBox = p_keyBox;
			m_optionsInput.StartKeyInput(this);
			m_label.color = m_selectedColor;
			if (IngameController.Instance != null)
			{
				IngameController.Instance.StartHotkeyInput(this);
			}
		}

		public void EndKeyInput()
		{
			m_optionsInput.EndKeyInput();
			m_label.color = m_normalColor;
			if (IngameController.Instance != null)
			{
				IngameController.Instance.StopHotkeyInput();
			}
		}

		public void DeleteCurrentKeyBinding()
		{
			m_currentInputKeyBox.DeleteKeyBinding();
		}

		public void RequestKeyBinding(KeyCode p_key)
		{
			m_optionsInput.RequestKeyBinding(p_key);
		}

		public void ConfirmKeyBinding()
		{
			m_currentInputKeyBox.ConfirmKeyBinding();
		}

		public void CancelKeyBinding()
		{
			m_currentInputKeyBox.CancelKeyBinding();
		}

		public void UpdateHotkeys()
		{
			m_keyPrimary.UpdateHotkey();
			m_keyAlternative.UpdateHotkey();
		}

		public void Activate()
		{
		}

		public void Deactivate()
		{
		}
	}
}
