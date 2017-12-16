using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.AI.Events;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class IvenAIHandler : MonsterAIHandler
	{
		private Int32 spawnerID = 99995;

		public IvenAIHandler(Monster p_owner) : base(p_owner)
		{
			m_aiEvents.Add(new AIEventHealthPercent(0.85f, m_owner));
			m_aiEvents[0].OnTrigger += StartSecondPhase;
			m_aiEvents.Add(new AIEventHealthPercent(0.35f, m_owner));
			m_aiEvents[1].OnTrigger += StartFinalPhase;
		}

		protected override void CheckAIEvents()
		{
			for (Int32 i = 0; i < m_aiEvents.Count; i++)
			{
				m_aiEvents[i].Update();
			}
		}

		private void StartSecondPhase()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			SpawnMonster(new Position(12, 2), grid, 250, EDirection.EAST);
			SpawnMonster(new Position(12, 2), grid, 250, EDirection.EAST);
		}

		private void StartFinalPhase()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			SpawnMonster(new Position(10, 5), grid, 250, EDirection.SOUTH);
			SpawnMonster(new Position(10, 5), grid, 720, EDirection.SOUTH);
		}

		private void SpawnMonster(Position p_position, Grid p_grid, Int32 p_staticID, EDirection p_direction)
		{
			Sign sign = new Sign(1, spawnerID);
			sign.Position = p_position;
			sign.Location = p_direction;
			SpawnCommand spawnCommand = new SpawnCommand();
			spawnCommand.Type = EInteraction.SPAWN_MONSTER;
			spawnCommand.RequiredState = EInteractiveObjectState.NONE;
			spawnCommand.Timing = EInteractionTiming.ON_EXECUTE;
			spawnCommand.TargetSpawnID = sign.SpawnerID;
			spawnCommand.Extra = p_staticID.ToString();
			spawnCommand.ActivateCount = -1;
			sign.Commands.Add(spawnCommand);
			sign.Enabled = false;
			LegacyLogic.Instance.MapLoader.Grid.AddInteractiveObject(sign);
			LegacyLogic.Instance.UpdateManager.InteractionsActor.AddObject(sign);
			spawnerID++;
		}

		public override void Destroy()
		{
			m_aiEvents[0].OnTrigger -= StartSecondPhase;
			m_aiEvents[1].OnTrigger -= StartFinalPhase;
			base.Destroy();
		}
	}
}
