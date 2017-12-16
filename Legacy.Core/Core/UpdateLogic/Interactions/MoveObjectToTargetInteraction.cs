using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class MoveObjectToTargetInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		protected InteractiveObject m_parent;

		protected Int32 m_moveTargetID;

		protected Boolean m_useMoveTargetOrientation;

		public MoveObjectToTargetInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		protected override void DoExecute()
		{
			InteractiveObject interactiveObject = Grid.FindInteractiveObject(m_targetSpawnID);
			if (interactiveObject == null)
			{
				throw new InvalidOperationException("Tried to move something that is not an object!");
			}
			InteractiveObject interactiveObject2 = Grid.FindInteractiveObject(m_moveTargetID);
			if (interactiveObject2 == null)
			{
				throw new InvalidOperationException("Tried to move to something that is not an object!");
			}
			if (interactiveObject == null)
			{
				return;
			}
			EDirection edirection = (!m_useMoveTargetOrientation) ? interactiveObject.Location : interactiveObject2.Location;
			MoveEntityEventArgs p_eventArgs = new MoveEntityEventArgs(interactiveObject.Position, interactiveObject2.Position, edirection);
			LegacyLogic.Instance.EventManager.InvokeEvent(interactiveObject, EEventType.MOVE_ENTITY, p_eventArgs);
			Grid.MoveObject(interactiveObject, interactiveObject2.Position);
			interactiveObject.Position = interactiveObject2.Position;
			interactiveObject.Location = edirection;
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
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length != 2)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					2
				}));
			}
			if (!Int32.TryParse(array[0], out m_moveTargetID))
			{
				throw new FormatException("First parameter " + array[0] + " was not an integer");
			}
			if (!Boolean.TryParse(array[1], out m_useMoveTargetOrientation))
			{
				throw new FormatException("Second parameter " + array[1] + " was not an boolean");
			}
		}
	}
}
