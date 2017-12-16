using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	internal class DamagePreventedEntryEventArgs : LogEntryEventArgs
	{
		public DamagePreventedEntryEventArgs(PartyBuff p_buff, Int32 p_preventedDamage)
		{
			Buff = p_buff;
			DamagePrevented = p_preventedDamage;
		}

		public PartyBuff Buff { get; private set; }

		public Int32 DamagePrevented { get; private set; }
	}
}
