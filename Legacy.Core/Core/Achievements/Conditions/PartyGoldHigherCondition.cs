using System;
using Legacy.Core.Api;

namespace Legacy.Core.Achievements.Conditions
{
	public class PartyGoldHigherCondition : AchievementCondition
	{
		private Int32 m_goldGoal;

		public PartyGoldHigherCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		internal Int32 GoldGoal => m_goldGoal;

	    public override void ParseParameter(String p_parameterString)
		{
			Int32.TryParse(p_parameterString, out m_goldGoal);
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = 0;
			return LegacyLogic.Instance.WorldManager.Party.Gold >= m_goldGoal;
		}
	}
}
