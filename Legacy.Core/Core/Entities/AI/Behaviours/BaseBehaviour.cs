using System;

namespace Legacy.Core.Entities.AI.Behaviours
{
	public abstract class BaseBehaviour
	{
		public BaseBehaviour(AIBrain p_brain, MovingEntity p_controller)
		{
			Brain = p_brain;
			Controller = p_controller;
		}

		public AIBrain Brain { get; private set; }

		public MovingEntity Controller { get; private set; }

		public abstract void BeginTurn();

		public abstract void UpdateTurn();

		public abstract void EndTurn();
	}
}
