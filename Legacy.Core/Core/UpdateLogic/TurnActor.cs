using System;
using Legacy.Core.Entities;
using Legacy.Core.Utilities.StateManagement;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic
{
	public abstract class TurnActor
	{
		protected internal TimeStateMachine<EState> m_stateMachine;

		public TurnActor()
		{
			m_stateMachine = new TimeStateMachine<EState>();
			m_stateMachine.AddState(new TimeState<EState>(EState.IDLE));
			m_stateMachine.AddState(new TimeState<EState>(EState.RUNNING));
			m_stateMachine.AddState(new TimeState<EState>(EState.FINISHED));
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public EState State => m_stateMachine.CurrentState.Id;

	    public virtual void AddEntity(BaseObject entity)
		{
		}

		public virtual void RemoveEntity(BaseObject entity)
		{
		}

		public virtual void StartTurn()
		{
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public virtual void FinishTurn()
		{
		}

		public virtual void UpdateTurn()
		{
			m_stateMachine.Update();
		}

		public virtual Boolean IsFinished()
		{
			return m_stateMachine.IsState(EState.FINISHED);
		}

		public virtual void ExecutionBreak()
		{
			LegacyLogger.Log(GetType().Name + " execution aborded!", false);
			m_stateMachine.ChangeState(EState.FINISHED);
		}

		public virtual void Clear()
		{
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public virtual void ClearAndDestroy()
		{
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public enum EState
		{
			IDLE,
			RUNNING,
			FINISHED
		}
	}
}
