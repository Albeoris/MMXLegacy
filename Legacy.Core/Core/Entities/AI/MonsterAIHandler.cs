using System;
using System.Collections.Generic;
using Legacy.Core.Abilities;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.AI.Events;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Pathfinding;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.MonsterSpells;

namespace Legacy.Core.Entities.AI
{
	public class MonsterAIHandler : ISaveGameObject
	{
		private List<GridSlot> m_PathBuffer = new List<GridSlot>();

		protected Monster m_owner;

		protected EMonsterStrategyDecision m_decision;

		protected List<AIEvent> m_aiEvents;

		protected GridSlot m_targetSlot;

		protected Boolean m_canOpenDoors;

		protected Boolean m_isFinished = true;

		public MonsterAIHandler(Monster p_owner)
		{
			m_owner = p_owner;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DOOR_STATE_CHANGED, new EventHandler(OnDoorStateChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnPositionChanged));
			m_aiEvents = new List<AIEvent>();
		}

		public Boolean IsFinished => m_isFinished;

	    public Boolean CalculatesRanged { get; private set; }

		public virtual void Destroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DOOR_STATE_CHANGED, new EventHandler(OnDoorStateChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnPositionChanged));
		}

		public virtual void UpdateDistanceToParty(Party p_party, Grid p_grid)
		{
			GridSlot slot = p_grid.GetSlot(m_owner.Position);
			GridSlot slot2 = p_grid.GetSlot(p_party.Position);
			Int32 num;
			if (m_owner.IsAggro)
			{
				num = AStarHelper<GridSlot>.Calculate(slot, slot2, GameConfig.MaxSteps, m_owner, true, false, m_canOpenDoors, null);
			}
			else
			{
				num = AStarHelper<GridSlot>.Calculate(slot, slot2, (Int32)m_owner.AggroRange + 1, m_owner, true, false, m_canOpenDoors, null);
			}
			if (num > 0)
			{
				m_owner.DistanceToParty = num - 1;
			}
			else
			{
				m_owner.DistanceToParty = 99f;
			}
		}

		private void OnPositionChanged(Object p_sender, EventArgs p_args)
		{
			if (p_sender is Party)
			{
				m_decision = EMonsterStrategyDecision.CALCULATE_STRATEGY;
				m_targetSlot = null;
			}
			if (p_sender is Monster)
			{
				m_decision = EMonsterStrategyDecision.CALCULATE_STRATEGY;
			}
		}

		private void OnDoorStateChanged(Object sender, EventArgs e)
		{
			m_owner.CheckAggroRange();
		}

	    internal virtual Boolean CalculatePath(GridSlot p_start, GridSlot p_target, List<GridSlot> p_pathBuffer)
	    {
            // Avoid to stack agro monsters due to cast of spell or long obstacles
            Int32 maxSteps = GameConfig.MaxSteps;
	        if (m_owner.IsAggro)
	            maxSteps *= 3;

	        return AStarHelper<GridSlot>.Calculate(p_start, p_target, maxSteps, m_owner, false, p_pathBuffer) > 0;
	    }

	    protected virtual Boolean DoCastSpell()
		{
			Boolean result = false;
			if (CanCastSpell() && CheckSpellCastChance())
			{
				MonsterSpell monsterSpell = m_owner.SpellHandler.SelectSpell();
				if (monsterSpell == null)
				{
					result = false;
				}
				else if (!CastSpell(monsterSpell))
				{
					result = false;
				}
				else
				{
					m_owner.CombatHandler.CastedSpell = monsterSpell;
					m_owner.CombatHandler.CastSpell = true;
					if (monsterSpell.TargetType == ETargetType.PARTY)
					{
						m_owner.DivideAttacksToPartyCharacters = true;
					}
					m_owner.BuffHandler.DoOnCastSpellEffects();
					result = true;
				}
			}
			return result;
		}

		protected virtual void CheckAIEvents()
		{
		}

		protected virtual void DoRanged(Boolean p_isMagic, Party p_party, Grid p_grid, GridSlot p_startSlot, out Boolean p_isMelee)
		{
			p_isMelee = true;
			if (m_decision == EMonsterStrategyDecision.RANGED || m_decision == EMonsterStrategyDecision.CALCULATE_STRATEGY)
			{
				if (m_owner.DistanceToParty <= 1.6f)
				{
					m_decision = EMonsterStrategyDecision.MELEE;
					p_isMelee = true;
				}
				if (!p_isMagic && m_owner.StaticData.SpellRanges > m_owner.CombatHandler.AttackRange && m_owner.DistanceToParty > 1.6f)
				{
					if (m_targetSlot == null)
					{
						List<GridSlot> rangedTargets = GetRangedTargets(p_grid, p_party, m_owner.StaticData.SpellRanges);
						if (rangedTargets.Count > 0)
						{
							rangedTargets.Sort(new Comparison<GridSlot>(DistSortAsc));
							if (TryMove(rangedTargets, p_grid, p_startSlot, p_party))
							{
								p_isMelee = false;
								return;
							}
							p_isMelee = true;
							m_decision = EMonsterStrategyDecision.MELEE;
						}
						else
						{
							p_isMelee = true;
							m_decision = EMonsterStrategyDecision.MELEE;
						}
					}
				}
				else
				{
					m_decision = EMonsterStrategyDecision.MELEE;
					p_isMelee = true;
				}
				if (!p_isMagic && m_owner.CombatHandler.AttackRange > 1f && m_owner.DistanceToParty > 1.6f)
				{
					p_isMelee = false;
					m_decision = EMonsterStrategyDecision.RANGED;
					if (p_grid.LineOfSight(m_owner.Position, p_party.Position, true) && Position.Distance(m_owner.Position, p_party.Position) <= m_owner.CombatHandler.AttackRange && m_owner.DistanceToParty >= 2f)
					{
						m_owner.CombatHandler.AttackRanged();
						return;
					}
					if (m_targetSlot == null && (m_owner.Position.X == p_party.Position.X || m_owner.Position.Y == p_party.Position.Y))
					{
						EDirection direction = EDirectionFunctions.GetDirection(p_party.Position, m_owner.Position);
						GridSlot gridSlot = p_grid.GetSlot(p_party.Position);
						for (Int32 i = 0; i < m_owner.StaticData.AttackRange; i++)
						{
							GridSlot neighborSlot = gridSlot.GetNeighborSlot(p_grid, direction);
							if (neighborSlot == null)
							{
								break;
							}
							gridSlot = neighborSlot;
						}
						if (gridSlot != null)
						{
							m_targetSlot = gridSlot;
						}
					}
					if (m_targetSlot == null)
					{
						List<GridSlot> rangedTargets2 = GetRangedTargets(p_grid, p_party);
						if (rangedTargets2.Count > 0)
						{
							rangedTargets2.Sort(new Comparison<GridSlot>(DistSortAsc));
							if (TryMove(rangedTargets2, p_grid, p_startSlot, p_party))
							{
								return;
							}
							p_isMelee = true;
							m_decision = EMonsterStrategyDecision.MELEE;
						}
						else
						{
							p_isMelee = true;
							m_decision = EMonsterStrategyDecision.MELEE;
						}
					}
					else
					{
						if (!(m_owner.Position != m_targetSlot.Position))
						{
							p_isMelee = true;
							m_decision = EMonsterStrategyDecision.MELEE;
							return;
						}
						if (TryMove(new List<GridSlot>
						{
							m_targetSlot
						}, p_grid, p_startSlot, p_party))
						{
							return;
						}
						p_isMelee = true;
						m_targetSlot = null;
						m_decision = EMonsterStrategyDecision.CALCULATE_STRATEGY;
					}
				}
			}
		}

		protected Boolean FindSlotsWithSummons(GridSlot p_slot)
		{
			return p_slot.CheckForSummons();
		}

		public virtual void FilterPossibleTargets(List<Character> p_characters)
		{
			for (Int32 i = p_characters.Count - 1; i >= 0; i--)
			{
				if (p_characters[i].ConditionHandler.HasOneCondition(ECondition.DEAD))
				{
					p_characters.RemoveAt(i);
				}
			}
		}

		protected virtual void DoMelee(Boolean p_isMelee, Party p_party, Grid p_grid, GridSlot p_startSlot)
		{
			if (p_isMelee)
			{
				m_decision = EMonsterStrategyDecision.MELEE;
				if (m_owner.DistanceToParty <= 1f)
				{
					EDirection direction = EDirectionFunctions.GetDirection(m_owner.Position, p_party.Position);
					if (p_startSlot.GetTransition(direction).TransitionType == EGridTransitionType.CLOSED)
					{
						if (m_canOpenDoors)
						{
							TryOpenDoor(p_startSlot, direction);
						}
						return;
					}
					m_owner.RangedAttack = false;
					m_owner.CombatHandler.Attack();
					return;
				}
				else
				{
					if (m_owner.DistanceToParty <= 1.6f)
					{
						return;
					}
					List<GridSlot> meleeTargets = GetMeleeTargets(p_grid, p_party);
					meleeTargets.Sort(new Comparison<GridSlot>(DistSortAsc));
					if (meleeTargets.Count > 0)
					{
						if (TryMove(meleeTargets, p_grid, p_startSlot, p_party))
						{
							return;
						}
					}
					else
					{
						Party party = LegacyLogic.Instance.WorldManager.Party;
						GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(1, 1));
						if (slot != null && slot.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot);
						}
						GridSlot slot2 = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(-1, -1));
						if (slot2 != null && slot2.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot2);
						}
						GridSlot slot3 = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(-1, 1));
						if (slot3 != null && slot3.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot3);
						}
						GridSlot slot4 = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(1, -1));
						if (slot4 != null && slot4.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot4);
						}
						slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(0, 2));
						if (slot != null && slot.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot);
						}
						slot2 = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(2, 0));
						if (slot2 != null && slot2.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot2);
						}
						slot3 = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(-2, 0));
						if (slot3 != null && slot3.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot3);
						}
						slot4 = LegacyLogic.Instance.MapLoader.Grid.GetSlot(party.Position + new Position(0, -2));
						if (slot4 != null && slot4.IsPassable(m_owner, false))
						{
							meleeTargets.Add(slot4);
						}
						meleeTargets.RemoveAll((GridSlot t) => t.Position == m_owner.Position);
						if (meleeTargets.Count > 0 && TryMove(meleeTargets, p_grid, p_startSlot, p_party))
						{
							return;
						}
					}
				}
			}
		}

		public virtual void DoTurn(Grid p_grid, Party p_party)
		{
			CalculatesRanged = false;
			if (m_owner.State == Monster.EState.SPAWNING)
			{
				return;
			}
			m_owner.AbilityHandler.ExecuteAttack(null, null, false, EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			m_owner.AbilityHandler.FlushActionLog(EExecutionPhase.BEGIN_OF_MONSTERS_TURN);
			if (PartyIsCaged(p_grid, p_party))
			{
				m_owner.SkipMovement.Trigger();
				return;
			}
			GridSlot slot = p_grid.GetSlot(m_owner.Position);
			Boolean p_isMelee = true;
			m_owner.BuffHandler.ModifyMonsterValues();
			m_owner.CombatHandler.MeleeStrikes += m_owner.CombatHandler.MeleeStrikesRoundBonus;
			m_owner.CombatHandler.MeleeStrikesRoundBonus = 0;
			if (m_owner.CombatHandler.IsFleeing)
			{
				m_decision = EMonsterStrategyDecision.FLEEING;
			}
			else if (m_decision == EMonsterStrategyDecision.FLEEING)
			{
				m_decision = EMonsterStrategyDecision.CALCULATE_STRATEGY;
			}
			CheckAIEvents();
			if (m_owner.State != Monster.EState.ACTION_FINISHED)
			{
				Boolean flag = DoCastSpell();
				CalculatesRanged = true;
				DoRanged(flag, p_party, p_grid, slot, out p_isMelee);
				CalculatesRanged = false;
				if (flag)
				{
					p_isMelee = false;
					m_owner.RangedAttack = false;
					m_owner.CombatHandler.Attack();
					return;
				}
				if (m_decision == EMonsterStrategyDecision.MELEE || m_decision == EMonsterStrategyDecision.CALCULATE_STRATEGY)
				{
					DoMelee(p_isMelee, p_party, p_grid, slot);
				}
				else if (m_decision == EMonsterStrategyDecision.FLEEING)
				{
					DoFlee(p_party, p_grid, slot);
				}
			}
			m_owner.SkipMovement.Trigger();
		}

		protected Boolean PartyIsCaged(Grid p_grid, Party p_party)
		{
			for (EDirection edirection = EDirection.NORTH; edirection <= EDirection.WEST; edirection++)
			{
				GridSlot slot = p_grid.GetSlot(p_party.Position + edirection);
				if (slot != null)
				{
					foreach (MovingEntity movingEntity in slot.Entities)
					{
						if (movingEntity is Monster && ((Monster)movingEntity).StaticData.Type == EMonsterType.CAGE)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void DoFlee(Party p_party, Grid p_grid, GridSlot startSlot)
		{
			EDirection direction;
			if (p_party.Position.X == m_owner.Position.X || p_party.Position.Y == m_owner.Position.Y)
			{
				direction = EDirectionFunctions.GetDirection(p_party.Position, m_owner.Position);
			}
			else
			{
				direction = m_owner.Direction;
			}
			GridSlot neighborSlot = p_grid.GetSlot(m_owner.Position).GetNeighborSlot(p_grid, direction);
			if (neighborSlot != null)
			{
				m_owner.Direction = direction;
				if (p_grid.MoveEntity(m_owner, m_owner.Direction))
				{
					m_targetSlot = neighborSlot;
					m_owner.StartMovement.Trigger();
				}
			}
		}

		public virtual void StartAITurn()
		{
		}

		public virtual void Update()
		{
		}

		protected virtual Boolean CheckSpellCastChance()
		{
			if (m_decision == EMonsterStrategyDecision.FLEEING)
			{
				return false;
			}
			if (Position.Distance(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position) > 1f)
			{
				return true;
			}
			Int32 num = Random.Range(0, 100);
			Int32 num2 = (Int32)(m_owner.StaticData.CastSpellChance * 100f);
			return num < num2;
		}

		protected virtual Boolean CanCastSpell()
		{
			return m_owner.SpellHandler.Count > 0 && Position.Distance(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position) <= m_owner.StaticData.SpellRanges && m_owner.CombatHandler.CanCastSpell;
		}

		public virtual Boolean CastSpell(MonsterSpell p_spell)
		{
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
						if (LegacyLogic.Instance.MapLoader.Grid.LineOfSight(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position, Position.Distance(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position) <= m_owner.StaticData.SpellRanges))
						{
							m_owner.SpellHandler.LastCastedSpell = p_spell;
							return true;
						}
						return false;
					}
				}
				else
				{
					if (LegacyLogic.Instance.MapLoader.Grid.LineOfSight(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position, Position.Distance(m_owner.Position, LegacyLogic.Instance.WorldManager.Party.Position) <= m_owner.StaticData.SpellRanges))
					{
						m_owner.SpellHandler.LastCastedSpell = p_spell;
						return true;
					}
					return false;
				}
			}
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(m_owner.Position);
			if (slot.Entities.Count > 0)
			{
				m_owner.SpellHandler.LastCastedSpell = p_spell;
				return true;
			}
			return false;
		}

		public virtual void Vanish()
		{
		}

		internal Boolean TryMove(List<GridSlot> p_targets, Grid p_grid, GridSlot p_startSlot, Party p_party)
		{
			if (m_owner.CombatHandler.CanMove)
			{
				List<GridSlot> pathBuffer = m_PathBuffer;
				GridSlot gridSlot = null;
				Int32 num = 999;
				GridSlot gridSlot2 = null;
				foreach (GridSlot gridSlot3 in p_targets)
				{
					pathBuffer.Clear();
					if (CalculatePath(p_startSlot, gridSlot3, pathBuffer) && pathBuffer.Count < num)
					{
						gridSlot = gridSlot3;
						num = pathBuffer.Count;
						gridSlot2 = pathBuffer[1];
					}
				}
				if (gridSlot != null)
				{
					EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(p_startSlot.Position, gridSlot2.Position);
					if (lineOfSightDirection != EDirection.CENTER)
					{
						m_owner.Direction = EDirectionFunctions.GetDirection(gridSlot2.Position, p_party.Position);
						if (p_grid.MoveEntity(m_owner, lineOfSightDirection))
						{
							m_targetSlot = gridSlot;
							GridSlot slot = p_grid.GetSlot(m_owner.Position);
							GridSlot slot2 = p_grid.GetSlot(p_party.Position);
							Int32 num2 = AStarHelper<GridSlot>.Calculate(slot, slot2, GameConfig.MaxSteps, m_owner, true, null);
							if (num2 > 0)
							{
								m_owner.DistanceToParty = num2;
							}
							else
							{
								m_owner.DistanceToParty = 99f;
							}
							m_owner.StartMovement.Trigger();
							return true;
						}
					}
				}
			}
			return false;
		}

		private Boolean TryOpenDoor(GridSlot p_slot, EDirection p_dir)
		{
			List<InteractiveObject> doors = p_slot.GetDoors();
			for (Int32 i = 0; i < doors.Count; i++)
			{
				Door door = (Door)doors[i];
				if (door.Location == p_dir && door.State == EInteractiveObjectState.DOOR_CLOSED && door.Commands.Count > 0)
				{
					door.Execute(LegacyLogic.Instance.MapLoader.Grid);
					door.Update();
					return true;
				}
			}
			GridSlot neighborSlot = p_slot.GetNeighborSlot(LegacyLogic.Instance.MapLoader.Grid, p_dir);
			doors = neighborSlot.GetDoors();
			for (Int32 j = 0; j < doors.Count; j++)
			{
				Door door2 = (Door)doors[j];
				if (door2.Location == EDirectionFunctions.GetOppositeDir(p_dir) && door2.State == EInteractiveObjectState.DOOR_CLOSED && door2.Commands.Count > 0)
				{
					door2.Execute(LegacyLogic.Instance.MapLoader.Grid);
					door2.Update();
					return true;
				}
			}
			return false;
		}

		public virtual List<GridSlot> GetMeleeTargets(Grid p_grid, Party p_party)
		{
			List<GridSlot> list = new List<GridSlot>();
			Int32 num = Math.Min(p_party.Position.X, m_owner.Position.X);
			Int32 num2 = Math.Min(p_party.Position.Y, m_owner.Position.Y);
			Int32 num3 = Math.Max(p_party.Position.X, m_owner.Position.X);
			Int32 num4 = Math.Max(p_party.Position.Y, m_owner.Position.Y);
			if (num == p_party.Position.X)
			{
				num--;
				num3 = num + 2;
			}
			if (num2 == p_party.Position.Y)
			{
				num2--;
				num4 = num2 + 2;
			}
			for (Int32 i = num; i <= num3; i++)
			{
				for (Int32 j = num2; j <= num4; j++)
				{
					Position position = new Position(i, j);
					if (!(position == p_party.Position) && !(position == m_owner.Position))
					{
						GridSlot slot = p_grid.GetSlot(position);
						if (slot != null && slot.IsPassable(m_owner, false) && Position.Distance(position, p_party.Position) == 1f)
						{
							EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(slot.Position, p_party.Position);
							if (slot.GetTransition(lineOfSightDirection).TransitionType == EGridTransitionType.OPEN)
							{
								list.Add(slot);
							}
						}
					}
				}
			}
			return list;
		}

		public virtual List<GridSlot> GetRangedTargets(Grid p_grid, Party p_party)
		{
			return GetRangedTargets(p_grid, p_party, m_owner.CombatHandler.AttackRange);
		}

		public virtual List<GridSlot> GetRangedTargets(Grid p_grid, Party p_party, Single p_range)
		{
			List<GridSlot> list = new List<GridSlot>();
			Single num = Math.Min(p_range, m_owner.DistanceToParty);
			if (num > 1f)
			{
				Int32 num2 = Math.Min(p_party.Position.X, m_owner.Position.X);
				Int32 num3 = Math.Min(p_party.Position.Y, m_owner.Position.Y);
				Int32 num4 = Math.Max(p_party.Position.X, m_owner.Position.X);
				Int32 num5 = Math.Max(p_party.Position.Y, m_owner.Position.Y);
				if (num2 == p_party.Position.X)
				{
					num2 -= (Int32)p_range;
					num4 = num2 + (Int32)p_range * 2;
				}
				if (num3 == p_party.Position.Y)
				{
					num3 -= (Int32)p_range;
					num5 = num3 + (Int32)p_range * 2;
				}
				for (Int32 i = num2; i <= num4; i++)
				{
					for (Int32 j = num3; j <= num5; j++)
					{
						Position position = new Position(i, j);
						if (!(position == p_party.Position) && !(position == m_owner.Position))
						{
							GridSlot slot;
							if ((slot = p_grid.GetSlot(position)) != null && slot.IsPassable(m_owner, false))
							{
								Single num6 = Position.Distance(slot.Position, p_party.Position);
								if ((i == p_party.Position.X || j == p_party.Position.Y) && num6 >= 2f && p_grid.LineOfSight(slot.Position, p_party.Position, true))
								{
									list.Add(slot);
								}
							}
						}
					}
				}
			}
			return list;
		}

		internal Int32 DistSortAsc(GridSlot a, GridSlot b)
		{
			Single num = Position.DistanceSquared(a.Position, m_owner.Position);
			Single value = Position.DistanceSquared(b.Position, m_owner.Position);
			return num.CompareTo(value);
		}

		internal Int32 DistSortDesc(GridSlot a, GridSlot b)
		{
			Single value = Position.DistanceSquared(a.Position, m_owner.Position);
			return Position.DistanceSquared(b.Position, m_owner.Position).CompareTo(value);
		}

		public virtual void Load(SaveGameData p_data)
		{
		}

		public virtual void Save(SaveGameData p_data)
		{
		}
	}
}
