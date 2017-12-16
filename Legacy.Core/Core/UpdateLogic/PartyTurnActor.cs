using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic.Actions;
using Legacy.Utilities;

namespace Legacy.Core.UpdateLogic
{
	public class PartyTurnActor : TurnActor
	{
		private const Single WAIT_TIMEOUT = 5f;

		private Boolean m_waitFinishActions;

		internal BaseAction[][] m_actions;

		internal List<BaseAction> m_activeActions = new List<BaseAction>();

		private List<BaseAction> m_finishedActions = new List<BaseAction>();

		private Boolean m_interactionOnlyRound;

		private Boolean m_gameOver;

		private Boolean m_turnsReseted;

		private Int32 m_cantDoAnythingCounter;

		private ReleaseLockRequest m_lock;

		private Command m_delayedCommand;

		public PartyTurnActor()
		{
			MoveAction moveAction = new MoveAction(m_activeActions);
			RotateAction rotateAction = new RotateAction(m_activeActions);
			RestAction restAction = new RestAction();
			InteractAction interactAction = new InteractAction();
			SelectNextInteractiveObjectAction selectNextInteractiveObjectAction = new SelectNextInteractiveObjectAction();
			m_actions = new BaseAction[4][];
			for (Int32 i = 0; i < m_actions.Length; i++)
			{
				m_actions[i] = new BaseAction[11];
				m_actions[i][0] = moveAction;
				m_actions[i][1] = rotateAction;
				m_actions[i][6] = restAction;
				m_actions[i][2] = interactAction;
				m_actions[i][10] = selectNextInteractiveObjectAction;
				m_actions[i][3] = new MeleeAttackAction(i);
				m_actions[i][4] = new RangedAttackAction(i);
				m_actions[i][9] = new CastSpellAction(i);
				m_actions[i][5] = new ConsumeItemAction(i);
				m_actions[i][8] = new DefendAction(i);
				m_actions[i][7] = new EquipAction(i);
			}
		}

		public List<BaseAction> ActiveActions => m_activeActions;

	    public void SetLock(ReleaseLockRequest p_releaseLockRequest)
		{
			m_lock = p_releaseLockRequest;
		}

		public void ReleaseLock()
		{
			m_lock = null;
			if (m_delayedCommand != null)
			{
				ProcessCommand(m_delayedCommand);
			}
			m_delayedCommand = null;
		}

		public void CancelReleaseLockRequest()
		{
			if (m_delayedCommand != null)
			{
				m_delayedCommand.CancelCommand();
			}
			m_delayedCommand = null;
		}

		public void PassiveUpdateTurn()
		{
			HandleActiveActions();
			if (m_activeActions.Count == 0)
			{
				Command command = LegacyLogic.Instance.CommandManager.PullCommand();
				if (command is RotateCommand)
				{
					ProcessCommand(command);
				}
				else if (command != null)
				{
					command.CancelCommand();
				}
			}
		}

		public override void UpdateTurn()
		{
			m_turnsReseted = false;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			HandleActiveActions();
			if (!m_waitFinishActions && !party.FightRoundDone() && !LegacyLogic.Instance.MapLoader.IsLoading && m_delayedCommand == null)
			{
				Command command = LegacyLogic.Instance.CommandManager.PullCommand();
				if (command != null)
				{
					ProcessCommand(command);
				}
			}
			CheckGameOver(false);
		}

		private void HandleActiveActions()
		{
			for (Int32 i = m_activeActions.Count - 1; i >= 0; i--)
			{
				BaseAction baseAction = m_activeActions[i];
				if (baseAction.IsActionDone())
				{
					m_activeActions.RemoveAt(i);
					m_finishedActions.Add(baseAction);
				}
			}
			foreach (BaseAction baseAction2 in m_finishedActions)
			{
				baseAction2.Finish();
			}
			m_finishedActions.Clear();
			if (m_waitFinishActions && m_activeActions.Count == 0)
			{
				m_waitFinishActions = false;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
			}
		}

