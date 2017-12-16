using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class OpenDoorInteraction : BaseDoorInteraction
	{
		private Boolean m_delayStateChange;

		public OpenDoorInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_delayStateChange = true;
		}

		public override void TriggerStateChange()
		{
			if (m_delayStateChange)
			{
				m_delayStateChange = false;
				SetStates();
				LegacyLogic.Instance.WorldManager.Party.AutoSelectMonster();
				LegacyLogic.Instance.UpdateManager.PartyTurnActor.CheckInCombat(false);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
				LegacyLogic.Instance.WorldManager.QuickSaveAllowed = true;
			}
		}

		protected override void SetStates()
		{
			if (m_targetDoor == null)
			{
				throw new InvalidOperationException("Tried to open something that is not a door!");
			}
			if (!m_delayStateChange)
			{
				m_targetDoor.Open();
				m_leaveTransition.TransitionType = EGridTransitionType.OPEN;
				if (m_enterTransition != null)
				{
					m_enterTransition.TransitionType = EGridTransitionType.OPEN;
				}
			}
			else
			{
				m_delayStateChange = true;
			}
		}

		protected override EInteractiveObjectState TargetDoorState()
		{
			return EInteractiveObjectState.DOOR_OPEN;
		}
	}
}
