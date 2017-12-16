using System;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Quests;

namespace Legacy.Core.NpcInteraction.Functions
{
	public class ForceSolveQuestFunction : DialogFunction
	{
		private Int32 m_questID;

		private Int32 m_fireNpcID;

		private Int32 m_removeTokenID;

		[XmlAttribute("questID")]
		public Int32 QuestID
		{
			get => m_questID;
		    set => m_questID = value;
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

		public override void Trigger(ConversationManager p_manager)
		{
			LegacyLogic.Instance.WorldManager.QuestHandler.StartDialog(p_manager.CurrentNpc, m_questID);
			if (m_fireNpcID != 0)
			{
				LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(m_fireNpcID);
			}
			if (m_removeTokenID != 0)
			{
				LegacyLogic.Instance.WorldManager.Party.TokenHandler.RemoveToken(m_removeTokenID);
			}
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_questID);
			if (step.QuestState != EQuestState.SOLVED)
			{
				step.SolveAllObjectives();
				if (step.CheckFinished())
				{
					LegacyLogic.Instance.WorldManager.QuestHandler.FinalizeStep(step);
				}
			}
		}
	}
}
