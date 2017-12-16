using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public class HirelingNotHiredCondition : DialogCondition
	{
		[XmlAttribute("npcID")]
		public Int32 NPCID { get; set; }

		public override EDialogState CheckCondition(Npc p_npc)
		{
			Npc p_npc2 = LegacyLogic.Instance.WorldManager.NpcFactory.Get(NPCID);
			if (!LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HirelingHired(p_npc2))
			{
				return EDialogState.NORMAL;
			}
			return FailState;
		}
	}
}
