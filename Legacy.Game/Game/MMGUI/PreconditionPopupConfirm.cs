using System;
using System.Threading;
using Legacy.Core;
using Legacy.Core.EventManagement;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PreconditionPopupConfirm")]
	public class PreconditionPopupConfirm : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private MultiTextControl m_textControl;

		[SerializeField]
		private UIButton m_okButton;

		private PreconditionGUIEventArgs m_args;

		private StringEventArgs m_msgArgs;

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
			m_msgArgs = null;
		}

		public void SetMessageArgs(EventArgs p_args)
		{
			m_msgArgs = (StringEventArgs)p_args;
			m_args = null;
		}

		public void Activate()
		{
			m_isActive = true;
			NGUITools.SetActive(gameObject, true);
			m_textControl.OnNextPageEvent += OnNextPage;
			if (m_args != null)
			{
				m_textControl.SetInternalText(LocaManager.GetText(m_args.m_condition.Maintext));
			}
			else if (m_msgArgs != null)
			{
				m_textControl.SetInternalText(m_msgArgs.text);
			}
			m_textControl.Show();
		}

		public void Deactivate()
		{
			m_isActive = false;
			NGUITools.SetActive(gameObject, false);
			m_textControl.OnNextPageEvent -= OnNextPage;
		}

		public void OnOkClick()
		{
			if (OnConfirm != null)
			{
				OnConfirm(null, EventArgs.Empty);
			}
		}

		public void ConfirmKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown && OnConfirm != null)
			{
				OnConfirm(null, EventArgs.Empty);
			}
		}

		public void CancelKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown && OnConfirm != null)
			{
				OnConfirm(null, EventArgs.Empty);
			}
		}

		private void OnNextPage(Object p_sender, EventArgs p_args)
		{
			NGUITools.SetActive(m_okButton.gameObject, m_textControl.AtEnd());
		}
	}
}
