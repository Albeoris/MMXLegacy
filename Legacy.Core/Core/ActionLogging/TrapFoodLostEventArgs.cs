using System;

namespace Legacy.Core.ActionLogging
{
	public class TrapFoodLostEventArgs : LogEntryEventArgs
	{
		public TrapFoodLostEventArgs(Int32 p_amount)
		{
			Amount = p_amount;
		}

		public Int32 Amount { get; private set; }
	}
}
