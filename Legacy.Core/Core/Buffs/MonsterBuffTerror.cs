using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffTerror : MonsterBuff
	{
		public MonsterBuffTerror(Single p_castersMagicFactor) : base(11, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.CombatHandler.IsFleeing = true;
		}

		public override Single GetBuffValue(Int32 p_valueIndex)
		{
			if (p_valueIndex == 0)
			{
				return m_staticData.GetDuration(m_level);
			}
			return base.GetBuffValue(p_valueIndex);
		}
	}
}
