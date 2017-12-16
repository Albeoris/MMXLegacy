using System;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public class PlainPrecondition : BasePrecondition
	{
		public PlainPrecondition() : base(EPreconditionType.PLAIN)
		{
		}

		public override void Trigger()
		{
			EPreconditionDecision decision = Decision;
			if (decision == EPreconditionDecision.WHO_WILL)
			{
				throw new Exception("Precondition WHO_WILL makes no sense here");
			}
			if (decision != EPreconditionDecision.TEXT_INPUT)
			{
				base.Trigger();
				return;
			}
			throw new Exception("Precondition TEXT_INPUT makes no sense here");
		}

		public override void OnResult(Object p_sender, EventArgs p_eventArgs)
		{
			if (Decision == EPreconditionDecision.NONE)
			{
				m_cancelled = false;
			}
			else if (Decision == EPreconditionDecision.YES_NO)
			{
				m_cancelled = ((PreconditionResultYesNoArgs)p_eventArgs).m_cancelled;
			}
			if (!m_cancelled)
			{
				m_result = Evaluate(p_eventArgs);
			}
			base.OnResult(p_sender, p_eventArgs);
		}

		private Boolean Evaluate(EventArgs p_args)
		{
			return Decision == EPreconditionDecision.NONE || (Decision == EPreconditionDecision.YES_NO && ((PreconditionResultYesNoArgs)p_args).m_accepted);
		}
	}
}
