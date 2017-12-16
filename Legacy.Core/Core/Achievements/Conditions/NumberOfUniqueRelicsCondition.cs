using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.Achievements.Conditions
{
	public class NumberOfUniqueRelicsCondition : AchievementCondition
	{
		public NumberOfUniqueRelicsCondition(Achievement p_achievement, Int32 p_count, String p_conditionParameter) : base(p_achievement, p_count, p_conditionParameter)
		{
		}

		public override void ParseParameter(String p_parameterString)
		{
		}

		public override Boolean CheckCondition(out Int32 p_count)
		{
			p_count = m_achievement.Count;
			if (p_count >= m_count)
			{
				return true;
			}
			BaseItem lastGatheredItem = LegacyLogic.Instance.WorldManager.AchievementManager.LastGatheredItem;
			if (lastGatheredItem is Equipment)
			{
				Equipment equipment = (Equipment)lastGatheredItem;
				if (equipment.IsRelic() && equipment.RelicLevel == 1)
				{
					List<AchievementManager.RelicIdentity> relicIDs = LegacyLogic.Instance.WorldManager.AchievementManager.RelicIDs;
					foreach (AchievementManager.RelicIdentity relicIdentity in relicIDs)
					{
						if (relicIdentity.dataType == equipment.GetItemType() && relicIdentity.staticId == equipment.StaticId)
						{
							return false;
						}
					}
					AchievementManager.RelicIdentity item = default(AchievementManager.RelicIdentity);
					item.dataType = equipment.GetItemType();
					item.staticId = equipment.StaticId;
					LegacyLogic.Instance.WorldManager.AchievementManager.RelicIDs.Add(item);
					p_count = m_achievement.Count + 1;
					return p_count >= m_count;
				}
			}
			return false;
		}
	}
}
