using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class GiveTokenFunction : DialogFunction
	{
		private Int32 m_tokenID;

		private Int32 m_dialogID;

		private Int32 m_fireNpcID;

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

		[XmlAttribute("fireNpcID")]
		public Int32 FireNpcID
		{
			get => m_fireNpcID;
		    set => m_fireNpcID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.TokenHandler.AddToken(m_tokenID);
			if (m_fireNpcID != 0)
			{
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(m_fireNpcID);
			}
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
