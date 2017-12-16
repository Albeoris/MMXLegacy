using System;
using System.Xml.Serialization;

namespace Legacy.Core.NpcInteraction.Conditions
{
	public abstract class DialogCondition
	{
		[XmlAttribute("conditionTarget")]
		public ETargetCondition ConditionTarget { get; set; }

		[XmlAttribute("failState")]
		public EDialogState FailState { get; set; }

		public abstract EDialogState CheckCondition(Npc p_npc);
	}
}
