using System;

namespace Legacy.Core.Utilities.StateManagement
{
	public class State<T> : State<T, Transition<T>>
	{
		public State(T p_id) : base(p_id, null)
		{
		}

		public State(T p_id, StateUpdateMethod p_stateUpdateMethod) : base(p_id, p_stateUpdateMethod)
		{
		}
	}
}
