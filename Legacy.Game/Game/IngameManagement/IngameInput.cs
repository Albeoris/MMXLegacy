using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.UpdateLogic;
using Legacy.Core.UpdateLogic.Actions;
using Legacy.Game.MMGUI;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.IngameManagement
{
	public class IngameInput : IIngameContext
	{
		private CommandManager m_commandManager;

		private MouseInteraction m_mouseInteraction;

		private Boolean m_active;

		private Boolean m_movementActive = true;

		public IngameInput(CommandManager p_controller, MouseInteraction p_mouseInteraction)
		{
			m_commandManager = p_controller;
			m_mouseInteraction = p_mouseInteraction;
			p_mouseInteraction.enabled = false;
			InputManager.RegisterHotkeyEvent(EHotkeyType.MOVE_FORWARD, new EventHandler<HotkeyEventArgs>(OnMoveForward));
			InputManager.RegisterHotkeyEvent(EHotkeyType.MOVE_BACKWARD, new EventHandler<HotkeyEventArgs>(OnMoveBackward));
			InputManager.RegisterHotkeyEvent(EHotkeyType.MOVE_LEFT, new EventHandler<HotkeyEventArgs>(OnMoveLeft));
			InputManager.RegisterHotkeyEvent(EHotkeyType.MOVE_RIGHT, new EventHandler<HotkeyEventArgs>(OnMoveRight));
			InputManager.RegisterHotkeyEvent(EHotkeyType.ROTATE_LEFT, new EventHandler<HotkeyEventArgs>(OnRotateLeft));
			InputManager.RegisterHotkeyEvent(EHotkeyType.ROTATE_RIGHT, new EventHandler<HotkeyEventArgs>(OnRotateRight));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnInteract));
			InputManager.RegisterHotkeyEvent(EHotkeyType.REST, new EventHandler<HotkeyEventArgs>(OnRest));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnOpenCloseMenu));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICKSAVE, new EventHandler<HotkeyEventArgs>(OnQuickSave));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICKLOAD, new EventHandler<HotkeyEventArgs>(OnQuickLoad));
			InputManager.RegisterHotkeyEvent(EHotkeyType.RESET_TURN_ACTOR, new EventHandler<HotkeyEventArgs>(OnResetTurnActor));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_SPELLBOOK, new EventHandler<HotkeyEventArgs>(OnToggleSpellBook));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_SKILLS, new EventHandler<HotkeyEventArgs>(OnToggleSkills));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_JOURNAL, new EventHandler<HotkeyEventArgs>(OnToggleJournal));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_BESTIARY, new EventHandler<HotkeyEventArgs>(OnToggleBestiary));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_LORE, new EventHandler<HotkeyEventArgs>(OnToggleLore));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember1));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember2));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember3));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember4));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_MAP, new EventHandler<HotkeyEventArgs>(OnOpenMap));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_AREA_MAP, new EventHandler<HotkeyEventArgs>(OnOpenAreaMap));
			InputManager.RegisterHotkeyEvent(EHotkeyType.SELECT_NEXT_INTERACTIVE_OBJECT, new EventHandler<HotkeyEventArgs>(OnSelectNextInteractiveObject));
		}

		public event EventHandler CancelBackClicked;

		public event EventHandler OpenSpellbookEvent;

		public event EventHandler OpenSkillsEvent;

		public event EventHandler OpenJournalEvent;

		public event EventHandler OpenBestiaryEvent;

		public event EventHandler OpenLoreEvent;

		public event EventHandler OpenMapEvent;

		public event EventHandler OpenAreaMapEvent;

		public event EventHandler OnStartedQuickSave;

		public Boolean Active => m_active;

	    public Boolean MovementActive
		{
			get => m_movementActive;
	        set => m_movementActive = value;
	    }

		public void CleanUp()
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.MOVE_FORWARD, new EventHandler<HotkeyEventArgs>(OnMoveForward));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.MOVE_BACKWARD, new EventHandler<HotkeyEventArgs>(OnMoveBackward));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.MOVE_LEFT, new EventHandler<HotkeyEventArgs>(OnMoveLeft));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.MOVE_RIGHT, new EventHandler<HotkeyEventArgs>(OnMoveRight));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.ROTATE_LEFT, new EventHandler<HotkeyEventArgs>(OnRotateLeft));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.ROTATE_RIGHT, new EventHandler<HotkeyEventArgs>(OnRotateRight));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnInteract));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.REST, new EventHandler<HotkeyEventArgs>(OnRest));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(OnOpenCloseMenu));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICKSAVE, new EventHandler<HotkeyEventArgs>(OnQuickSave));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICKLOAD, new EventHandler<HotkeyEventArgs>(OnQuickLoad));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.RESET_TURN_ACTOR, new EventHandler<HotkeyEventArgs>(OnResetTurnActor));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_SPELLBOOK, new EventHandler<HotkeyEventArgs>(OnToggleSpellBook));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_SKILLS, new EventHandler<HotkeyEventArgs>(OnToggleSkills));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_JOURNAL, new EventHandler<HotkeyEventArgs>(OnToggleJournal));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_BESTIARY, new EventHandler<HotkeyEventArgs>(OnToggleBestiary));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_LORE, new EventHandler<HotkeyEventArgs>(OnToggleLore));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_1, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember1));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_2, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember2));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_3, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember3));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_PARTY_MEMBER_4, new EventHandler<HotkeyEventArgs>(OnSelectPartyMember4));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_MAP, new EventHandler<HotkeyEventArgs>(OnOpenMap));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_AREA_MAP, new EventHandler<HotkeyEventArgs>(OnOpenAreaMap));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.SELECT_NEXT_INTERACTIVE_OBJECT, new EventHandler<HotkeyEventArgs>(OnSelectNextInteractiveObject));
		}

		public void Activate()
		{
			m_active = true;
			m_mouseInteraction.enabled = m_active;
		}

		public void Deactivate()
		{
			m_active = false;
			m_mouseInteraction.enabled = m_active;
		}

		public void Update()
		{
			if (!Input.anyKey && m_commandManager.ContiniousCommandCount > 0)
			{
				m_commandManager.ClearContiniousQueue();
			}
		}

		private void OnSelectPartyMember1(Object sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(0);
			}
		}

		private void OnSelectPartyMember2(Object sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(1);
			}
		}

		private void OnSelectPartyMember3(Object sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(2);
			}
		}

		private void OnSelectPartyMember4(Object sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				IngameController.Instance.HUDControl.OnCharacterPressed(3);
			}
		}

		private void OnOpenMap(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && OpenMapEvent != null)
			{
				OpenMapEvent(this, EventArgs.Empty);
			}
		}

		private void OnOpenAreaMap(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && OpenAreaMapEvent != null)
			{
				OpenAreaMapEvent(this, EventArgs.Empty);
			}
		}

		private void OnMoveForward(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && m_movementActive)
			{
				m_commandManager.AddCommand(MoveCommand.Forward);
			}
			else
			{
				m_commandManager.EndCommand(MoveCommand.Forward);
			}
		}

		private void OnMoveBackward(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && m_movementActive)
			{
				m_commandManager.AddCommand(MoveCommand.Backward);
			}
			else
			{
				m_commandManager.EndCommand(MoveCommand.Backward);
			}
		}

		private void OnMoveLeft(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && m_movementActive)
			{
				m_commandManager.AddCommand(MoveCommand.Left);
			}
			else
			{
				m_commandManager.EndCommand(MoveCommand.Left);
			}
		}

		private void OnMoveRight(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && m_movementActive)
			{
				m_commandManager.AddCommand(MoveCommand.Right);
			}
			else
			{
				m_commandManager.EndCommand(MoveCommand.Right);
			}
		}

		private void OnRotateLeft(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && m_movementActive)
			{
				m_commandManager.AddCommand(RotateCommand.Left);
			}
			else
			{
				m_commandManager.EndCommand(RotateCommand.Left);
			}
		}

		private void OnRotateRight(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && m_movementActive)
			{
				m_commandManager.AddCommand(RotateCommand.Right);
			}
			else
			{
				m_commandManager.EndCommand(RotateCommand.Right);
			}
		}

		private void OnInteract(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				m_commandManager.AddCommand(new InteractCommand(null));
			}
		}

		private void OnSelectNextInteractiveObject(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				if (party.InCombat)
				{
					party.AutoSelectNextMonster();
				}
				else
				{
					m_commandManager.AddCommand(SelectNextInteractiveObjectCommand.Instance);
				}
			}
		}

		private void OnOpenCloseMenu(Object p_sender, HotkeyEventArgs p_args)
		{
			Boolean flag = false;
			List<Character> fightingCharacters = LegacyLogic.Instance.WorldManager.Party.GetFightingCharacters();
			for (Int32 i = 0; i < fightingCharacters.Count; i++)
			{
				if (!fightingCharacters[i].ConditionHandler.CantDoAnything())
				{
					flag = true;
					break;
				}
			}
			if (p_args.KeyDown && m_active && CancelBackClicked != null && flag)
			{
				CancelBackClicked(this, EventArgs.Empty);
			}
		}

		private void OnQuickSave(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				if (!LegacyLogic.Instance.WorldManager.QuickSaveAllowed)
				{
					return;
				}
				if (!LegacyLogic.Instance.WorldManager.Party.InCombat && !LegacyLogic.Instance.WorldManager.Party.HasAggro() && !LegacyLogic.Instance.ConversationManager.IsOpen)
				{
					if (OnStartedQuickSave != null)
					{
						LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.QUICK;
						LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
						LegacyLogic.Instance.WorldManager.SaveGameName = LocaManager.GetText("SAVEGAMETYPE_QUICK");
						OnStartedQuickSave(this, EventArgs.Empty);
					}
				}
				else
				{
					PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_NOT_IN_COMBAT_OR_AGGRO"), null);
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.CANT_ACT);
				}
			}
		}

		private void OnQuickLoad(Object p_sender, HotkeyEventArgs p_args)
		{
			if (LegacyLogic.Instance.UpdateManager.CurrentTurnActor == LegacyLogic.Instance.UpdateManager.PartyTurnActor)
			{
				Boolean flag = false;
				List<Character> fightingCharacters = LegacyLogic.Instance.WorldManager.Party.GetFightingCharacters();
				for (Int32 i = 0; i < fightingCharacters.Count; i++)
				{
					if (!fightingCharacters[i].ConditionHandler.CantDoAnything())
					{
						flag = true;
						break;
					}
				}
				if (p_args.KeyDown && m_active && flag)
				{
					if (LegacyLogic.Instance.WorldManager.SaveGameManager.SaveGameExists(LocaManager.GetText("SAVEGAMETYPE_QUICK")))
					{
						IngameController.Instance.CloseSavegameMenus();
						PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("SAVEGAME_MENU_LOAD_REQUEST_POPUP"), new PopupRequest.RequestCallback(QuickLoadCallback));
					}
					else
					{
						PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM, String.Empty, LocaManager.GetText("SAVEGAME_ERROR_NO_QUICKSAVE_FOUND"), null);
					}
				}
			}
		}

		private void QuickLoadCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				Boolean flag = false;
				List<Character> fightingCharacters = LegacyLogic.Instance.WorldManager.Party.GetFightingCharacters();
				for (Int32 i = 0; i < fightingCharacters.Count; i++)
				{
					if (!fightingCharacters[i].ConditionHandler.CantDoAnything())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					LegacyLogic.Instance.WorldManager.IsSaveGame = true;
					LegacyLogic.Instance.WorldManager.Load(LocaManager.GetText("SAVEGAMETYPE_QUICK"));
				}
			}
		}

		private void OnRest(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active)
			{
				foreach (BaseAction baseAction in LegacyLogic.Instance.UpdateManager.PartyTurnActor.ActiveActions)
				{
					if (baseAction is RestAction)
					{
						return;
					}
				}
				m_commandManager.AddCommand(RestCommand.Instance);
			}
		}

		private void OnResetTurnActor(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown)
			{
				m_active = true;
				LegacyLogic.Instance.UpdateManager.ResetCurrentTurnActor();
			}
		}

		private void OnToggleSpellBook(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && OpenSpellbookEvent != null)
			{
				OpenSpellbookEvent(this, EventArgs.Empty);
			}
		}

		private void OnToggleSkills(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && OpenSkillsEvent != null)
			{
				OpenSkillsEvent(this, EventArgs.Empty);
			}
		}

		private void OnToggleJournal(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && OpenJournalEvent != null)
			{
				OpenJournalEvent(this, EventArgs.Empty);
			}
		}

		private void OnToggleBestiary(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && OpenBestiaryEvent != null)
			{
				OpenBestiaryEvent(this, EventArgs.Empty);
			}
		}

		private void OnToggleLore(Object p_sender, HotkeyEventArgs p_args)
		{
			if (p_args.KeyDown && m_active && OpenLoreEvent != null)
			{
				OpenLoreEvent(this, EventArgs.Empty);
			}
		}
	}
}
