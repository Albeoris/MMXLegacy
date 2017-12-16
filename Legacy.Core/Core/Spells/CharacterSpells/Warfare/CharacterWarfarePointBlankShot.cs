using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfarePointBlankShot : CharacterSpell
	{
		public CharacterWarfarePointBlankShot() : base(ECharacterSpell.WARFARE_POINT_BLANK_SHOT)
		{
		}

		public override Boolean CheckSpellConditions(Character p_sorcerer)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			return party.SelectedCharacter.Equipment.IsRangedAttackWeaponEquiped() && party.HasAggro() && LegacyLogic.Instance.WorldManager.Party.SelectedMonster != null;
		}

		public override Int32 GetTargets(List<Object> buffer)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Monster selectedMonster = party.SelectedMonster;
			if (selectedMonster != null && grid.LineOfSight(party.Position, selectedMonster.Position, true))
			{
				GridSlot slot = grid.GetSlot(selectedMonster.Position);
				foreach (MovingEntity movingEntity in slot.Entities)
				{
					if (movingEntity is Monster)
					{
						buffer.Add(movingEntity);
					}
				}
				return buffer.Count;
			}
			return 0;
		}

		protected override void HandleMonsters(Character p_sorcerer, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			foreach (Object obj in p_targets)
			{
				Monster monster = (Monster)obj;
				if (monster != null)
				{
					ExecuteRangedAttackOnTarget(monster, p_sorcerer);
				}
			}
		}

		private List<AttackResult> ExecuteRangedAttackOnTarget(Monster p_target, Character p_sorcerer)
		{
			if (p_target != null && p_target.IsAttackable)
			{
				List<Attack> p_attacks = CalculateStrikes(p_sorcerer);
				List<AttackResult> list = ExecuteRangedStrikes(p_attacks, p_target, p_sorcerer);
				for (Int32 i = 0; i < list.Count; i++)
				{
					p_target.ApplyDamages(list[i], p_sorcerer);
				}
				return list;
			}
			return null;
		}

		private List<AttackResult> ExecuteRangedStrikes(List<Attack> p_attacks, Monster p_target, Character p_sorcerer)
		{
			List<AttackResult> list = new List<AttackResult>();
			AttacksEventArgs attacksEventArgs = new AttacksEventArgs(false);
			foreach (Attack attack in p_attacks)
			{
				p_sorcerer.SelectedMonster = p_target;
				if (p_target != null && p_target.CurrentHealth > 0)
				{
					p_sorcerer.EnchantmentHandler.AddExtraDamageFromSuffix(attack, attack.AttackHand, p_target);
					Equipment equipment = p_sorcerer.Equipment.GetItemAt(attack.AttackHand) as Equipment;
					p_sorcerer.EnchantmentHandler.AfflictDamageOfTargetsHP(p_target, equipment.Suffixes, attack);
					AttackResult attackResult = p_target.CombatHandler.AttackMonster(p_sorcerer, attack, false, true, EDamageType.PHYSICAL, false, 0);
					list.Add(attackResult);
					p_sorcerer.FightHandler.FeedDelayedActionLog(attackResult);
					attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(p_target, attackResult));
					p_sorcerer.EnchantmentHandler.ResolveCombatEffects(attackResult, attack.AttackHand, p_target, false);
					p_sorcerer.SkillHandler.ResolveCombatEffects(attackResult, attack.AttackHand, p_target);
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(p_sorcerer, EEventType.CHARACTER_ATTACKS_RANGED, attacksEventArgs);
			p_sorcerer.SkillHandler.ResolveStrikeCombatEffects(true, p_target);
			return list;
		}

		private List<Attack> CalculateStrikes(Character p_sorcerer)
		{
			List<Attack> list = new List<Attack>();
			Attack attack = p_sorcerer.FightHandler.GetAttack(EEquipSlots.RANGE_WEAPON, false);
			if (attack != null)
			{
				list.Add(attack);
				Int32 num = p_sorcerer.SkillHandler.GetAdditionalStrikeCount(attack.AttackHand);
				for (Int32 i = 0; i < num; i++)
				{
					Attack attack2 = p_sorcerer.FightHandler.GetAttack(EEquipSlots.RANGE_WEAPON, false);
					if (attack2 != null)
					{
						list.Add(attack2);
					}
				}
				num = p_sorcerer.EnchantmentHandler.AddAdditionalStrikeSuffix(attack.AttackHand);
				for (Int32 j = 0; j < num; j++)
				{
					Attack attack3 = p_sorcerer.FightHandler.GetAttack(EEquipSlots.RANGE_WEAPON, false);
					if (attack3 != null)
					{
						list.Add(attack3);
					}
				}
			}
			return list;
		}
	}
}
