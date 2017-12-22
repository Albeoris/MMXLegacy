using System;
using Legacy.Core.Abilities;
using Legacy.Core.Configuration;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Pathfinding;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class SpiderQueenAIBehaviour : AverageAIHandler
	{
		private Boolean m_firstAction = true;

		public SpiderQueenAIBehaviour(Monster p_owner) : base(p_owner)
		{
		}

		public override void DoTurn(Grid p_grid, Party p_party)
		{
			if (m_owner.State == Monster.EState.SPAWNING)
			{
				return;
			}
			if (m_firstAction)
			{
				m_owner.AbilityHandler.ExecuteAttacks(null, null, false, EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
				m_owner.AbilityHandler.FlushActionLog(EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
				m_owner.BuffHandler.ModifyMonsterValues();
				m_owner.CombatHandler.MeleeStrikes += m_owner.CombatHandler.MeleeStrikesRoundBonus;
				m_owner.CombatHandler.MeleeStrikesRoundBonus = 0;
				GridSlot neighborSlot = p_grid.GetSlot(m_owner.Position).GetNeighborSlot(p_grid, m_owner.Direction);
				if (p_grid.MoveEntity(m_owner, m_owner.Direction))
				{
					m_targetSlot = neighborSlot;
					GridSlot slot = p_grid.GetSlot(m_owner.Position);
					GridSlot slot2 = p_grid.GetSlot(p_party.Position);
					Int32 num = AStarHelper<GridSlot>.Calculate(slot, slot2, GameConfig.MaxSteps, m_owner, true, null);
					if (num > 0)
					{
						m_owner.DistanceToParty = num;
					}
					else
					{
						m_owner.DistanceToParty = 99f;
					}
					m_owner.StartMovement.Trigger();
					m_firstAction = false;
					return;
				}
			}
			else
			{
				base.DoTurn(p_grid, p_party);
			}
		}
	}
}
