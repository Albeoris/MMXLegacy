using System;
using System.Threading;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PreconditionPopupWhoWill")]
	public class PreconditionPopupWhoWill : MonoBehaviour, IIngameContext
	{
		[SerializeField]
		private MultiTextControl m_textControl;

		private PreconditionGUIEventArgs m_args;

		private Boolean m_isActive;

		public event EventHandler OnConfirm;

		private void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CancelKeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(SelectMember1KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(SelectMember2KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(SelectMember3KeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(SelectMember4KeyPressed));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
		}

		private void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CancelKeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(SelectMember1KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(SelectMember2KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(SelectMember3KeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(SelectMember4KeyPressed));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHARACTER_SELECTED, new EventHandler(OnCharacterSelected));
		}

		public void SetArgs(EventArgs p_args)
		{
			m_args = (PreconditionGUIEventArgs)p_args;
		}

		public void Activate()
		{
			m_isActive = true;
			NGUITools.SetActive(gameObject, true);
			m_textControl.OnNextPageEvent += OnNextPage;
			m_textControl.SetInternalText(LocaManager.GetText(m_args.m_condition.Maintext));
			m_textControl.Show();
		}

		public void Deactivate()
		{
			m_isActive = false;
			NGUITools.SetActive(gameObject, false);
			m_textControl.OnNextPageEvent -= OnNextPage;
		}

		private void OnNextPage(Object p_sender, EventArgs p_args)
		{
		}

		private void OnCharacterSelected(Object sender, EventArgs p_args)
		{
			if (m_isActive)
			{
				OnResult(false);
			}
		}

		public void OnCancelClick()
		{
			OnResult(true);
		}

		public void CancelKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				OnResult(true);
			}
		}

		public void SelectMember1KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(0);
			}
		}

		public void SelectMember2KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(1);
			}
		}

		public void SelectMember3KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(2);
			}
		}

		public void SelectMember4KeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (m_isActive && p_args.KeyDown)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(3);
			}
		}

		private void OnResult(Boolean p_cancelled)
		{
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			PreconditionResultWhoWillArgs p_eventArgs = new PreconditionResultWhoWillArgs(selectedCharacter, p_cancelled);
			if (OnConfirm != null)
			{
				OnConfirm(null, EventArgs.Empty);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PRECONDITION_GUI_DONE, p_eventArgs);
		}
	}
}
