using System;
using System.Collections.Generic;

namespace Legacy.Core.Utilities.StateManagement
{
	public class StateMachine<TStateId> : StateMachine<TStateId, State<TStateId, Transition<TStateId>>, Transition<TStateId>>
	{
		public StateMachine()
		{
		}

		public StateMachine(IEqualityComparer<TStateId> p_idComparer) : base(EqualityComparer<TStateId>.Default)
		{
		}
	}
}
