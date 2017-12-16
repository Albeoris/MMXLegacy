using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffMemoryGap : MonsterBuff
	{
		public MonsterBuffMemoryGap(Single p_castersMagicFactor) : base(4, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.CanCastSpell = false;
		}
	}
}
