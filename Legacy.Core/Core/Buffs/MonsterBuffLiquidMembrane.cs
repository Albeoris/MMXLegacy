using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffLiquidMembrane : MonsterBuff
	{
		private Single m_percentValue;

		public MonsterBuffLiquidMembrane(Single p_castersMagicFactor) : base(14, p_castersMagicFactor)
		{
			m_percentValue = m_castersMagicFactor * 0.1f;
		}

		public override void ManipulateAttack(Attack p_attack, Monster p_monster)
		{
			for (Int32 i = 0; i < p_attack.Damages.Count; i++)
			{
				Damage value = p_attack.Damages[i];
				value.Value -= (Int32)(value.Value * (m_percentValue * BuffValues[0] * 0.01f));
				p_attack.Damages[i] = value;
			}
		}

		public override Single GetBuffValue(Int32 p_valueIndex)
		{
			if (p_valueIndex < 0 || p_valueIndex >= BuffValues.Length)
			{
				return 0f;
			}
			if (p_valueIndex == 0)
			{
				return (Single)Math.Round(BuffValues[0] * m_castersMagicFactor * 0.1f, MidpointRounding.AwayFromZero);
			}
			return BuffValues[p_valueIndex];
		}
	}
}
