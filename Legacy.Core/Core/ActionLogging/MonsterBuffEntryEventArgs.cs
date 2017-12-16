using System;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;

namespace Legacy.Core.ActionLogging
{
	public class MonsterBuffEntryEventArgs : LogEntryEventArgs
	{
		public MonsterBuffEntryEventArgs(Monster p_target, MonsterBuff p_buff, Boolean p_success)
		{
			Target = p_target;
			Buff = p_buff;
			Successful = p_success;
		}

		public Monster Target { get; private set; }

		public MonsterBuff Buff { get; private set; }

		public Boolean Successful { get; private set; }
	}
}
