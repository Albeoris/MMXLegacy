using System;

namespace Legacy.Core.EventManagement
{
	public class HealedTarget : SpellTarget
	{
		public readonly Int32 Value;

		public readonly Boolean IsCritical;

		public HealedTarget(Object p_Target, Int32 p_Value, Boolean p_IsCritical) : base(p_Target)
		{
			Value = p_Value;
			IsCritical = p_IsCritical;
		}

		public override String ToString()
		{
			return String.Concat(new Object[]
			{
				"[HealedTarget Target=",
				Target,
				" Value=",
				Value,
				" IsCritical=",
				IsCritical,
				"]"
			});
		}
	}
}
