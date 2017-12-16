using System;

namespace Legacy.Core.ActionLogging
{
	public class TrapTriggeredEventArgs : LogEntryEventArgs
	{
		public TrapTriggeredEventArgs(Object p_trap)
		{
			Trap = p_trap;
		}

		public Object Trap { get; private set; }
	}
}
