using System;
using System.Collections.Generic;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.ActionLogging
{
	public class ChestLootEventArgs : LogEntryEventArgs
	{
		public ChestLootEventArgs(List<BaseItem> p_items, Int32 p_gold)
		{
			Items = p_items;
			Gold = p_gold;
		}

		public List<BaseItem> Items { get; private set; }

		public Int32 Gold { get; private set; }
	}
}
