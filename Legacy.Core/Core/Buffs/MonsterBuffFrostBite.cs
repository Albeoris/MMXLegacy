using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffFrostBite : MonsterBuff
	{
		public MonsterBuffFrostBite(Single p_castersMagicFactor) : base(2, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.AttackRange = 1f;
		}
	}
}
