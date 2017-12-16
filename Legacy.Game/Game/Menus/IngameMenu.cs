using System;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Game.Context;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.Menus
{
	[AddComponentMenu("MM Legacy/Menus/IngameMenu")]
	public class IngameMenu : MonoBehaviour, IIngameContext
	{
		private Boolean m_active;

		[SerializeField]
		private UIButton m_saveButton;

		[SerializeField]
		private UIButton m_loadButton;

		[SerializeField]
		private UIButton m_optionsButton;

		[SerializeField]
		private UIButton m_quitButton;

		public event EventHandler OpenOptionsEvent;

		public event EventHandler OpenSaveMenu;

		public event EventHandler OpenLoadMenu;

		public event EventHandler OpenHelpScreenEvent;

		public event EventHandler CancelBackClicked;

		public Boolean IsVisible => m_active;

	    private void Awake()
		{
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnOpenCloseMenu));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnConfirm));
		}

		private void OnDestroy()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnOpenCloseMenu));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnConfirm));
		}

		public void Activate()
		{
			m_active = true;
			DragDropManager.Instance.CancelDragAction();
			NGUITools.SetActiveSelf(gameObject, true);
		}

		public void Deactivate()
		{
			m_active = false;
			if (!PopupRequest.Instance.IsActive)
			{
				NGUITools.SetActiveSelf(gameObject, false);
			}
		}

		private void OnOpenCloseMenu(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && CancelBackClicked != null)
			{
				CancelBackClicked(this, EventArgs.Empty);
			}
		}

		private void OnConfirm(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && CancelBackClicked != null && !IngameController.Instance.IsAnyIngameMenuSubscreenOpen())
			{
				CancelBackClicked(this, EventArgs.Empty);
			}
		}

		public void OnExitToMenuButtonClick()
		{
			if (!LegacyLogic.Instance.WorldManager.Party.InCombat && !LegacyLogic.Instance.WorldManager.Party.HasAggro())
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(AutoSaveSaved));
				LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
				LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.AUTO;
				LegacyLogic.Instance.WorldManager.SaveGameName = "AutoSave";
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_STARTED_SAVE, EventArgs.Empty);
			}
			else
			{
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_LEAVE_GAME_IN_COMBAT_OR_AGGRORANGE"), new PopupRequest.RequestCallback(OnLeaveWithoutSaveRequestClosed));
			}
		}

		private void OnLeaveWithoutSaveRequestClosed(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				BackToMainMenu();
			}
		}

		private void AutoSaveSaved(Object p_sender, EventArgs p_args)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(AutoSaveSaved));
			BackToMainMenu();
		}

		private void BackToMainMenu()
		{
			LegacyLogic.Instance.WorldManager.ClearAndDestroy();
			LegacyLogic.Instance.WorldManager.SaveGameName = String.Empty;
			LegacyLogic.Instance.WorldManager.IsSaveGame = false;
			ContextManager.ChangeContext(EContext.Mainmenu);
		}

		public void OnSaveButtonClick()
		{
			if (!LegacyLogic.Instance.WorldManager.Party.InCombat && !LegacyLogic.Instance.WorldManager.Party.HasAggro())
			{
				if (OpenSaveMenu != null)
				{
					OpenSaveMenu(this, EventArgs.Empty);
				}
			}
			else
			{
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_NOT_IN_COMBAT_OR_AGGRO"), null);
			}
		}

		public void OnLoadButtonClick()
		{
			if (OpenLoadMenu != null)
			{
				OpenLoadMenu(this, EventArgs.Empty);
			}
		}

		public void OnOptionsButtonClick()
		{
			if (OpenOptionsEvent != null)
			{
				OpenOptionsEvent(this, EventArgs.Empty);
			}
		}

		public void OnResumeButtonClick()
		{
			if (m_active && CancelBackClicked != null)
			{
				CancelBackClicked(this, EventArgs.Empty);
			}
		}

		public void OnHelpButtonClick()
		{
			if (OpenHelpScreenEvent != null)
			{
				OpenHelpScreenEvent(this, EventArgs.Empty);
			}
		}

		public void OnQuitButtonClick()
		{
			PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("INGAME_MENU_QUIT"), new PopupRequest.RequestCallback(QuitGame));
		}

		public void QuitGame(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				Main.Instance.QuitGame();
			}
		}

		private void DisableButtons()
		{
			m_saveButton.isEnabled = false;
			m_loadButton.isEnabled = false;
			m_optionsButton.isEnabled = false;
			m_quitButton.isEnabled = false;
		}
	}
}
