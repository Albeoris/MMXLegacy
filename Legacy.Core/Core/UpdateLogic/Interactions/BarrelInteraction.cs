using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class BarrelInteraction : BaseInteraction
	{
		private Barrel m_parent;

		private Character m_targetCharacter;

		public BarrelInteraction()
		{
		}

		public BarrelInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = (Grid.FindInteractiveObject(m_parentID) as Barrel);
			if (m_parent == null)
			{
				throw new InvalidOperationException("Tried to add a BarrelInteraction to something that is not a barrel!");
			}
		}

		protected override void Validate(Object p_sender, EventArgs p_args)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PRECONDITION_EVALUATED, new EventHandler(Validate));
			if (((PreconditionEvaluateArgs)p_args).Cancelled)
			{
				CancelInteraction();
			}
			else if (((PreconditionEvaluateArgs)p_args).Passed)
			{
				m_targetCharacter = ((PreconditionEvaluateArgs)p_args).Character;
				String successText = GetSuccessText(p_args);
				if (successText == String.Empty)
				{
					StartExecution();
				}
				else
				{
					ShowSuccess(successText);
				}
			}
		}

		public override void Execute()
		{
			m_parent.Open();
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
			m_parent.GiveBonus(m_targetCharacter);
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
