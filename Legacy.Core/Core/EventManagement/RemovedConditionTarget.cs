using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class RemovedConditionTarget : SpellTarget
	{
		public readonly ECondition Condition;

		public RemovedConditionTarget(Object p_Target, ECondition p_Condition) : base(p_Target)
		{
			Condition = p_Condition;
		}

		public override String ToString()
		{
			return String.Concat(new Object[]
			{
				"[RemovedConditionTarget Target=",
				Target,
				" Condition=",
				Condition,
				"]"
			});
		}
	}
}
