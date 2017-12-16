using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class RemoveSetFunction : DialogFunction
	{
		private Int32 m_setID;

		private Int32 m_dialogID;

		[XmlAttribute("setID")]
		public Int32 SetID
		{
			get => m_setID;
		    set => m_setID = value;
		}

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			party.TokenHandler.RemoveSet(m_setID);
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
