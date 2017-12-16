using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellCrystalCage : MonsterSpellCageBase
	{
		public MonsterSpellCrystalCage(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.CRYSTAL_CAGE, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			m_grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			GridSlot gridSlot = CheckSlot(party.Position);
			if (gridSlot == null)
			{
				gridSlot = FindTargetSlot(p_monster.Position);
			}
			if (gridSlot != null)
			{
				if (gridSlot.Position != party.Position)
				{
					MoveParty(gridSlot);
				}
				SpawnCagePiece(gridSlot.Position + EDirection.WEST, true);
				SpawnCagePiece(gridSlot.Position + EDirection.NORTH, false);
				SpawnCagePiece(gridSlot.Position + EDirection.EAST, false);
				SpawnCagePiece(gridSlot.Position + EDirection.SOUTH, false);
			}
		}

		private GridSlot FindTargetSlot(Position p_monsterPos)
		{
			GridSlot gridSlot = FindSlotInCorner(p_monsterPos, EDirection.NORTH, EDirection.WEST);
			if (gridSlot == null)
			{
				gridSlot = FindSlotInCorner(p_monsterPos, EDirection.NORTH, EDirection.EAST);
			}
			if (gridSlot == null)
			{
				gridSlot = FindSlotInCorner(p_monsterPos, EDirection.SOUTH, EDirection.WEST);
			}
			if (gridSlot == null)
			{
				gridSlot = FindSlotInCorner(p_monsterPos, EDirection.SOUTH, EDirection.EAST);
			}
			return gridSlot;
		}

		private GridSlot FindSlotInCorner(Position p_monsterPos, EDirection p_offset1, EDirection p_offset2)
		{
			Position position = p_monsterPos + p_offset1 + p_offset2;
			GridSlot gridSlot = CheckSlot(position);
			if (gridSlot == null)
			{
				gridSlot = CheckSlot(position + p_offset1);
			}
			if (gridSlot == null)
			{
				gridSlot = CheckSlot(position + p_offset2);
			}
			if (gridSlot == null)
			{
				gridSlot = CheckSlot(position + p_offset1 + p_offset2);
			}
			return gridSlot;
		}

		private GridSlot CheckSlot(Position position)
		{
			GridSlot slot = m_grid.GetSlot(position);
			GridSlot slot2 = m_grid.GetSlot(position + EDirection.WEST);
			if (slot2.TerrainType != ETerrainType.PASSABLE || slot2.Entities.Count > 0)
			{
				return null;
			}
			slot2 = m_grid.GetSlot(position + EDirection.EAST);
			if (slot2.TerrainType != ETerrainType.PASSABLE || slot2.Entities.Count > 0)
			{
				return null;
			}
			slot2 = m_grid.GetSlot(position + EDirection.NORTH);
			if (slot2.TerrainType != ETerrainType.PASSABLE || slot2.Entities.Count > 0)
			{
				return null;
			}
			slot2 = m_grid.GetSlot(position + EDirection.SOUTH);
			if (slot2.TerrainType != ETerrainType.PASSABLE || slot2.Entities.Count > 0)
			{
				return null;
			}
			return slot;
		}

		private void MoveParty(GridSlot p_targetSlot)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			GridSlot slot = m_grid.GetSlot(party.Position);
			slot.RemoveEntity(party);
			p_targetSlot.AddEntity(party);
			if (!p_targetSlot.VisitedByParty)
			{
				p_targetSlot.VisitedByParty = true;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UNCOVERED_TILES, EventArgs.Empty);
			}
			party.Position = p_targetSlot.Position;
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(party, party.Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(party, EEventType.TELEPORT_ENTITY, p_eventArgs);
		}

		protected override void SpawnCagePiece(Position p_position, Boolean p_isVisible)
		{
			Monster monster = new Monster((!p_isVisible) ? 716 : 715, -1);
			monster.Position = p_position;
			monster.Direction = EDirection.EAST;
			monster.SpawnAnim = 1;
			LegacyLogic.Instance.WorldManager.MonsterGroupHandler.GetGroup(9999);
			monster.MonsterGroupID = 9999;
			if (m_grid.AddMovingEntity(monster.Position, monster))
			{
				LegacyLogic.Instance.WorldManager.SpawnObject(monster, monster.Position);
			}
		}
	}
}
