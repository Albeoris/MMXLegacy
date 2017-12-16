using System;
using Legacy.Core.Map;

namespace Legacy.Core.EventManagement
{
	public class AnnounceLateMonsterSpawnArgs : EventArgs
	{
		public AnnounceLateMonsterSpawnArgs(Int32 p_monsterID, Int32 p_spawnerID, Position p_spawnPosition)
		{
			MonsterID = p_monsterID;
			SpawnerID = p_spawnerID;
			MonsterPosition = p_spawnPosition;
		}

		public Int32 MonsterID { get; private set; }

		public Int32 SpawnerID { get; private set; }

		public Position MonsterPosition { get; private set; }
	}
}
