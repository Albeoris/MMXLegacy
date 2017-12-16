using System;
using System.Threading;

namespace Legacy.Core.Utilities.StateManagement
{
	public class Transition<T>
	{
		private TriggerHelper m_trigger;

		private T m_targetState;

		private Boolean m_resetTriggerWhenStateEntered = true;

		private Boolean m_resetTriggerWhenProcessed = true;

		private Boolean m_negateTrigger;

		public Transition(T p_targetState, TriggerHelper p_trigger)
		{
			m_targetState = p_targetState;
			m_trigger = p_trigger;
		}

		public Transition(T p_targetState, TransitionConditionMethod p_condition)
		{
			m_targetState = p_targetState;
			ConditionMethod = p_condition;
		}

		public Transition(T p_targetState, TriggerHelper p_trigger, TransitionActionMethod p_action)
		{
			m_targetState = p_targetState;
			m_trigger = p_trigger;
			ActionMethod = p_action;
		}

		public Transition(T p_targetState, TransitionConditionMethod p_condition, TransitionActionMethod p_action)
		{
			m_targetState = p_targetState;
			ConditionMethod = p_condition;
			ActionMethod = p_action;
		}

		public Transition(T p_targetState, TriggerHelper p_trigger, TransitionConditionMethod p_condition, TransitionActionMethod p_action)
		{
			m_targetState = p_targetState;
			m_trigger = p_trigger;
			ConditionMethod = p_condition;
			ActionMethod = p_action;
		}

		public event TransitionActionMethod ActionMethod;

		public event TransitionConditionMethod ConditionMethod;

		public T TargetState => m_targetState;

	    public Boolean ResetTriggerWhenStateEntered
		{
			get => m_resetTriggerWhenStateEntered;
	        set => m_resetTriggerWhenStateEntered = value;
	    }

		public Boolean ResetTriggerWhenProcessed
		{
			get => m_resetTriggerWhenProcessed;
		    set => m_resetTriggerWhenProcessed = value;
		}

		public Boolean NegateTrigger
		{
			get => m_negateTrigger;
		    set => m_negateTrigger = value;
		}

		public virtual void Reset()
		{
			if (m_resetTriggerWhenStateEntered && m_trigger != null)
			{
				m_trigger.Reset();
			}
		}

		public virtual Boolean Evaluate()
		{
			Boolean flag = ConditionMethod == null || ConditionMethod();
			Boolean flag2 = true;
			if (m_trigger != null)
			{
				if (m_negateTrigger)
				{
					flag2 = !m_trigger.IsTriggered;
				}
				else
				{
					flag2 = m_trigger.IsTriggered;
				}
				if (flag2 && m_resetTriggerWhenProcessed)
				{
					m_trigger.Reset();
				}
			}
			Boolean flag3 = flag2 && flag;
			if (flag3 && ActionMethod != null)
			{
				ActionMethod();
			}
			return flag3;
		}

		public delegate void TransitionActionMethod();

		public delegate Boolean TransitionConditionMethod();
	}
}
