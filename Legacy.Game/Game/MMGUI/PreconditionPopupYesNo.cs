using System;
using System.Threading;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PreconditionPopupYesNo")]
	public class PreconditionPopupYesNo : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private MultiTextControl m_textControl;

		[SerializeField]
		private UIButton m_yesButton;

		[SerializeField]
		private UIButton m_noButton;

		private PreconditionGUIEventArgs m_args;

		private Boolean m_isActive;

		public event EventHandler OnConfirm;

		private void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(ConfirmKeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CancelKeyPressed));
		}

		private void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(ConfirmKeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CancelKeyPressed));
		}

		public void SetArgs(EventArgs p_args)
		{
			m_args = (PreconditionGUIEventArgs)p_args;
		}

		public void Activate()
		{
			m_isActive = true;
			NGUITools.SetActive(gameObject, true);
			m_textControl.SetInternalText(LocaManager.GetText(m_args.m_condition.Maintext));
			m_textControl.Show();
			NGUITools.SetActive(m_yesButton.gameObject, m_textControl.AtEnd());
			NGUITools.SetActive(m_noButton.gameObject, m_textControl.AtEnd());
		}

		public void Deactivate()
		{
			m_isActive = false;
			NGUITools.SetActive(gameObject, false);
		}

		public void OnYesClick()
		{
			OnResult(true, false);
		}

		public void OnNoClick()
		{
			OnResult(false, false);
		}

		public void OnCancelClick()
		{
			OnResult(false, true);
		}

		public void ConfirmKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				OnResult(true, false);
			}
		}

		public void CancelKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				OnResult(false, true);
			}
		}

		private void OnResult(Boolean p_result, Boolean p_cancelled)
		{
			PreconditionResultYesNoArgs p_eventArgs = new PreconditionResultYesNoArgs(p_result, p_cancelled);
			if (OnConfirm != null)
			{
				OnConfirm(null, EventArgs.Empty);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PRECONDITION_GUI_DONE, p_eventArgs);
		}

		private void OnNextPage(Object p_sender, EventArgs p_args)
		{
			NGUITools.SetActive(m_yesButton.gameObject, m_textControl.AtEnd());
			NGUITools.SetActive(m_noButton.gameObject, m_textControl.AtEnd());
		}
	}
}
