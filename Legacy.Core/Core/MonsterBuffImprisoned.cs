using System;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;

namespace Legacy.Core
{
	public class MonsterBuffImprisoned : MonsterBuff
	{
		public MonsterBuffImprisoned(Single p_castersMagicFactor) : base(19, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			if (p_monster.IsAggro)
			{
				p_monster.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
			}
		}

		public override void DoImmediateEffect(Monster p_monster)
		{
			p_monster.IsAttackable = false;
		}
	}
}
