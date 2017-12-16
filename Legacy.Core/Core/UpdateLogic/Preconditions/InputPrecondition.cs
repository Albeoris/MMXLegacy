using System;
using Legacy.Core.Api;
using Legacy.Core.Internationalization;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public class InputPrecondition : BasePrecondition
	{
		private String m_wantedInput;

		public InputPrecondition() : base(EPreconditionType.INPUT)
		{
		}

		public String SuccessText { get; set; }

		public String FailText { get; set; }

		public String WantedInput
		{
			get => m_wantedInput;
		    set => m_wantedInput = value;
		}

		public override void Trigger()
		{
			if (Decision == EPreconditionDecision.NONE)
			{
				throw new Exception("Precontion NONE makes no sense here");
			}
			if (Decision == EPreconditionDecision.WHO_WILL)
			{
				throw new Exception("Precontion WHO_WILL makes no sense here");
			}
			if (Decision == EPreconditionDecision.YES_NO)
			{
				throw new Exception("Precontion YES_NO makes no sense here");
			}
			base.Trigger();
		}

		public override void OnResult(Object p_sender, EventArgs p_eventArgs)
		{
			m_cancelled = ((PreconditionResultInputArgs)p_eventArgs).m_cancelled;
			if (!m_cancelled)
			{
				m_result = Evaluate((PreconditionResultInputArgs)p_eventArgs);
			}
			base.OnResult(p_sender, p_eventArgs);
		}

		private Boolean Evaluate(PreconditionResultInputArgs p_args)
		{
			String[] array = m_wantedInput.Split(new Char[]
			{
				';'
			});
			for (Int32 i = 0; i < array.Length; i++)
			{
				if (p_args.m_textInput.ToUpper() == Localization.Instance.GetText(array[i]).ToUpper())
				{
					return true;
				}
			}
			LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.INPUT_FAIL);
			return false;
		}
	}
}
