using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareSnatch : CharacterSpell
	{
		private Boolean m_didHitMonster;

		public CharacterWarfareSnatch() : base(ECharacterSpell.WARFARE_SNATCH)
		{
		}

		protected override void HandleMonsters(Character p_attacker, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			if (p_targets.Count > 0 && p_targets[0] is Monster)
			{
				Monster monster = (Monster)p_targets[0];
				LegacyLogic.Instance.WorldManager.Party.SelectedMonster = monster;
				base.HandleMonsters(p_attacker, p_result, p_targets, p_magicFactor);
				if (m_didHitMonster)
				{
					Drag(monster, p_result);
				}
				m_didHitMonster = false;
			}
		}

		protected override List<AttackResult> MeleeAttackMonster(Character p_sorcerer, Monster p_monster)
		{
			if (p_monster != null && p_monster.IsAttackable)
			{
				m_didHitMonster = false;
				List<Attack> p_attacks = p_sorcerer.FightHandler.CalculateMeleeStrikes();
				List<AttackResult> list = p_sorcerer.FightHandler.ExecuteRangedStrikes(p_attacks, p_monster, true);
				for (Int32 i = 0; i < list.Count; i++)
				{
					p_monster.ApplyDamages(list[i], p_sorcerer);
					if (list[i].Result != EResultType.BLOCK && list[i].Result != EResultType.EVADE)
					{
						m_didHitMonster = true;
					}
				}
				return list;
			}
			return null;
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			return true;
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, m_staticData.Range);
		}

		protected void Drag(Monster p_monster, SpellEventArgs p_result)
		{
			if (p_monster.CurrentHealth <= 0)
			{
				return;
			}
			if (p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.LARGE) || p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.BOSS) || p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.STATIC_OBJECT))
			{
				return;
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			EDirection direction = EDirectionFunctions.GetDirection(p_monster.Position, party.Position);
			Int32 num = (Int32)Position.Distance(p_monster.Position, party.Position) - 1;
			for (Int32 i = 0; i < num; i++)
			{
				if (!grid.CanMoveEntity(p_monster, direction))
				{
					break;
				}
				Position p_pos = p_monster.Position + direction;
				GridSlot slot = grid.GetSlot(p_monster.Position);
				if (grid.GetSlot(p_pos).AddEntity(p_monster))
				{
					slot.RemoveEntity(p_monster);
				}
			}
			if (Position.Distance(p_monster.Position, party.Position) > 1f)
			{
				LegacyLogic.Instance.WorldManager.Party.SelectedMonster = null;
			}
		}
	}
}
