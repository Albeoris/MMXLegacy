using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Actions
{
	public class MoveAction : BaseAction
	{
		private List<BaseAction> m_activeActions;

		public MoveAction(List<BaseAction> p_activeActions)
		{
			m_activeActions = p_activeActions;
			m_consumeType = EConsumeType.CONSUME_PARTY_TURN;
		}

		public override void DoAction(Command p_command)
		{
			Grid grid = Grid;
			MoveCommand moveCommand = (MoveCommand)p_command;
			EDirection edirection = (EDirection)(((Int32)Party.Direction + (Int32)moveCommand.Direction) % (Int32)EDirection.COUNT);
			List<InteractiveObject> activeInteractiveObjects = grid.GetSlot(Party.Position).GetActiveInteractiveObjects(edirection);
			List<Trap> activeTraps = grid.GetSlot(Party.Position).GetActiveTraps(edirection);
			Boolean flag = grid.MoveEntity(Party, edirection);
			if (flag)
			{
				Party.MovementDone.Reset();
				LegacyLogic.Instance.GameTime.UpdateTime(grid.Type, ETimeChangeReason.Movement);
				GridSlot slot = grid.GetSlot(Party.Position);
				activeInteractiveObjects.AddRange(slot.GetActiveInteractiveObjects(EDirection.CENTER));
				activeTraps.AddRange(slot.GetActiveTraps(EDirection.CENTER));
				LegacyLogic.Instance.UpdateManager.InteractionsActor.AddObject(activeInteractiveObjects);
				if (!Party.HasAggro())
				{
					CheckChestInFront();
				}
				foreach (Trap trap in activeTraps)
				{
					trap.ResolveTrapEffect(grid, Party);
				}
				if (Party.HasSpotSecrets())
				{
					grid.SpotSecrets(Party.Position, Party.Direction);
				}
				if (Party.HasClairvoyance())
				{
					grid.SpotTraps(Party.Position, Party.Direction);
				}
				Party.SelectedInteractiveObject = null;
			}
		}

		public override void DontDoAction(Command p_command)
		{
			MoveCommand moveCommand = (MoveCommand)p_command;
			EDirection p_dir = (EDirection)(((Int32)Party.Direction + (Int32)moveCommand.Direction) % (Int32)EDirection.COUNT);
			Grid.ReboundOffWall(Party, p_dir);
		}

		public override Boolean ActionAvailable()
		{
			foreach (BaseAction baseAction in m_activeActions)
			{
				if (baseAction is RotateAction)
				{
					return false;
				}
			}
			return !Party.InCombat && !Party.FightRoundStarted();
		}

		public override Boolean CanDoAction(Command p_command)
		{
			MoveCommand moveCommand = (MoveCommand)p_command;
			EDirection p_dir = (EDirection)(((Int32)Party.Direction + (Int32)moveCommand.Direction) % (Int32)EDirection.COUNT);
			return !Party.InCombat && Grid.CanMoveEntity(Party, p_dir) && !LegacyLogic.Instance.ConversationManager.IsOpen;
		}

		public override Boolean IsActionDone()
		{
			return Party.MovementDone.IsTriggered;
		}

		public override Boolean CanProgressBeforeActionIsDone()
		{
			return true;
		}

		public override Boolean CanBeDelayedByLock()
		{
			return false;
		}

		public override void Finish()
		{
			Party.MovementDone.Reset();
			Party.AutoSelectInteractiveObject();
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
