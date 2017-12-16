using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffCrystalShell : MonsterBuff
	{
		public MonsterBuffCrystalShell(Single p_castersMagicFactor) : base(24, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.ArmorValue += (Int32)GetBuffValue(0);
		}
	}
}
