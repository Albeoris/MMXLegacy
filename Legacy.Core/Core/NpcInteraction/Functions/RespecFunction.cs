using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class RespecFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Int32 m_price;

		private Character m_char;

		public RespecFunction(Character p_char, Int32 p_price, Int32 p_dialogID)
		{
			m_price = p_price;
			m_char = p_char;
			m_dialogID = p_dialogID;
		}

		public RespecFunction()
		{
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("price")]
		public Int32 Price
		{
			get => m_price;
		    set => m_price = value;
		}

		public Character Chara => m_char;

	    public override void Trigger(ConversationManager p_manager)
		{
			m_char.Respec();
			LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_price);
			if (m_dialogID > 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, m_dialogID);
			}
			else if (m_dialogID == 0)
			{
				p_manager._ChangeDialog(p_manager.CurrentNpc.StaticID, p_manager.CurrentConversation.RootDialog.ID);
			}
			else
			{
				p_manager.CloseNpcContainer(null);
			}
		}
	}
}
