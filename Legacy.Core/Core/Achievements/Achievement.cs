using System;
using Legacy.Core.Achievements.Conditions;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Achievements
{
	public class Achievement : ISaveGameObject
	{
		private AchievementManager m_manager;

		private AchievementStaticData m_staticData;

		private AchievementCondition m_condition;

		private Single m_progress;

		private Int32 m_count;

		private Boolean m_aquired;

		public Achievement(AchievementManager p_manager, Int32 p_staticID)
		{
			m_staticData = StaticDataHandler.GetStaticData<AchievementStaticData>(EDataType.ACHIEVEMENT, p_staticID);
			m_manager = p_manager;
			CreateCondition(m_staticData.ConditionID);
		}

		public AchievementManager Manager => m_manager;

	    public Boolean Aquired
		{
			get => m_aquired;
	        set => m_aquired = value;
	    }

		public String NameKey => m_staticData.NameKey;

	    public Int32 Count
		{
			get => m_count;
	        set => m_count = value;
	    }

		public Int32 StaticID => m_staticData.StaticID;

	    public Boolean IsGlobal => m_staticData.Global;

	    public Single Progress => m_progress;

	    public ETriggerType MainTrigger => m_staticData.TriggerType[0];

	    public AchievementCondition Condition => m_condition;

	    internal void CreateCondition(EAchievementConditionType p_conditionType)
		{
			switch (p_conditionType)
			{
			case EAchievementConditionType.QUEST_COMPLETED:
				m_condition = new QuestStepCompleteCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.RELICS_AMOUNT_EQUIPPED:
				m_condition = new RelicOnBothItemSlotsCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.ALL_CHARACTERS_ON_LEVEL:
				m_condition = new AllCharactersOnLevelCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.PARTY_GOLD_AMOUNT_HIGHER_THAN_GOLD:
				m_condition = new PartyGoldHigherCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.INGAME_DAYS_PASSED:
				m_condition = new GenericCounterCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.ALL_MEMBERS_UNLOCKED_ADVANCED_CLASS:
				m_condition = new AllMembersUnlockedAdvancedClassCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.NUMBER_OF_LEARNED_SPELLS:
				m_condition = new GenericCounterCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.NUMBER_OF_UNIQUE_RELICS:
				m_condition = new NumberOfUniqueRelicsCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.NUMBER_OF_ENTERED_TILES:
				m_condition = new GenericCounterCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.NUMBER_OF_REVIVINGS:
				m_condition = new GenericCounterCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.NUMBER_OF_INGAME_DAYS_SINCE_RESTING:
				m_condition = new NumberOfDaysSinceLastRestingCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.NUMBER_OF_BLOCKS:
				m_condition = new NumberOfBlocksCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.NUMBER_OF_DEFEATED_MONSTERS:
				m_condition = new GenericCounterCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.QUEST_COMPLETED_WITH_EXPLICIT_CLASS_IDS:
				m_condition = new QuestStepCompletedWithClassIDsCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.QUEST_COMPLETED_WITH_NO_DEATHS:
				m_condition = new QuestStepCompletedWithNoDeaths(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.QUEST_COMPLETED_WITHOUT_CASTING_SPELL:
				m_condition = new QuestStepCompletedWithoutCastingSpell(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			case EAchievementConditionType.QUEST_COMPLETED_WITH_PARTY_OF_CLASSTYPE:
				m_condition = new QuestStepCompletedWithClassTypeCondition(this, m_staticData.Count, m_staticData.ConditionParameter);
				break;
			}
		}

		public Boolean HasTrigger(ETriggerType p_trigger)
		{
			for (Int32 i = 0; i < m_staticData.TriggerType.Length; i++)
			{
				if (p_trigger == m_staticData.TriggerType[i])
				{
					return true;
				}
			}
			return false;
		}

		public void CheckCondition()
		{
			if (m_condition.CheckCondition(out m_count))
			{
				m_aquired = true;
				m_progress = 1f;
			}
			if (m_staticData.Count != 0)
			{
				m_progress = m_count / m_staticData.Count;
			}
		}

		public void Load(SaveGameData p_data)
		{
			m_aquired = p_data.Get<Boolean>("aquired", false);
			m_count = p_data.Get<Int32>("count", 0);
			m_progress = ((m_staticData.Count == 0) ? 0f : m_count / m_staticData.Count);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", m_staticData.StaticID);
			p_data.Set<Boolean>("aquired", m_aquired);
			p_data.Set<Int32>("count", m_count);
		}
	}
}
