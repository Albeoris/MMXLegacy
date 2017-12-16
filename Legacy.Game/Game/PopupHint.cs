using System;
using Legacy.Core.Api;
using Legacy.Core.Hints;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PopupHint")]
	public class PopupHint : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private UILabel m_title;

		[SerializeField]
		private UILabel m_text;

		[SerializeField]
		private UICheckbox m_checkBox;

		private Hint m_hint;

		private Boolean m_active;

		private void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnConfirmPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnConfirmPressed));
		}

		private void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnConfirmPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnConfirmPressed));
		}

		public void SetHint(Hint p_hint)
		{
			m_hint = p_hint;
		}

		public void Activate()
		{
			m_active = true;
			NGUITools.SetActiveSelf(gameObject, true);
			m_title.text = m_hint.Title;
			m_text.text = m_hint.Text;
			m_checkBox.isChecked = false;
			AudioController.Play("SOU_ANNO4_PopUp_Hint");
		}

		public void Confirm()
		{
			if (m_checkBox.isChecked)
			{
				LegacyLogic.Instance.WorldManager.HintManager.SetActive(false);
			}
			IngameController.Instance.HideHint();
		}

		public void OnClose()
		{
			IngameController.Instance.HideHint();
		}

		public void Deactivate()
		{
			m_active = false;
			NGUITools.SetActiveSelf(gameObject, false);
		}

		private void OnConfirmPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				Confirm();
			}
		}
	}
}
