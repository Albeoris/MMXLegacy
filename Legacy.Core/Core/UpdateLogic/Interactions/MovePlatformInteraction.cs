using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class MovePlatformInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 1;

		protected InteractiveObject m_parent;

		private EDirection m_dir;

		public MovePlatformInteraction()
		{
		}

		public MovePlatformInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
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
			Position position = interactiveObject.Position;
			Position position2 = interactiveObject.Position;
			Boolean flag = false;
			while (!flag)
			{
				GridSlot slot = Grid.GetSlot(position2 + m_dir);
				if ((slot.TerrainType & ETerrainType.LAVA) != ETerrainType.NONE)
				{
					position2 += m_dir;
				}
				else
				{
					flag = true;
				}
			}
			if (position != position2)
			{
				GridSlot slot2 = Grid.GetSlot(position);
				slot2.TerrainType = ETerrainType.LAVA;
				slot2 = Grid.GetSlot(position2);
				slot2.TerrainType = ETerrainType.PASSABLE;
				interactiveObject.Position = position2;
				Grid.MoveObject(interactiveObject, position2);
				MoveEntityEventArgs p_eventArgs = new MoveEntityEventArgs(position, position2);
				LegacyLogic.Instance.EventManager.InvokeEvent(interactiveObject, EEventType.PLATFORM_MOVED, p_eventArgs);
			}
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
			if (array.Length != 1)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params ",
					p_extra,
					" because it contains ",
					array.Length,
					" arguments instead of ",
					1
				}));
			}
			if (Enum.IsDefined(typeof(EDirection), array[0]))
			{
				m_dir = (EDirection)Enum.Parse(typeof(EDirection), array[0]);
				return;
			}
			throw new FormatException("First parameter was not an InteractiveObjectData key!");
		}
	}
}
