using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core.Entities;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic
{
	public class SummonTurnActor : TurnActor
	{
		private ESummonExecutionOrder m_execOrder;

		private Boolean m_turnStated;

		private List<MovingEntity> m_removedEntities = new List<MovingEntity>();

		private List<MovingEntity> m_entities = new List<MovingEntity>();

		public SummonTurnActor(ESummonExecutionOrder p_execOrder)
		{
			m_execOrder = p_execOrder;
		}

		public List<MovingEntity> Entities => m_entities;

	    public override void AddEntity(BaseObject entity)
		{
			base.AddEntity(entity);
			if (!(entity is Summon) || ((Summon)entity).StaticData.ExecutionOrder != m_execOrder)
			{
				return;
			}
			m_entities.Add((MovingEntity)entity);
		}

		public override void RemoveEntity(BaseObject entity)
		{
			base.RemoveEntity(entity);
			if (!(entity is Summon))
			{
				return;
			}
			if (m_turnStated)
			{
				m_removedEntities.Add((MovingEntity)entity);
			}
			else
			{
				m_entities.Remove((MovingEntity)entity);
			}
		}

		public override void StartTurn()
		{
			m_turnStated = true;
			base.StartTurn();
			foreach (MovingEntity movingEntity in m_entities)
			{
				if (!movingEntity.IsDestroyed)
				{
					movingEntity.BeginTurn();
				}
			}
			CleanupEntityList();
			m_stateMachine.ChangeState(EState.RUNNING);
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			Boolean flag = true;
			foreach (MovingEntity movingEntity in m_entities)
			{
				if (!movingEntity.IsDestroyed)
				{
					if (!movingEntity.IsFinishTurn)
					{
						movingEntity.UpdateTurn();
					}
					flag &= movingEntity.IsFinishTurn;
				}
			}
			CleanupEntityList();
			if (flag)
			{
				m_stateMachine.ChangeState(EState.FINISHED);
			}
		}

		public override void FinishTurn()
		{
			base.FinishTurn();
			foreach (MovingEntity movingEntity in m_entities)
			{
				if (!movingEntity.IsDestroyed)
				{
					movingEntity.EndTurn();
				}
			}
			CleanupEntityList();
			m_turnStated = false;
		}

		public override void ExecutionBreak()
		{
			base.ExecutionBreak();
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Entity count " + m_entities.Count);
				stringBuilder.AppendLine("-----Not finish turn entities-----");
				foreach (MovingEntity movingEntity in m_entities)
				{
					if (!movingEntity.IsFinishTurn)
					{
						stringBuilder.AppendLine(String.Concat(new Object[]
						{
							"StaticID=",
							movingEntity.StaticID,
							"; SpawnerID=",
							movingEntity.SpawnerID
						}));
						stringBuilder.AppendLine(movingEntity.ToString());
						stringBuilder.AppendLine();
					}
				}
				LegacyLogger.Log(stringBuilder.ToString(), false);
			}
			catch (Exception arg)
			{
				LegacyLogger.Log("Error during dump generation: " + arg, false);
			}
		}

		public override void Clear()
		{
			base.Clear();
			m_entities.Clear();
			m_removedEntities.Clear();
		}

		public override void ClearAndDestroy()
		{
			Clear();
			base.ClearAndDestroy();
		}

		private void CleanupEntityList()
		{
			if (m_removedEntities.Count > 0)
			{
				foreach (MovingEntity item in m_removedEntities)
				{
					m_entities.Remove(item);
				}
			}
		}
	}
}
