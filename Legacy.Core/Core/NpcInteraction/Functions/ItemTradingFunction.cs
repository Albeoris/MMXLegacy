using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ItemTradingFunction : DialogFunction
	{
		public override Boolean RequireGold => true;

	    public override void Trigger(ConversationManager p_manager)
		{
			p_manager.CurrentNpc.TradingInventory.StartTrade();
		}

        public override void OnShow(Func<String, String> localisation)
        {
            LegacyLogic.Instance.EventManager.Get<InitServiceDialogArgs>().TryInvoke(() =>
            {
                String caption = localisation("GUI_TRADE_NPC_BUY_BUTTON") + ":"; // Buy:
                String title = localisation("TT_HELP_CATEGORY_ITEMS"); // Items

                return new InitServiceDialogArgs(caption, title);
            });
        }
    }
}
