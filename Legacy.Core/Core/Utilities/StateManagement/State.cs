using System;
using System.Collections.Generic;
using System.Threading;

namespace Legacy.Core.Utilities.StateManagement
{
	public class State<TId, TTransition> where TTransition : Transition<TId>
	{
		protected TId m_id;

		protected StateUpdateMethod m_stateUpdateMethod;

		internal List<TTransition> m_transitions;

		public State(TId p_id) : this(p_id, null)
		{
		}

		public State(TId p_id, StateUpdateMethod p_stateUpdateMethod)
		{
			m_id = p_id;
			m_stateUpdateMethod = p_stateUpdateMethod;
		}

		public event StateEnterMethod EnterMethod;

		public event StateLeaveMethod LeaveMethod;

		public TId Id => m_id;

	    public void AddTransition(TTransition p_transition)
		{
			if (m_transitions == null)
			{
				m_transitions = new List<TTransition>();
			}
			m_transitions.Add(p_transition);
		}

		public virtual void EnterState()
		{
			if (m_transitions != null)
			{
				foreach (TTransition ttransition in m_transitions)
				{
					ttransition.Reset();
				}
			}
			if (EnterMethod != null)
			{
				EnterMethod();
			}
		}

		public virtual void LeaveState()
		{
			if (LeaveMethod != null)
			{
				LeaveMethod();
			}
		}

		public Boolean CheckTransitions(out TId targetState)
		{
			if (m_transitions != null)
			{
				foreach (TTransition ttransition in m_transitions)
				{
					if (ttransition.Evaluate())
					{
						targetState = ttransition.TargetState;
						return true;
					}
				}
			}
			targetState = default(TId);
			return false;
		}

		public virtual void Update()
		{
			if (m_stateUpdateMethod != null)
			{
				m_stateUpdateMethod();
			}
		}

		public delegate void StateUpdateMethod();

		public delegate void StateEnterMethod();

		public delegate void StateLeaveMethod();
	}
}
