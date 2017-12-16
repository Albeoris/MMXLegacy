using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;

namespace Legacy.Core.Achievements.Conditions
{
	public class QuestStepCompletedWithClassIDsCondition : AchievementCondition
	{
		private Int32 m_questID;

		private Int32[] m_classIDs;

		public QuestStepCompletedWithClassIDsCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		internal Int32[] ClassIDs => m_classIDs;

	    public Int32 QuestID => m_questID;

	    public override void ParseParameter(String p_parameterString)
		{
			String[] array = p_parameterString.Split(new Char[]
			{
				','
			});
			Int32.TryParse(array[0], out m_questID);
			m_classIDs = new Int32[4];
			for (Int32 i = 0; i < 4; i++)
			{
				Int32.TryParse(array[i + 1], out m_classIDs[i]);
			}
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = 0;
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_questID);
			if (step != null && step.QuestState == EQuestState.SOLVED && CheckClasses())
			{
				p_count = 1;
				return true;
			}
			return false;
		}

		private Boolean CheckClasses()
		{
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i].Class.Class != (EClass)m_classIDs[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
