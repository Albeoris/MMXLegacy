using System;
using System.Collections.Generic;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class StationaryAIHandler : MonsterAIHandler
	{
		public StationaryAIHandler(Monster owner) : base(owner)
		{
		}

		internal override Boolean CalculatePath(GridSlot p_start, GridSlot p_target, List<GridSlot> p_pathBuffer)
		{
			return false;
		}

		protected override void DoRanged(Boolean p_isMagic, Party p_party, Grid p_grid, GridSlot p_startSlot, out Boolean p_isMelee)
		{
			p_isMelee = false;
			if (p_isMagic)
			{
				return;
			}
			if (m_decision != EMonsterStrategyDecision.RANGED && m_decision != EMonsterStrategyDecision.CALCULATE_STRATEGY)
			{
				return;
			}
			m_decision = EMonsterStrategyDecision.RANGED;
			Single attackRange = m_owner.CombatHandler.AttackRange;
			if (attackRange * attackRange >= Position.DistanceSquared(m_owner.Position, p_party.Position))
			{
				m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
				m_owner.RangedAttack = true;
				m_owner.CombatHandler.DoAttack();
				return;
			}
		}
	}
}
