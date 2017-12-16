using System;
using Legacy.Core.Abilities;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Entities.AI.Events;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class AdiraAIHandler : MonsterAIHandler
	{
		private Int32 m_phase = 1;

		private AIEventHealthPercent m_percentEvent;

		private AIEventHealthPercent m_percentEvent2;

		private AIEventHealthPercent m_percentEvent3;

		private Boolean m_strategyA = true;

		private Int32 m_roundSubState;

		private Boolean m_subStateFinished;

		private Boolean m_castedFlickeringLight;

		private Boolean m_castedFlicker;

		public AdiraAIHandler(Monster p_owner) : base(p_owner)
		{
			m_percentEvent = new AIEventHealthPercent(0.84f, m_owner);
			m_percentEvent.OnTrigger += m_percentEvent_OnTrigger;
			m_percentEvent2 = new AIEventHealthPercent(0.67f, m_owner);
			m_percentEvent2.OnTrigger += m_percentEvent2_OnTrigger;
			m_percentEvent3 = new AIEventHealthPercent(0.5f, m_owner);
			m_percentEvent3.OnTrigger += m_percentEvent3_OnTrigger;
		}

		private void m_percentEvent_OnTrigger()
		{
			m_phase = 2;
		}

		private void m_percentEvent2_OnTrigger()
		{
			m_phase = 3;
		}

		private void m_percentEvent3_OnTrigger()
		{
			m_owner.Die(false);
			m_owner.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
			m_phase = 4;
			m_owner.m_stateMachine.ChangeState(Monster.EState.IDLE);
			m_owner.ForceAggro(false);
			m_owner.AlwaysTriggerAggro = false;
			m_owner.Disappear();
			LegacyLogic.Instance.UpdateManager.PartyTurnActor.CheckInCombat(true);
			m_isFinished = true;
			LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.MONSTER_DIED, EventArgs.Empty);
			LegacyLogic.Instance.WorldManager.Party.InCombat = false;
			LegacyLogic.Instance.CommandManager.AllowContinuousCommands = false;
			LegacyLogic.Instance.ConversationManager.OpenNpcDialog(LegacyLogic.Instance.WorldManager.NpcFactory.Get(276), 1);
		}

		protected override void CheckAIEvents()
		{
			m_percentEvent.Update();
			m_percentEvent2.Update();
			m_percentEvent3.Update();
		}

		public override void DoTurn(Grid p_grid, Party p_party)
		{
			m_owner.AbilityHandler.ExecuteAttacks(null, null, false, EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.AbilityHandler.FlushActionLog(EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.BuffHandler.ModifyMonsterValues();
			m_owner.CombatHandler.MeleeStrikes += m_owner.CombatHandler.MeleeStrikesRoundBonus;
			m_owner.CombatHandler.MeleeStrikesRoundBonus = 0;
			CheckAIEvents();
			m_subStateFinished = true;
			m_castedFlickeringLight = false;
		}

		public override void Update()
		{
			if (m_owner.IsAggro)
			{
				if ((!m_subStateFinished && m_owner.State == Monster.EState.IDLE) || m_owner.State == Monster.EState.ACTION_FINISHED)
				{
					m_subStateFinished = true;
					m_roundSubState++;
					LegacyLogic.Instance.WorldManager.Party.InCombat = LegacyLogic.Instance.MapLoader.Grid.CheckInCombat(LegacyLogic.Instance.WorldManager.Party.Position);
				}
				if (!m_isFinished && m_subStateFinished)
				{
					switch (m_phase)
					{
					case 1:
						HandlePhase1();
						break;
					case 2:
						HandlePhase2();
						break;
					case 3:
						HandlePhase3();
						break;
					}
				}
			}
		}

		public override void StartAITurn()
		{
			m_roundSubState = 0;
			m_isFinished = !m_owner.IsAggro;
			m_castedFlicker = false;
		}

		private void HandlePhase1()
		{
			m_subStateFinished = false;
			switch (m_roundSubState)
			{
			case 1:
				if (!LegacyLogic.Instance.WorldManager.Party.InCombat)
				{
					MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(37);
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					CastSpell(p_spell);
				}
				else if (m_owner.Direction == EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
				{
					m_roundSubState++;
					m_subStateFinished = true;
				}
				else
				{
					m_owner.Rotate(EDirectionFunctions.RotationCount(m_owner.Direction, EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position)), true);
				}
				break;
			case 2:
			case 3:
				if (LegacyLogic.Instance.WorldManager.Party.InCombat)
				{
					m_owner.CombatHandler.CastSpell = false;
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					m_owner.CombatHandler.DoAttack();
					if (m_roundSubState == 3)
					{
						m_isFinished = true;
					}
				}
				else
				{
					m_subStateFinished = true;
					m_isFinished = true;
					m_owner.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
				}
				break;
			}
		}

		private void HandlePhase2()
		{
			m_subStateFinished = false;
			switch (m_roundSubState)
			{
			case 0:
				if (!m_owner.BuffHandler.HasBuff(EMonsterBuffType.FLICKERING_LIGHT))
				{
					MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(36);
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					m_castedFlickeringLight = true;
					CastSpell(p_spell);
				}
				else
				{
					m_roundSubState++;
					m_subStateFinished = true;
				}
				break;
			case 1:
				if (!LegacyLogic.Instance.WorldManager.Party.InCombat)
				{
					MonsterSpell p_spell2 = m_owner.SpellHandler.SelectSpellByID(37);
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					CastSpell(p_spell2);
				}
				else
				{
					m_roundSubState++;
					m_subStateFinished = true;
				}
				break;
			case 2:
				if (m_owner.Direction != EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
				{
					m_owner.Rotate(EDirectionFunctions.RotationCount(m_owner.Direction, EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position)), true);
				}
				else
				{
					m_roundSubState++;
					m_subStateFinished = true;
				}
				break;
			case 3:
				if (!m_castedFlickeringLight && LegacyLogic.Instance.WorldManager.Party.InCombat)
				{
					MonsterSpell p_spell3 = m_owner.SpellHandler.SelectSpellByID(34);
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					CastSpell(p_spell3);
				}
				else
				{
					m_roundSubState++;
					m_subStateFinished = true;
				}
				break;
			case 4:
				if (LegacyLogic.Instance.WorldManager.Party.InCombat)
				{
					m_owner.CombatHandler.CastSpell = false;
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					m_owner.CombatHandler.DoAttack();
				}
				else
				{
					m_isFinished = true;
					m_owner.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
				}
				break;
			case 5:
				m_owner.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
				m_isFinished = true;
				break;
			}
		}

		private void HandlePhase3()
		{
			m_subStateFinished = false;
			if (!m_strategyA)
			{
				switch (m_roundSubState)
				{
				case 0:
					if (LegacyLogic.Instance.WorldManager.Party.InCombat)
					{
						if (m_owner.Direction != EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
						{
							m_owner.Rotate(EDirectionFunctions.RotationCount(m_owner.Direction, EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position)), true);
						}
						else
						{
							m_roundSubState++;
							m_subStateFinished = true;
						}
					}
					else
					{
						m_roundSubState++;
						m_subStateFinished = true;
					}
					break;
				case 1:
					if (!LegacyLogic.Instance.WorldManager.Party.InCombat)
					{
						m_strategyA = true;
						m_roundSubState = 0;
					}
					else if (!m_owner.BuffHandler.HasBuff(EMonsterBuffType.FLICKERING_LIGHT))
					{
						MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(36);
						m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
						CastSpell(p_spell);
					}
					else
					{
						m_owner.CombatHandler.CastSpell = false;
						m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
						m_owner.CombatHandler.DoAttack();
					}
					break;
				case 2:
					m_owner.CombatHandler.CastSpell = false;
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					m_owner.CombatHandler.DoAttack();
					break;
				case 3:
				{
					MonsterSpell p_spell2 = m_owner.SpellHandler.SelectSpellByID(37);
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					CastSpell(p_spell2);
					break;
				}
				}
				if (m_roundSubState >= 4)
				{
					m_isFinished = true;
					m_owner.m_stateMachine.ChangeState(Monster.EState.IDLE);
					LegacyLogic.Instance.WorldManager.Party.InCombat = LegacyLogic.Instance.MapLoader.Grid.CheckInCombat(LegacyLogic.Instance.WorldManager.Party.Position);
					m_strategyA = !m_strategyA;
					return;
				}
			}
			if (m_strategyA)
			{
				switch (m_roundSubState)
				{
				case 0:
					if (!m_owner.BuffHandler.HasBuff(EMonsterBuffType.FLICKERING_LIGHT))
					{
						MonsterSpell p_spell3 = m_owner.SpellHandler.SelectSpellByID(36);
						m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
						CastSpell(p_spell3);
					}
					else
					{
						m_roundSubState++;
						m_subStateFinished = true;
					}
					break;
				case 1:
					if (LegacyLogic.Instance.WorldManager.Party.InCombat || LegacyLogic.Instance.MapLoader.Grid.LineOfSight(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
					{
						if (m_owner.Direction != EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
						{
							m_owner.Rotate(EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position), true);
						}
						else
						{
							m_roundSubState++;
							m_subStateFinished = true;
						}
					}
					else
					{
						m_roundSubState++;
						m_subStateFinished = true;
					}
					break;
				case 2:
					if (!LegacyLogic.Instance.MapLoader.Grid.LineOfSight(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
					{
						MonsterSpell p_spell4 = m_owner.SpellHandler.SelectSpellByID(37);
						m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
						CastSpell(p_spell4);
						m_castedFlicker = true;
					}
					else
					{
						MonsterSpell p_spell5 = m_owner.SpellHandler.SelectSpellByID(34);
						m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
						CastSpell(p_spell5);
					}
					break;
				case 3:
					if (m_owner.Direction != EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
					{
						m_owner.Rotate(EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position), true);
					}
					if (LegacyLogic.Instance.MapLoader.Grid.LineOfSight(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position))
					{
						MonsterSpell p_spell6 = m_owner.SpellHandler.SelectSpellByID(34);
						m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
						CastSpell(p_spell6);
					}
					else
					{
						m_roundSubState++;
						m_subStateFinished = true;
					}
					break;
				}
				if (m_roundSubState >= 4)
				{
					m_isFinished = true;
					m_owner.m_stateMachine.ChangeState(Monster.EState.IDLE);
					m_strategyA = !m_strategyA;
				}
			}
		}

		public override Boolean CastSpell(MonsterSpell p_spell)
		{
			if (base.CastSpell(p_spell))
			{
				m_owner.CombatHandler.CastedSpell = p_spell;
				m_owner.CombatHandler.CastSpell = true;
				if (p_spell.TargetType == ETargetType.PARTY)
				{
					m_owner.DivideAttacksToPartyCharacters = true;
				}
				m_owner.BuffHandler.DoOnCastSpellEffects();
				m_owner.CombatHandler.DoAttack();
				return true;
			}
			return false;
		}
	}
}
