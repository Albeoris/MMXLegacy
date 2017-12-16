using System;
using System.Collections.Generic;
using Legacy.Core.Abilities;
using Legacy.Core.Api;
using Legacy.Core.Entities.AI.Events;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities.AI.MonsterBehaviours
{
	public class ErebosAIHandler : MonsterAIHandler
	{
		private Int32 m_roundCounter;

		private Int32 m_appearMaxCounter = 4;

		private Single m_appearProbability = 0.65f;

		private Single m_meleeProbability = 0.55f;

		private Boolean m_firstFightRound = true;

		private EMonsterSpell m_lastCastedSpell;

		private EErebosState m_state = EErebosState.VANISHED;

		private AIEventHealthPercent m_percentEvent;

		public ErebosAIHandler(Monster p_owner) : base(p_owner)
		{
			m_percentEvent = new AIEventHealthPercent(0.7f, m_owner, true);
			m_percentEvent.OnTrigger += OnHealAndVanishEvent;
			m_state = EErebosState.IDLE;
		}

		public EErebosState ErebosState => m_state;

	    public override void StartAITurn()
		{
			if (m_owner.IsAggro)
			{
				if (m_state == EErebosState.VANISHED)
				{
					if (LegacyLogic.Instance.WorldManager.Party.CurrentMapArea == EMapArea.SAFE_ZONE)
					{
						m_owner.ForceAggro(false);
					}
					else
					{
						m_roundCounter++;
						if (m_roundCounter >= m_appearMaxCounter && Random.Value <= m_appearProbability)
						{
							MakeAttackDesicion();
							m_roundCounter = 0;
						}
					}
				}
				if (m_firstFightRound)
				{
					m_state = EErebosState.VANISHING;
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					DoVanish();
					m_firstFightRound = false;
				}
			}
		}

		public override void UpdateDistanceToParty(Party p_party, Grid p_grid)
		{
			base.UpdateDistanceToParty(p_party, p_grid);
		}

		public override void Update()
		{
			if (m_state == EErebosState.APPEARING && m_owner.AppearAnimationDone.IsTriggered)
			{
				m_owner.AppearAnimationDone.Reset();
				AttackParty();
			}
			if (m_state == EErebosState.VANISHING && m_owner.VanishAnimationDone.IsTriggered)
			{
				m_state = EErebosState.VANISHED;
				m_owner.VanishAnimationDone.Reset();
				m_owner.EndTurn();
				m_owner.m_stateMachine.ChangeState(Monster.EState.IDLE);
			}
			if (m_state == EErebosState.SPELLCASTING && (m_owner.State == Monster.EState.ACTION_FINISHED || m_owner.State == Monster.EState.IDLE))
			{
				if (m_lastCastedSpell != EMonsterSpell.BACKSTAB)
				{
					m_state = EErebosState.VANISHING;
					DoVanish();
				}
				else
				{
					m_state = EErebosState.IDLE;
					m_owner.m_stateMachine.ChangeState(Monster.EState.ACTION_FINISHED);
				}
			}
			if (!m_owner.IsAggro && LegacyLogic.Instance.WorldManager.Party.CurrentMapArea == EMapArea.NONE && m_state == EErebosState.VANISHED)
			{
				m_owner.IsAggro = true;
			}
		}

		public override void DoTurn(Grid p_grid, Party p_party)
		{
			m_owner.AbilityHandler.ExecuteAttacks(null, null, false, EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.AbilityHandler.FlushActionLog(EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.BuffHandler.ModifyMonsterValues();
			m_owner.CombatHandler.MeleeStrikes += m_owner.CombatHandler.MeleeStrikesRoundBonus;
			m_owner.CombatHandler.MeleeStrikesRoundBonus = 0;
			CheckAIEvents();
			if (m_state == EErebosState.IDLE && m_decision == EMonsterStrategyDecision.CALCULATE_STRATEGY)
			{
				m_decision = EMonsterStrategyDecision.MELEE;
			}
			if (m_state == EErebosState.IDLE && m_decision == EMonsterStrategyDecision.MELEE && Position.Distance(m_owner.Position, p_party.Position) <= 1f)
			{
				AttackParty();
			}
		}

		protected override void CheckAIEvents()
		{
			if (m_state != EErebosState.VANISHED)
			{
				m_percentEvent.Update();
			}
		}

		private void OnHealAndVanishEvent()
		{
			MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(21);
			m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
			CastSpell(p_spell);
		}

		public override Boolean CastSpell(MonsterSpell p_spell)
		{
			if (CastSpellErebos(p_spell))
			{
				m_owner.CombatHandler.CastedSpell = p_spell;
				m_owner.CombatHandler.CastSpell = true;
				if (p_spell.TargetType == ETargetType.PARTY)
				{
					m_owner.DivideAttacksToPartyCharacters = true;
				}
				m_owner.BuffHandler.DoOnCastSpellEffects();
				m_owner.CombatHandler.DoAttack();
				m_lastCastedSpell = p_spell.SpellType;
				m_state = EErebosState.SPELLCASTING;
				return true;
			}
			return false;
		}

		private Boolean CastSpellErebos(MonsterSpell p_spell)
		{
			m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
			ETargetType targetType = p_spell.TargetType;
			if (targetType != ETargetType.SINGLE_MONSTER)
			{
				if (targetType != ETargetType.SINGLE_PARTY_MEMBER)
				{
					if (targetType != ETargetType.ALL_MONSTERS_ON_TARGET_SLOT)
					{
						if (targetType != ETargetType.PARTY)
						{
							return false;
						}
						if (LegacyLogic.Instance.MapLoader.Grid.LineOfSight(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position, true))
						{
							return true;
						}
						return false;
					}
				}
				else
				{
					if (LegacyLogic.Instance.MapLoader.Grid.LineOfSight(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position, true))
					{
						return true;
					}
					return false;
				}
			}
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(m_owner.Position);
			if (slot.Entities.Count > 0)
			{
				return true;
			}
			return false;
		}

		private void MakeAttackDesicion()
		{
			m_decision = ((Random.Value < m_meleeProbability) ? EMonsterStrategyDecision.MELEE : EMonsterStrategyDecision.RANGED);
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			List<GridSlot> list = (m_decision != EMonsterStrategyDecision.RANGED) ? GetMeleeTargets(LegacyLogic.Instance.MapLoader.Grid, party) : GetRangedTargets(LegacyLogic.Instance.MapLoader.Grid, party);
			if (list.Count > 0)
			{
				if (m_decision == EMonsterStrategyDecision.RANGED)
				{
					m_targetSlot = list[Random.Range(0, list.Count)];
				}
				else
				{
					GetMeleePosition(grid, party, list);
					m_decision = EMonsterStrategyDecision.MELEE;
				}
				Appear();
			}
		}

		private void GetMeleePosition(Grid p_grid, Party p_party, List<GridSlot> p_slots)
		{
			EDirection oppositeDir = EDirectionFunctions.GetOppositeDir(p_party.Direction);
			EDirection p_dir = EDirectionFunctions.Add(p_party.Direction, 3);
			EDirection p_dir2 = EDirectionFunctions.Add(p_party.Direction, 1);
			EDirection direction = p_party.Direction;
			Int32 num = Random.Range(0, 13);
			GridSlot neighborSlot;
			if (num >= 5)
			{
				neighborSlot = p_grid.GetSlot(p_party.Position).GetNeighborSlot(p_grid, oppositeDir);
			}
			else if (num >= 3)
			{
				neighborSlot = p_grid.GetSlot(p_party.Position).GetNeighborSlot(p_grid, p_dir);
			}
			else if (num >= 1)
			{
				neighborSlot = p_grid.GetSlot(p_party.Position).GetNeighborSlot(p_grid, p_dir2);
			}
			else
			{
				neighborSlot = p_grid.GetSlot(p_party.Position).GetNeighborSlot(p_grid, direction);
			}
			if (neighborSlot.IsPassable(m_owner, false))
			{
				m_targetSlot = neighborSlot;
			}
			else
			{
				m_targetSlot = p_slots[Random.Range(0, p_slots.Count)];
			}
		}

		private void Appear()
		{
			if (m_targetSlot != null)
			{
				Grid grid = LegacyLogic.Instance.MapLoader.Grid;
				Position position = m_owner.Position;
				grid.GetSlot(m_owner.Position).RemoveEntity(m_owner);
				m_targetSlot.AddEntity(m_owner);
				m_owner.Position = m_targetSlot.Position;
				m_owner.Direction = EDirectionFunctions.GetDirection(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position);
				m_state = EErebosState.APPEARING;
				UpdateDistanceToParty(LegacyLogic.Instance.WorldManager.Party, grid);
				m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(m_owner, m_owner.Position, "APPEARING");
				LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.SET_ENTITY_POSITION, p_eventArgs);
			}
		}

		public override void Vanish()
		{
			DoVanish();
		}

		public void DoVanish()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = m_owner.Position;
			grid.GetSlot(m_owner.Position).RemoveEntity(m_owner);
			grid.GetSlot(Position.Zero).AddEntity(m_owner);
			m_owner.Position = Position.Zero;
			m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(m_owner, m_owner.Position, "VANISHING");
			LegacyLogic.Instance.EventManager.InvokeEvent(m_owner, EEventType.SET_ENTITY_POSITION, p_eventArgs);
		}

		private void AttackParty()
		{
			if (m_decision == EMonsterStrategyDecision.MELEE || Position.Distance(LegacyLogic.Instance.WorldManager.Party.Position, m_owner.Position) < 1.1f)
			{
				if (m_state == EErebosState.APPEARING)
				{
					MonsterSpell p_spell = m_owner.SpellHandler.SelectSpellByID(28);
					CastSpell(p_spell);
				}
				else
				{
					m_state = EErebosState.IDLE;
					m_owner.m_stateMachine.ChangeState(Monster.EState.MOVING);
					DoMelee(true, LegacyLogic.Instance.WorldManager.Party, LegacyLogic.Instance.MapLoader.Grid, LegacyLogic.Instance.MapLoader.Grid.GetSlot(m_owner.Position));
				}
			}
			else if (m_decision == EMonsterStrategyDecision.RANGED || Position.Distance(LegacyLogic.Instance.WorldManager.Party.Position, m_owner.Position) > 1.1f)
			{
				MonsterSpell p_spell2 = m_owner.SpellHandler.SelectSpellByID(20);
				CastSpell(p_spell2);
				m_state = EErebosState.SPELLCASTING;
			}
		}

		public override List<GridSlot> GetMeleeTargets(Grid p_grid, Party p_party)
		{
			List<GridSlot> list = new List<GridSlot>();
			GridSlot slot = p_grid.GetSlot(p_party.Position);
			for (Int32 i = 0; i <= 3; i++)
			{
				GridSlot neighborSlot = slot.GetNeighborSlot(p_grid, (EDirection)i);
				if (neighborSlot.IsPassable(m_owner, false))
				{
					list.Add(neighborSlot);
				}
			}
			return list;
		}

		public override List<GridSlot> GetRangedTargets(Grid p_grid, Party p_party)
		{
			List<GridSlot> list = new List<GridSlot>();
			GridSlot slot = p_grid.GetSlot(p_party.Position);
			GridSlot slot2 = p_grid.GetSlot(new Position(slot.Position.X, slot.Position.Y + (Position.Up * m_owner.StaticData.AttackRange).Y));
			if (slot2.IsPassable(m_owner, false) && p_grid.LineOfSight(slot2.Position, p_party.Position, true))
			{
				list.Add(slot2);
			}
			GridSlot slot3 = p_grid.GetSlot(new Position(slot.Position.X + (Position.Right * m_owner.StaticData.AttackRange).X, slot.Position.Y));
			if (slot3.IsPassable(m_owner, false) && p_grid.LineOfSight(slot3.Position, p_party.Position, true))
			{
				list.Add(slot3);
			}
			GridSlot slot4 = p_grid.GetSlot(new Position(slot.Position.X, slot.Position.Y + (Position.Down * m_owner.StaticData.AttackRange).Y));
			if (slot4.IsPassable(m_owner, false) && p_grid.LineOfSight(slot4.Position, p_party.Position, true))
			{
				list.Add(slot4);
			}
			GridSlot slot5 = p_grid.GetSlot(new Position(slot.Position.X + (Position.Left * m_owner.StaticData.AttackRange).X, slot.Position.Y));
			if (slot5.IsPassable(m_owner, false) && p_grid.LineOfSight(slot5.Position, p_party.Position, true))
			{
				list.Add(slot5);
			}
			return list;
		}

		public override void Load(SaveGameData p_data)
		{
			m_firstFightRound = p_data.Get<Boolean>("FirstRound", true);
			m_lastCastedSpell = p_data.Get<EMonsterSpell>("LastCastedSpell", m_lastCastedSpell);
			m_state = p_data.Get<EErebosState>("State", m_state);
		}

		public override void Save(SaveGameData p_data)
		{
			p_data.Set<Boolean>("FirstRound", m_firstFightRound);
			p_data.Set<EMonsterSpell>("LastCastedSpell", m_lastCastedSpell);
			p_data.Set<EErebosState>("State", m_state);
		}

		public enum EErebosState
		{
			APPEARING,
			SPELLCASTING,
			VANISHING,
			VANISHED,
			IDLE
		}
	}
}
