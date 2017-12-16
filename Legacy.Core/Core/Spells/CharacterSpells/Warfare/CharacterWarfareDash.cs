using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareDash : CharacterSpell
	{
		public CharacterWarfareDash() : base(ECharacterSpell.WARFARE_DASH)
		{
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = party.Position;
			Position p_pos = position + party.Direction;
			if (grid.GetSlot(p_pos) == null)
			{
				return false;
			}
			Boolean flag = false;
			flag |= CheckForMonsterWithSpiritBound(grid.GetSlot(position + EDirection.NORTH));
			flag |= CheckForMonsterWithSpiritBound(grid.GetSlot(position + EDirection.SOUTH));
			flag |= CheckForMonsterWithSpiritBound(grid.GetSlot(position + EDirection.EAST));
			flag |= CheckForMonsterWithSpiritBound(grid.GetSlot(position + EDirection.WEST));
			return !flag && grid.CanMoveEntity(party, party.Direction);
		}

		private Boolean CheckForMonsterWithSpiritBound(GridSlot p_slot)
		{
			if (p_slot != null)
			{
				foreach (MovingEntity movingEntity in p_slot.Entities)
				{
					Monster monster = movingEntity as Monster;
					if (monster != null)
					{
						return monster.AbilityHandler.HasAbility(EMonsterAbilityType.SPIRIT_BOUND);
					}
				}
				return false;
			}
			return false;
		}

		protected override void HandlePartyCharacters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = party.Position;
			Position p_pos = position + party.Direction;
			if (grid.GetSlot(p_pos) == null)
			{
				return;
			}
			if (grid.CanMoveEntity(party, party.Direction))
			{
				p_result.SpellTargets.Add(new SpellTarget(party));
				LegacyLogic.Instance.UpdateManager.PartyTurnActor.MoveBySpell(MoveCommand.Forward);
			}
			p_sorcerer.ChangeMP(-p_sorcerer.ManaPoints);
		}

		public override Boolean HasResources(Character p_sorcerer)
		{
			return p_sorcerer.ManaPoints >= 1;
		}
	}
}
