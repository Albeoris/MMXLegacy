using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Achievements.Conditions
{
	public class AllCharactersOnLevelCondition : AchievementCondition
	{
		private Int32 m_levelGoal;

		public AllCharactersOnLevelCondition(Achievement p_achievement, Int32 p_count, String p_conditionParameter) : base(p_achievement, p_count, p_conditionParameter)
		{
		}

		internal Int32 LevelGoal => m_levelGoal;

	    public override void ParseParameter(String p_parameterString)
		{
			Int32.TryParse(p_parameterString, out m_levelGoal);
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = 0;
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i].Level >= m_levelGoal)
				{
					p_count++;
				}
			}
			return p_count >= members.Length;
		}
	}
}
