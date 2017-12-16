using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffImmobilised : MonsterBuff
	{
		public MonsterBuffImmobilised(Single p_castersMagicFactor) : base(5, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.CanMove = false;
		}
	}
}
