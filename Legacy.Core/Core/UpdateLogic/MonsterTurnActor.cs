using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.AI.MonsterGroups;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic
{
	public class MonsterTurnActor : TurnActor
	{
		internal List<Monster> m_monsters = new List<Monster>();

		internal List<Monster> m_addMonstersAfterTurn = new List<Monster>();

		internal List<Monster> m_removeMonstersAfterTurn = new List<Monster>();

		private Boolean m_oneAggro;

		public override void AddEntity(BaseObject entity)
		{
			base.AddEntity(entity);
			if (entity is Monster)
			{
				m_addMonstersAfterTurn.Add((Monster)entity);
			}
		}

		public override void RemoveEntity(BaseObject entity)
		{
			base.RemoveEntity(entity);
			if (entity is Monster)
			{
				m_removeMonstersAfterTurn.Add((Monster)entity);
			}
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (party.AttackableCharacterCount == 0)
			{
				m_stateMachine.ChangeState(EState.FINISHED);
				return;
			}
			if (State == EState.IDLE)
			{
			    UpdateIdleState(grid, party);
			}
			else if (State == EState.RUNNING)
			{
			    UpdateRunningState(party, grid);
			}
			foreach (Monster monster5 in m_monsters)
			{
				monster5.Update();
				monster5.AiHandler.Update();
			}
		}

	    private void UpdateIdleState(Grid grid, Party party)
	    {
	        if (m_monsters.Count <= 0)
	        {
	            m_stateMachine.ChangeState(EState.FINISHED);
	            return;
	        }

	        Boolean firstAgroProcessed = false;
	        Boolean hasAgroBeforeTurn = false;
	        foreach (Monster monster in m_monsters)
	        {
	            if (monster.IsAggro)
	                hasAgroBeforeTurn = true;
	        }

	        InitMonsterTurns();

	        m_oneAggro = false;
	        foreach (Monster monster in m_monsters)
	        {
	            if (monster.IsAggro)
	            {
	                m_oneAggro = true;
	                if (!firstAgroProcessed)
	                {
	                    firstAgroProcessed = true;
	                    LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_ROUND_STARTED, EventArgs.Empty);
	                    if (!hasAgroBeforeTurn)
	                    {
	                        BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(monster, monster.Position);
	                        LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTERS_TURNED_AGGRO, p_eventArgs);
	                    }
	                }
	                monster.AiHandler.DoTurn(grid, party);
	            }
	            else
	            {
	                monster.SkipMovement.Trigger();
	            }
	        }

	        AvoidPermanentAgro();
	        m_stateMachine.ChangeState(EState.RUNNING);
	    }

	    private void AvoidPermanentAgro()
	    {
	        Boolean updateHUDDangerSense = false;

	        foreach (IGrouping<Int32, Monster> monsterGroup in m_monsters.GroupBy(m => m.MonsterGroupID))
	        {
	            foreach (Monster monster in monsterGroup)
	            {
	                if (!monster.IsAggro)
	                    goto nextGroup;

	                if (monster.AggroRange >= monster.DistanceToParty)
	                    goto nextGroup;

	                if (!monster.SkipMovement.IsTriggered)
	                    goto nextGroup;
                    
	                if (monster.StartMovement.IsTriggered)
	                    goto nextGroup;
	            }

	            // Reset agro for group
	            foreach (Monster monster in monsterGroup)
	                monster.IsAggro = false;

	            updateHUDDangerSense = true;

	            nextGroup:
	            continue;
	        }

	        if (updateHUDDangerSense)
	            LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_HUD_DANGER_SENSE, EventArgs.Empty);
	    }

	    private void UpdateRunningState(Party party, Grid grid)
	    {
	        Boolean flag3 = true;
	        foreach (Monster monster3 in m_monsters)
	        {
	            if (monster3.State != Monster.EState.ACTION_FINISHED && monster3.State != Monster.EState.IDLE)
	            {
	                flag3 = false;
	                break;
	            }
	            if (!monster3.AiHandler.IsFinished)
	            {
	                flag3 = false;
	                break;
	            }
	        }
	        if (flag3)
	        {
	            m_stateMachine.ChangeState(EState.FINISHED);
	            for (Int32 i = m_monsters.Count - 1; i >= 0; i--)
	            {
	                m_monsters[i].TurnIdle.Trigger();
	                m_monsters[i].EndTurn();
	            }
	            LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_ROUND_FINISHED, EventArgs.Empty);
	            if (m_oneAggro)
	            {
	                foreach (Monster monster4 in m_monsters)
	                {
	                    monster4.AiHandler.UpdateDistanceToParty(party, grid);
	                }
	            }
	        }
	    }

	    public void InitMonsterTurns()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			foreach (Monster monster in m_monsters)
			{
				monster.AiHandler.UpdateDistanceToParty(party, grid);
				monster.CheckSightAggro();
				monster.AiHandler.StartAITurn();
			}
			m_monsters.Sort(delegate(Monster a, Monster b)
			{
				Int32 num = a.DistanceToParty.CompareTo(b.DistanceToParty);
				if (num == 0)
				{
					num = b.MovePrio.CompareTo(a.MovePrio);
				}
				return num;
			});
		}

		public override void StartTurn()
		{
			base.StartTurn();
			foreach (Monster item in m_addMonstersAfterTurn)
			{
				m_monsters.Add(item);
			}
			foreach (Monster item2 in m_removeMonstersAfterTurn)
			{
				m_monsters.Remove(item2);
			}
			m_addMonstersAfterTurn.Clear();
			m_removeMonstersAfterTurn.Clear();
			for (Int32 i = 0; i < m_monsters.Count; i++)
			{
				m_monsters[i].ResetRoundValues();
			}
		}

		public override void FinishTurn()
		{
			foreach (Monster item in m_addMonstersAfterTurn)
			{
				m_monsters.Add(item);
			}
			foreach (Monster item2 in m_removeMonstersAfterTurn)
			{
				m_monsters.Remove(item2);
			}
			m_addMonstersAfterTurn.Clear();
			m_removeMonstersAfterTurn.Clear();
		}

		public void OverrideAggroRanges(Int32 p_range)
		{
			foreach (Monster monster in m_monsters)
			{
				monster.IntruderAlertAggroRange = p_range;
			}
		}

		public override void ExecutionBreak()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("MonsterTurnActor execution aborded!");
			try
			{
				stringBuilder.AppendLine("Left state: " + m_stateMachine.CurrentState.Id);
				stringBuilder.AppendLine("Monster count: " + m_monsters.Count);
				stringBuilder.AppendLine("-----monsters-----");
				foreach (Monster monster in m_monsters)
				{
					stringBuilder.AppendLine("Monster: " + monster.Name);
					stringBuilder.AppendLine("Monster: State " + monster.State);
					stringBuilder.AppendLine("Monster: Has aggro " + monster.IsAggro);
					stringBuilder.AppendLine("Monster: Is movement skipped " + monster.SkipMovement.IsTriggered);
					stringBuilder.AppendLine("Monster: Is movement started " + monster.StartMovement.IsTriggered);
					stringBuilder.AppendLine("Monster: Is attacking done " + monster.AttackingDone.IsTriggered);
					stringBuilder.AppendLine("Monster: Is direction " + monster.Direction);
					stringBuilder.AppendLine(String.Concat(new Object[]
					{
						"Position: x=",
						monster.Position.X,
						" y=",
						monster.Position.Y
					}));
					stringBuilder.AppendLine("Monster: Spawner id " + monster.SpawnerID);
					stringBuilder.AppendLine("Movement done: " + monster.MovementDone.IsTriggered);
					stringBuilder.AppendLine("Monster: Health " + monster.CurrentHealth);
					stringBuilder.AppendLine("Monster: Distance to party " + monster.DistanceToParty);
					stringBuilder.AppendLine("Monster: General block attempts " + monster.GeneralBlockAttempts);
					stringBuilder.AppendLine("Monster: Move prio " + monster.MovePrio);
					stringBuilder.AppendLine("Monster: Can cast spell " + monster.CombatHandler.CanCastSpell);
					stringBuilder.AppendLine("Monster: Can move " + monster.CombatHandler.CanMove);
					stringBuilder.AppendLine("-----------");
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine("Error during dump generation: " + ex.StackTrace);
			}
			LegacyLogger.Log(stringBuilder.ToString(), false);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MONSTER_ROUND_FINISHED, EventArgs.Empty);
			m_stateMachine.ChangeState(EState.FINISHED);
		}

		public override void Clear()
		{
			base.Clear();
			m_monsters.Clear();
			m_addMonstersAfterTurn.Clear();
			m_removeMonstersAfterTurn.Clear();
		}

		public override void ClearAndDestroy()
		{
			foreach (Monster monster in m_monsters)
			{
				monster.Destroy();
			}
			foreach (Monster monster2 in m_addMonstersAfterTurn)
			{
				monster2.Destroy();
			}
			Clear();
			base.ClearAndDestroy();
		}
	}
}
