using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.ServiceWrapper;

namespace Legacy.Core.UpdateLogic.Preconditions
{
	public class UnlockKeyPrecondition : BasePrecondition
	{
		private Int32 m_wantedPrivilege;

		public UnlockKeyPrecondition() : base(EPreconditionType.UNLOCK_KEY)
		{
		}

		public String SuccessText { get; set; }

		public String FailText { get; set; }

		public String FailTextForDisplay { get; set; }

		public Int32 WantedPrivilege
		{
			get => m_wantedPrivilege;
		    set => m_wantedPrivilege = value;
		}

		public override void Trigger()
		{
			FailTextForDisplay = FailText;
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
			if (LegacyLogic.Instance.ServiceWrapper.IsPrivilegeAvailable(m_wantedPrivilege))
			{
				PreconditionEvaluateArgs preconditionEvaluateArgs = new PreconditionEvaluateArgs(true, false, null);
				preconditionEvaluateArgs.ShowMessage = false;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PRECONDITION_EVALUATED, preconditionEvaluateArgs);
			}
			else if (!LegacyLogic.Instance.ServiceWrapper.IsConnectedToServer())
			{
				PreconditionEvaluateArgs preconditionEvaluateArgs2 = new PreconditionEvaluateArgs(false, false, null);
				preconditionEvaluateArgs2.ShowMessage = false;
				FailTextForDisplay = "CHEST_PROMOTION_KEY_NO_CONNECTION";
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PRECONDITION_EVALUATED, preconditionEvaluateArgs2);
			}
			else
			{
				base.Trigger();
			}
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
			IServiceWrapperStrategy serviceWrapper = LegacyLogic.Instance.ServiceWrapper;
			serviceWrapper.UnlockPrivilege(p_args.m_textInput, m_wantedPrivilege);
			EActivateKeyResult eactivateKeyResult;
			for (eactivateKeyResult = EActivateKeyResult.ACTIVATE_WAITING; eactivateKeyResult == EActivateKeyResult.ACTIVATE_WAITING; eactivateKeyResult = serviceWrapper.GetUnlockPrivilegeState())
			{
			}
			if (eactivateKeyResult == EActivateKeyResult.ACTIVATE_SUCCESSFUL)
			{
				serviceWrapper.UpdatePrivilegesRewards();
			}
			else if (eactivateKeyResult == EActivateKeyResult.ACTIVATE_INVALID_KEY || eactivateKeyResult == EActivateKeyResult.ACTIVATE_WRONG_PRIVILAGE)
			{
				FailTextForDisplay = "ACTIVATE_KEY_ERROR_INVALID_KEY";
			}
			else if (eactivateKeyResult == EActivateKeyResult.ACTIVATE_KEY_ALREADY_IN_USE)
			{
				FailTextForDisplay = "ACTIVATE_KEY_ERROR_KEY_ALREADY_USED";
			}
			else
			{
				FailTextForDisplay = FailText;
			}
			return serviceWrapper.IsPrivilegeAvailable(m_wantedPrivilege);
		}
	}
}
