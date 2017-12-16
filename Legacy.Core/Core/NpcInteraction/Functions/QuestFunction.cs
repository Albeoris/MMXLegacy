using System;
using System.Xml.Serialization;
using Legacy.Core.Api;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class QuestFunction : DialogFunction
	{
		private Int32 m_dialogID;

		private Boolean m_needHireling;

		private Int32 m_tokenID = -1;

		[XmlAttribute("dialogID")]
		public Int32 DialogID
		{
			get => m_dialogID;
		    set => m_dialogID = value;
		}

		[XmlAttribute("needHireling")]
		public Boolean NeedHireling
		{
			get => m_needHireling;
		    set => m_needHireling = value;
		}

		[XmlAttribute("questID")]
		public Int32 QuestID { get; set; }

		[XmlAttribute("tokenID")]
		public Int32 TokenID
		{
			get => m_tokenID;
		    set => m_tokenID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			if (!m_needHireling)
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.ActivateQuest(QuestID);
				if (m_tokenID > 0)
				{
					LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddToken(TokenID);
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
			else if (LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HasFreeSlot())
			{
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Hire(LegacyLogic.Instance.ConversationManager.CurrentNpc);
				LegacyLogic.Instance.WorldManager.QuestHandler.ActivateQuest(QuestID);
				if (m_tokenID > 0)
				{
					LegacyLogic.Instance.WorldManager.Party.TokenHandler.AddToken(TokenID);
				}
				p_manager._ChangeDialog(0, m_dialogID);
			}
		}

		private void ExecuteQuestFunction(ConversationManager p_manager)
		{
		}
	}
}
