using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class BuyTokenFunction : DialogFunction
	{
		private Int32 m_tokenID;

		private Int32 m_dialogID;

		private Int32 m_price;

		public BuyTokenFunction()
		{
		}

		public BuyTokenFunction(Int32 p_dialogId, Int32 p_price, Int32 p_tokenID)
		{
			m_dialogID = p_dialogId;
			m_price = p_price;
			m_tokenID = p_tokenID;
		}

		[XmlAttribute("tokenID")]
		public Int32 TokenID
		{
			get => m_tokenID;
		    set => m_tokenID = value;
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

		public override void Trigger(ConversationManager p_manager)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.TokenHandler.AddToken(m_tokenID);
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
