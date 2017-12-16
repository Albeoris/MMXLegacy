using System;
using Legacy.Core.Api;

namespace Legacy.Core.Achievements.Conditions
{
	public class NumberOfDaysSinceLastRestingCondition : AchievementCondition
	{
		public NumberOfDaysSinceLastRestingCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
			m_achievement = p_achievement;
		}

		public override void ParseParameter(String p_parameterString)
		{
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = LegacyLogic.Instance.WorldManager.AchievementManager.DaysWithoutResting;
			return p_count >= m_count;
		}
	}
}
