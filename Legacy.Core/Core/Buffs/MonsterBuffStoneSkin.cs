using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffStoneSkin : MonsterBuff
	{
		public MonsterBuffStoneSkin(Single p_castersMagicFactor) : base(15, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.ArmorValue += BuffValues[0] * m_castersMagicFactor;
		}
	}
}
