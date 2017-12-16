using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.EventManagement;

namespace Legacy.Core.Achievements.Conditions
{
	public class NumberOfBlocksCondition : AchievementCondition
	{
		private Int32 m_blockCount;

		public NumberOfBlocksCondition(Achievement p_achievement, Int32 p_count, String p_parameterString) : base(p_achievement, p_count, p_parameterString)
		{
		}

		public override void ParseParameter(String p_parameterString)
		{
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			List<AttacksEventArgs.AttackedTarget> attackedTargets = LegacyLogic.Instance.WorldManager.AchievementManager.AttackedTargets;
			for (Int32 i = 0; i < attackedTargets.Count; i++)
			{
				if (attackedTargets[i].AttackResult.Result == EResultType.BLOCK)
				{
					m_blockCount++;
				}
			}
			p_count = m_blockCount;
			return m_blockCount >= m_count;
		}

		public override void ResetRoundData()
		{
			m_blockCount = 0;
		}
	}
}
