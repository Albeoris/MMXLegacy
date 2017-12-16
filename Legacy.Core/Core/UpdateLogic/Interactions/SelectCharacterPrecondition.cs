using System;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic.Preconditions;

namespace Legacy.Core.UpdateLogic.Interactions
{
	internal class SelectCharacterPrecondition : BasePrecondition
	{
		private String m_successText;

		private String m_failText;

		public SelectCharacterPrecondition() : base(EPreconditionType.SELECT_CHARACTER)
		{
		}

		public String SuccessText
		{
			get => m_successText;
		    set => m_successText = value;
		}

		public String FailText
		{
			get => m_failText;
		    set => m_failText = value;
		}

		public override void Trigger()
		{
			if (Decision == EPreconditionDecision.NONE)
			{
				throw new Exception("Precontion NONE makes no sense here");
			}
			if (Decision == EPreconditionDecision.TEXT_INPUT)
			{
				throw new Exception("Precontion TEXT_INPUT makes no sense here");
			}
			if (Decision == EPreconditionDecision.YES_NO)
			{
				throw new Exception("Precontion YES_NO makes no sense here");
			}
			base.Trigger();
		}

		public override void OnResult(Object p_sender, EventArgs p_eventArgs)
		{
			m_cancelled = ((PreconditionResultWhoWillArgs)p_eventArgs).m_cancelled;
			if (!m_cancelled)
			{
				m_result = Evaluate((PreconditionResultWhoWillArgs)p_eventArgs);
			}
			base.OnResult(p_sender, p_eventArgs);
		}

		public Boolean Evaluate(PreconditionResultWhoWillArgs p_args)
		{
			Character selectedCharacter = p_args.m_selectedCharacter;
			selectedCharacter.BarkHandler.TriggerBark(EBarks.SUCCESS);
			return true;
		}
	}
}
