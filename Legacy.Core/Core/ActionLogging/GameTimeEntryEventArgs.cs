using System;
using Legacy.Core.Api;

namespace Legacy.Core.ActionLogging
{
	public class GameTimeEntryEventArgs : LogEntryEventArgs
	{
		public GameTimeEntryEventArgs(EDayState p_newDayState)
		{
			NewDayState = p_newDayState;
		}

		public EDayState NewDayState { get; private set; }
	}
}
