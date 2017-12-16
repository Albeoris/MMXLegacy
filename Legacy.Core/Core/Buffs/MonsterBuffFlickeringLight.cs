using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffFlickeringLight : MonsterBuff
	{
		public MonsterBuffFlickeringLight(Single p_castersMagicFactor) : base(29, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.EvadeValue += GetBuffValue(0);
		}
	}
}
