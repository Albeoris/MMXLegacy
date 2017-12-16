using System;
using Legacy.Core.Entities;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffSleeping : MonsterBuff
	{
		public MonsterBuffSleeping(Single p_castersMagicFactor) : base(7, p_castersMagicFactor)
		{
		}

		public override void DoImmediateEffect(Monster p_monster)
		{
			DoEffect(p_monster);
		}

		public override void DoEffect(Monster p_monster)
		{
			if (p_monster.IsAggro)
			{
				p_monster.CombatHandler.EvadeValue = 0f;
				p_monster.MeleeBlockAttempts = 0;
				p_monster.GeneralBlockAttempts = 0;
				p_monster.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
			}
			else
			{
				ExpireBuff();
			}
		}

		public override void DoOnCastSpellEffect(Monster p_monster)
		{
			p_monster.BuffHandler.RemoveBuff(this);
		}

		public override void DoOnGetDamageEffect(Monster p_monster)
		{
			p_monster.BuffHandler.RemoveBuff(this);
		}
	}
}
