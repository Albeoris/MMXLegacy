using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class DisarmTrapInteraction : BaseInteraction
	{
		protected InteractiveObject m_interactiveObject;

		private List<Int32> m_trapsToDisarm;

		public DisarmTrapInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
		}

		protected override void ParseExtra(String p_extra)
		{
			m_trapsToDisarm = new List<Int32>();
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			for (Int32 i = 0; i < array.Length; i++)
			{
				Int32 item = Convert.ToInt32(array[i]);
				m_trapsToDisarm.Add(item);
			}
		}

		public Boolean IsExecutable()
		{
			return !DisarmingSelf() || m_interactiveObject.TrapActive;
		}

		public override void Execute()
		{
			m_precondition = ParsePrecondition(m_preconditionString);
			if (IsExecutable())
			{
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
			else
			{
				m_stateMachine.ChangeState(2);
			}
		}

		private Boolean DisarmingSelf()
		{
			return m_trapsToDisarm.Contains(m_targetSpawnID);
		}

		protected override void DoExecute()
		{
			if (LegacyLogic.Instance.WorldManager.Party.HasClairvoyance())
			{
				foreach (Int32 p_spawnerID in m_trapsToDisarm)
				{
					InteractiveObject interactiveObject = Grid.FindInteractiveObject(p_spawnerID);
					if (interactiveObject != null)
					{
						interactiveObject.TrapActive = false;
						LegacyLogic.Instance.EventManager.InvokeEvent(interactiveObject, EEventType.TRAP_DISARMED, EventArgs.Empty);
					}
				}
				Party party = LegacyLogic.Instance.WorldManager.Party;
				party.SelectedInteractiveObject = null;
				FinishExecution();
			}
			else
			{
				m_stateMachine.ChangeState(3);
			}
		}

		public override void FinishExecution()
		{
			if (m_interactiveObject != null && m_activateCount > 0)
			{
				m_activateCount--;
				m_interactiveObject.DecreaseActivate(m_commandIndex);
			}
			base.FinishExecution();
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
			else
			{
				m_interactiveObject.ResolveTrapEffect(Grid, LegacyLogic.Instance.WorldManager.Party);
				String failText = GetFailText(p_args);
				if (failText == String.Empty)
				{
					m_stateMachine.ChangeState(3);
				}
				else
				{
					ShowFailure(failText);
				}
			}
		}
	}
}
