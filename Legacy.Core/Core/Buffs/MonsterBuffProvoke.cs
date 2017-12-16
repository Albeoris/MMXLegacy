using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffProvoke : MonsterBuff
	{
		public MonsterBuffProvoke(Single p_casterMagicFactor) : base(18, p_casterMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.PreselectedTarget = Causer;
		}
	}
}
