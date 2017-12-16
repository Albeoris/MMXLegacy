using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class InteractAction : BaseAction
	{
		public InteractAction()
		{
			m_consumeType = EConsumeType.CONSUME_PARTY_TURN;
		}

		public override void DoAction(Command p_command)
		{
			InteractCommand interactCommand = (InteractCommand)p_command;
			if (interactCommand.Target != null)
			{
				if (interactCommand.Target.Type == EObjectType.COMMAND_CONTAINER || (interactCommand.Target.Type == EObjectType.DOOR && interactCommand.Target.State == EInteractiveObjectState.DOOR_OPEN))
				{
					return;
				}
				LegacyLogic.Instance.UpdateManager.InteractionsActor.AddObject(interactCommand.Target);
			}
			else if (Party.SelectedInteractiveObject != null)
			{
				if (Party.SelectedInteractiveObject.Type == EObjectType.COMMAND_CONTAINER || (Party.SelectedInteractiveObject.Type == EObjectType.DOOR && Party.SelectedInteractiveObject.State == EInteractiveObjectState.DOOR_OPEN))
				{
					return;
				}
				LegacyLogic.Instance.UpdateManager.InteractionsActor.AddObject(Party.SelectedInteractiveObject);
			}
			else
			{
				Position position = Party.Position;
				GridSlot slot = Grid.GetSlot(position);
				List<InteractiveObject> passiveInteractiveObjects = slot.GetPassiveInteractiveObjects(Party.Direction, Party.HasSpotSecrets(), Party.HasClairvoyance(), false);
				passiveInteractiveObjects.AddRange(slot.GetPassiveInteractiveObjects(EDirection.CENTER, Party.HasSpotSecrets(), Party.HasClairvoyance(), false));
				if (passiveInteractiveObjects.Count > 0)
				{
					for (Int32 i = passiveInteractiveObjects.Count - 1; i >= 0; i--)
					{
						if (passiveInteractiveObjects[i].Type == EObjectType.COMMAND_CONTAINER)
						{
							passiveInteractiveObjects.RemoveAt(i);
						}
					}
					for (Int32 j = passiveInteractiveObjects.Count - 1; j >= 0; j--)
					{
						if (passiveInteractiveObjects[j].Type == EObjectType.DOOR && passiveInteractiveObjects[j].State == EInteractiveObjectState.DOOR_OPEN)
						{
							passiveInteractiveObjects.RemoveAt(j);
						}
					}
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.PASSIVE_INTERACTIVE_OBJECT_FOUND, new InteractiveObjectListEventArgs(passiveInteractiveObjects));
					LegacyLogic.Instance.UpdateManager.InteractionsActor.AddObject(passiveInteractiveObjects);
				}
			}
		}

		public override Boolean IsActionDone()
		{
			return true;
		}

		public override Boolean CanDoAction(Command p_command)
		{
			InteractCommand interactCommand = (InteractCommand)p_command;
			Position position = Party.Position;
			GridSlot slot = Grid.GetSlot(position);
			Boolean flag = interactCommand.Target != null;
			flag |= (Party.SelectedInteractiveObject != null);
			flag |= (slot.GetPassiveInteractiveObjects(Party.Direction, Party.HasSpotSecrets(), Party.HasClairvoyance(), false).Count > 0);
			return flag | slot.GetPassiveInteractiveObjects(EDirection.CENTER, Party.HasSpotSecrets(), Party.HasClairvoyance(), false).Count > 0;
		}

		public override Boolean ActionAvailable()
		{
			return !Party.HasAggro();
		}
	}
}
