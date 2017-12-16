using System;
using System.Collections.Generic;
using Legacy.Core.Map;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities.AI.Behaviours
{
	public class CycloneBehaviour : BaseSummonBehaviour
	{
		private List<GridSlot> m_path = new List<GridSlot>();

		private List<Object> m_attackBuffer = new List<Object>();

		private GridSlot m_target;

		private StateMachine<EState> m_stateMachine;

		public CycloneBehaviour(AIBrain p_brain, Summon p_controller) : base(p_brain, p_controller)
		{
			m_stateMachine = new StateMachine<EState>();
			m_stateMachine.AddState(new State<EState>(EState.IDLE));
			m_stateMachine.AddState(new State<EState>(EState.WAITING));
			State<EState> state = new State<EState>(EState.MOVEMENT);
			state.AddTransition(new Transition<EState>(EState.MOVEMENT_DONE, p_controller.MovementDone));
			m_stateMachine.AddState(state);
			m_stateMachine.AddState(new State<EState>(EState.MOVEMENT_DONE));
			State<EState> state2 = new State<EState>(EState.ATTACKING);
			state2.AddTransition(new Transition<EState>(EState.ATTACKING_DONE, p_controller.AttackingDone));
			m_stateMachine.AddState(state2);
			m_stateMachine.AddState(new State<EState>(EState.ATTACKING_DONE));
			m_stateMachine.ChangeState(EState.WAITING);
		}

		public EState CurrentState => m_stateMachine.CurrentState.Id;

	    public override void BeginTurn()
		{
			base.BeginTurn();
			m_stateMachine.ChangeState(EState.WAITING);
			m_target = GetMoveTarget(m_path);
			if (m_target == null)
			{
				Controller.Destroy();
			}
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			m_stateMachine.Update();
			if (m_stateMachine.CurrentStateID == EState.WAITING && Controller.ViewIsDone.IsTriggered)
			{
				m_stateMachine.ChangeState(EState.IDLE);
			}
			if (m_stateMachine.CurrentStateID == EState.IDLE)
			{
				if (m_path.Count > Brain.Data.Range && Move(m_path))
				{
					m_stateMachine.ChangeState(EState.MOVEMENT);
				}
				else
				{
					m_stateMachine.ChangeState(EState.MOVEMENT_DONE);
				}
			}
			else if (m_stateMachine.CurrentStateID == EState.MOVEMENT_DONE)
			{
				m_attackBuffer.Clear();
				if (m_path.Count <= Brain.Data.Range && Attack(m_attackBuffer))
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
			MOVEMENT,
			MOVEMENT_DONE,
			ATTACKING,
			ATTACKING_DONE,
			WAITING
		}
	}
}
