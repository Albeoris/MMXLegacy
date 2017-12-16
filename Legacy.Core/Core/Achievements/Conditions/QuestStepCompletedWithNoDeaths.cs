using System;
using Legacy.Core.Api;
using Legacy.Core.Quests;

namespace Legacy.Core.Achievements.Conditions
{
	public class QuestStepCompletedWithNoDeaths : AchievementCondition
	{
		private Int32 m_questID;

		public QuestStepCompletedWithNoDeaths(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		internal Int32 QuestID => m_questID;

	    public override void ParseParameter(String p_parameterString)
		{
			Int32.TryParse(p_parameterString, out m_questID);
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = 0;
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_questID);
			return step != null && step.QuestState == EQuestState.SOLVED && !LegacyLogic.Instance.WorldManager.AchievementManager.CharacterDied;
		}
	}
}
