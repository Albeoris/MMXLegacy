using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class RewardUnlockedCondition : DialogCondition
	{
		[XmlAttribute("rewardID")]
		public Int32 RewardID { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			if (LegacyLogic.Instance.ServiceWrapper.IsRewardAvailable(RewardID))
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
