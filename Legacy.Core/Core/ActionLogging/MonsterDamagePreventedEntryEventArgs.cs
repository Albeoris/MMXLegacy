using System;
using Legacy.Core.Buffs;

namespace Legacy.Core.ActionLogging
{
	internal class MonsterDamagePreventedEntryEventArgs : LogEntryEventArgs
	{
		public MonsterDamagePreventedEntryEventArgs(MonsterBuff p_buff, Int32 p_preventedDamage)
		{
			Buff = p_buff;
			DamagePrevented = p_preventedDamage;
		}

		public MonsterBuff Buff { get; private set; }

		public Int32 DamagePrevented { get; private set; }
	}
}
