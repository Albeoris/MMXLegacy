using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class StatusChangedEventArgs : EventArgs
	{
		public StatusChangedEventArgs(EChangeType p_type)
		{
			Type = p_type;
			Condition = ECondition.NONE;
		}

		public StatusChangedEventArgs(EChangeType p_type, ECondition p_newCondition)
		{
			BecameUnableToDoAnything = false;
			Type = p_type;
			Condition = p_newCondition;
		}

		public EChangeType Type { get; private set; }

		public ECondition Condition { get; private set; }

		public Boolean BecameUnableToDoAnything { get; set; }

		public enum EChangeType
		{
			STATUS,
			HEALTH_POINTS,
			MANA_POINTS,
			CONDITIONS,
			STARVED_ALERT,
			STARVED_WARNING
		}
	}
}
