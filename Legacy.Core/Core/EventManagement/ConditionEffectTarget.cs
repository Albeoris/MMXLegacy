using System;
using Legacy.Core.Combat;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class ConditionEffectTarget : SpellTarget
	{
		public readonly ECondition Source;

		public readonly AttackResult Damage;

		public ConditionEffectTarget(Object p_Target, ECondition p_Source, AttackResult p_Damage) : base(p_Target)
		{
			Source = p_Source;
			Damage = p_Damage;
		}

		public override String ToString()
		{
			return String.Format(String.Concat(new Object[]
			{
				"[ConditionEffectTarget Target=",
				Target,
				" Source=",
				Source,
				" Damage=",
				Damage,
				"]"
			}), new Object[0]);
		}
	}
}
