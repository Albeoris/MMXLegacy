using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class ConditionChangedEntryEventArgs : LogEntryEventArgs
	{
		public ConditionChangedEntryEventArgs(Object p_object, ECondition p_condition)
		{
			Object = p_object;
			Condition = p_condition;
			Set = true;
		}

		public ConditionChangedEntryEventArgs(Object p_object, ECondition p_condition, Boolean p_set)
		{
			Object = p_object;
			Condition = p_condition;
			Set = p_set;
		}

		public Object Object { get; private set; }

		public ECondition Condition { get; private set; }

		public Boolean Set { get; private set; }
	}
}
