using System;
using System.Collections.Generic;
using System.Threading;

namespace Legacy.Core.Utilities.StateManagement
{
	public class StateMachine<TStateId, TState, TTransition> where TState : State<TStateId, TTransition> where TTransition : Transition<TStateId>
	{
		protected readonly IEqualityComparer<TStateId> m_IdComparer;

		protected TState m_currentState;

		protected Dictionary<TStateId, TState> m_states = new Dictionary<TStateId, TState>();

		public StateMachine(IEqualityComparer<TStateId> p_idComparer)
		{
			if (p_idComparer == null)
			{
				throw new ArgumentNullException("p_idComparer");
			}
			m_IdComparer = p_idComparer;
		}

		public StateMachine() : this(EqualityComparer<TStateId>.Default)
		{
		}

		public event RaiseStateChangedMethod StateChangedMethod;

		public TState CurrentState => m_currentState;

	    public TStateId CurrentStateID => m_currentState.Id;

	    public TState GetStateById(TStateId p_stateId)
		{
			TState result;
			m_states.TryGetValue(p_stateId, out result);
			return result;
		}

		public Boolean IsState(TStateId p_stateId)
		{
			return m_IdComparer.Equals(m_currentState.Id, p_stateId);
		}

		public virtual void ChangeState(TStateId p_newStateId)
		{
			TStateId p_fromState = default(TStateId);
			if (m_currentState != null)
			{
				m_currentState.LeaveState();
				p_fromState = m_currentState.Id;
			}
			if (!m_states.TryGetValue(p_newStateId, out m_currentState))
			{
				throw new KeyNotFoundException("StateID not found! " + p_newStateId);
			}
			if (StateChangedMethod != null)
			{
				StateChangedMethod(p_fromState, p_newStateId);
			}
			m_currentState.EnterState();
		}

		public void AddState(TState p_state)
		{
			m_states.Add(p_state.Id, p_state);
		}

		public void Update()
		{
			if (m_currentState != null)
			{
				m_currentState.Update();
				TStateId p_newStateId;
				if (m_currentState.CheckTransitions(out p_newStateId))
				{
					ChangeState(p_newStateId);
				}
			}
		}

		public delegate void RaiseStateChangedMethod(TStateId p_fromState, TStateId p_toState);
	}
}
