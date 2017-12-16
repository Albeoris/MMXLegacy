using System;
using System.IO;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class HirelingFunction : DialogFunction
	{
		private Int32 m_dialogID = -1;

		private Int32 m_npcID = -1;

		private Int32 m_price;

		private Single m_sharePrice;

		private String m_mapName;

		private Int32 m_targetSpawnId;

		private Int32 m_questID;

		public HirelingFunction()
		{
		}

		public HirelingFunction(ETargetCondition p_condition, Int32 p_dialogID)
		{
			ConditionTarget = p_condition;
			m_dialogID = p_dialogID;
		}

		public HirelingFunction(ETargetCondition p_condition, Int32 p_dialogID, Int32 p_price, Single p_sharePrice) : this(p_condition, p_dialogID)
		{
			m_price = p_price;
			m_sharePrice = p_sharePrice;
		}

		[XmlAttribute("conditionTarget")]
		public ETargetCondition ConditionTarget { get; set; }

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("npcID")]
		public Int32 NpcID
		{
			get => m_npcID;
		    set => m_npcID = value;
		}

		[XmlAttribute("mapName")]
		public String MapName
		{
			get => m_mapName;
		    set => m_mapName = value;
		}

		[XmlAttribute("targetSpawnerId")]
		public Int32 TargetSpawnerId
		{
			get => m_targetSpawnId;
		    set => m_targetSpawnId = value;
		}

		[XmlAttribute("questID")]
		public Int32 QuestID
		{
			get => m_questID;
		    set => m_questID = value;
		}

		public Int32 Price => m_price;

	    public override Boolean RequireGold => m_price > 0;

	    public override void Trigger(ConversationManager p_manager)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			ETargetCondition conditionTarget = ConditionTarget;
			switch (conditionTarget)
			{
			case ETargetCondition.HIRE_REVIVE:
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Revive();
				party.ChangeGold(-p_manager.CurrentNpc.GetCosts(ETargetCondition.HIRE_REVIVE));
				p_manager.CurrentNpc.SetDayBreakCount(ETargetCondition.HIRE_REVIVE);
				GoTo(p_manager);
				break;
			case ETargetCondition.HIRE_RESTORE:
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Restore();
				party.ChangeGold(-p_manager.CurrentNpc.GetCosts(ETargetCondition.HIRE_RESTORE));
				p_manager.CurrentNpc.SetDayBreakCount(ETargetCondition.HIRE_RESTORE);
				GoTo(p_manager);
				break;
			default:
				switch (conditionTarget)
				{
				case ETargetCondition.HIRE_IDENTIFY:
					LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Identify();
					GoTo(p_manager);
					break;
				case ETargetCondition.HIRE_CURE:
					LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Cure();
					GoTo(p_manager);
					break;
				case ETargetCondition.HIRE_REPAIR:
					foreach (Character character in party.Members)
					{
						if (character != null && character.Equipment.Equipment.HasBrokenItems() && character.Equipment.Equipment.RepairAllItems())
						{
							character.CalculateCurrentAttributes();
						}
					}
					if (party.Inventory.Inventory.HasBrokenItems() || party.MuleInventory.Inventory.HasBrokenItems())
					{
						party.Inventory.Inventory.RepairAllItems();
						party.MuleInventory.Inventory.RepairAllItems();
					}
					GoTo(p_manager);
					break;
				default:
					if (conditionTarget == ETargetCondition.HIRE)
					{
						Npc p_npc = (m_npcID != 0) ? LegacyLogic.Instance.WorldManager.NpcFactory.Get(m_npcID) : p_manager.CurrentNpc;
						LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Hire(p_npc);
						LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_price);
						if (m_dialogID > 0)
						{
							p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
						}
						else if (m_dialogID == 0)
						{
							p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, p_manager.CurrentConversation.RootDialog.ID);
						}
						else
						{
							p_manager.CloseNpcContainer(null);
						}
					}
					break;
				}
				break;
			case ETargetCondition.HIRE_DEFBUFF:
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.DefBuff();
				p_manager.CurrentNpc.SetDayBreakCount(ETargetCondition.HIRE_DEFBUFF);
				GoTo(p_manager);
				break;
			case ETargetCondition.FIRE:
				if (m_npcID == -1)
				{
					LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(p_manager.CurrentNpc);
				}
				else
				{
					LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(m_npcID);
					if (m_npcID == 16)
					{
						LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddToken(500);
					}
				}
				if (m_dialogID == -1)
				{
					p_manager.CloseNpcContainer(null);
				}
				else
				{
					p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
				}
				break;
			case ETargetCondition.HIRE_AND_CHANGE_MAP:
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.StartDialog(p_manager.CurrentNpc, m_questID);
				Npc p_npc2 = (m_npcID != 0) ? LegacyLogic.Instance.WorldManager.NpcFactory.Get(m_npcID) : p_manager.CurrentNpc;
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Hire(p_npc2);
				p_manager.CloseNpcContainer(null);
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
				LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
				LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.AUTO;
				LegacyLogic.Instance.WorldManager.SaveGameName = Localization.Instance.GetText("SAVEGAMETYPE_AUTO");
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_STARTED_SAVE, EventArgs.Empty);
				Party party2 = LegacyLogic.Instance.WorldManager.Party;
				party2.SelectedInteractiveObject = null;
				break;
			}
			case ETargetCondition.FIRE_AND_GIVE_QUEST:
				if (m_questID > 0)
				{
					LegacyLogic.Instance.WorldManager.QuestHandler.ActivateQuest(m_questID);
				}
				if (m_npcID == -1)
				{
					LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(p_manager.CurrentNpc);
				}
				else
				{
					LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(m_npcID);
					if (m_npcID == 16)
					{
						LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddToken(500);
					}
				}
				if (m_dialogID == -1)
				{
					p_manager.CloseNpcContainer(null);
				}
				else
				{
					p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
				}
				break;
			}
		}

		private void GoTo(ConversationManager p_manager)
		{
			if (m_dialogID == -1)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, p_manager.CurrentConversation.RootDialog.ID);
			}
			else
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
			}
		}

		private void OnGameSaved(Object sender, EventArgs e)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
			LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_price);
			LegacyLogic.Instance.WorldManager.Party.StartSpawnerID = m_targetSpawnId;
			LegacyLogic.Instance.MapLoader.LoadMap(Path.ChangeExtension(m_mapName, ".xml"));
		}
	}
}
