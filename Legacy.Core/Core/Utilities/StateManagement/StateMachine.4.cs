using System;
using System.Collections.Generic;

namespace Legacy.Core.Utilities.StateManagement
{
	public class StateMachine : StateMachine<Int32, State, Transition<Int32>>
	{
		public StateMachine()
		{
		}

		public StateMachine(IEqualityComparer<Int32> p_idComparer) : base(EqualityComparer<Int32>.Default)
		{
		}

		public void AddState(Int32 stateId)
		{
			m_states.Add(stateId, new State(stateId));
		}
	}
}
