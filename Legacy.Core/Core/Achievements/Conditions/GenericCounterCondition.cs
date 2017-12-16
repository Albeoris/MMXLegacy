using System;

namespace Legacy.Core.Achievements.Conditions
{
	public class GenericCounterCondition : AchievementCondition
	{
		public GenericCounterCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
			m_achievement = p_achievement;
		}

		public override void ParseParameter(String p_parameterString)
		{
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = m_achievement.Count + 1;
			return p_count >= m_count;
		}
	}
}
