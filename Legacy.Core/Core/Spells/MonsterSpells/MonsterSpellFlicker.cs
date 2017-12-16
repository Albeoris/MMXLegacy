using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellFlicker : MonsterSpell
	{
		public MonsterSpellFlicker(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.FLICKER, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = p_monster.Position;
			GridSlot slot = grid.GetSlot(position);
			Int32 num;
			if (LegacyLogic.Instance.WorldManager.Party.InCombat)
			{
				num = Random.Range(0, 4);
			}
			else
			{
				List<GridSlot> list = new List<GridSlot>();
				for (Int32 i = 0; i < 4; i++)
				{
					GridSlot slot2 = grid.GetSlot(position + (EDirection)i);
					if (slot2 != null && p_monster.CanPassTerrain(slot2.TerrainType))
					{
						list.Add(slot2);
					}
				}
				list.Sort((GridSlot a, GridSlot b) => Position.Distance(a.Position, LegacyLogic.Instance.WorldManager.Party.Position).CompareTo(Position.Distance(b.Position, LegacyLogic.Instance.WorldManager.Party.Position)));
				num = (Int32)EDirectionFunctions.GetDirection(position, list[0].Position);
			}
			EDirection p_dir = (EDirection)num;
			if (grid.AddMovingEntity(position + p_dir, p_monster))
			{
				slot.RemoveEntity(p_monster);
			}
			else
			{
				Int32 num2 = num - 1;
				while (num != num2)
				{
					if (num2 < 0)
					{
						num2 = 3;
					}
					if (grid.AddMovingEntity(position + (EDirection)num2, p_monster))
					{
						slot.RemoveEntity(p_monster);
						break;
					}
					num2--;
				}
			}
			if (position != p_monster.Position && LegacyLogic.Instance.WorldManager.Party.SelectedMonster == p_monster)
			{
				LegacyLogic.Instance.WorldManager.Party.SelectedMonster = null;
			}
		}
	}
}
