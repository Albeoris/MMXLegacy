using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.UpdateLogic.Interactions;

namespace Legacy.Core
{
	public class RechargerInteraction : BaseInteraction
	{
		protected RechargingObject m_parent;

		public RechargerInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = (Grid.FindInteractiveObject(m_parentID) as RechargingObject);
			if (m_parent == null)
			{
				throw new InvalidOperationException("Tried to add a RechargerInteraction to something that is not a Recharging Object!" + m_parentID);
			}
		}

		public override void Execute()
		{
			m_precondition = ParsePrecondition(m_preconditionString);
			if (m_precondition != null)
			{
				m_stateMachine.ChangeState(1);
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PRECONDITION_EVALUATED, new EventHandler(Validate));
				m_precondition.Trigger();
			}
			else
			{
				StartExecution();
			}
		}

		protected override void DoExecute()
		{
			m_parent.Interact();
			FinishExecution();
		}

		public override void FinishExecution()
		{
			if (m_parent != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_parent.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
		}

		protected override void ParseExtra(String p_extra)
		{
		}
	}
}
