using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffBattleSpirit : MonsterBuff
	{
		public MonsterBuffBattleSpirit(Single p_castersMagicFactor) : base(13, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.MeleeDamageModifier += BuffValues[0];
			p_monster.CombatHandler.RangeDamageModifier += BuffValues[0];
		}

		public override void DoImmediateEffect(Monster p_monster)
		{
			DoEffect(p_monster);
		}
	}
}
