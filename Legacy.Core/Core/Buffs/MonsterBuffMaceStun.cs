using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffMaceStun : MonsterBuff
	{
		public MonsterBuffMaceStun(Single p_castersMagicFactor) : base(16, p_castersMagicFactor)
		{
		}

		public override void DoEffect(Monster p_monster)
		{
			if (p_monster.IsAggro)
			{
				p_monster.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
				p_monster.CombatHandler.EvadeValue = 0f;
				p_monster.CombatHandler.CannotBlockThisTurn = true;
			}
		}
	}
}
