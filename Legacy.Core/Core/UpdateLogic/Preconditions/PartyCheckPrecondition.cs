using System;
using Legacy.Core.Api;
using Legacy.Core.Quests;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public class PartyCheckPrecondition : BasePrecondition
	{
		private Int32 m_requiredTokenID;

		private Int32 m_requiredBlessingID;

		private Int32 m_requiredInactiveQuestStepID;

		private Int32 m_requiredActiveQuestStepID;

		private Int32 m_requiredFinishedQuestStepID;

		private Int32 m_requiredHirelingID;

		private Int32 m_withoutHirelingID;

		private Int32 m_withoutTokenID;

		public PartyCheckPrecondition() : base(EPreconditionType.PARTY_CHECK)
		{
			m_requiredTokenID = -1;
			m_requiredBlessingID = -1;
			m_requiredInactiveQuestStepID = -1;
			m_requiredActiveQuestStepID = -1;
			m_requiredFinishedQuestStepID = -1;
			m_requiredHirelingID = -1;
			m_withoutHirelingID = -1;
			m_withoutTokenID = -1;
		}

		public String SuccessText { get; set; }

		public String FailText { get; set; }

		public Int32 RequiredTokenID
		{
			get => m_requiredTokenID;
		    set => m_requiredTokenID = value;
		}

		public Int32 WithoutTokenID
		{
			get => m_withoutTokenID;
		    set => m_withoutTokenID = value;
		}

		public Int32 RequiredBlessingID
		{
			get => m_requiredBlessingID;
		    set => m_requiredBlessingID = value;
		}

		public Int32 RequiredInactiveQuestStepID
		{
			get => m_requiredInactiveQuestStepID;
		    set => m_requiredInactiveQuestStepID = value;
		}

		public Int32 RequiredActiveQuestStepID
		{
			get => m_requiredActiveQuestStepID;
		    set => m_requiredActiveQuestStepID = value;
		}

		public Int32 RequiredFinishedQuestStepID
		{
			get => m_requiredFinishedQuestStepID;
		    set => m_requiredFinishedQuestStepID = value;
		}

		public Int32 RequiredHirelingID
		{
			get => m_requiredHirelingID;
		    set => m_requiredHirelingID = value;
		}

		public Int32 WithoutHirelingID
		{
			get => m_withoutHirelingID;
		    set => m_withoutHirelingID = value;
		}

		public override void Trigger()
		{
			if (Decision == EPreconditionDecision.TEXT_INPUT)
			{
				throw new Exception("Precontion TEXT_INPUT makes no sense here");
			}
			if (Decision == EPreconditionDecision.WHO_WILL)
			{
				throw new Exception("Precontion WHO_WILL makes no sense here");
			}
			if (Decision == EPreconditionDecision.YES_NO)
			{
				throw new Exception("Precontion YES_NO makes no sense here");
			}
			m_result = Evaluate();
			m_cancelled = false;
			base.Trigger();
		}

		public override void OnResult(Object p_sender, EventArgs p_eventArgs)
		{
			base.OnResult(p_sender, p_eventArgs);
		}

		public Boolean Evaluate()
		{
			Boolean result = true;
			if (m_requiredTokenID > 0 && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(m_requiredTokenID) == 0)
			{
				result = false;
			}
			if (m_withoutTokenID > 0 && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(m_withoutTokenID) > 0)
			{
				result = false;
			}
			if (m_requiredBlessingID > 0 && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(m_requiredBlessingID) == 0)
			{
				result = false;
			}
			if (m_requiredInactiveQuestStepID > 0)
			{
				QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_requiredInactiveQuestStepID);
				if (step != null && step.QuestState != EQuestState.INACTIVE)
				{
					result = false;
				}
			}
			if (m_requiredActiveQuestStepID > 0)
			{
				QuestStep step2 = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_requiredActiveQuestStepID);
				if (step2 != null && step2.QuestState != EQuestState.ACTIVE)
				{
					result = false;
				}
				if (step2 == null)
				{
					result = false;
				}
			}
			if (m_requiredFinishedQuestStepID > 0)
			{
				QuestStep step3 = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(m_requiredFinishedQuestStepID);
				if (step3 != null && step3.QuestState != EQuestState.SOLVED)
				{
					result = false;
				}
				if (step3 == null)
				{
					result = false;
				}
			}
			if (m_requiredHirelingID > 0 && !LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HirelingHired(m_requiredHirelingID))
			{
				result = false;
			}
			if (m_withoutHirelingID > 0 && LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HirelingHired(m_withoutHirelingID))
			{
				result = false;
			}
			return result;
		}
	}
}
