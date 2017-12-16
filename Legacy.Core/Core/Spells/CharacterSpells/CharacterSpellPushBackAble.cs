using System;
using System.Collections.Generic;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells
{
	public abstract class CharacterSpellPushBackAble : CharacterSpell
	{
		protected Int32 m_PushCount = 1;

		private static HashSet<Monster> m_CachePushed = new HashSet<Monster>();

		public CharacterSpellPushBackAble(ECharacterSpell type) : base(type)
		{
		}

		protected override void HandleMonsters(Character p_attacker, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			base.HandleMonsters(p_attacker, p_result, p_targets, p_magicFactor);
			m_CachePushed.Clear();
			for (Int32 i = p_result.SpellTargets.Count - 1; i >= 0; i--)
			{
				SpellTarget spellTarget = p_result.SpellTargets[i];
				Monster monster = spellTarget.Target as Monster;
				if (monster != null && m_CachePushed.Add(monster))
				{
					AttackedTarget attackedTarget = spellTarget as AttackedTarget;
					if (attackedTarget != null && attackedTarget.Result.Result != EResultType.EVADE)
					{
						PushBack(monster, p_result);
					}
					MonsterBuffTarget monsterBuffTarget = spellTarget as MonsterBuffTarget;
					if (monsterBuffTarget != null)
					{
						PushBack(monster, p_result);
					}
				}
			}
		}

		protected virtual void PushBack(Monster p_monster, SpellEventArgs p_result)
		{
			if (p_monster.CurrentHealth <= 0)
			{
				return;
			}
			if (p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.LARGE) || p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.BOSS) || p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.STATIC_OBJECT) || p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.SPIRIT_BOUND))
			{
				p_result.SpellTargets.Add(new PushedTarget(p_monster, p_monster.Position, false));
				if (p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.LARGE))
				{
					MonsterAbilityBase ability = p_monster.AbilityHandler.GetAbility(EMonsterAbilityType.LARGE);
					AbilityTriggeredEventArgs p_args = new AbilityTriggeredEventArgs(p_monster, ability);
					p_monster.AbilityHandler.AddEntry(ability.ExecutionPhase, p_args);
				}
				else if (p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.BOSS))
				{
					MonsterAbilityBase ability2 = p_monster.AbilityHandler.GetAbility(EMonsterAbilityType.BOSS);
					AbilityTriggeredEventArgs p_args2 = new AbilityTriggeredEventArgs(p_monster, ability2);
					p_monster.AbilityHandler.AddEntry(ability2.ExecutionPhase, p_args2);
				}
				else if (p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.STATIC_OBJECT))
				{
					MonsterAbilityBase ability3 = p_monster.AbilityHandler.GetAbility(EMonsterAbilityType.STATIC_OBJECT);
					AbilityTriggeredEventArgs p_args3 = new AbilityTriggeredEventArgs(p_monster, ability3);
					p_monster.AbilityHandler.AddEntry(ability3.ExecutionPhase, p_args3);
				}
				else if (p_monster.AbilityHandler.HasAbility(EMonsterAbilityType.SPIRIT_BOUND))
				{
					MonsterAbilityBase ability4 = p_monster.AbilityHandler.GetAbility(EMonsterAbilityType.SPIRIT_BOUND);
					AbilityTriggeredEventArgs p_args4 = new AbilityTriggeredEventArgs(p_monster, ability4);
					p_monster.AbilityHandler.AddEntry(ability4.ExecutionPhase, p_args4);
				}
				return;
			}
			Position position = p_monster.Position;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			EDirection direction = EDirectionFunctions.GetDirection(party.Position, p_monster.Position);
			for (Int32 i = 0; i < m_PushCount; i++)
			{
				if (!grid.CanMoveEntity(p_monster, direction))
				{
					break;
				}
				Position p_pos = p_monster.Position + direction;
				GridSlot slot = grid.GetSlot(p_monster.Position);
				if (grid.GetSlot(p_pos).AddEntity(p_monster))
				{
					if (party.SelectedMonster == p_monster)
					{
						party.SelectedMonster = null;
					}
					slot.RemoveEntity(p_monster);
				}
			}
			if (position != p_monster.Position)
			{
				p_result.SpellTargets.Add(new PushedTarget(p_monster, p_monster.Position, true));
			}
		}
	}
}
