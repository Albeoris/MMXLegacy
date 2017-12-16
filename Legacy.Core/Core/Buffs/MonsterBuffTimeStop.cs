using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffTimeStop : MonsterBuff
	{
		public MonsterBuffTimeStop(Single p_castersMagicFactor) : base(22, p_castersMagicFactor)
		{
		}

		public override void DoImmediateEffect(Monster p_monster)
		{
			DoEffect(p_monster);
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.IsAggro = false;
			p_monster.IsAttackable = false;
			p_monster.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
		}
	}
}
