using System;
using Legacy.Core.Api;
using Legacy.Core.Map;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Entrance : IndoorSceneBase
	{
		public Entrance() : this(0, 0)
		{
		}

		public Entrance(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.ENTRANCE, p_spawnerID)
		{
		}

		public override void Execute(Grid p_grid)
		{
			if (IsExecutable(p_grid) && QuestObjectives.Count > 0)
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.ObjectInteraction(this);
			}
			base.Execute(p_grid);
		}

		public void StartConversation(String p_level, Int32 p_targetSpawnID)
		{
			LegacyLogic.Instance.ConversationManager.OpenDungeonEntrance(this, p_level, p_targetSpawnID);
		}
	}
}
