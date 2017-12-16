using System;
using System.Xml.Serialization;

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

		public override void Trigger(ConversationManager p_manager)
		{
			p_manager.CurrentNpc.TradingSpells.StartTrade(m_dialogID);
		}
	}
}
