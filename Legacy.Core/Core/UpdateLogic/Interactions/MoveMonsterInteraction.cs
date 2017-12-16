using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class MoveMonsterInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		protected InteractiveObject m_parent;

		private Int32 m_posX;

		private Int32 m_posY;

		public MoveMonsterInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public MoveMonsterInteraction()
		{
		}

		protected override void DoExecute()
		{
			Monster monster = LegacyLogic.Instance.WorldManager.FindObjectBySpawnerId<Monster>(m_targetSpawnID);
			if (monster == null)
			{
				LegacyLogger.Log("Tried to move something that is not a monster " + m_targetSpawnID);
			}
			else
			{
				Position position = monster.Position;
				Position position2 = new Position(m_posX, m_posY);
				GridSlot slot = Grid.GetSlot(monster.Position);
				if (Grid.GetSlot(position2).AddEntity(monster))
				{
					slot.RemoveEntity(monster);
				}
				MoveEntityEventArgs p_eventArgs = new MoveEntityEventArgs(position, position2);
				LegacyLogic.Instance.EventManager.InvokeEvent(monster, EEventType.MOVE_ENTITY, p_eventArgs);
				FinishExecution();
			}
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
			if (!Int32.TryParse(array[0], out m_posX))
			{
				throw new FormatException("First parameter " + array[0] + " was not an integer");
			}
			if (!Int32.TryParse(array[1], out m_posY))
			{
				throw new FormatException("Second parameter " + array[1] + " was not an integer");
			}
		}
	}
}
