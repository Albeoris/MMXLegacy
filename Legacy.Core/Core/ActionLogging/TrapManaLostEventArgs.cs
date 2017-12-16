using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class TrapManaLostEventArgs : LogEntryEventArgs
	{
		public TrapManaLostEventArgs(Character p_target, Int32 p_amount)
		{
			Target = p_target;
			Amount = p_amount;
		}

		public Character Target { get; private set; }

		public Int32 Amount { get; private set; }
	}
}
