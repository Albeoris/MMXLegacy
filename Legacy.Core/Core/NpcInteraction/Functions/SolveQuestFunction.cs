using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Quests;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class SolveQuestFunction : DialogFunction
	{
		private Int32 m_questID;

		private Int32 m_dialogID;

		private Int32 m_fireNpcID;

		private Int32 m_removeTokenID;

		private Int32 m_questNpcID;

		[XmlAttribute("questID")]
		public Int32 QuestID
		{
			get => m_questID;
		    set => m_questID = value;
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

		[XmlAttribute("removeTokenID")]
		public Int32 RemoveTokenID
		{
			get => m_removeTokenID;
		    set => m_removeTokenID = value;
		}

		[XmlAttribute("questNpcID")]
		public Int32 QuestNpcID
		{
			get => m_questNpcID;
		    set => m_questNpcID = value;
		}

		public override void Trigger(ConversationManager p_manager)
		{
			if (QuestNpcID > 0)
			{
				Npc sender = LegacyLogic.Instance.WorldManager.NpcFactory.Get(QuestNpcID);
				LegacyLogic.Instance.WorldManager.QuestHandler.StartDialog(sender, m_questID);
			}
			else
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.StartDialog(p_manager.CurrentNpc, m_questID);
			}
			if (m_fireNpcID != 0)
			{
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(m_fireNpcID);
			}
			if (m_removeTokenID != 0)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.RemoveToken(m_removeTokenID);
			}
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_questID);
			if (step.QuestState != EQuestState.SOLVED && step.CheckFinished())
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.FinalizeStep(step);
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
