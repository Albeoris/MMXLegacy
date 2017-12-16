using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.ActionLogging
{
	public class MonsterLootEntryEventArgs : LogEntryEventArgs
	{
		public MonsterLootEntryEventArgs(Monster p_monster)
		{
			Monster = p_monster;
		}

		public Int32 Gold { get; set; }

		public Monster Monster { get; private set; }

		public BaseItem Item { get; set; }
	}
}
