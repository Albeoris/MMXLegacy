using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Quests
{
	public class QuestStep : ISaveGameObject, IComparable
	{
		private QuestStepStaticData m_staticData;

		private List<QuestObjective> m_objectives;

		private EQuestState m_questState;

		public QuestStep()
		{
			m_objectives = new List<QuestObjective>();
		}

		public QuestStep(QuestStepStaticData p_data) : this()
		{
			if (p_data == null)
			{
				throw new ArgumentNullException("p_data");
			}
			m_staticData = p_data;
			m_questState = EQuestState.INACTIVE;
			String[] array = m_staticData.Objectives.Split(new Char[]
			{
				','
			});
			for (Int32 i = 0; i < array.Length; i += 2)
			{
				QuestObjective item = new QuestObjective(Convert.ToInt32(array[i]), Convert.ToInt32(array[i + 1]));
				m_objectives.Add(item);
			}
		}

		public EQuestState QuestState
		{
			get => m_questState;
		    set => m_questState = value;
		}

		public QuestStepStaticData StaticData => m_staticData;

	    public List<QuestObjective> Objectives => m_objectives;

	    public void Activate()
		{
			m_questState = EQuestState.ACTIVE;
			foreach (QuestObjective questObjective in m_objectives)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				if (questObjective.StaticData.TokenID > 0 && party.TokenHandler.GetTokens(questObjective.StaticData.TokenID) >= questObjective.MaxCounter)
				{
					questObjective.Solve();
					CheckForReturn();
				}
				else
				{
					if (questObjective.StaticData.TokenID > 0 && party.TokenHandler.GetTokens(questObjective.StaticData.TokenID) > 0)
					{
						questObjective.SetCurrentCounter(party.TokenHandler.GetTokens(questObjective.StaticData.TokenID));
					}
					if (questObjective.StaticData.MonsterStaticID > 0)
					{
						Int32 num = 0;
						if (LegacyLogic.Instance.WorldManager.BestiaryHandler.KilledMonsters.TryGetValue(questObjective.StaticData.MonsterStaticID, out num) && num > 0)
						{
							questObjective.Solve();
							CheckForReturn();
							continue;
						}
					}
					if (questObjective.StaticData.MonsterType != EMonsterType.NONE)
					{
						Int32 killedMonstersOfType = LegacyLogic.Instance.WorldManager.BestiaryHandler.GetKilledMonstersOfType(questObjective.StaticData.MonsterType);
						questObjective.SetCurrentCounter(killedMonstersOfType);
						if (questObjective.CurrentCounter == questObjective.MaxCounter)
						{
							questObjective.Solve();
							CheckForReturn();
							continue;
						}
					}
					if (!RepairCollectionObjectives(questObjective))
					{
						if (!questObjective.StaticData.IsReturn)
						{
							questObjective.Activate();
						}
					}
				}
			}
		}

		public void SetState(EQuestState p_state)
		{
			m_questState = p_state;
		}

		public void Deactivate()
		{
			m_questState = EQuestState.INACTIVE;
			foreach (QuestObjective questObjective in m_objectives)
			{
				questObjective.Deactivate();
			}
		}

		public QuestObjective GetObjective(Int32 p_id)
		{
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.StaticData.StaticID == p_id)
				{
					return questObjective;
				}
			}
			return null;
		}

		public void Solve()
		{
			m_questState = EQuestState.SOLVED;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (!questObjective.StaticData.IsReturn)
				{
					questObjective.Solve();
				}
			}
		}

		public Boolean MonsterDied(Monster sender)
		{
			Boolean flag = false;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.MonsterDied(sender))
				{
					flag = true;
				}
			}
			if (flag)
			{
				CheckForReturn();
			}
			return flag;
		}

		public Boolean CheckTime(MMTime p_time)
		{
			Boolean flag = false;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.CheckTime(p_time))
				{
					flag = true;
				}
			}
			if (flag)
			{
				CheckForReturn();
			}
			return flag;
		}

		public Boolean PartyMoved(ETerrainType p_type)
		{
			Boolean flag = false;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.PartyMoved(p_type))
				{
					flag = true;
				}
			}
			if (flag)
			{
				CheckForReturn();
			}
			return flag;
		}

		public Boolean StartDialog(Npc sender)
		{
			Boolean flag = false;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.StartDialog(sender))
				{
					flag = true;
				}
			}
			if (flag)
			{
				CheckForReturn();
			}
			return flag;
		}

		public Boolean ObjectInteraction(Int32 p_objectiveID)
		{
			Boolean flag = false;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.ObjectInteraction(p_objectiveID))
				{
					flag = true;
				}
			}
			if (flag)
			{
				CheckForReturn();
			}
			return flag;
		}

		public Boolean ObjectInteraction(InteractiveObject sender)
		{
			Boolean flag = false;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.StaticData.StaticID != 7 || questObjective.QuestState != EQuestState.INACTIVE)
				{
					if (questObjective.ObjectInteraction(sender))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				CheckForReturn();
			}
			return flag;
		}

		public Boolean TokenAdded(Int32 tokenID)
		{
			Boolean flag = false;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.TokenAdded(tokenID))
				{
					flag = true;
				}
			}
			if (flag)
			{
				CheckForReturn();
			}
			return flag;
		}

		public Boolean CheckFinished()
		{
			Boolean flag = true;
			foreach (QuestObjective questObjective in m_objectives)
			{
				flag &= (questObjective.QuestState == EQuestState.SOLVED);
				if (questObjective.QuestState == EQuestState.SOLVED && questObjective.IsMainObjective)
				{
					m_questState = EQuestState.SOLVED;
					SolveAllObjectives();
					return true;
				}
			}
			if (flag)
			{
				m_questState = EQuestState.SOLVED;
			}
			return flag;
		}

		public void SolveAllObjectives()
		{
			for (Int32 i = 0; i < m_objectives.Count; i++)
			{
				m_objectives[i].Solve();
			}
		}

		public List<QuestObjective> GetNonInactiveObjectives()
		{
			List<QuestObjective> list = new List<QuestObjective>();
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.QuestState == EQuestState.ACTIVE || questObjective.QuestState == EQuestState.SOLVED)
				{
					list.Add(questObjective);
				}
			}
			return list;
		}

		private void CheckForReturn()
		{
			Boolean flag = true;
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (!questObjective.StaticData.IsReturn && questObjective.QuestState != EQuestState.SOLVED)
				{
					flag = false;
				}
			}
			if (flag)
			{
				foreach (QuestObjective questObjective2 in m_objectives)
				{
					if (questObjective2.StaticData.IsReturn && questObjective2.QuestState == EQuestState.INACTIVE)
					{
						questObjective2.Activate();
					}
				}
			}
		}

		public void RemoveObjective(Int32 p_id)
		{
			foreach (QuestObjective questObjective in m_objectives)
			{
				if (questObjective.StaticData.StaticID == p_id)
				{
					m_objectives.Remove(questObjective);
					break;
				}
			}
		}

		public void Load(SaveGameData p_data)
		{
			m_objectives.Clear();
			Int32 p_staticId = p_data.Get<Int32>("StaticID", 0);
			m_staticData = StaticDataHandler.GetStaticData<QuestStepStaticData>(EDataType.QUEST_STEPS, p_staticId);
			m_questState = p_data.Get<EQuestState>("QuestState", EQuestState.INACTIVE);
			Int32 num = p_data.Get<Int32>("ObjectiveCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("Objective" + i, null);
				if (saveGameData != null)
				{
					QuestObjective questObjective = new QuestObjective();
					questObjective.Load(saveGameData);
					if (questObjective.StaticData != null)
					{
						m_objectives.Add(questObjective);
					}
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", m_staticData.StaticID);
			p_data.Set<Int32>("QuestState", (Int32)m_questState);
			p_data.Set<Int32>("ObjectiveCount", m_objectives.Count);
			for (Int32 i = 0; i < m_objectives.Count; i++)
			{
				SaveGameData saveGameData = new SaveGameData("Objective" + i);
				if (m_objectives[i].StaticData != null)
				{
					m_objectives[i].Save(saveGameData);
					p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
				}
			}
		}

		public Int32 CompareTo(Object obj)
		{
			QuestStep questStep = obj as QuestStep;
			if (questStep == null)
			{
				return -1;
			}
			return ((Int32)m_staticData.Type).CompareTo((Int32)questStep.m_staticData.Type);
		}

		public void Repair()
		{
			if (m_staticData.StaticID == 63)
			{
				QuestObjective objective = GetObjective(138);
				RepairCollectionObjectives(objective);
			}
			if (m_staticData.StaticID == 91)
			{
				QuestObjective objective = GetObjective(189);
				RepairCollectionObjectives(objective);
			}
		}

		private Boolean RepairCollectionObjectives(QuestObjective qo)
		{
			if (qo.StaticData.StaticID == 189)
			{
				List<LoreBookStaticData> booksForCategory = LegacyLogic.Instance.WorldManager.LoreBookHandler.GetBooksForCategory(ELoreBookCategories.POEM, false);
				Int32 count = booksForCategory.Count;
				qo.SetCurrentCounter(count);
				if (qo.CurrentCounter == qo.MaxCounter)
				{
					qo.Solve();
					CheckForReturn();
					return true;
				}
			}
			if (qo.StaticData.StaticID == 138)
			{
				Int32 num = 0;
				if (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(520) > 0)
				{
					num++;
				}
				if (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(521) > 0)
				{
					num++;
				}
				if (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(522) > 0)
				{
					num++;
				}
				if (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(523) > 0)
				{
					num++;
				}
				qo.SetCurrentCounter(num);
				if (qo.CurrentCounter == qo.MaxCounter)
				{
					qo.Solve();
					CheckForReturn();
					return true;
				}
			}
			return false;
		}
	}
}
