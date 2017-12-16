using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class RotateAction : BaseAction
	{
		private List<BaseAction> m_activeActions;

		public RotateAction(List<BaseAction> p_activeActions)
		{
			m_activeActions = p_activeActions;
		}

		public override void DoAction(Command p_command)
		{
			Party.RotationDone.Reset();
			RotateCommand rotateCommand = (RotateCommand)p_command;
			Party.Rotate((Int32)rotateCommand.Rotation, false);
			if (Party.HasSpotSecrets())
			{
				Grid.SpotSecrets(Party.Position, Party.Direction);
			}
			if (Party.HasClairvoyance())
			{
				Grid.SpotTraps(Party.Position, Party.Direction);
			}
			if (!Party.HasAggro())
			{
				CheckChestInFront();
			}
			Party.SelectedInteractiveObject = null;
		}

		public override Boolean IsActionDone()
		{
			return Party.RotationDone.IsTriggered;
		}

		public override Boolean ActionAvailable()
		{
			return m_activeActions.Count == 0;
		}

		public override Boolean CanBeDelayedByLock()
		{
			return false;
		}

		public override Boolean CanDoAction(Command p_command)
		{
			return !LegacyLogic.Instance.MapLoader.IsLoading;
		}

		public override void Finish()
		{
			Party.RotationDone.Reset();
			Party.AutoSelectMonster();
			Party.AutoSelectInteractiveObject();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
		}

		private void CheckChestInFront()
		{
			List<InteractiveObject> passiveInteractiveObjects = Grid.GetSlot(Party.Position).GetPassiveInteractiveObjects(Party.Direction, Party.HasSpotSecrets(), Party.HasClairvoyance(), false);
			List<InteractiveObject> passiveInteractiveObjects2 = Grid.GetSlot(Party.Position).GetPassiveInteractiveObjects(EDirection.CENTER, Party.HasSpotSecrets(), Party.HasClairvoyance(), false);
			CheckChestInFront(passiveInteractiveObjects);
			CheckChestInFront(passiveInteractiveObjects2);
		}

		private void CheckChestInFront(List<InteractiveObject> p_interactiveObjectList)
		{
			foreach (InteractiveObject interactiveObject in p_interactiveObjectList)
			{
				if (interactiveObject is Container || interactiveObject is Barrel)
				{
					if ((interactiveObject is Container && !((Container)interactiveObject).IsEmpty()) || (interactiveObject is Barrel && ((Barrel)interactiveObject).State != EInteractiveObjectState.BARREL_EMPTY))
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(interactiveObject, EEventType.PARTY_INTERACTIVE_OBJECT_IN_FRONT, EventArgs.Empty);
					}
				}
				else
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(interactiveObject, EEventType.PARTY_INTERACTIVE_OBJECT_IN_FRONT, EventArgs.Empty);
				}
			}
		}
	}
}
