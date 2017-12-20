using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class SpellFunction : DialogFunction
	{
		private Int32 m_dialogID;

		public SpellFunction(Int32 p_dialogID)
		{
			m_dialogID = p_dialogID;
		}

		public SpellFunction()
		{
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

        public override void OnShow(Func<string, string> localisation)
        {
            RaiseEventShow(localisation);
        }

	    public static void RaiseEventShow(Func<String, String> localisation)
	    {
	        LegacyLogic.Instance.EventManager.Get<InitServiceDialogArgs>().TryInvoke(() =>
	        {
	            String caption = localisation("GUI_TRADE_NPC_BUY_BUTTON") + ":"; // Buy:
	            String title = localisation("DIALOG_SHORTCUT_SPELLS"); // Spells

	            return new InitServiceDialogArgs(caption, title);
	        });
	    }

	    public override void Trigger(ConversationManager p_manager)
		{
			p_manager.CurrentNpc.TradingSpells.StartTrade(m_dialogID);
		}
	}
}
