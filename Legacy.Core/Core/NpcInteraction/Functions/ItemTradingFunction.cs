using System;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ItemTradingFunction : DialogFunction
	{
		public override Boolean RequireGold => true;

	    public override void Trigger(ConversationManager p_manager)
		{
			p_manager.CurrentNpc.TradingInventory.StartTrade();
		}
	}
}
