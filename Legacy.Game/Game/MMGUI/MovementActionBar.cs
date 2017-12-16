using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.UpdateLogic;
using Legacy.Game.IngameManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/MovementActionBar")]
	public class MovementActionBar : MonoBehaviour
	{
		[SerializeField]
		private MovementActionButton m_turnLeftButton;

		[SerializeField]
		private MovementActionButton m_turnRightButton;

		[SerializeField]
		private MovementActionButton m_strifeRightButton;

		[SerializeField]
		private MovementActionButton m_strifeLeftButton;

		[SerializeField]
		private MovementActionButton m_moveForwardButton;

		[SerializeField]
		private MovementActionButton m_moveBackwardButton;

		[SerializeField]
		private MovementActionButton m_inventoryButton;

		[SerializeField]
		private MovementActionButton m_spellbookButton;

		[SerializeField]
		private MovementActionButton m_journalButton;

		[SerializeField]
		private RessourceView m_ressourceView;

		private IngameInput m_ingameInput;

		private Boolean m_isShowingArrows = true;

		public void Init(IngameInput p_ingameInput)
		{
			m_ingameInput = p_ingameInput;
		}

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.UPDATE_AVAILABLE_ACTIONS, new EventHandler(OnUpdateAvailableActions));
			LegacyLogic.Instance.ConversationManager.BeginConversation += HandleBeginConversation;
			LegacyLogic.Instance.ConversationManager.EndConversation += HandleEndConversation;
			m_inventoryButton.HotKey = 12;
			m_spellbookButton.HotKey = 13;
			m_journalButton.HotKey = 15;
			ShowArrowsOrResources();
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.UPDATE_AVAILABLE_ACTIONS, new EventHandler(OnUpdateAvailableActions));
			LegacyLogic.Instance.ConversationManager.BeginConversation -= HandleBeginConversation;
			LegacyLogic.Instance.ConversationManager.EndConversation -= HandleEndConversation;
		}

		private void Update()
		{
			if (ConfigManager.Instance.Options.IsLeftActionBarShowingArrows != m_isShowingArrows)
			{
				ShowArrowsOrResources();
			}
		}

		private void ShowArrowsOrResources()
		{
			if (ConfigManager.Instance.Options.IsLeftActionBarShowingArrows)
			{
				ShowArrows();
			}
			else
			{
				ShowResources();
			}
		}

		private void ShowArrows()
		{
			m_isShowingArrows = true;
			m_turnLeftButton.gameObject.SetActive(true);
			m_turnRightButton.gameObject.SetActive(true);
			m_strifeRightButton.gameObject.SetActive(true);
			m_strifeLeftButton.gameObject.SetActive(true);
			m_moveForwardButton.gameObject.SetActive(true);
			m_moveBackwardButton.gameObject.SetActive(true);
			m_ressourceView.gameObject.SetActive(false);
		}

		private void ShowResources()
		{
			m_isShowingArrows = false;
			m_turnLeftButton.gameObject.SetActive(false);
			m_turnRightButton.gameObject.SetActive(false);
			m_strifeRightButton.gameObject.SetActive(false);
			m_strifeLeftButton.gameObject.SetActive(false);
			m_moveForwardButton.gameObject.SetActive(false);
			m_moveBackwardButton.gameObject.SetActive(false);
			m_ressourceView.gameObject.SetActive(true);
		}

		private void HandleBeginConversation(Object sender, EventArgs e)
		{
			SetIsEnabledOfButtons(false);
		}

		private void HandleEndConversation(Object sender, EventArgs e)
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
		}

		private void OnUpdateAvailableActions(Object p_sender, EventArgs p_args)
		{
			PartyTurnActor partyTurnActor = LegacyLogic.Instance.UpdateManager.PartyTurnActor;
			Int32 currentCharacter = LegacyLogic.Instance.WorldManager.Party.CurrentCharacter;
			Boolean isOpen = LegacyLogic.Instance.ConversationManager.IsOpen;
			m_moveForwardButton.SetEnabled(partyTurnActor.CanDoCommand(MoveCommand.Forward, currentCharacter));
			m_moveBackwardButton.SetEnabled(partyTurnActor.CanDoCommand(MoveCommand.Backward, currentCharacter));
			m_strifeLeftButton.SetEnabled(partyTurnActor.CanDoCommand(MoveCommand.Left, currentCharacter));
			m_strifeRightButton.SetEnabled(partyTurnActor.CanDoCommand(MoveCommand.Right, currentCharacter));
			m_turnLeftButton.SetEnabled(partyTurnActor.CanDoCommand(RotateCommand.Left, currentCharacter));
			m_turnRightButton.SetEnabled(partyTurnActor.CanDoCommand(RotateCommand.Right, currentCharacter));
			m_inventoryButton.SetEnabled(!isOpen);
			m_spellbookButton.SetEnabled(!isOpen);
			m_journalButton.SetEnabled(!isOpen);
		}

		public void OnClickedButton(GameObject p_sender)
		{
			if (m_ingameInput.Active)
			{
				if (p_sender == m_turnLeftButton.gameObject)
				{
					LegacyLogic.Instance.CommandManager.AddCommand(RotateCommand.Left);
				}
				else if (p_sender == m_turnRightButton.gameObject)
				{
					LegacyLogic.Instance.CommandManager.AddCommand(RotateCommand.Right);
				}
				else if (p_sender == m_strifeRightButton.gameObject)
				{
					LegacyLogic.Instance.CommandManager.AddCommand(MoveCommand.Right);
				}
				else if (p_sender == m_strifeLeftButton.gameObject)
				{
					LegacyLogic.Instance.CommandManager.AddCommand(MoveCommand.Left);
				}
				else if (p_sender == m_moveForwardButton.gameObject)
				{
					LegacyLogic.Instance.CommandManager.AddCommand(MoveCommand.Forward);
				}
				else if (p_sender == m_moveBackwardButton.gameObject)
				{
					LegacyLogic.Instance.CommandManager.AddCommand(MoveCommand.Backward);
				}
				else if (p_sender == m_inventoryButton.gameObject)
				{
					IngameController.Instance.ToggleInventory(this, EventArgs.Empty);
				}
				else if (p_sender == m_spellbookButton.gameObject)
				{
					IngameController.Instance.ToggleSpellbook(this, EventArgs.Empty);
				}
				else if (p_sender == m_journalButton.gameObject)
				{
					IngameController.Instance.HandleOpenJournalEvent(this, EventArgs.Empty);
				}
			}
		}

		private void SetIsEnabledOfButtons(Boolean p_IsEnabled)
		{
			m_moveForwardButton.SetEnabled(p_IsEnabled);
			m_moveBackwardButton.SetEnabled(p_IsEnabled);
			m_strifeLeftButton.SetEnabled(p_IsEnabled);
			m_strifeRightButton.SetEnabled(p_IsEnabled);
			m_turnLeftButton.SetEnabled(p_IsEnabled);
			m_turnRightButton.SetEnabled(p_IsEnabled);
			m_inventoryButton.SetEnabled(p_IsEnabled);
			m_spellbookButton.SetEnabled(p_IsEnabled);
			m_journalButton.SetEnabled(p_IsEnabled);
		}
	}
}
