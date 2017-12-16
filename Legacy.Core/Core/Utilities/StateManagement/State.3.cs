using System;

namespace Legacy.Core.Utilities.StateManagement
{
	public class State : State<Int32, Transition<Int32>>
	{
		public State(Int32 p_id) : base(p_id, null)
		{
		}

		public State(Int32 p_id, StateUpdateMethod p_stateUpdateMethod) : base(p_id, p_stateUpdateMethod)
		{
		}
	}
}
