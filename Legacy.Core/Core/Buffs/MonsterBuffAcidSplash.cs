using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffAcidSplash : MonsterBuff
	{
		public MonsterBuffAcidSplash(Single p_castersMagicFactor) : base(8, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.ArmorValue = Math.Max(0f, p_monster.CombatHandler.ArmorValue - (Int32)Math.Ceiling(p_monster.CombatHandler.ArmorValue * GetBuffValue(0)));
			p_monster.GeneralBlockAttempts = Math.Max(0, p_monster.GeneralBlockAttempts - (Int32)Math.Ceiling(p_monster.GeneralBlockAttempts * GetBuffValue(1)));
			p_monster.MeleeBlockAttempts = Math.Max(0, p_monster.MeleeBlockAttempts - (Int32)Math.Ceiling(p_monster.MeleeBlockAttempts * GetBuffValue(1)));
		}

		public override Single GetBuffValue(Int32 p_valueIndex)
		{
			if (p_valueIndex >= 0 && p_valueIndex < BuffValues.Length)
			{
				return BuffValues[p_valueIndex];
			}
			return 0f;
		}

		public override String GetBuffValueForTooltip(Int32 p_valueIndex)
		{
			return GetBuffValue(p_valueIndex) * 100f + " %";
		}
	}
}
