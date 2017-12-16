using System;

namespace Legacy.Core.Utilities.StateManagement
{
	public class TimeState<T> : State<T, Transition<T>>
	{
		public readonly Single StateDuration;

		public TimeState(T p_id) : this(p_id, 0f, null)
		{
		}

		public TimeState(T p_id, Single duration) : this(p_id, duration, null)
		{
		}

		public TimeState(T p_id, Single p_duration, StateUpdateMethod p_stateUpdateMethod) : base(p_id, p_stateUpdateMethod)
		{
			if (p_duration < 0f)
			{
				throw new ArgumentOutOfRangeException("p_duration", "p_duration < 0");
			}
			StateDuration = p_duration;
		}
	}
}
