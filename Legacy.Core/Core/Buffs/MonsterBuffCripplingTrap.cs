using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffCripplingTrap : MonsterBuff
	{
		public MonsterBuffCripplingTrap(Single p_castersMagicFactor) : base(26, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.MeleeStrikes = Math.Max(p_monster.CombatHandler.MeleeStrikes - (Int32)GetBuffValue(0), Math.Min(p_monster.StaticData.MeleeAttackStrikesAmount, 1));
			p_monster.CombatHandler.RangedStrikes = Math.Max(p_monster.CombatHandler.RangedStrikes - (Int32)GetBuffValue(0), Math.Min(p_monster.StaticData.RangedAttackStrikesAmount, 1));
		}

		public override Single GetBuffValue(Int32 p_valueIndex)
		{
			if (p_valueIndex < BuffValues.Length)
			{
				return (Int32)BuffValues[p_valueIndex];
			}
			return 0f;
		}
	}
}
