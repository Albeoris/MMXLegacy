using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public abstract class BaseDoorInteraction : BaseInteraction
	{
		protected Door m_targetDoor;

		protected GridTransition m_leaveTransition;

		protected GridTransition m_enterTransition;

		protected InteractiveObject m_parent;

		public BaseDoorInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			m_targetDoor = (Door)Grid.FindInteractiveObject(m_targetSpawnID);
			if (m_targetDoor == null)
			{
				m_valid = false;
				return;
			}
			GridSlot slot = Grid.GetSlot(m_targetDoor.Position);
			m_leaveTransition = slot.GetTransition(m_targetDoor.Location);
			Position p_pos = m_targetDoor.Position + m_targetDoor.Location;
			GridSlot slot2 = Grid.GetSlot(p_pos);
			if (slot2 != null)
			{
				m_enterTransition = slot2.GetTransition(EDirectionFunctions.GetOppositeDir(m_targetDoor.Location));
			}
		}

		public InteractiveObject Target => m_targetDoor;

	    public override void ReleaseInteractLock()
		{
			m_targetDoor.InteractLock = false;
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

		protected abstract EInteractiveObjectState TargetDoorState();

		protected override void DoExecute()
		{
			LegacyLogic.Instance.WorldManager.QuickSaveAllowed = false;
			SetStates();
			if (!String.IsNullOrEmpty(m_targetDoor.Prefab))
			{
				if (!m_targetDoor.InteractLock)
				{
					m_targetDoor.InteractLock = true;
					EInteractiveObjectState p_targetState = TargetDoorState();
					m_targetDoor.UpdateNextState(p_targetState);
					DoorEntityEventArgs p_eventArgs = new DoorEntityEventArgs(m_targetDoor, true, p_targetState);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DOOR_STATE_CHANGED, p_eventArgs);
				}
			}
			else
			{
				TriggerStateChange();
			}
			FinishExecution();
		}

		protected abstract void SetStates();

		protected override void ParseExtra(String p_extra)
		{
		}
	}
}
