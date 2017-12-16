using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic
{
	public class SpawnTurnActor : TurnActor
	{
		internal List<Spawn> m_spawnObjects = new List<Spawn>();

		internal List<Spawn> m_invalidSpawners = new List<Spawn>();

		internal Boolean m_isFirstUpdate;

		private List<InteractiveObject> m_spawnedInteractiveObjects = new List<InteractiveObject>();

		public void AddSpawner(Spawn p_spawn)
		{
			m_spawnObjects.Add(p_spawn);
			m_invalidSpawners.Remove(p_spawn);
		}

		public void RemoveSpawner(Spawn p_spawn)
		{
			m_spawnObjects.Remove(p_spawn);
			m_invalidSpawners.Add(p_spawn);
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			if (State == EState.IDLE)
			{
				Position position = LegacyLogic.Instance.WorldManager.Party.Position;
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				for (Int32 i = m_invalidSpawners.Count - 1; i >= 0; i--)
				{
					Spawn spawn = m_invalidSpawners[i];
					if (spawn.ObjectType == EObjectType.MONSTER && spawn.InSpawnRange(position) && !m_spawnObjects.Contains(spawn))
					{
						m_invalidSpawners.RemoveAt(i);
						m_spawnObjects.Add(spawn);
					}
				}
				if (m_spawnObjects.Count > 0)
				{
					m_spawnedInteractiveObjects.Clear();
					for (Int32 j = m_spawnObjects.Count - 1; j >= 0; j--)
					{
						Spawn spawn2 = m_spawnObjects[j];
						BaseObject baseObject = LegacyLogic.Instance.WorldManager.FindObjectBySpawnerId(spawn2.ID);
						if (baseObject == null && spawn2.ObjectType == EObjectType.MONSTER && !spawn2.InSpawnRange(position))
						{
							m_spawnObjects.RemoveAt(j);
							m_invalidSpawners.Add(spawn2);
							spawn2.SetCanRespawn();
						}
						else if (baseObject == null)
						{
							BaseObject baseObject2 = spawn2.SpawnObject();
							if (baseObject2 != null)
							{
								spawn2.EndTurn();
								LegacyLogic.Instance.UpdateManager.SpawnTurnActor.RemoveSpawner(spawn2);
								LegacyLogic.Instance.WorldManager.InvalidSpawner.Add(spawn2.ID);
								Boolean flag = false;
								if (baseObject2 is MovingEntity)
								{
									MovingEntity movingEntity = (MovingEntity)baseObject2;
									if (!grid.AddMovingEntity(movingEntity.Position, movingEntity))
									{
										LegacyLogger.LogError(String.Concat(new Object[]
										{
											"Cannot add object to GRID! SpawnerID: ",
											movingEntity.SpawnerID,
											", staticID: ",
											movingEntity.StaticID
										}));
									}
									else
									{
										flag = true;
									}
								}
								else if (baseObject2 is InteractiveObject)
								{
									InteractiveObject interactiveObject = (InteractiveObject)baseObject2;
									grid.AddInteractiveObject(interactiveObject);
									interactiveObject.OnAfterCreate(grid);
									interactiveObject.OnAfterSpawn(grid);
									m_spawnedInteractiveObjects.Add(interactiveObject);
									flag = true;
								}
								if (flag)
								{
									LegacyLogic.Instance.WorldManager.SpawnObject(baseObject2, spawn2.Position);
								}
							}
						}
					}
					foreach (InteractiveObject interactiveObject2 in m_spawnedInteractiveObjects)
					{
						interactiveObject2.OnPrewarm(grid);
					}
					m_stateMachine.ChangeState(EState.RUNNING);
				}
				else
				{
					m_stateMachine.ChangeState(EState.FINISHED);
				}
			}
			else if (State == EState.RUNNING)
			{
				Boolean flag2 = true;
				foreach (Spawn spawn3 in m_spawnObjects)
				{
					if (spawn3.State != Spawn.EState.ACTION_FINISHED && spawn3.State != Spawn.EState.IDLE)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					if (m_isFirstUpdate)
					{
						LegacyLogic.Instance.MapLoader.OnLevelLoaded();
						m_isFirstUpdate = false;
					}
					m_stateMachine.ChangeState(EState.FINISHED);
					foreach (Spawn spawn4 in m_spawnObjects)
					{
						spawn4.TurnIdle.Trigger();
					}
				}
			}
			foreach (Spawn spawn5 in m_spawnObjects)
			{
				spawn5.Update();
			}
		}

		public override void ExecutionBreak()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SpawnTurnActor execution aborded!");
			stringBuilder.AppendLine("Left state: " + m_stateMachine.CurrentState.Id);
			stringBuilder.AppendLine("-----spawn objects-----");
			foreach (Spawn spawn in m_spawnObjects)
			{
				stringBuilder.AppendLine("Spawn object: " + spawn.ID);
				stringBuilder.AppendLine("Spawn object: direction " + spawn.Direction);
				stringBuilder.AppendLine(String.Concat(new Object[]
				{
					"Spawn object: position x=",
					spawn.Position.X,
					"y=",
					spawn.Position.Y
				}));
				stringBuilder.AppendLine("Spawn object: object id " + spawn.ObjectID);
				stringBuilder.AppendLine("Spawn object: state " + spawn.State);
				stringBuilder.AppendLine("-----------");
				spawn.TurnIdle.Trigger();
			}
			stringBuilder.AppendLine("Left state: " + m_stateMachine.CurrentState);
			LegacyLogger.Log(stringBuilder.ToString(), false);
			m_stateMachine.ChangeState(EState.FINISHED);
		}

		public override void Clear()
		{
			base.Clear();
			m_spawnObjects.Clear();
			m_invalidSpawners.Clear();
		}

		public override void ClearAndDestroy()
		{
			Clear();
			base.ClearAndDestroy();
		}
	}
}
