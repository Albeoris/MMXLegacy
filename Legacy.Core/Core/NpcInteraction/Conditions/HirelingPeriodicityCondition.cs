using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class HirelingPeriodicityCondition : DialogCondition
	{
		[XmlAttribute("npcID")]
		public Int32 NPCID { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Npc p_npc2 = LegacyLogic.Instance.WorldManager.NpcFactory.Get(NPCID);
			if (LegacyLogic.Instance.WorldManager.Party.HirelingHandler.AllowedPeriodicity(p_npc2, ConditionTarget))
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
