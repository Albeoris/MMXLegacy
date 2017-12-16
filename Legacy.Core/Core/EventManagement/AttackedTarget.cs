using System;
using Legacy.Core.Combat;

namespace Legacy.Core.EventManagement
{
	public class AttackedTarget : SpellTarget
	{
		public readonly AttackResult Result;

		public AttackedTarget(Object p_Target, AttackResult p_Result) : base(p_Target)
		{
			Result = p_Result;
		}

		public override String ToString()
		{
			return String.Concat(new Object[]
			{
				"[AttackedTarget Target=",
				Target,
				" Result=",
				Result,
				"]"
			});
		}
	}
}
