using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Map;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities.AI.Behaviours
{
	public class TimeStopBehaviour : BaseSummonBehaviour
	{
		private List<Object> m_targetBuffer = new List<Object>();

		private StateMachine<EState> m_stateMachine;

		public TimeStopBehaviour(AIBrain p_brain, Summon p_controller) : base(p_brain, p_controller)
		{
			m_stateMachine = new StateMachine<EState>();
			m_stateMachine.AddState(new State<EState>(EState.IDLE));
			State<EState> state = new State<EState>(EState.DO_EFFECT);
			state.AddTransition(new Transition<EState>(EState.EFFECT_DONE, p_controller.AttackingDone));
			m_stateMachine.AddState(state);
			m_stateMachine.AddState(new State<EState>(EState.EFFECT_DONE));
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public override void BeginTurn()
		{
			base.BeginTurn();
			if (Position.Distance(Controller.Position, LegacyLogic.Instance.WorldManager.Party.Position) > Controller.StaticData.Range)
			{
				RemoveTimeStopBuff();
				Controller.Destroy();
				m_stateMachine.ChangeState(EState.EFFECT_DONE);
			}
			else
			{
				m_stateMachine.ChangeState(EState.IDLE);
			}
		}

		public override void UpdateTurn()
		{
			base.UpdateTurn();
			m_stateMachine.Update();
			if (m_stateMachine.CurrentStateID == EState.IDLE)
			{
				m_targetBuffer.Clear();
				GetAllTargetsInRange(m_targetBuffer, true, false);
				AddTimeStopBuff(m_targetBuffer);
				m_stateMachine.ChangeState(EState.EFFECT_DONE);
			}
			else if (m_stateMachine.CurrentStateID == EState.EFFECT_DONE)
			{
				Controller.IsFinishTurn = true;
			}
		}

		public override void EndTurn()
		{
			base.EndTurn();
			if (Controller.LifeTime <= 0)
			{
				RemoveTimeStopBuff();
			}
		}

		private void AddTimeStopBuff(List<Object> p_targets)
		{
			MonsterBuff p_buff = BuffFactory.CreateMonsterBuff(EMonsterBuffType.TIME_STOP, 1f);
			foreach (Object obj in p_targets)
			{
				Monster monster = obj as Monster;
				if (monster != null && !monster.BuffHandler.HasBuff(EMonsterBuffType.TIME_STOP))
				{
					monster.BuffHandler.AddBuff(p_buff);
				}
			}
		}

		private void RemoveTimeStopBuff()
		{
			m_targetBuffer.Clear();
			MonsterBuff p_buff = BuffFactory.CreateMonsterBuff(EMonsterBuffType.TIME_STOP, 1f);
			foreach (Monster monster in LegacyLogic.Instance.WorldManager.GetObjectsByTypeIterator<Monster>())
			{
				if (monster.BuffHandler.HasBuff(EMonsterBuffType.TIME_STOP))
				{
					monster.BuffHandler.RemoveBuff(p_buff);
				}
			}
		}

		public enum EState
		{
			IDLE,
			DO_EFFECT,
			EFFECT_DONE
		}
	}
}
