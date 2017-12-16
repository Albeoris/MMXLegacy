using System;
using Legacy.Core.Api;

namespace Legacy.Core.EventManagement
{
	public class GameTimeEventArgs : EventArgs
	{
		public GameTimeEventArgs(EDayState currentDayState, EDayState lastDayState, ETimeChangeReason reason)
		{
			CurrentDayState = currentDayState;
			LastDayState = lastDayState;
			Reason = reason;
		}

		public EDayState CurrentDayState { get; private set; }

		public EDayState LastDayState { get; private set; }

		public ETimeChangeReason Reason { get; private set; }
	}
}
