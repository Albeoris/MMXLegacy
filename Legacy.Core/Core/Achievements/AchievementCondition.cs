using System;

namespace Legacy.Core.Achievements
{
	public abstract class AchievementCondition
	{
		protected Int32 m_count;

		protected Achievement m_achievement;

		public AchievementCondition(Achievement p_achievement, Int32 p_count, String p_parameterString)
		{
			m_achievement = p_achievement;
			ParseParameter(p_parameterString);
			m_count = p_count;
		}

		public abstract void ParseParameter(String p_parameterString);

		public abstract Boolean CheckCondition(out Int32 p_count);

		public virtual void ResetRoundData()
		{
		}
	}
}
