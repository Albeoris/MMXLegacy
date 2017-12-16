using System;

namespace Legacy.Core.Entities.AI.Behaviours
{
	internal class TrapBehaviour : StationaryBehaviour
	{
		public TrapBehaviour(AIBrain p_brain, Summon p_controller) : base(p_brain, p_controller)
		{
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			if (m_stateMachine.IsState(EState.ATTACKING) && (Controller.LifeTime > 1 || Controller.LifeTime == -1))
			{
				Controller.LifeTime = 0;
			}
		}
	}
}
