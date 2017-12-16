using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Quests
{
	public class QuestObjective : ISaveGameObject
	{
		private QuestObjectiveStaticData m_staticData;

		private Int32 m_currentCounter;

		private Int32 m_maxCounter;

		private EQuestState m_questState;

		private MMTime m_activationTime;

		private Int32 m_currentStepsOnTerrain;

		public QuestObjective()
		{
		}

		public QuestObjective(Int32 staticID, Int32 count)
		{
			m_staticData = StaticDataHandler.GetStaticData<QuestObjectiveStaticData>(EDataType.QUEST_OBJECTIVES, staticID);
			m_currentCounter = 0;
			m_maxCounter = count;
			m_questState = EQuestState.INACTIVE;
			m_activationTime = default(MMTime);
			m_currentStepsOnTerrain = 0;
		}

		public QuestObjectiveStaticData StaticData => m_staticData;

	    public EQuestState QuestState
		{
			get => m_questState;
	        set => m_questState = value;
	    }

		public Int32 CurrentCounter => m_currentCounter;

	    public void SetCurrentCounter(Int32 amount)
		{
			m_currentCounter = amount;
		}

		public Int32 MaxCounter => m_maxCounter;

	    public Boolean IsMainObjective => m_staticData.IsMainObjective;

	    public void Activate()
		{
			m_questState = EQuestState.ACTIVE;
			m_activationTime = LegacyLogic.Instance.GameTime.Time;
		}

		public void Deactivate()
		{
			m_questState = EQuestState.INACTIVE;
		}

		public void Solve()
		{
			m_questState = EQuestState.SOLVED;
		}

		public Boolean MonsterDied(Monster sender)
		{
			Boolean flag = false;
			if (m_questState != EQuestState.ACTIVE)
			{
				return flag;
			}
			if (m_staticData.MonsterClass != EMonsterClass.NONE)
			{
				flag = (m_staticData.MonsterClass == sender.StaticData.Class);
			}
			else if (m_staticData.MonsterType != EMonsterType.NONE)
			{
				flag = (m_staticData.MonsterType == sender.StaticData.Type);
			}
			else if (m_staticData.MonsterStaticID > 0)
			{
				flag = (m_staticData.MonsterStaticID == sender.StaticID);
			}
			if (flag)
			{
				UpdateCounter();
			}
			return flag;
		}

		public Boolean CheckTime(MMTime p_time)
		{
			Boolean flag = false;
			if (m_questState != EQuestState.ACTIVE)
			{
				return flag;
			}
			if (m_staticData.DaysToPass > 0 && (p_time - m_activationTime).TotalDays >= m_staticData.DaysToPass)
			{
				flag = true;
			}
			if (flag)
			{
				UpdateCounter();
			}
			return flag;
		}

		public Boolean PartyMoved(ETerrainType p_type)
		{
			Boolean flag = false;
			if (m_staticData.StepsOnTerrain.NumberOfSteps == 0 || m_questState != EQuestState.ACTIVE)
			{
				return flag;
			}
			if (p_type == m_staticData.StepsOnTerrain.TerrainType)
			{
				m_currentStepsOnTerrain++;
				if (m_currentStepsOnTerrain >= m_staticData.StepsOnTerrain.NumberOfSteps)
				{
					flag = true;
				}
			}
			if (flag)
			{
				UpdateCounter();
			}
			return flag;
		}

		public Boolean StartDialog(Npc sender)
		{
			Boolean flag = false;
			if (sender.StaticID == m_staticData.NpcID)
			{
				flag = true;
			}
			if (flag)
			{
				UpdateCounter();
			}
			return flag;
		}

		public Boolean ObjectInteraction(Int32 p_objectiveID)
		{
			Boolean flag = p_objectiveID == m_staticData.StaticID;
			if (flag)
			{
				UpdateCounter();
			}
			return flag;
		}

		public Boolean ObjectInteraction(InteractiveObject sender)
		{
			Boolean flag = false;
			if (m_questState == EQuestState.SOLVED)
			{
				return flag;
			}
			if (m_questState != EQuestState.ACTIVE)
			{
				foreach (SpawnQuestObjective spawnQuestObjective in sender.QuestObjectives)
				{
					if (spawnQuestObjective.ID == StaticData.StaticID)
					{
						flag = true;
					}
				}
				return flag;
			}
			foreach (SpawnQuestObjective spawnQuestObjective2 in sender.QuestObjectives)
			{
				if (spawnQuestObjective2.ID == StaticData.StaticID)
				{
					flag = true;
				}
			}
			if (flag)
			{
				UpdateCounter();
			}
			return flag;
		}

		public Boolean TokenAdded(Int32 tokenID)
		{
			Boolean flag = false;
			if (m_questState != EQuestState.ACTIVE)
			{
				return flag;
			}
			if (m_staticData.TokenID > 0)
			{
				flag = (m_staticData.TokenID == tokenID);
			}
			if (flag)
			{
				UpdateCounter();
			}
			return flag;
		}

		private void UpdateCounter()
		{
			m_currentCounter++;
			if (m_currentCounter >= m_maxCounter)
			{
				m_questState = EQuestState.SOLVED;
			}
		}

		public void Load(SaveGameData p_data)
		{
			Int32 p_staticId = p_data.Get<Int32>("StaticID", 1);
			m_staticData = StaticDataHandler.GetStaticData<QuestObjectiveStaticData>(EDataType.QUEST_OBJECTIVES, p_staticId);
			m_currentCounter = p_data.Get<Int32>("CurrentCounter", 0);
			m_maxCounter = p_data.Get<Int32>("MaxCounter", 0);
			m_questState = (EQuestState)p_data.Get<Int32>("QuestState", 0);
			m_currentStepsOnTerrain = p_data.Get<Int32>("CurrentStepsOnTerrain", 0);
			SaveGameData saveGameData = p_data.Get<SaveGameData>("ActivationTime", null);
			if (saveGameData != null)
			{
				m_activationTime.Load(saveGameData);
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", m_staticData.StaticID);
			p_data.Set<Int32>("CurrentCounter", m_currentCounter);
			p_data.Set<Int32>("MaxCounter", m_maxCounter);
			p_data.Set<Int32>("QuestState", (Int32)m_questState);
			p_data.Set<Int32>("CurrentStepsOnTerrain", m_currentStepsOnTerrain);
			SaveGameData saveGameData = new SaveGameData("ActivationTime");
			m_activationTime.Save(saveGameData);
			p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
		}

		public override String ToString()
		{
			return String.Format("[QuestObjective: StaticData={0}, QuestState={1}, CurrentCounter={2}, MaxCounter={3}, IsMainObjective={4}]", new Object[]
			{
				StaticData.ToString(),
				QuestState,
				CurrentCounter,
				MaxCounter,
				IsMainObjective
			});
		}
	}
}
