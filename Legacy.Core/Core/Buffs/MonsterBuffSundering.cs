using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffSundering : MonsterBuff
	{
		public MonsterBuffSundering(Single p_castersMagicFactor) : base(12, p_castersMagicFactor)
		{
		}

		public override void DoImmediateEffect(Monster p_monster)
		{
			DoEffect(p_monster);
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.ArmorValue = Math.Max(p_monster.CombatHandler.ArmorValue - GetBuffValue(0), 0f);
		}
	}
}
