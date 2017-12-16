using System;
using Legacy.Core.NpcInteraction;

namespace Legacy.Core.EventManagement
{
	public class NPCTradeEventArgs : EventArgs
	{
		public NPCTradeEventArgs(TradingInventoryController p_inventory)
		{
			Inventory = p_inventory;
		}

		public TradingInventoryController Inventory { get; private set; }
	}
}
