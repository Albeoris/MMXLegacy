using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic.Interactions
{
	public class SpawnMonsterInteraction : BaseInteraction
	{
		private Int32 m_monsterID;

		private Int32 m_spawnAnim;

		private Int32 m_spawnID;

		private InteractiveObject m_spawnTarget;

		private InteractiveObject m_parent;

		public SpawnMonsterInteraction()
		{
		}

		public SpawnMonsterInteraction(SpawnCommand p_command, Int32 p_parentID, Int32 p_commandIndex) : base(p_command, p_parentID, p_commandIndex)
		{
			m_spawnTarget = Grid.FindInteractiveObject(m_targetSpawnID);
			m_parent = Grid.FindInteractiveObject(m_parentID);
		}

		public Int32 MonsterID => m_monsterID;

	    public Int32 SpawnAnim => m_spawnAnim;

	    protected override void DoExecute()
		{
			if (m_spawnTarget == null)
			{
				throw new InvalidOperationException("Tried to spawn a monster into an invalid target!");
			}
			Int32 spawnerID = GetSpawnerID();
			Monster monster = new Monster(m_monsterID, spawnerID);
			monster.Position = m_spawnTarget.Position;
			monster.Direction = m_spawnTarget.Location;
			monster.SpawnAnim = m_spawnAnim;
			if (Grid.AddMovingEntity(monster.Position, monster))
			{
				LegacyLogic.Instance.WorldManager.SpawnObject(monster, monster.Position);
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

		public override void PrewarmAfterCreate()
		{
			base.PrewarmAfterCreate();
			if (m_spawnTarget == null)
			{
				LegacyLogger.Log("ERROR: Will try to spawn a monster into an invalid target! SpawnMonsterInteraction: PrewarmAfterCreate skipped!");
			}
			else if (m_activateCount != 0)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ANNOUNCE_LATE_MONSTER_SPAWN, new AnnounceLateMonsterSpawnArgs(MonsterID, GetSpawnerID(), m_spawnTarget.Position));
			}
		}

		protected override void ParseExtra(String p_extra)
		{
			String[] array = p_extra.Split(new Char[]
			{
				','
			});
			if (array.Length < 1)
			{
				throw new FormatException(String.Concat(new Object[]
				{
					"Could not parse interaction params '",
					p_extra,
					"' because it contains ",
					array.Length,
					" arguments instead of ",
					2
				}));
			}
			if (!Int32.TryParse(array[0], out m_monsterID))
			{
				throw new FormatException("First parameter was not a monster ID!");
			}
			if (array.Length >= 2)
			{
				if (!Int32.TryParse(array[1], out m_spawnAnim))
				{
					throw new FormatException("Second parameter was not an int!");
				}
				if (array.Length > 2 && !Int32.TryParse(array[2], out m_spawnID))
				{
					throw new FormatException("Third parameter was not an int!");
				}
			}
		}

		private Int32 GetSpawnerID()
		{
			return (m_spawnID != 0) ? m_spawnID : m_targetSpawnID;
		}
	}
}
