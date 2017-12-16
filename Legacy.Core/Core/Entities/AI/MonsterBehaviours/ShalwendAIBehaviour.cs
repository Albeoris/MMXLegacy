using System;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class ShalwendAIBehaviour : AverageAIHandler
	{
		private Boolean m_firstAction = true;

		public ShalwendAIBehaviour(Monster p_owner) : base(p_owner)
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
				m_owner.EndTurn();
				m_firstAction = false;
			}
			else
			{
				base.DoTurn(p_grid, p_party);
			}
		}
	}
}