		internal void CheckGameOver(Boolean p_startTurn)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Boolean flag = true;
			Boolean flag2 = true;
			for (Int32 i = 0; i < party.Members.Length; i++)
			{
				if (party.Members[i] != null && !party.Members[i].ConditionHandler.CantDoAnythingForGameOver())
				{
					flag = false;
				}
				if (party.Members[i] != null && !party.Members[i].ConditionHandler.CantDoAnything())
				{
					flag2 = false;
				}
			}
			if (p_startTurn)
			{
				if (flag2)
				{
					m_cantDoAnythingCounter++;
					if (m_cantDoAnythingCounter > ConfigManager.Instance.Game.CantDoAnythingTurnCount)
					{
						flag = true;
					}
				}
				else
				{
					m_cantDoAnythingCounter = 0;
				}
			}
			if (flag && !m_gameOver)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAME_OVER, EventArgs.Empty);
				LegacyLogic.Instance.TrackingManager.TrackPartyDied();
				m_gameOver = true;
			}
		}

		private void ProcessCommand(Command p_command)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			BaseAction baseAction = m_actions[party.CurrentCharacter][(Int32)p_command.Type];
			if (baseAction.ActionAvailable())
			{
				if (baseAction.CanDoAction(p_command))
				{
					if (!baseAction.CanBeDelayedByLock() || m_lock == null)
					{
						baseAction.DoAction(p_command);
						m_activeActions.Add(baseAction);
						if (baseAction.ConsumeType == BaseAction.EConsumeType.CONSUME_CHARACTER_TURN)
						{
							if (party.HasAggro())
							{
								Boolean flag = false;
								if (party.SelectedCharacter.Class.IsBlademaster() && baseAction is MeleeAttackAction && (baseAction as MeleeAttackAction).MonsterWasKilled)
								{
									flag = true;
									party.AutoSelectMonster();
									(baseAction as MeleeAttackAction).MonsterWasKilled = false;
									if (party.SelectedMonster != null)
									{
										ExtraAttackEventArgs p_args = new ExtraAttackEventArgs(party.SelectedCharacter);
										party.SelectedCharacter.FightHandler.FeedActionLog(p_args);
									}
								}
								if (!flag && !m_turnsReseted)
								{
									party.FinishCurrentCharacterTurn();
								}
							}
						}
						else if (baseAction.ConsumeType == BaseAction.EConsumeType.CONSUME_PARTY_TURN)
						{
							if (baseAction is InteractAction)
							{
								m_interactionOnlyRound = true;
							}
							party.FinishPartyRound();
						}
						CheckInCombat(false);
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
					}
					else
					{
						m_delayedCommand = p_command;
						m_lock();
					}
				}
				else
				{
					baseAction.DontDoAction(p_command);
					p_command.CancelCommand();
				}
			}
			else
			{
				p_command.CancelCommand();
				if (LegacyLogic.Instance.WorldManager.Party.InCombat)
				{
					Command.ECommandTypes type = p_command.Type;
					if (type != Command.ECommandTypes.MOVE)
					{
						if (type == Command.ECommandTypes.REST)
						{
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.CANT_ACT);
						}
					}
					else
					{
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.CANT_ACT);
					}
				}
				if (LegacyLogic.Instance.WorldManager.Party.HasAggro())
				{
					Command.ECommandTypes type = p_command.Type;
					if (type != Command.ECommandTypes.INTERACT)
					{
						if (type == Command.ECommandTypes.REST)
						{
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.REST_DANGER);
						}
					}
					else
					{
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.CANT_ACT);
					}
				}
			}
		}

		public void MoveBySpell(MoveCommand p_command)
		{
			BaseAction baseAction = m_actions[0][0];
			baseAction.DoAction(p_command);
			m_activeActions.Add(baseAction);
			m_waitFinishActions = true;
			if (!LegacyLogic.Instance.WorldManager.Party.HasAggro())
			{
				LegacyLogic.Instance.WorldManager.Party.FinishPartyRound();
				CheckInCombat(false);
			}
		}

		public Boolean CanDoCommand(Command p_command, Int32 p_character)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			BaseAction baseAction = m_actions[p_character][(Int32)p_command.Type];
			return baseAction != null && !party.FightRoundDone() && !m_waitFinishActions && baseAction.ActionAvailable() && baseAction.CanDoAction(p_command);
		}

		public override Boolean IsFinished()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (!party.FightRoundDone())
			{
				return false;
			}
			Boolean result = true;
			foreach (BaseAction baseAction in m_activeActions)
			{
				if (!baseAction.CanProgressBeforeActionIsDone())
				{
					result = false;
				}
			}
			return result;
		}

		public override void StartTurn()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			LegacyLogic.Instance.WorldManager.AchievementManager.ResetRoundData();
			if (!m_interactionOnlyRound)
			{
				for (Int32 i = 0; i < party.Members.Length; i++)
				{
					if (party.Members[i] != null)
					{
						party.Members[i].ConditionHandler.DoPoisonDamage();
						if (party.SelectedMonster == null || (party.SelectedMonster != null && !party.SelectedMonster.AttackingDone.IsTriggered))
						{
							party.Members[i].ConditionHandler.FlushDelayedActionLog();
						}
					}
				}
			}
			if (party.HasAggro())
			{
				LegacyLogic.Instance.CommandManager.ClearQueue();
			}
			party.Buffs.FlushActionLog();
			party.IsPushed = false;
			CheckGameOver(true);
			if (m_activeActions.Count > 0)
			{
				m_waitFinishActions = true;
			}
			for (Int32 j = 0; j < party.Members.Length; j++)
			{
				if (party.Members[j] != null && party.Members[j].IsDefendModeActive)
				{
					party.Members[j].FightHandler.ResetDefendMode();
				}
			}
			party.StartTurn();
			party.Buffs.Update();
			m_interactionOnlyRound = false;
			CheckInCombat(false);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
		}

		public void CheckInCombat(Boolean p_resetTurn)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Boolean inCombat = party.InCombat;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			party.InCombat = grid.CheckInCombat(party.Position);
			if (p_resetTurn && inCombat && !party.InCombat)
			{
				Boolean flag = true;
				Boolean flag2 = false;
				for (Int32 i = 0; i < 4; i++)
				{
					flag2 |= party.GetMember(i).DoneTurn;
					flag &= party.GetMember(i).DoneTurn;
				}
				if (flag2 && !flag)
				{
					m_turnsReseted = true;
					party.StartTurn();
				}
			}
		}

		public override void FinishTurn()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			foreach (Character character in party.Members)
			{
				if (character != null)
				{
					character.ConditionHandler.Update();
					character.ConditionHandler.FlushActionLog();
					character.EndPartyTurn();
				}
			}
			party.HirelingHandler.ResetHirelingTurn();
			party.Buffs.FlushActionLog();
		}

		public override void ExecutionBreak()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("PartyTurnActor execution aborded!");
			try
			{
				stringBuilder.AppendLine("Left state: " + m_stateMachine.CurrentState.Id);
				foreach (BaseAction baseAction in m_activeActions)
				{
					stringBuilder.AppendLine("Active action: " + baseAction.ToString());
				}
				if (party.SelectedCharacter != null)
				{
					stringBuilder.AppendLine("Selected character: " + party.SelectedCharacter.Name);
				}
				if (party.SelectedMonster != null)
				{
					stringBuilder.AppendLine("Selected monster: " + party.SelectedMonster.Name);
				}
				stringBuilder.AppendLine("Height: " + party.Height);
				stringBuilder.AppendLine("Gold: " + party.Gold);
				stringBuilder.AppendLine("Supplies: " + party.Supplies);
				stringBuilder.AppendLine("Movement done: " + party.MovementDone.IsTriggered);
				stringBuilder.AppendLine(String.Concat(new Object[]
				{
					"Position: x=",
					party.Position.X,
					" y=",
					party.Position.Y
				}));
				stringBuilder.AppendLine("Direction: " + party.Direction);
				stringBuilder.AppendLine("-----characters-----");
				foreach (Character character in party.Members)
				{
					stringBuilder.AppendLine("Character: name " + character.Name);
					stringBuilder.AppendLine("Character: class " + character.Class.Class);
					stringBuilder.AppendLine("Character: attribute destiny " + character.CurrentAttributes.Destiny);
					stringBuilder.AppendLine("Character: attribute magic " + character.CurrentAttributes.Magic);
					stringBuilder.AppendLine("Character: attribute might " + character.CurrentAttributes.Might);
					stringBuilder.AppendLine("Character: attribute perception " + character.CurrentAttributes.Perception);
					stringBuilder.AppendLine("Character: health points " + character.HealthPoints);
					stringBuilder.AppendLine("Character: mana points " + character.ManaPoints);
					stringBuilder.AppendLine("Character: level " + character.Level);
					stringBuilder.AppendLine("Character: turn done " + character.DoneTurn);
					stringBuilder.AppendLine("Character: Can do anything " + character.ConditionHandler.CantDoAnything());
					stringBuilder.AppendLine("Character: Conditions: " + character.ConditionHandler.Condition);
					stringBuilder.AppendLine("-----------");
				}
				LegacyLogger.Log(stringBuilder.ToString(), false);
				LegacyLogic.Instance.CommandManager.ClearContiniousQueue();
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine("Error during dump generation: " + ex.StackTrace);
			}
			m_stateMachine.ChangeState(EState.FINISHED);
			party.StartTurn();
		}

		public override void Clear()
		{
			base.Clear();
			m_waitFinishActions = false;
			m_activeActions.Clear();
			m_finishedActions.Clear();
			m_gameOver = false;
		}

		public override void ClearAndDestroy()
		{
			Clear();
			base.ClearAndDestroy();
		}

		public delegate void ReleaseLockRequest();
	}
}
