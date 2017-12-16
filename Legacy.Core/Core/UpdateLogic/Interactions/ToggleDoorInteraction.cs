using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ToggleDoorInteraction : BaseDoorInteraction
	{
		private Boolean m_delayStateChange = true;

		public ToggleDoorInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
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
				throw new InvalidOperationException("Tried to open or close something that is not a door!");
			}
			if (!m_delayStateChange)
			{
				m_targetDoor.ToggleState();
				m_leaveTransition.ToggleState();
				if (m_enterTransition != null)
				{
					m_enterTransition.ToggleState();
				}
				GridSlot neighborSlot = Grid.GetSlot(m_targetDoor.Position).GetNeighborSlot(Grid, m_targetDoor.Location);
				if (neighborSlot != null)
				{
					List<InteractiveObject> doors = neighborSlot.GetDoors();
					for (Int32 i = 0; i < doors.Count; i++)
					{
						if (doors[i].Type == EObjectType.DOOR && EDirectionFunctions.GetOppositeDir(doors[i].Location) == m_targetDoor.Location)
						{
							Door door = (Door)doors[i];
							if (m_targetDoor.State == EInteractiveObjectState.DOOR_OPEN)
							{
								door.Open();
							}
							else
							{
								door.Close();
							}
							break;
						}
					}
				}
			}
			else
			{
				m_delayStateChange = true;
			}
		}

		protected override EInteractiveObjectState TargetDoorState()
		{
			return (m_targetDoor.State != EInteractiveObjectState.DOOR_OPEN) ? EInteractiveObjectState.DOOR_OPEN : EInteractiveObjectState.DOOR_CLOSED;
		}
	}
}
