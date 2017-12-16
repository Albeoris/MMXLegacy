using System;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Achievements.Conditions
{
	public class AllMembersUnlockedAdvancedClassCondition : AchievementCondition
	{
		public AllMembersUnlockedAdvancedClassCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		public override void ParseParameter(String p_parameterString)
		{
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = 0;
			Character[] members = LegacyLogic.Instance.WorldManager.Party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i].UnlockedAdvancedClass)
				{
					p_count++;
				}
			}
			return p_count == 4;
		}
	}
}
