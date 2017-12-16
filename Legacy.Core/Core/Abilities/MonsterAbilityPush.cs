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
	public class MonsterAbilityPush : MonsterAbilityBase
	{
		public MonsterAbilityPush() : base(EMonsterAbilityType.PUSH)
		{
			m_executionPhase = EExecutionPhase.END_OF_MONSTERS_TURN;
		}

		public override void HandleAttacks(List<Attack> p_attackList, Monster p_monster, Character p_character, Boolean p_isMagic)
		{
			if (p_monster.DistanceToParty > 1f)
			{
				return;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = party.Position;
			Position position2 = position + EDirectionFunctions.GetLineOfSightDirection(p_monster.Position, party.Position);
			GridSlot slot = grid.GetSlot(position2);
			if (slot == null)
			{
				return;
			}
			party.IsPushed = true;
			if (!slot.HasEntities && grid.CanMoveEntity(party, p_monster.Direction))
			{
				GridSlot slot2 = grid.GetSlot(position);
				slot2.RemoveEntity(party);
				party.Position = position2;
				grid.GetSlot(position2).AddEntity(party);
				MoveEntityEventArgs p_eventArgs = new MoveEntityEventArgs(position, position2);
				LegacyLogic.Instance.EventManager.InvokeEvent(party, EEventType.MOVE_ENTITY, p_eventArgs);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ENTITY_ABILITY_ADDED, new AbilityEventArgs(p_monster, this));
				AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, this);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			else
			{
				party.IsPushed = false;
			}
		}
	}
}
