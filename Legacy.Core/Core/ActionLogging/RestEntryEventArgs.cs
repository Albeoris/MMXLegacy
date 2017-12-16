using System;

namespace Legacy.Core.ActionLogging
{
	public class RestEntryEventArgs : LogEntryEventArgs
	{
		public RestEntryEventArgs(Int32 p_consumedFoddAmount)
		{
			ConsumedFoodAmount = p_consumedFoddAmount;
		}

		public Int32 ConsumedFoodAmount { get; private set; }
	}
}
