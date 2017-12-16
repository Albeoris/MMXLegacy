using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class CloseDoorInteraction : BaseDoorInteraction
	{
		public CloseDoorInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
		}

		protected override void SetStates()
		{
			if (m_targetDoor == null)
			{
				throw new InvalidOperationException("Tried to close something that is not a door!");
			}
			m_targetDoor.Close();
			m_leaveTransition.TransitionType = EGridTransitionType.CLOSED;
			if (m_enterTransition != null)
			{
				m_enterTransition.TransitionType = EGridTransitionType.CLOSED;
			}
			LegacyLogic.Instance.WorldManager.QuickSaveAllowed = true;
		}

		protected override void ParseExtra(String p_extra)
		{
		}

		protected override EInteractiveObjectState TargetDoorState()
		{
			return EInteractiveObjectState.DOOR_CLOSED;
		}
	}
}
