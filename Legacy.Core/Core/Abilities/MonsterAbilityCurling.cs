using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Abilities
{
	public class MonsterAbilityCurling : MonsterAbilityBase
	{
		public MonsterAbilityCurling() : base(EMonsterAbilityType.CURLING)
		{
			m_executionPhase = EExecutionPhase.BEFORE_MONSTER_ATTACK;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (p_monster.DistanceToParty > 1f)
			{
				return;
			}
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = LegacyLogic.Instance.WorldManager.Party.Position;
			EDirection direction = LegacyLogic.Instance.WorldManager.Party.Direction;
			EDirection p_dir = (direction - EDirection.EAST != -1) ? (direction - 1) : EDirection.WEST;
			EDirection p_dir2 = (direction + 1 != EDirection.COUNT) ? (direction + 1) : EDirection.NORTH;
			Boolean flag = true;
			if (direction != EDirectionFunctions.GetOppositeDir(p_monster.Direction) && direction != p_monster.Direction)
			{
				p_dir = direction;
				p_dir2 = EDirectionFunctions.GetOppositeDir(direction);
				flag = false;
			}
			p_monster.CombatHandler.MeleeStrikes = 3;
			Position position2 = p_monster.Position;
			Position p_pos = position + p_dir;
			Position position3 = position + p_dir2;
			Boolean flag2 = Random.Range(0, 2) == 1;
			if (flag2)
			{
				if (grid.GetSlot(p_pos).AddEntity(p_monster))
				{
					grid.GetSlot(position2).RemoveEntity(p_monster);
					p_monster.Rotate(EDirectionFunctions.RotationCount(p_monster.Direction, EDirectionFunctions.GetOppositeDir(direction)), false);
					if (flag)
					{
						p_monster.Rotate(-1, false);
					}
				}
				else
				{
					flag2 = false;
				}
			}
			if (!flag2)
			{
				if (Position.Distance(p_monster.Position, position3) < 2f)
				{
					if (grid.GetSlot(position3).AddEntity(p_monster))
					{
						grid.GetSlot(position2).RemoveEntity(p_monster);
						p_monster.Rotate(EDirectionFunctions.RotationCount(p_monster.Direction, EDirectionFunctions.GetOppositeDir(direction)), false);
						if (flag)
						{
							p_monster.Rotate(1, false);
						}
					}
				}
				else if (grid.GetSlot(p_pos).AddEntity(p_monster))
				{
					grid.GetSlot(position2).RemoveEntity(p_monster);
					p_monster.Rotate(EDirectionFunctions.RotationCount(p_monster.Direction, EDirectionFunctions.GetOppositeDir(direction)), false);
					if (flag)
					{
						p_monster.Rotate(-1, false);
					}
				}
			}
			if (position2 != p_monster.Position)
			{
				MoveEntityEventArgs p_eventArgs = new MoveEntityEventArgs(position2, p_monster.Position);
				LegacyLogic.Instance.EventManager.InvokeEvent(p_monster, EEventType.MOVE_ENTITY, p_eventArgs);
				AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
			}
		}
	}
}
