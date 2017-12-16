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
	[AddComponentMenu("MM Legacy/MMGUI/PreconditionPopupInput")]
	public class PreconditionPopupInput : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private MultiTextControl m_textControl;

		[SerializeField]
		private UIInput m_textInput;

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
			m_textInput.text = String.Empty;
			m_textInput.selected = true;
			m_textControl.Show();
		}

		public void Deactivate()
		{
			m_isActive = false;
			NGUITools.SetActive(gameObject, false);
		}

		private void OnNextPage(Object p_sender, EventArgs p_args)
		{
			m_textControl.Show();
			NGUITools.SetActive(m_textInput.gameObject, m_textControl.AtEnd());
		}

		public void OnOkClick()
		{
			OnResult(false);
		}

		public void OnCancelClick()
		{
			OnResult(true);
		}

		public void ConfirmKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				if (m_textInput.selected && p_args.Action == EHotkeyType.INTERACT)
				{
					return;
				}
				OnResult(false);
			}
		}

		public void CancelKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				OnResult(true);
			}
		}

		private void OnResult(Boolean p_cancelled)
		{
			PreconditionResultInputArgs p_eventArgs = new PreconditionResultInputArgs(m_textInput.text, p_cancelled);
			if (OnConfirm != null)
			{
				OnConfirm(null, EventArgs.Empty);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PRECONDITION_GUI_DONE, p_eventArgs);
		}
	}
}
