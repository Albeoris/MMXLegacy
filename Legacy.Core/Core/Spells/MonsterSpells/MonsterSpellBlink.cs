using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellBlink : MonsterSpell
	{
		public MonsterSpellBlink(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.BLINK, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Position position = LegacyLogic.Instance.WorldManager.Party.Position;
			Int32 num = 0;
			EDirection direction = EDirectionFunctions.GetDirection(position, p_monster.Position);
			Position position2 = p_monster.Position + direction;
			GridSlot slot = grid.GetSlot(position2);
			while (slot != null && slot.IsPassable(p_monster, false) && num < 4)
			{
				num++;
				position2 += direction;
				slot = grid.GetSlot(position2);
			}
			Int32 num2 = 0;
			EDirection direction2 = EDirectionFunctions.GetDirection(p_monster.Position, position);
			position2 = position + direction2;
			slot = grid.GetSlot(position2);
			while (slot != null && slot.IsPassable(p_monster, false) && num2 < 3)
			{
				num2++;
				position2 += direction2;
				slot = grid.GetSlot(position2);
			}
			if (num2 > 0)
			{
				num2++;
			}
			Position position3 = p_monster.Position;
			if (num2 > num)
			{
				for (Int32 i = 0; i < num2; i++)
				{
					position3 += direction2;
				}
			}
			else
			{
				for (Int32 i = 0; i < num; i++)
				{
					position3 += direction;
				}
			}
			Position position4 = p_monster.Position;
			grid.GetSlot(p_monster.Position).RemoveEntity(p_monster);
			grid.GetSlot(position3).AddEntity(p_monster);
			p_monster.Position = position3;
			p_monster.Direction = EDirectionFunctions.GetDirection(p_monster.Position, position);
			BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(p_monster, p_monster.Position);
			LegacyLogic.Instance.EventManager.InvokeEvent(p_monster, EEventType.SET_ENTITY_POSITION, p_eventArgs);
		}
	}
}
