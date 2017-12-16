using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.UpdateLogic;

namespace Legacy.Core.Buffs
{
	public class MonsterBuffHarmony : MonsterBuff
	{
		public MonsterBuffHarmony(Single p_castersMagicFactor) : base(21, p_castersMagicFactor)
		{
		}

		public override void DoImmediateEffect(Monster p_monster)
		{
			DoEffect(p_monster);
		}

		public override void DoEffect(Monster p_monster)
		{
			p_monster.ForceAggro(false);
			p_monster.AggroRange = 2f;
			p_monster.CheckAggroRange();
			if (!p_monster.IsAggro)
			{
				p_monster.IsAttackable = false;
				p_monster.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
			}
			if (!LegacyLogic.Instance.WorldManager.Party.HasAggro() && LegacyLogic.Instance.UpdateManager.CurrentTurnActor is PartyTurnActor)
			{
				LegacyLogic.Instance.WorldManager.Party.FinishPartyRound();
			}
		}

		public override Single GetBuffValue(Int32 p_valueIndex)
		{
			if (p_valueIndex == 0)
			{
				Int32 num = (Int32)(m_castersMagicFactor * m_staticData.GetDuration(m_level) + 0.5f);
				return num;
			}
			return base.GetBuffValue(p_valueIndex);
		}
	}
}
