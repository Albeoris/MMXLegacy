using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffHourOfJustice : MonsterBuff
	{
		public MonsterBuffHourOfJustice(Single p_castersMagicFactor) : base(28, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.MeleeDamageModifier = BuffValues[0] * m_castersMagicFactor;
			p_monster.CombatHandler.RangeDamageModifier = BuffValues[0] * m_castersMagicFactor;
			p_monster.MagicPowerModifier = 1f + BuffValues[0] * m_castersMagicFactor;
			p_monster.AttackValueModifier = 1f + BuffValues[1] * m_castersMagicFactor;
			p_monster.SpellHandler.UpdateMagicPower();
		}
	}
}
