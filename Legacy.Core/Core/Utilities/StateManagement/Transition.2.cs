using System;

namespace Legacy.Core.Utilities.StateManagement
{
	public class Transition : Transition<Int32>
	{
		public Transition(Int32 p_targetState, TriggerHelper p_trigger) : base(p_targetState, p_trigger)
		{
		}

		public Transition(Int32 p_targetState, TransitionConditionMethod p_condition) : base(p_targetState, p_condition)
		{
		}

		public Transition(Int32 p_targetState, TriggerHelper p_trigger, TransitionActionMethod p_action) : base(p_targetState, p_trigger, p_action)
		{
		}

		public Transition(Int32 p_targetState, TransitionConditionMethod p_condition, TransitionActionMethod p_action) : base(p_targetState, p_condition, p_action)
		{
		}

		public Transition(Int32 p_targetState, TriggerHelper p_trigger, TransitionConditionMethod p_condition, TransitionActionMethod p_action) : base(p_targetState, p_trigger, p_condition, p_action)
		{
		}
	}
}
