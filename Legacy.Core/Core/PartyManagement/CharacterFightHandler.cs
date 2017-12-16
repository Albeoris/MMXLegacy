using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core.Abilities;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.NpcInteraction;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class CharacterFightHandler : BaseCombatHandler
	{
		private Character m_fighter;

		private Int32 m_currentMeleeBlockAttempts;

		private Int32 m_currentGeneralBlockAttempts;

		private List<LogEntryEventArgs> m_logEntries;

		private List<LogEntryEventArgs> m_delayedLogEntries;

		private List<LogEntryEventArgs> m_counterAttackLogEntries;

		private Character m_interceptingCharacter;

		public CharacterFightHandler(Character p_fighter)
		{
			m_fighter = p_fighter;
			m_logEntries = new List<LogEntryEventArgs>();
			m_delayedLogEntries = new List<LogEntryEventArgs>();
			m_counterAttackLogEntries = new List<LogEntryEventArgs>();
		}

		public event BlockSubstituteMethod BlockSubstitute;

		public Int32 CurrentMeleeBlockAttempts
		{
			get => m_currentMeleeBlockAttempts;
		    set => m_currentMeleeBlockAttempts = value;
		}

		public Int32 CurrentGeneralBlockAttempts
		{
			get => m_currentGeneralBlockAttempts;
		    set => m_currentGeneralBlockAttempts = value;
		}

		public Character InterceptingCharacter
		{
			get => m_interceptingCharacter;
		    set => m_interceptingCharacter = value;
		}

		public Boolean HasLogEntries => m_logEntries.Count > 0;

	    public List<AttackResult> ExecuteMeleeAttack()
		{
			return ExecuteMeleeAttack(false, 0f, false, false, false);
		}

		public List<AttackResult> ExecuteMeleeAttack(Boolean p_firstAttackAllwaysSuccessful, Single p_firstAttackDmgBonus, Boolean p_firstStrikeAllwaysCritical, Boolean p_addBonusDmg, Boolean p_addPercentageBonusDmg)
		{
			List<Attack> list = CalculateMeleeStrikes();
			if (p_firstAttackDmgBonus > 0f && list.Count > 0)
			{
				Attack attack = list[0];
				for (Int32 i = 0; i < attack.Damages.Count; i++)
				{
					Damage item = attack.Damages[0];
					attack.Damages.RemoveAt(0);
					if (!p_addBonusDmg)
					{
						if (!p_addPercentageBonusDmg)
						{
							item.Value *= (Int32)Math.Round(1f + p_firstAttackDmgBonus, MidpointRounding.AwayFromZero);
						}
						else
						{
							item.Value += (Int32)Math.Round(item.Value * p_firstAttackDmgBonus, MidpointRounding.AwayFromZero);
						}
					}
					else
					{
						item.Value += (Int32)Math.Round(p_firstAttackDmgBonus, MidpointRounding.AwayFromZero);
					}
					attack.Damages.Add(item);
				}
			}
			if (m_fighter.Class.IsWindsword() && m_fighter.SelectedMonster.CurrentHealth == m_fighter.SelectedMonster.StaticData.MaxHealthpoints)
			{
				p_firstStrikeAllwaysCritical = true;
				p_firstAttackAllwaysSuccessful = true;
				m_fighter.SelectedMonster.CombatHandler.CannotBlockThisTurn = true;
			}
			if (p_firstStrikeAllwaysCritical && list.Count > 0)
			{
				Attack attack2 = list[0];
				attack2.CriticalChance = 1f;
			}
			if (m_fighter.SelectedMonster.IsAttackable)
			{
				List<AttackResult> list2 = ExecuteStrikes(list, m_fighter.SelectedMonster, false, p_firstAttackAllwaysSuccessful);
				if (m_fighter.SelectedMonster != null)
				{
					for (Int32 j = 0; j < list2.Count; j++)
					{
						m_fighter.SelectedMonster.ApplyDamages(list2[j], m_fighter);
					}
				}
				return list2;
			}
			return null;
		}

		public Monster ExecuteRangedAttack()
		{
			Monster monster = LegacyLogic.Instance.WorldManager.Party.SelectedMonster;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (monster == null || !grid.LineOfSight(party.Position, monster.Position, true))
			{
				monster = LegacyLogic.Instance.WorldManager.Party.GetRandomMonsterInLineOfSight(true);
			}
			if (monster != null && !monster.IsAttackable)
			{
				GridSlot slot = grid.GetSlot(monster.Position);
				foreach (MovingEntity movingEntity in slot.Entities)
				{
					Monster monster2 = movingEntity as Monster;
					if (monster2 != null && monster2.IsAttackable)
					{
						monster = monster2;
						break;
					}
				}
			}
			if (monster != null && monster.IsAttackable)
			{
				List<Attack> p_attacks = CalculateRangedStrikes(monster.DistanceToParty <= 1f);
				List<AttackResult> list = ExecuteRangedStrikes(p_attacks, monster, false);
				for (Int32 i = 0; i < list.Count; i++)
				{
					monster.ApplyDamages(list[i], m_fighter);
				}
			}
			return monster;
		}

		public List<AttackResult> ExecuteRangedAttackOnTarget(Monster p_target)
		{
			if (p_target != null && p_target.IsAttackable)
			{
				List<Attack> p_attacks = CalculateRangedStrikes(p_target.DistanceToParty <= 1f);
				List<AttackResult> list = ExecuteRangedStrikes(p_attacks, p_target, false);
				for (Int32 i = 0; i < list.Count; i++)
				{
					p_target.ApplyDamages(list[i], m_fighter);
				}
				return list;
			}
			return null;
		}

		public void ExecuteRangedPierceAttack(Monster p_piercedTarget)
		{
			Monster nextRandomMonsterInLineOfSight = LegacyLogic.Instance.WorldManager.Party.GetNextRandomMonsterInLineOfSight(p_piercedTarget);
			if (nextRandomMonsterInLineOfSight != null && nextRandomMonsterInLineOfSight.IsAttackable && m_fighter.InRangedAttackRange((Int32)nextRandomMonsterInLineOfSight.DistanceToParty))
			{
				List<Attack> p_attacks = CalculateRangedStrikes(p_piercedTarget.DistanceToParty <= 1f);
				List<AttackResult> list = ExecuteRangedStrikes(p_attacks, nextRandomMonsterInLineOfSight, false);
				for (Int32 i = 0; i < list.Count; i++)
				{
					nextRandomMonsterInLineOfSight.ApplyDamages(list[i], m_fighter);
				}
			}
		}

		public void ExecuteCounterAttack(Monster p_target)
		{
			List<Attack> list = new List<Attack>(1);
			BaseItem itemAt = m_fighter.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			MeleeWeapon meleeWeapon = itemAt as MeleeWeapon;
			MagicFocus magicFocus = itemAt as MagicFocus;
			if (itemAt != null)
			{
				if (meleeWeapon != null)
				{
					Attack meleeAttack = GetMeleeAttack(EEquipSlots.MAIN_HAND);
					list.Add(meleeAttack);
				}
				else if (magicFocus != null)
				{
					Attack magicFocusAttack = GetMagicFocusAttack(EEquipSlots.MAIN_HAND);
					list.Add(magicFocusAttack);
				}
			}
			List<AttackResult> list2 = ExecuteStrikes(list, p_target, true, false);
			for (Int32 i = 0; i < list2.Count; i++)
			{
				p_target.ApplyDamages(list2[i], m_fighter);
			}
		}

		public void ExecuteDefend()
		{
			if (!m_fighter.IsDefendModeActive)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(m_fighter, EEventType.CHARACTER_DEFEND, EventArgs.Empty);
				m_fighter.IsDefendModeActive = true;
				m_fighter.CalculateCurrentAttributes();
			}
		}

		public void ResetDefendMode()
		{
			if (m_fighter.IsDefendModeActive)
			{
				m_fighter.IsDefendModeActive = false;
				m_fighter.CalculateCurrentAttributes();
			}
		}

		public List<Attack> CalculateMeleeStrikes()
		{
			List<Attack> list = new List<Attack>();
			BaseItem itemAt = m_fighter.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			MeleeWeapon meleeWeapon = itemAt as MeleeWeapon;
			MagicFocus magicFocus = itemAt as MagicFocus;
			if (itemAt != null)
			{
				if (meleeWeapon != null)
				{
					Attack meleeAttack = GetMeleeAttack(EEquipSlots.MAIN_HAND);
					list.Add(meleeAttack);
					AddStrikesFromSkills(list, EEquipSlots.MAIN_HAND, true);
					AddStrikesFromEnchantments(list, EEquipSlots.MAIN_HAND, true);
				}
				else if (magicFocus != null)
				{
					Attack magicFocusAttack = GetMagicFocusAttack(EEquipSlots.MAIN_HAND);
					list.Add(magicFocusAttack);
				}
				else
				{
					LegacyLogic.Instance.ActionLog.PushEntry(new MessageEventArgs("COMBAT_MESSAGE_NO_MELEE_WEAPON"));
				}
			}
			BaseItem itemAt2 = m_fighter.Equipment.GetItemAt(EEquipSlots.OFF_HAND);
			if (itemAt2 != null && ((meleeWeapon != null && meleeWeapon.GetSubType() != EEquipmentType.TWOHANDED) || (magicFocus != null && magicFocus.GetMagicfocusType() != EEquipmentType.MAGIC_FOCUS_TWOHANDED) || (meleeWeapon == null && magicFocus == null)))
			{
				MeleeWeapon meleeWeapon2 = itemAt2 as MeleeWeapon;
				Shield shield = itemAt2 as Shield;
				MagicFocus magicFocus2 = itemAt2 as MagicFocus;
				if (meleeWeapon2 != null)
				{
					Attack meleeAttack2 = GetMeleeAttack(EEquipSlots.OFF_HAND);
					list.Add(meleeAttack2);
					AddStrikesFromSkills(list, EEquipSlots.OFF_HAND, true);
					AddStrikesFromEnchantments(list, EEquipSlots.OFF_HAND, true);
				}
				else if (magicFocus2 != null)
				{
					Attack magicFocusAttack2 = GetMagicFocusAttack(EEquipSlots.OFF_HAND);
					list.Add(magicFocusAttack2);
				}
				else if (shield == null)
				{
					LegacyLogic.Instance.ActionLog.PushEntry(new MessageEventArgs("COMBAT_MESSAGE_NO_MELEE_WEAPON"));
				}
			}
			return list;
		}

		public Attack GetAttack(EEquipSlots p_attackHand, Boolean p_isMelee)
		{
			Attack result = null;
			switch (p_attackHand)
			{
			case EEquipSlots.MAIN_HAND:
			case EEquipSlots.OFF_HAND:
				result = GetMeleeAttack(p_attackHand);
				break;
			case EEquipSlots.RANGE_WEAPON:
				result = GetRangedAttack(p_isMelee);
				break;
			}
			return result;
		}

		public Attack GetMeleeAttack(EEquipSlots p_attackHand)
		{
			Boolean flag = p_attackHand == EEquipSlots.OFF_HAND;
			Single armorIgnoreValuePercent = m_fighter.SkillHandler.GetArmorIgnoreValuePercent(p_attackHand);
			List<Damage> meleeDamage = m_fighter.FightValues.GetMeleeDamage(flag, armorIgnoreValuePercent);
			Single p_attackValue = (!flag) ? m_fighter.FightValues.MainHandAttackValue : m_fighter.FightValues.OffHandAttackValue;
			Single p_criticalChance = (!flag) ? m_fighter.FightValues.CriticalMainHandHitChance : m_fighter.FightValues.CriticalOffHandHitChance;
			return new Attack(p_attackValue, p_criticalChance, meleeDamage)
			{
				AttackHand = p_attackHand
			};
		}

		public Attack GetMagicFocusAttack(EEquipSlots p_attackHand)
		{
			Boolean p_offHand = p_attackHand == EEquipSlots.OFF_HAND;
			List<Damage> meleeDamage = m_fighter.FightValues.GetMeleeDamage(p_offHand, 0f);
			return new Attack(0f, 0f, meleeDamage)
			{
				AttackHand = p_attackHand
			};
		}

		public Single CalculateAttackValue(EEquipSlots p_attackHand)
		{
			Single result = 0f;
			Int32 armorPenalty = GetArmorPenalty();
			Int32 attackValuePenaltyReduction = m_fighter.FightValues.AttackValuePenaltyReduction;
			Int32 num = Math.Max(armorPenalty - attackValuePenaltyReduction, 0);
			if (p_attackHand == EEquipSlots.MAIN_HAND)
			{
				result = ConfigManager.Instance.Game.MainHandAttackValue - num + m_fighter.CurrentAttributes.Perception;
			}
			else if (p_attackHand == EEquipSlots.OFF_HAND)
			{
				result = ConfigManager.Instance.Game.OffHandAttackValue - num + m_fighter.CurrentAttributes.Perception;
			}
			return result;
		}

		public Int32 GetArmorPenalty()
		{
			Int32 result = 0;
			Armor armor = m_fighter.Equipment.GetItemAt(EEquipSlots.BODY) as Armor;
			if (armor != null)
			{
				EEquipmentType subType = armor.GetSubType();
				Int32[] array;
				if (subType == EEquipmentType.LIGHT_ARMOR)
				{
					array = ConfigManager.Instance.Game.AttackValuePenaltiesLightArmor;
				}
				else
				{
					if (subType != EEquipmentType.HEAVY_ARMOR)
					{
						return 0;
					}
					array = ConfigManager.Instance.Game.AttackValuePenaltiesHeavyArmor;
				}
				Int32 num = armor.ModelLevel - 1;
				if (array == null || num < 0 || num >= array.Length)
				{
					throw new Exception("No attack value penalty defined for this item model!");
				}
				result = array[num];
			}
			return result;
		}

		private Attack GetRangedAttack(Boolean p_isMelee)
		{
			Attack attack = null;
			BaseItem itemAt = m_fighter.Equipment.GetItemAt(EEquipSlots.RANGE_WEAPON);
			if (itemAt != null)
			{
				List<Damage> rangedDamage = m_fighter.FightValues.GetRangedDamage(p_isMelee);
				Single rangedAttackValue = m_fighter.FightValues.RangedAttackValue;
				Single criticalRangeHitChance = m_fighter.FightValues.CriticalRangeHitChance;
				attack = new Attack(rangedAttackValue, criticalRangeHitChance, rangedDamage);
				attack.AttackHand = EEquipSlots.RANGE_WEAPON;
			}
			return attack;
		}

		public void ApplyWeaponDamageMods(DamageCollection p_damages, EEquipSlots p_hand)
		{
			MagicFocus magicFocus = m_fighter.Equipment.GetItemAt(p_hand) as MagicFocus;
			for (EDamageType edamageType = EDamageType.PHYSICAL; edamageType < EDamageType._MAX_; edamageType++)
			{
				DamageData damageData = p_damages[edamageType];
				if (damageData.Type == EDamageType.PHYSICAL)
				{
					if (p_hand == EEquipSlots.MAIN_HAND || p_hand == EEquipSlots.OFF_HAND)
					{
						Single factor = CalculateMeleeDamageFactor(p_hand);
						damageData *= factor;
					}
					else if (p_hand == EEquipSlots.RANGE_WEAPON)
					{
						Single factor2 = CalculateRangeDamageFactor();
						damageData *= factor2;
					}
					p_damages[edamageType] = damageData;
				}
				else if (magicFocus != null && damageData.Type == EDamageType.PRIMORDIAL)
				{
					Single num = CalculateMagicFocusDamageFactor(p_hand);
					damageData += magicFocus.GetBaseDamage() * (num - 1f);
					p_damages[edamageType] = damageData;
				}
			}
		}

		public Single CalculateCriticalHitChance(EEquipSlots p_hand)
		{
			Single result = 0f;
			Single num = m_fighter.CurrentAttributes.Destiny;
			BaseItem itemAt = m_fighter.Equipment.GetItemAt(p_hand);
			if (itemAt is MagicFocus)
			{
				return 0f;
			}
			if (p_hand == EEquipSlots.MAIN_HAND)
			{
				Single mainHandCriticalHitDestinyMultiplier = m_fighter.FightValues.MainHandCriticalHitDestinyMultiplier;
				result = mainHandCriticalHitDestinyMultiplier * num * 0.01f;
			}
			else if (p_hand == EEquipSlots.OFF_HAND)
			{
				Single offHandCriticalHitDestinyMultiplier = m_fighter.FightValues.OffHandCriticalHitDestinyMultiplier;
				result = offHandCriticalHitDestinyMultiplier * num * 0.01f;
			}
			else if (p_hand == EEquipSlots.RANGE_WEAPON)
			{
				Single rangedCriticalHitDestinyMultiplier = m_fighter.FightValues.RangedCriticalHitDestinyMultiplier;
				result = rangedCriticalHitDestinyMultiplier * num * 0.01f;
			}
			return result;
		}

		private Single CalculateMeleeDamageFactor(EEquipSlots p_hand)
		{
			Single num = 1f;
			Single num2 = m_fighter.CurrentAttributes.Might;
			if (p_hand == EEquipSlots.MAIN_HAND)
			{
				Single num3 = ConfigManager.Instance.Game.MainHandDamage;
				Single num4 = m_fighter.FightValues.MainHandSkillLevelBonus;
				num = m_fighter.FightValues.MainHandDamageFactor + num3 * num2 + num4;
			}
			else if (p_hand == EEquipSlots.OFF_HAND)
			{
				Single num3 = ConfigManager.Instance.Game.OffHandDamage;
				Single num4 = m_fighter.FightValues.OffHandSkillLevelBonus;
				Single dualWieldDamageBonus = m_fighter.FightValues.DualWieldDamageBonus;
				num = m_fighter.FightValues.OffHandDamageFactor + (num3 + dualWieldDamageBonus) * num2 + num4;
			}
			if (m_fighter.Class.IsWarmonger())
			{
				Single num5 = 1f - m_fighter.HealthPoints / (Single)m_fighter.MaximumHealthPoints;
				num *= 1f + num5;
			}
			return num;
		}

		private Single CalculateMagicFocusDamageFactor(EEquipSlots p_hand)
		{
			if (p_hand == EEquipSlots.MAIN_HAND)
			{
				return m_fighter.FightValues.MainHandDamageFactor;
			}
			if (p_hand == EEquipSlots.OFF_HAND)
			{
				return m_fighter.FightValues.OffHandDamageFactor;
			}
			return 1f;
		}

		private Single CalculateRangeDamageFactor()
		{
			Single num = m_fighter.CurrentAttributes.Perception;
			Single rangedSkillLevelBonus = m_fighter.FightValues.RangedSkillLevelBonus;
			Single rangedDamage = ConfigManager.Instance.Game.RangedDamage;
			Single num2 = 1f + rangedDamage * num + rangedSkillLevelBonus;
			if (m_fighter.Class.IsWarmonger())
			{
				Single num3 = 1f - m_fighter.HealthPoints / m_fighter.MaximumHealthPoints;
				num2 *= 1f + num3;
			}
			return num2;
		}

		public void CalculateMagicFactors(FightValues p_fightValues)
		{
			Single num = m_fighter.CurrentAttributes.Magic;
			Single magicDamage = ConfigManager.Instance.Game.MagicDamage;
			foreach (ESkillID key in p_fightValues.MagicPowerKeys)
			{
				Single num2 = p_fightValues.MagicFactorSkillBonus[key];
				p_fightValues.MagicPowers[key] = 1f + magicDamage * num + num2;
				if (m_fighter.Class.IsWarmonger())
				{
					Single num3 = 1f - m_fighter.HealthPoints / (Single)m_fighter.MaximumHealthPoints;
					p_fightValues.MagicPowers[key] = p_fightValues.MagicPowers[key] * (1f + num3);
				}
			}
		}

		public Single CalculateMagicCriticalHitChance()
		{
			Single magicCritChance = ConfigManager.Instance.Game.MagicCritChance;
			Single num = m_fighter.CurrentAttributes.Destiny;
			return magicCritChance * num * 0.01f;
		}

		public Single CalculateMagicCriticalDamageFactor()
		{
			Single magicCritFactor = ConfigManager.Instance.Game.MagicCritFactor;
			return 1f + magicCritFactor;
		}

		private void AddStrikesFromSkills(List<Attack> strikes, EEquipSlots p_attackHand, Boolean p_isMelee)
		{
			Int32 additionalStrikeCount = m_fighter.SkillHandler.GetAdditionalStrikeCount(p_attackHand);
			for (Int32 i = 0; i < additionalStrikeCount; i++)
			{
				Attack attack = GetAttack(p_attackHand, p_isMelee);
				if (attack != null)
				{
					strikes.Add(attack);
				}
			}
		}

		private void AddStrikesFromEnchantments(List<Attack> strikes, EEquipSlots p_attackHand, Boolean p_isMelee)
		{
			Int32 num = m_fighter.EnchantmentHandler.AddAdditionalStrikeSuffix(p_attackHand);
			for (Int32 i = 0; i < num; i++)
			{
				Attack attack = GetAttack(p_attackHand, p_isMelee);
				if (attack != null)
				{
					strikes.Add(attack);
				}
			}
		}

		private List<AttackResult> ExecuteStrikes(List<Attack> p_strikes, Monster p_target, Boolean p_counterAttack, Boolean p_firstStrikeAllwaysSuccessful)
		{
			p_target.AbilityHandler.ExecuteAttacks(m_fighter, p_strikes, false, EExecutionPhase.ON_CHARACTER_ATTACKS_MONSTER_BEFORE_DAMAGE_REDUCTION);
			List<AttackResult> list = new List<AttackResult>(p_strikes.Count);
			AttacksEventArgs attacksEventArgs = new AttacksEventArgs(p_counterAttack);
			for (Int32 i = 0; i < p_strikes.Count; i++)
			{
				Equipment equipment = m_fighter.Equipment.GetItemAt(p_strikes[i].AttackHand) as Equipment;
				Boolean flag = equipment is MagicFocus;
				m_fighter.EnchantmentHandler.AddExtraDamageFromSuffix(p_strikes[i], p_strikes[i].AttackHand, p_target);
				Boolean p_canBeBlockedOrEvaded = (!p_firstStrikeAllwaysSuccessful || i > 0) && !flag;
				m_fighter.EnchantmentHandler.AfflictDamageOfTargetsHP(p_target, equipment.Suffixes, p_strikes[i]);
				AttackResult attackResult = p_target.CombatHandler.AttackMonster(m_fighter, p_strikes[i], true, p_canBeBlockedOrEvaded, EDamageType.PHYSICAL, false, 0);
				list.Add(attackResult);
				if (p_counterAttack)
				{
					FeedCounterAttackActionLog(attackResult, p_target);
				}
				else
				{
					FeedActionLog(attackResult);
				}
				attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(p_target, attackResult));
				m_fighter.EnchantmentHandler.ResolveCombatEffects(attackResult, p_strikes[i].AttackHand, p_target, p_counterAttack);
				m_fighter.SkillHandler.ResolveCombatEffects(attackResult, p_strikes[i].AttackHand, p_target);
				List<Attack> additionalStrikesFromResult = m_fighter.SkillHandler.GetAdditionalStrikesFromResult(attackResult, p_strikes[i].AttackHand);
				if (additionalStrikesFromResult != null && additionalStrikesFromResult.Count > 0)
				{
					List<AttackResult> collection = ExecuteStrikes(additionalStrikesFromResult, p_target, p_counterAttack, false);
					list.AddRange(collection);
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(m_fighter, EEventType.CHARACTER_ATTACKS, attacksEventArgs);
			return list;
		}

		public List<AttackResult> ExecuteRangedStrikes(List<Attack> p_attacks, Monster p_target, Boolean p_pushToParty)
		{
			List<AttackResult> list = new List<AttackResult>();
			AttacksEventArgs attacksEventArgs = new AttacksEventArgs(false);
			attacksEventArgs.IsTryingToPushToParty = p_pushToParty;
			Boolean flag = false;
			foreach (Attack attack in p_attacks)
			{
				m_fighter.SelectedMonster = p_target;
				if (p_target != null && p_target.CurrentHealth > 0)
				{
					m_fighter.EnchantmentHandler.AddExtraDamageFromSuffix(attack, attack.AttackHand, p_target);
					Equipment equipment = m_fighter.Equipment.GetItemAt(attack.AttackHand) as Equipment;
					m_fighter.EnchantmentHandler.AfflictDamageOfTargetsHP(p_target, equipment.Suffixes, attack);
					AttackResult attackResult = p_target.CombatHandler.AttackMonster(m_fighter, attack, false, true, EDamageType.PHYSICAL, false, 0);
					list.Add(attackResult);
					FeedDelayedActionLog(attackResult);
					attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(p_target, attackResult));
					if (attackResult.Result != EResultType.EVADE && attackResult.Result != EResultType.BLOCK)
					{
						flag = true;
					}
					m_fighter.EnchantmentHandler.ResolveCombatEffects(attackResult, attack.AttackHand, p_target, false);
					m_fighter.SkillHandler.ResolveCombatEffects(attackResult, attack.AttackHand, p_target);
				}
			}
			attacksEventArgs.PushToParty = (p_pushToParty && flag);
			LegacyLogic.Instance.EventManager.InvokeEvent(m_fighter, EEventType.CHARACTER_ATTACKS_RANGED, attacksEventArgs);
			m_fighter.SkillHandler.ResolveStrikeCombatEffects(true, p_target);
			return list;
		}

		public void AttackAllMonstersOnTargetsTile(Monster p_target, Int32 damageValue, EDamageType type, Boolean p_counterAttack)
		{
			List<Monster> allMonstersOnTargetTile = LegacyLogic.Instance.WorldManager.Party.GetAllMonstersOnTargetTile(p_target);
			List<Damage> list = new List<Damage>();
			AttacksEventArgs attacksEventArgs = new AttacksEventArgs(p_counterAttack);
			foreach (Monster monster in allMonstersOnTargetTile)
			{
				list.Clear();
				Damage item = new Damage(type, damageValue, 0f, 1f);
				list.Add(item);
				Attack p_attack = new Attack(damageValue, 0f, list);
				AttackResult attackResult = monster.CombatHandler.AttackMonster(m_fighter, p_attack, false, true, type, true, 0);
				monster.ApplyDamages(attackResult, m_fighter);
				FeedActionLog(attackResult, monster);
				attacksEventArgs.Attacks.Add(new AttacksEventArgs.AttackedTarget(monster, attackResult));
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(m_fighter, EEventType.CHARACTER_ATTACKS, attacksEventArgs);
		}

		public void FeedActionLog(String p_message)
		{
			MessageEventArgs item = new MessageEventArgs(p_message);
			m_logEntries.Add(item);
		}

		public void FeedActionLog(SkillEffectStaticData p_skillEffect)
		{
			SkillEffectEventArgs item = new SkillEffectEventArgs(p_skillEffect);
			m_logEntries.Add(item);
		}

		public void FeedActionLog(EnchantmentEffectEventArgs suffixEffectEntry)
		{
			m_logEntries.Add(suffixEffectEntry);
		}

		public void FeedActionLog(AttackResult p_attackResult, Monster p_monster)
		{
			CombatEntryEventArgs item = new CombatEntryEventArgs(m_fighter, p_monster, p_attackResult, null);
			m_logEntries.Add(item);
		}

		public void FeedActionLog(AttackResult p_attackResult)
		{
			CombatEntryEventArgs item = new CombatEntryEventArgs(m_fighter, m_fighter.SelectedMonster, p_attackResult, null);
			m_logEntries.Add(item);
		}

		public void FeedActionLog(ExtraAttackEventArgs p_args)
		{
			m_logEntries.Add(p_args);
		}

		public void FeedActionLog(CastSpellEntryEventArgs p_args)
		{
			m_logEntries.Add(p_args);
		}

		public void FeedActionLog(SkillTierBonusEventArgs p_args)
		{
			m_counterAttackLogEntries.Add(p_args);
		}

		public void FeedDelayedActionLog(SpellEffectEntryEventArgs p_args)
		{
			m_delayedLogEntries.Add(p_args);
		}

		public void FeedDelayedActionLog(AttackResult p_result)
		{
			CombatEntryEventArgs item = new CombatEntryEventArgs(m_fighter, m_fighter.SelectedMonster, p_result, null);
			m_delayedLogEntries.Add(item);
		}

		public void FeedCounterAttackActionLog(AttackResult p_result, Monster p_target)
		{
			CombatEntryEventArgs item = new CombatEntryEventArgs(m_fighter, p_target, p_result, null);
			m_counterAttackLogEntries.Add(item);
		}

		public void FlushActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_logEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_logEntries.Clear();
		}

		public void FlushDelayedActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_delayedLogEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_delayedLogEntries.Clear();
		}

		public void FlushCounterAttackActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_counterAttackLogEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_counterAttackLogEntries.Clear();
		}

		private List<Attack> CalculateRangedStrikes(Boolean p_isMelee)
		{
			List<Attack> list = new List<Attack>();
			Attack rangedAttack = GetRangedAttack(p_isMelee);
			if (rangedAttack != null)
			{
				list.Add(rangedAttack);
				AddStrikesFromSkills(list, EEquipSlots.RANGE_WEAPON, p_isMelee);
				AddStrikesFromEnchantments(list, EEquipSlots.RANGE_WEAPON, p_isMelee);
			}
			return list;
		}

		public AttackResult RecalculateDamage(EResultType p_result, Attack p_attack, Boolean p_isMelee, EDamageType p_damageType, Int32 p_resistanceIgnore)
		{
			AttackResult attackResult = new AttackResult();
			attackResult.Result = p_result;
			CalculateDamage(p_attack.Damages, attackResult.Result == EResultType.CRITICAL_HIT, p_isMelee, p_damageType, attackResult.DamageResults, p_resistanceIgnore);
			return attackResult;
		}

		public override AttackResult AttackEntity(Attack p_attack, Boolean p_isMelee, EDamageType p_damageType, Boolean p_skipBlockEvadeTest, Int32 p_resistanceIgnore, Boolean skipBlock = false)
		{
			m_fighter.SkillHandler.OnCombatBegin(p_attack, p_isMelee, p_damageType);
			AttackResult attackResult = new AttackResult();
			Single num = m_fighter.FightValues.EvadeValue;
			if (!p_isMelee)
			{
				num += m_fighter.FightValues.EvadeValueRangedBonus;
			}
			skipBlock = (skipBlock && p_isMelee);
			if (LegacyLogic.Instance.WorldManager.Party.Buffs.HasActiveBuff(EPartyBuffs.SHADOW_CLOAK))
			{
				attackResult.Result = EResultType.EVADE;
			}
			else if (!p_skipBlockEvadeTest && TestEvade(p_attack.AttackValue, num, p_damageType, 0))
			{
				attackResult.Result = EResultType.EVADE;
			}
			else if (!p_skipBlockEvadeTest && !skipBlock && p_damageType == EDamageType.PHYSICAL && TestBlock(p_isMelee, true))
			{
				attackResult.Result = EResultType.BLOCK;
			}
			else
			{
				attackResult.Result = ((!TestCritical(p_attack.CriticalChance)) ? EResultType.HIT : EResultType.CRITICAL_HIT);
				CalculateDamage(p_attack.Damages, attackResult.Result == EResultType.CRITICAL_HIT, p_isMelee, p_damageType, attackResult.DamageResults, p_resistanceIgnore);
			}
			if (m_fighter.HealthPoints <= 0)
			{
				m_fighter.EndTurn();
			}
			m_fighter.SkillHandler.OnCombatEnd(attackResult, p_isMelee, p_damageType);
			if (attackResult.Result == EResultType.CRITICAL_HIT && p_damageType == EDamageType.PHYSICAL)
			{
				Equipment equipment = m_fighter.Equipment.DoArmorBreakCheck();
				if (equipment != null)
				{
					attackResult.BrokenItem = equipment;
				}
			}
			if (attackResult.Result == EResultType.BLOCK && p_damageType == EDamageType.PHYSICAL)
			{
				Equipment equipment2 = m_fighter.Equipment.DoShieldBreakCheck();
				if (equipment2 != null)
				{
					attackResult.BrokenItem = equipment2;
				}
			}
			return attackResult;
		}

		internal BloodMagicEventArgs ChangeHealth(AttackResult p_result, Monster p_attacker)
		{
			BloodMagicEventArgs result = null;
			m_fighter.ChangeHP(-p_result.DamageDone, p_attacker);
			if (m_fighter.Class.IsBloodcaller())
			{
				Single num = p_result.DamageDone / (Single)m_fighter.MaximumHealthPoints;
				Single num2 = num / 2f;
				Int32 num3 = (Int32)(m_fighter.CurrentAttributes.ManaPoints * num2 + 0.5f);
				if (num3 > 0)
				{
					m_fighter.ChangeMP(num3);
					StatusChangedEventArgs p_eventArgs = new StatusChangedEventArgs(StatusChangedEventArgs.EChangeType.MANA_POINTS);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_fighter, EEventType.CHARACTER_STATUS_CHANGED, p_eventArgs);
					result = new BloodMagicEventArgs(m_fighter, num3);
				}
			}
			return result;
		}

		public Boolean TestBlock(Boolean p_melee, Boolean p_canBeSubstituted)
		{
			Npc lastBlockSubstitute;
			if (HirelingBlock(out lastBlockSubstitute))
			{
				LegacyLogic.Instance.WorldManager.Party.LastBlockSubstitute = lastBlockSubstitute;
				return true;
			}
			if (m_fighter.ConditionHandler.HasCondition(ECondition.UNCONSCIOUS) || m_fighter.ConditionHandler.HasCondition(ECondition.DEAD) || m_fighter.ConditionHandler.HasCondition(ECondition.PARALYZED) || m_fighter.ConditionHandler.HasCondition(ECondition.SLEEPING) || m_fighter.ConditionHandler.HasCondition(ECondition.STUNNED))
			{
				return false;
			}
			Single generalBlockChance = m_fighter.FightValues.GeneralBlockChance;
			if (p_melee && m_currentMeleeBlockAttempts > 0)
			{
				return TestBlock(generalBlockChance, ref m_currentMeleeBlockAttempts, m_fighter.Class.IsShieldGuard());
			}
			if (m_currentGeneralBlockAttempts > 0)
			{
				return TestBlock(generalBlockChance, ref m_currentGeneralBlockAttempts, m_fighter.Class.IsShieldGuard());
			}
			return !p_melee && p_canBeSubstituted && BlockSubstitute != null && BlockSubstitute();
		}

		private Boolean HirelingBlock(out Npc p_npc)
		{
			HirelingHandler hirelingHandler = LegacyLogic.Instance.WorldManager.Party.HirelingHandler;
			NpcEffect npcEffect;
			Npc npc;
			if (hirelingHandler.HasEffect(ETargetCondition.HIRE_BLOCK, out npcEffect, out npc) && npcEffect.TargetEffect != ETargetCondition.NONE && hirelingHandler.AllowedPeriodicity(npc, ETargetCondition.HIRE_BLOCK))
			{
				npc.SetTurnCount(ETargetCondition.HIRE_BLOCK, npc.GetTurnCount(ETargetCondition.HIRE_BLOCK) + 1);
				Boolean flag = Random.Range(0f, 1f) < ConfigManager.Instance.Game.HirelingBlockChance;
				if (flag)
				{
					p_npc = npc;
					return true;
				}
			}
			p_npc = null;
			return false;
		}

		internal void CalculateDamage(List<Damage> p_damages, Boolean p_isCritical, Boolean p_isMelee, EDamageType p_damageType, List<DamageResult> p_damageResults, Int32 p_resistanceIgnoreValue)
		{
			for (Int32 i = 0; i < p_damages.Count; i++)
			{
				Damage damage = p_damages[i];
				damage.Value = (Int32)(LegacyLogic.Instance.WorldManager.DamageReceiveMultiplicator * damage.Value + 0.5f);
				Resistance resistanceByType = GetResistanceByType(damage.Type);
				Resistance resistance = new Resistance(resistanceByType.Type, Math.Max(0, resistanceByType.Value - p_resistanceIgnoreValue));
				DamageResult item = DamageResult.Create(damage, resistance);
				if (p_isCritical)
				{
					Single num = CalculateEffectiveCriticalBonusMod(damage.CriticalBonusMod);
					item.EffectiveValue = (Int32)Math.Round(item.EffectiveValue * (1f + num), MidpointRounding.AwayFromZero);
					item.ResistedValue = (Int32)Math.Round(item.ResistedValue * (1f + num), MidpointRounding.AwayFromZero);
				}
				m_fighter.SkillHandler.OnCombatReceivedDamage(ref damage, ref resistance, ref item, p_isCritical, p_isMelee, p_damageType);
				p_damageResults.Add(item);
			}
		}

		private Single CalculateEffectiveCriticalBonusMod(Single p_criticalBonusMod)
		{
			return p_criticalBonusMod - m_fighter.SkillHandler.GetIncomingCriticalDamageReduction();
		}

		public override Resistance GetResistanceByType(EDamageType type)
		{
			return m_fighter.FightValues.Resistance[type];
		}

		public delegate Boolean BlockSubstituteMethod();
	}
}
