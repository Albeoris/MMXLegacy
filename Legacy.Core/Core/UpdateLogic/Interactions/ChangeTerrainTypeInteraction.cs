using System;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class ChangeTerrainTypeInteraction : BaseInteraction
	{
		public const Int32 PARAM_COUNT = 2;

		private ETerrainType m_oldTerrain;

		private ETerrainType m_newTerrain;

		protected InteractiveObject m_parent;

		protected InteractiveObject m_targetObj;

		public ChangeTerrainTypeInteraction()
		{
		}

		public ChangeTerrainTypeInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_parent = Grid.FindInteractiveObject(m_parentID);
			m_targetObj = Grid.FindInteractiveObject(m_targetSpawnID);
		}

		protected override void DoExecute()
		{
			if (m_targetObj != null)
			{
				GridSlot slot = Grid.GetSlot(m_targetObj.Position);
				if (slot.TerrainType == m_oldTerrain)
				{
					slot.TerrainType = m_newTerrain;
				}
			}
			else
			{
				Position p_pos = new Position(0, 0);
				for (Int32 i = 0; i < Grid.Width; i++)
				{
					for (Int32 j = 0; j < Grid.Height; j++)
					{
						p_pos.X = i;
						p_pos.Y = j;
						GridSlot slot2 = Grid.GetSlot(p_pos);
						if (slot2.TerrainType == m_oldTerrain)
						{
							slot2.TerrainType = m_newTerrain;
						}
					}
				}
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
			m_oldTerrain = (ETerrainType)Enum.Parse(typeof(ETerrainType), array[0]);
			m_newTerrain = (ETerrainType)Enum.Parse(typeof(ETerrainType), array[1]);
		}
	}
}
