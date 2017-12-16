using System;
using System.Collections.Generic;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities.AI.Behaviours
{
	internal class StationaryBehaviour : BaseSummonBehaviour
	{
		private List<Object> m_attackBuffer = new List<Object>();

		protected StateMachine<EState> m_stateMachine;

		public StationaryBehaviour(AIBrain p_brain, Summon p_controller) : base(p_brain, p_controller)
		{
			m_stateMachine = new StateMachine<EState>();
			m_stateMachine.AddState(new State<EState>(EState.IDLE));
			State<EState> state = new State<EState>(EState.ATTACKING);
			state.AddTransition(new Transition<EState>(EState.ATTACKING_DONE, p_controller.AttackingDone));
			m_stateMachine.AddState(state);
			m_stateMachine.AddState(new State<EState>(EState.ATTACKING_DONE));
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public override void BeginTurn()
		{
			base.BeginTurn();
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			m_stateMachine.Update();
			if (m_stateMachine.CurrentStateID == EState.IDLE)
			{
				m_attackBuffer.Clear();
				if (Attack(m_attackBuffer))
				{
					m_stateMachine.ChangeState(EState.ATTACKING);
				}
				else
				{
					m_stateMachine.ChangeState(EState.ATTACKING_DONE);
				}
			}
			else if (m_stateMachine.CurrentStateID == EState.ATTACKING_DONE)
			{
				Controller.IsFinishTurn = true;
			}
		}

		public enum EState
		{
			IDLE,
			ATTACKING,
			ATTACKING_DONE
		}
	}
}
