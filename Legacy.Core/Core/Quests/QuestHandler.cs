using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Quests
{
	public class QuestHandler : ISaveGameObject
	{
		private List<QuestStep> m_quests;

		internal QuestHandler()
		{
			m_quests = new List<QuestStep>();
		}

		internal void Initialize()
		{
			Cleanup();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnGameTimeChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
		}

		internal void Cleanup()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MONSTER_DIED, new EventHandler(OnMonsterDied));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.GAMETIME_TIME_CHANGED, new EventHandler(OnGameTimeChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
		}

		public void ClearQuests()
		{
			LoadDefaultQuestSteps();
		}

		internal void LoadDefaultQuestSteps()
		{
			m_quests.Clear();
			IEnumerable<QuestStepStaticData> iterator = StaticDataHandler.GetIterator<QuestStepStaticData>(EDataType.QUEST_STEPS);
			foreach (QuestStepStaticData p_data in iterator)
			{
				QuestStep item = new QuestStep(p_data);
				m_quests.Add(item);
			}
		}

		public void ActivateQuest(Int32 p_id)
		{
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.StaticData.StaticID == p_id)
				{
					questStep.Activate();
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.NEW_QUEST, questStep));
					if (p_id != 1)
					{
						if (p_id != 5)
						{
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.NEW_QUEST);
						}
						else
						{
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.STORY_3);
						}
					}
					else
					{
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.STORY_1);
					}
					if (questStep.CheckFinished())
					{
						FinalizeStep(questStep);
					}
				}
			}
		}

		public List<QuestStep> GetStepsByState(EQuestState p_state)
		{
			List<QuestStep> list = new List<QuestStep>();
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.QuestState == p_state)
				{
					list.Add(questStep);
				}
			}
			return list;
		}

		public List<QuestStep> GetActiveStepsByCategory(EQuestType p_type)
		{
			List<QuestStep> list = new List<QuestStep>();
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.StaticData.Type == p_type && questStep.QuestState == EQuestState.ACTIVE)
				{
					list.Add(questStep);
				}
			}
			return list;
		}

		public QuestStep GetStep(Int32 p_id)
		{
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.StaticData.StaticID == p_id)
				{
					return questStep;
				}
			}
			return null;
		}

		private void OnMonsterDied(Object sender, EventArgs p_args)
		{
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.QuestState == EQuestState.ACTIVE && questStep.MonsterDied((Monster)sender))
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE, questStep));
					if (questStep.CheckFinished())
					{
						FinalizeStep(questStep);
					}
				}
			}
		}

		private void OnGameTimeChanged(Object p_sender, EventArgs p_args)
		{
			MMTime time = LegacyLogic.Instance.GameTime.Time;
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.QuestState == EQuestState.ACTIVE && questStep.CheckTime(time))
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE, questStep));
					if (questStep.CheckFinished())
					{
						FinalizeStep(questStep);
					}
				}
			}
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			if (p_sender is Party)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				ETerrainType terrainType = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position).TerrainType;
				foreach (QuestStep questStep in m_quests)
				{
					if (questStep.QuestState == EQuestState.ACTIVE && questStep.PartyMoved(terrainType))
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE, questStep));
						if (questStep.CheckFinished())
						{
							FinalizeStep(questStep);
						}
					}
				}
			}
		}

		internal void StartDialog(Object sender, Int32 p_questID)
		{
			QuestStep step = GetStep(p_questID);
			if (step != null && step.QuestState == EQuestState.ACTIVE && step.StartDialog((Npc)sender))
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE, step));
				if (step.CheckFinished())
				{
					FinalizeStep(step);
				}
			}
		}

		private void OnTokenAdded(Object sender, EventArgs p_args)
		{
			Int32 tokenID = ((TokenEventArgs)p_args).TokenID;
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.QuestState == EQuestState.ACTIVE && questStep.TokenAdded(tokenID))
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE, questStep));
					if (questStep.CheckFinished())
					{
						FinalizeStep(questStep);
					}
				}
			}
		}

		internal void ObjectInteraction(Int32 p_objectivID)
		{
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.ObjectInteraction(p_objectivID) && questStep.QuestState == EQuestState.ACTIVE)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE, questStep));
					if (questStep.CheckFinished())
					{
						FinalizeStep(questStep);
					}
				}
			}
		}

		internal void ObjectInteraction(InteractiveObject obj)
		{
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.QuestState == EQuestState.ACTIVE && questStep.ObjectInteraction(obj))
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_OBJECTIVE, questStep));
					if (questStep.CheckFinished())
					{
						FinalizeStep(questStep);
					}
				}
			}
		}

		internal void FinalizeStep(QuestStep p_step)
		{
			LootHandler lootHandler = new LootHandler(p_step);
			lootHandler.DistributeRewards(null);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.COMPLETED_QUEST, p_step));
			if (p_step.StaticData.FollowupStep > 0)
			{
				ActivateQuest(p_step.StaticData.FollowupStep);
				LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.IMPLICIT_QUEST_END);
			}
			else
			{
				LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.FINAL_QUEST);
			}
			LegacyLogic.Instance.TrackingManager.TrackQuestStepCompleted(p_step.StaticData.StaticID);
		}

		public Boolean HasNPCInactiveQuest(Int32 p_npcID)
		{
			foreach (QuestStep questStep in m_quests)
			{
				if (questStep.QuestState == EQuestState.INACTIVE && questStep.StaticData.GivenByNPCID == p_npcID)
				{
					return true;
				}
			}
			return false;
		}

		public void Load(SaveGameData p_data)
		{
			m_quests.Clear();
			Int32 num = p_data.Get<Int32>("Count", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("QuestStep" + i, null);
				if (saveGameData != null)
				{
					QuestStep questStep = new QuestStep();
					questStep.Load(saveGameData);
					if (questStep.StaticData != null)
					{
						m_quests.Add(questStep);
					}
				}
			}
			AddDLCQuest(109);
			AddDLCQuest(111);
			AddDLCQuest(112);
			AddDLCQuest(114);
			AddDLCQuest(115);
			AddDLCQuest(116);
			AddDLCQuest(117);
			AddDLCQuest(118);
			AddDLCQuest(119);
			AddDLCQuest(120);
			if (GetStep(110) == null)
			{
				QuestStepStaticData staticData = StaticDataHandler.GetStaticData<QuestStepStaticData>(EDataType.QUEST_STEPS, 110);
				QuestStep questStep2 = new QuestStep(staticData);
				QuestStep step = GetStep(3);
				if (step.QuestState == EQuestState.ACTIVE)
				{
					if (step.GetObjective(4).QuestState == EQuestState.SOLVED)
					{
						step.QuestState = EQuestState.SOLVED;
						questStep2.QuestState = EQuestState.ACTIVE;
					}
				}
				else
				{
					questStep2.QuestState = step.QuestState;
				}
				questStep2.GetObjective(5).QuestState = step.GetObjective(5).QuestState;
				questStep2.GetObjective(6).QuestState = step.GetObjective(6).QuestState;
				questStep2.GetObjective(7).QuestState = step.GetObjective(7).QuestState;
				step.RemoveObjective(5);
				step.RemoveObjective(6);
				step.RemoveObjective(7);
				step.StaticData.UpdateFollowupStep(110);
				m_quests.Add(questStep2);
			}
			QuestStep step2 = GetStep(100);
			if (step2 != null)
			{
				QuestObjectiveStaticData staticData2 = StaticDataHandler.GetStaticData<QuestObjectiveStaticData>(EDataType.QUEST_OBJECTIVES, 167);
				QuestObjective objective = step2.GetObjective(167);
				objective.StaticData.SetMainObjective(staticData2.IsMainObjective);
				objective.StaticData.SetNpcID(staticData2.NpcID);
				objective.StaticData.SetTokenID(staticData2.TokenID);
			}
			step2 = GetStep(63);
			if (step2.QuestState == EQuestState.ACTIVE)
			{
				step2.Repair();
				if (step2.CheckFinished())
				{
					FinalizeStep(step2);
				}
			}
		}

		private void AddDLCQuest(Int32 p_step)
		{
			if (GetStep(p_step) == null)
			{
				QuestStepStaticData staticData = StaticDataHandler.GetStaticData<QuestStepStaticData>(EDataType.QUEST_STEPS, p_step);
				QuestStep item = new QuestStep(staticData);
				m_quests.Add(item);
			}
		}

		public void RemoveUnnecessaryTokens()
		{
			QuestStep step = GetStep(32);
			if (step.QuestState == EQuestState.SOLVED)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.RemoveSet(11);
			}
			step = GetStep(64);
			if (step.QuestState == EQuestState.SOLVED)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.RemoveSet(1);
			}
		}

		public void RepairLorebookQuest()
		{
			QuestStep step = GetStep(91);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				step.Repair();
				if (step.CheckFinished())
				{
					FinalizeStep(step);
				}
			}
		}

		public void RepairObeliskQuest()
		{
			QuestStep step = GetStep(83);
			if (step.QuestState == EQuestState.INACTIVE)
			{
				return;
			}
			step = GetStep(90);
			if (step.QuestState != EQuestState.INACTIVE)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(790);
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(791);
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(792);
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(793);
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(794);
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(795);
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(796);
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(797);
				return;
			}
			Int32 num = 0;
			step = GetStep(83);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				num++;
			}
			step = GetStep(84);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				num++;
			}
			step = GetStep(85);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				num++;
			}
			step = GetStep(86);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				num++;
			}
			step = GetStep(87);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				num++;
			}
			step = GetStep(88);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				num++;
			}
			step = GetStep(89);
			if (step.QuestState == EQuestState.ACTIVE)
			{
				num++;
			}
			MapData mapData = LegacyLogic.Instance.WorldManager.MapData["theworld"];
			List<ObelisksStaticData> list = new List<ObelisksStaticData>(StaticDataHandler.GetIterator<ObelisksStaticData>(EDataType.OBELISKS));
			foreach (ObelisksStaticData obelisksStaticData in list)
			{
				if (num > 0)
				{
					if (mapData.GetTerrainData(obelisksStaticData.Position).Visited)
					{
						LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddTokenIfNull(obelisksStaticData.TokenID);
					}
					num--;
				}
			}
		}

		public void FixBattleOfKarthal()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			QuestStep step = GetStep(18);
			if (step.QuestState == EQuestState.ACTIVE && party.TokenHandler.GetTokens(699) > 0)
			{
				step.SolveAllObjectives();
				if (step.CheckFinished())
				{
					LegacyLogic.Instance.WorldManager.QuestHandler.FinalizeStep(step);
				}
				step = GetStep(31);
				step.SolveAllObjectives();
				if (step.CheckFinished())
				{
					LegacyLogic.Instance.WorldManager.QuestHandler.FinalizeStep(step);
				}
				party.TokenHandler.AddToken(760);
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("Count", m_quests.Count);
			for (Int32 i = 0; i < m_quests.Count; i++)
			{
				SaveGameData saveGameData = new SaveGameData("QuestStep" + i);
				if (m_quests[i].StaticData != null)
				{
					m_quests[i].Save(saveGameData);
					p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
				}
			}
		}
	}
}
