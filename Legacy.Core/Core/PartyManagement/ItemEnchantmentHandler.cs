using System;
using System.Collections.Generic;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.Spells;
using Legacy.Core.StaticData.Items;

namespace Legacy.Core.PartyManagement
{
	public class ItemEnchantmentHandler
	{
		private Character m_character;

		private FightValues m_fightValues;

		private Equipment m_equipment;

		private AttackResult m_attackResult;

		private Monster m_target;

		private Boolean m_counterAttack;

		private List<LogEntryEventArgs> m_logEntries;

		public void Init(Character p_character)
		{
			m_character = p_character;
			m_logEntries = new List<LogEntryEventArgs>();
		}

		public void FlushActionLog()
		{
			foreach (LogEntryEventArgs p_args in m_logEntries)
			{
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
			m_logEntries.Clear();
		}

		public void ResolveSkillLevelEffects(Equipment p_equip)
		{
			List<SuffixStaticData> suffixes = p_equip.Suffixes;
			Int32 suffixLevel = p_equip.SuffixLevel;
			foreach (SuffixStaticData suffixStaticData in suffixes)
			{
				Int32 p_value = (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
				switch (suffixStaticData.Effect)
				{
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_DAGGER:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_DAGGER, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_AXE:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_AXE, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_MACE:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_MACE, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_SWORD:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_SWORD, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_SPEAR:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_SPEAR, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_TWOHANDED:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_TWOHANDED, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_DUAL_WIELD:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_DUAL_WIELD, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_BOW:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_BOW, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_CROSSBOW:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_CROSSBOW, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_SHIELD:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_SHIELD, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_WARFARE:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_WARFARE, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_MAGICAL_FOCUS:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_MAGICAL_FOCUS, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_ENDURANCE:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_ENDURANCE, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_MYSTICISM:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_MYSTICISM, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_ARCANE_DISCIPLINE:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_ARCANE_DISCIPLINE, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_DODGE:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_DODGE, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_MEDIUM_ARMOR:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_MEDIUM_ARMOR, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_HEAVY_ARMOR:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_HEAVY_ARMOR, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_AIR:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_AIR_MAGIC, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_EARTH:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_EARTH_MAGIC, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_WATER:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_WATER_MAGIC, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_FIRE:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_FIRE_MAGIC, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_DARK:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_DARK_MAGIC, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_LIGHT:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_LIGHT_MAGIC, p_value);
					break;
				case ESuffixEffect.ADD_VIRTUAL_SKILL_LEVEL_PRIMORDIAL:
					m_character.SkillHandler.IncreaseVirtualSkillLevel(ESkillID.SKILL_PRIMORDIAL_MAGIC, p_value);
					break;
				}
			}
		}

		public void ResolveAttributeEffects(Equipment p_equip, ref Attributes p_attributes)
		{
			List<SuffixStaticData> suffixes = p_equip.Suffixes;
			Int32 suffixLevel = p_equip.SuffixLevel;
			foreach (SuffixStaticData suffixStaticData in suffixes)
			{
				switch (suffixStaticData.Effect)
				{
				case ESuffixEffect.INCREASE_HP:
					p_attributes.HealthPoints += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				case ESuffixEffect.INCREASE_MANA:
					p_attributes.ManaPoints += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				case ESuffixEffect.INCREASE_MIGHT:
					p_attributes.Might += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				case ESuffixEffect.INCREASE_MAGIC:
					p_attributes.Magic += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				case ESuffixEffect.INCREASE_PERCEPTION:
					p_attributes.Perception += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				case ESuffixEffect.INCREASE_VITALITY:
					p_attributes.Vitality += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				case ESuffixEffect.INCREASE_SPIRIT:
					p_attributes.Spirit += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				case ESuffixEffect.INCREASE_DESTINY:
					p_attributes.Destiny += (Int32)Math.Round(suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
					break;
				}
			}
		}

		public void AddExtraDamageFromSuffix(Attack p_mainAttack, EEquipSlots p_slot, Monster m_monster)
		{
			Equipment equipment = (Equipment)m_character.Equipment.GetItemAt(p_slot);
			List<SuffixStaticData> suffixes = equipment.Suffixes;
			foreach (SuffixStaticData suffixStaticData in suffixes)
			{
				if ((suffixStaticData.Effect == ESuffixEffect.ADDITIONAL_DAMAGE_UNDEAD && m_monster.StaticData.Class == EMonsterClass.UNDEAD) || (suffixStaticData.Effect == ESuffixEffect.ADDITIONAL_DAMAGE_ELEMENTALS && m_monster.StaticData.Class == EMonsterClass.ELEMENTAL))
				{
					for (Int32 i = 0; i < p_mainAttack.Damages.Count; i++)
					{
						Damage item = p_mainAttack.Damages[i];
						p_mainAttack.Damages.RemoveAt(i);
						item.Value += (Int32)Math.Round(item.Value * suffixStaticData.GetValueForLevel(equipment.SuffixLevel, equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
						p_mainAttack.Damages.Insert(i, item);
					}
				}
				if (suffixStaticData.Effect == ESuffixEffect.ADDITIONAL_DAMAGE_DEMON)
				{
					for (Int32 j = 0; j < m_monster.StaticData.Abilities.Length; j++)
					{
						if (m_monster.StaticData.Abilities[j].AbilityType == EMonsterAbilityType.DEMONIC_LINEAGE)
						{
							for (Int32 k = 0; k < p_mainAttack.Damages.Count; k++)
							{
								Damage item2 = p_mainAttack.Damages[k];
								p_mainAttack.Damages.RemoveAt(k);
								item2.Value += (Int32)Math.Round(item2.Value * suffixStaticData.GetValueForLevel(equipment.SuffixLevel, equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
								p_mainAttack.Damages.Insert(k, item2);
							}
							break;
						}
					}
				}
			}
		}

		public Int32 AddAdditionalStrikeSuffix(EEquipSlots p_slot)
		{
			Equipment equipment = (Equipment)m_character.Equipment.GetItemAt(p_slot);
			List<SuffixStaticData> suffixes = equipment.Suffixes;
			Int32 num = 0;
			foreach (SuffixStaticData suffixStaticData in suffixes)
			{
				if (suffixStaticData.Effect == ESuffixEffect.ADDITIONAL_STRIKE)
				{
					num++;
				}
			}
			return num;
		}

		public void ResolveFightValueEffects(EEquipSlots p_equipSlot, Equipment p_equip, FightValues p_fightValues)
		{
			m_fightValues = p_fightValues;
			Int32 prefixLevel = p_equip.PrefixLevel;
			List<PrefixStaticData> prefixes = p_equip.Prefixes;
			foreach (PrefixStaticData prefixStaticData in prefixes)
			{
				if (prefixStaticData.Effect == EPrefixEffect.INCREASE_ELEMENTAL_PROTECTION)
				{
					IncreaseElementalProtection(prefixStaticData, prefixLevel);
				}
				else if (prefixStaticData.Effect == EPrefixEffect.ADD_ELEMENTAL_DAMAGE)
				{
					AddElementalDamage(prefixStaticData, prefixLevel, p_equipSlot);
				}
			}
			Int32 suffixLevel = p_equip.SuffixLevel;
			List<SuffixStaticData> suffixes = p_equip.Suffixes;
			foreach (SuffixStaticData suffixStaticData in suffixes)
			{
				ESuffixEffect effect = suffixStaticData.Effect;
				switch (effect)
				{
				case ESuffixEffect.INCREASE_MELEE_ATTACK_VALUE:
					if (p_equipSlot == EEquipSlots.OFF_HAND)
					{
						m_fightValues.OffHandAttackValue += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					}
					else
					{
						m_fightValues.MainHandAttackValue += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					}
					break;
				case ESuffixEffect.INCREASE_RANGE_ATTACK_VALUE:
					m_fightValues.RangedAttackValue += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					break;
				default:
					switch (effect)
					{
					case ESuffixEffect.PROTECTION_AGAINST_PARALYZED:
						m_fightValues.ConditionProtectionParalysis += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
						break;
					case ESuffixEffect.PROTECTION_AGAINST_SLEEPING:
						m_fightValues.ConditionProtectionSleep += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
						break;
					case ESuffixEffect.PROTECTION_AGAINST_POISONED:
						m_fightValues.ConditionProtectionPoison += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
						break;
					case ESuffixEffect.PROTECTION_AGAINST_CONFUSED:
						m_fightValues.ConditionProtectionConfusion += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
						break;
					case ESuffixEffect.PROTECTION_AGAINST_WEAK:
						m_fightValues.ConditionProtectionWeakness += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
						break;
					case ESuffixEffect.PROTECTION_AGAINST_CURSED:
						m_fightValues.ConditionProtectionCurses += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
						break;
					case ESuffixEffect.PROTECTION_AGAINST_KNOCKOUT:
						m_fightValues.ConditionProtectionKnockOut += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
						break;
					}
					break;
				case ESuffixEffect.INCREASE_CRITICAL_HIT_CHANCE:
					if (p_equipSlot == EEquipSlots.OFF_HAND)
					{
						m_fightValues.CriticalOffHandHitChance += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) / 100f;
					}
					else if (p_equipSlot == EEquipSlots.RANGE_WEAPON)
					{
						m_fightValues.CriticalRangeHitChance += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) / 100f;
					}
					else
					{
						m_fightValues.CriticalMainHandHitChance += suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) / 100f;
					}
					break;
				case ESuffixEffect.INCREASE_CRITICAL_DAMAGE:
					if (p_equipSlot == EEquipSlots.OFF_HAND)
					{
						m_fightValues.OffHandCriticalDamageMod += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) / 100f;
					}
					else if (p_equipSlot == EEquipSlots.RANGE_WEAPON)
					{
						m_fightValues.RangeCriticalDamageMod += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) / 100f;
					}
					else
					{
						m_fightValues.MainHandCriticalDamageMod += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND) / 100f;
					}
					break;
				case ESuffixEffect.INCREASE_EVADE_VALUE:
					m_fightValues.EvadeValue += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					break;
				case ESuffixEffect.INCREASE_ARMOR_VALUE:
					m_fightValues.ArmorValue += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					break;
				case ESuffixEffect.INCREASE_MELEE_BLOCK_ATTEMPTS:
					m_fightValues.MeleeBlockAttempts += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					break;
				case ESuffixEffect.INCREASE_GENERAL_BLOCK_ATTEMPTS:
					m_fightValues.GeneralBlockAttempts += (Int32)suffixStaticData.GetValueForLevel(suffixLevel, p_equip.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
					break;
				}
			}
		}

		private void IncreaseElementalProtection(PrefixStaticData p_prefix, Int32 p_level)
		{
			EDamageType school = p_prefix.School;
			Int32 p_value = (Int32)Math.Round(p_prefix.GetValueForLevel(p_level), MidpointRounding.AwayFromZero);
			m_fightValues.Resistance.Add(school, p_value);
			if (school == EDamageType.PRIMORDIAL)
			{
				m_fightValues.Resistance.Add(EDamageType.AIR, p_value);
				m_fightValues.Resistance.Add(EDamageType.FIRE, p_value);
				m_fightValues.Resistance.Add(EDamageType.WATER, p_value);
				m_fightValues.Resistance.Add(EDamageType.EARTH, p_value);
				m_fightValues.Resistance.Add(EDamageType.DARK, p_value);
				m_fightValues.Resistance.Add(EDamageType.LIGHT, p_value);
			}
		}

		public void ResolveCombatEffects(AttackResult p_result, EEquipSlots p_attackHand, Monster p_target, Boolean p_counterAttack)
		{
			m_attackResult = p_result;
			m_target = p_target;
			m_counterAttack = p_counterAttack;
			BaseItem itemAt = m_character.Equipment.GetItemAt(p_attackHand);
			if (itemAt != null)
			{
				ResolveCombatEffects(itemAt as Equipment);
			}
			ResolveCombatEffects(m_character.Equipment.GetItemAt(EEquipSlots.BODY) as Equipment);
			ResolveCombatEffects(m_character.Equipment.GetItemAt(EEquipSlots.FEET) as Equipment);
			ResolveCombatEffects(m_character.Equipment.GetItemAt(EEquipSlots.HEAD) as Equipment);
			ResolveCombatEffects(m_character.Equipment.GetItemAt(EEquipSlots.HANDS) as Equipment);
		}

		private void ResolveCombatEffects(Equipment p_equip)
		{
			if (p_equip != null)
			{
				m_equipment = p_equip;
				ResolveCombatEffects();
			}
		}

		public void ResolveIncomingAttackEffects(AttackResult p_result, Monster p_attacker, Attack p_attackvalue)
		{
			if (p_result.Result == EResultType.HIT || p_result.Result == EResultType.CRITICAL_HIT)
			{
				for (Int32 i = 0; i < 10; i++)
				{
					Equipment equipment = m_character.Equipment.GetItemAt((EEquipSlots)i) as Equipment;
					if (equipment == null)
					{
						break;
					}
					foreach (SuffixStaticData suffixStaticData in equipment.Suffixes)
					{
						if (suffixStaticData.Effect == ESuffixEffect.DECREASE_INCOMING_DAMAGE)
						{
							for (Int32 j = 0; j < p_attackvalue.Damages.Count; j++)
							{
								Damage item = p_attackvalue.Damages[j];
								p_attackvalue.Damages.RemoveAt(j);
								item.Value -= (Int32)Math.Round(item.Value * suffixStaticData.GetValueForLevel(equipment.SuffixLevel, equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
								p_attackvalue.Damages.Insert(j, item);
							}
						}
					}
				}
			}
			Shield shield = m_character.Equipment.GetItemAt(EEquipSlots.OFF_HAND) as Shield;
			if (shield != null && p_result.Result == EResultType.BLOCK)
			{
				m_equipment = shield;
				m_target = p_attacker;
				m_attackResult = p_result;
				foreach (SuffixStaticData suffixStaticData2 in shield.Suffixes)
				{
					if (suffixStaticData2.ConditionHitByType == EHitTypeCondition.BLOCKED && IsEffectTriggered(suffixStaticData2))
					{
						ResolveSuffixEffects(suffixStaticData2, true);
					}
				}
			}
		}

		private void ResolveCombatEffects()
		{
			List<SuffixStaticData> suffixes = m_equipment.Suffixes;
			foreach (SuffixStaticData p_suffix in suffixes)
			{
				if (IsEffectTriggered(p_suffix))
				{
					if (m_attackResult.Result == EResultType.HIT)
					{
						ResolveNormalHitEffects(p_suffix);
					}
					if (m_attackResult.Result == EResultType.CRITICAL_HIT)
					{
						ResolveCriticalHitEffects(p_suffix);
					}
				}
			}
		}

		private Boolean IsEffectTriggered(SuffixStaticData p_suffix)
		{
			Single chanceForLevel = p_suffix.GetChanceForLevel(m_equipment.SuffixLevel, m_equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
			return chanceForLevel > 0f && (chanceForLevel >= 1f || !p_suffix.HasCondition() || Random.Range(0f, 1f) < chanceForLevel);
		}

		private void ResolveNormalHitEffects(SuffixStaticData p_suffix)
		{
			if (p_suffix.ConditionHits == EHitTypeCondition.SUCCESSFUL)
			{
				ResolveSuffixEffects(p_suffix, false);
			}
		}

		private void ResolveCriticalHitEffects(SuffixStaticData p_suffix)
		{
			if (p_suffix.ConditionHits == EHitTypeCondition.SUCCESSFUL)
			{
				ResolveSuffixEffects(p_suffix, false);
			}
			if (p_suffix.ConditionHits == EHitTypeCondition.CRITICAL)
			{
				ResolveSuffixEffects(p_suffix, false);
			}
		}

		private void ResolveSuffixEffects(SuffixStaticData p_suffix, Boolean p_buffEndDelayed)
		{
			switch (p_suffix.Effect)
			{
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_CHILLED:
				AfflictDebuff(m_target, EMonsterBuffType.CHILLED, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_IMMOBILISED:
				AfflictDebuff(m_target, EMonsterBuffType.IMMOBILISED, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_IMMOBILISED_GROUP:
				AfflictDebuffOnTargetTile(m_target, EMonsterBuffType.IMMOBILISED, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_MANASURGE:
				AfflictDebuff(m_target, EMonsterBuffType.MANASURGE, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_MEMORYGAP:
				AfflictDebuff(m_target, EMonsterBuffType.MEMORYGAP, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_POISON:
				AfflictDebuff(m_target, EMonsterBuffType.POISONED, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_SLEEP:
				AfflictDebuff(m_target, EMonsterBuffType.SLEEPING, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_IMPRISSONED:
				AfflictDebuff(m_target, EMonsterBuffType.IMPRISONED, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_ACIDSPLASH:
				AfflictDebuff(m_target, EMonsterBuffType.ACIDSPLASH, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_WEAKNESS:
				AfflictDebuff(m_target, EMonsterBuffType.WEAKNESS, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_AGONY:
				AfflictDebuff(m_target, EMonsterBuffType.AGONY, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_TERROR:
				AfflictDebuff(m_target, EMonsterBuffType.TERROR, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_SUNDERING:
				AfflictDebuff(m_target, EMonsterBuffType.SUNDERING, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_STUNNED:
				AfflictDebuff(m_target, EMonsterBuffType.MACE_STUN, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_GASH:
				AfflictDebuff(m_target, EMonsterBuffType.GASH, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_PROVOKE:
				AfflictDebuff(m_target, EMonsterBuffType.PROVOKE, p_suffix, p_buffEndDelayed);
				break;
			case ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_AIR:
				TargetDealMagicDamageOnTile(p_suffix);
				break;
			case ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_EARTH:
				TargetDealMagicDamageOnTile(p_suffix);
				break;
			case ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_WATER:
				TargetDealMagicDamageOnTile(p_suffix);
				break;
			case ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_FIRE:
				TargetDealMagicDamageOnTile(p_suffix);
				break;
			case ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_DARKNESS:
				TargetDealMagicDamageOnTile(p_suffix);
				break;
			case ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_LIGHT:
				TargetDealMagicDamageOnTile(p_suffix);
				break;
			case ESuffixEffect.HIT_TILE_MONSTERS_MAGIC_DAMAGE_PRIMORDIAL:
				TargetDealMagicDamageOnTile(p_suffix);
				break;
			case ESuffixEffect.LOSE_ALL_BLOCK_ATTEMPTS:
				RemoveAllBlockAttempts(p_suffix);
				break;
			case ESuffixEffect.GAIN_HP_PERCENTAGE_FROM_DEALT_DAMAGE:
				GainFromDealtDamage(p_suffix);
				break;
			case ESuffixEffect.GAIN_MP_PERCENTAGE_FROM_DEALT_DAMAGE:
				GainFromDealtDamage(p_suffix);
				break;
			}
		}

		public void AfflictDamageOfTargetsHP(Monster p_target, List<SuffixStaticData> e, Attack p_attack)
		{
			Int32 currentHealth = p_target.CurrentHealth;
			for (Int32 i = 0; i < e.Count; i++)
			{
				if (e[i].Effect == ESuffixEffect.DEAL_PRIMORDIAL_DAMAGE_IN_HP_AMOUNT)
				{
					for (Int32 j = 0; j < p_attack.Damages.Count; j++)
					{
						Damage item = p_attack.Damages[j];
						p_attack.Damages.RemoveAt(j);
						item.Value = currentHealth;
						p_attack.Damages.Insert(j, item);
					}
				}
			}
		}

		public List<Attack> DealDamageToAttacker(Monster p_attacker)
		{
			List<Attack> list = new List<Attack>();
			for (Int32 i = 0; i < 10; i++)
			{
				Equipment equipment = m_character.Equipment.GetItemAt((EEquipSlots)i) as Equipment;
				if (equipment != null)
				{
					if (i != 1 || !(equipment is MeleeWeapon) || (equipment as MeleeWeapon).GetSubType() != EEquipmentType.TWOHANDED)
					{
						List<SuffixStaticData> suffixes = equipment.Suffixes;
						for (Int32 j = 0; j < suffixes.Count; j++)
						{
							if (suffixes[j].Effect == ESuffixEffect.ATTACKING_MONSTER_DAMAGED)
							{
								EDamageType edamageType = suffixes[j].MagicSchool;
								if (edamageType == EDamageType.NONE)
								{
									edamageType = EDamageType.PHYSICAL;
								}
								Damage p_damages = new Damage(edamageType, (Int32)suffixes[j].GetValueForLevel(equipment.SuffixLevel, equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), 0f, 1f);
								Attack item = new Attack(m_character.FightValues.MainHandAttackValue, 0f, p_damages);
								EnchantmentEffectEventArgs item2 = new EnchantmentEffectEventArgs(suffixes[j], p_damages.Value);
								m_logEntries.Add(item2);
								list.Add(item);
							}
						}
					}
				}
			}
			return list;
		}

		private void AfflictDebuff(Monster p_target, EMonsterBuffType p_buff, SuffixStaticData p_suffix, Boolean p_buffEndDelayed)
		{
			if (p_suffix.MagicSchool != EDamageType.NONE && p_target.CombatHandler.TestEvadeSpell(p_suffix.MagicSchool, 0))
			{
				return;
			}
			MonsterBuff monsterBuff = BuffFactory.CreateMonsterBuff(p_buff, p_suffix.GetValueForLevel(m_equipment.SuffixLevel, m_equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND));
			monsterBuff.Causer = m_character;
			monsterBuff.DontTriggerOnFirstDamage = true;
			if (p_buffEndDelayed)
			{
				monsterBuff.Duration++;
			}
			p_target.AddBuff(monsterBuff);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ITEM_SUFFIX_APPLY_BUFF, new ItemSuffixApplyBuffEventArgs(p_target, p_suffix.Description, p_buff));
			MonsterBuffEntryEventArgs item = new MonsterBuffEntryEventArgs(p_target, monsterBuff, p_target.BuffHandler.HasBuff(monsterBuff));
			m_logEntries.Add(item);
		}

		private void AfflictDebuffOnTargetTile(Monster p_target, EMonsterBuffType p_buff, SuffixStaticData p_suffix, Boolean p_buffEndDelayed)
		{
			List<Monster> allMonstersOnTargetTile = LegacyLogic.Instance.WorldManager.Party.GetAllMonstersOnTargetTile(p_target);
			foreach (Monster p_target2 in allMonstersOnTargetTile)
			{
				AfflictDebuff(p_target2, p_buff, p_suffix, p_buffEndDelayed);
			}
		}

		private void GainFromDealtDamage(SuffixStaticData p_suffix)
		{
			Int32 suffixLevel = m_equipment.SuffixLevel;
			Single valueForLevel = p_suffix.GetValueForLevel(suffixLevel, m_equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
			Single num = m_attackResult.DamageDone;
			EnchantmentEffectEventArgs suffixEffectEntry;
			if (p_suffix.Effect == ESuffixEffect.GAIN_HP_PERCENTAGE_FROM_DEALT_DAMAGE)
			{
				Int32 num2 = Math.Max(1, (Int32)Math.Round(num * valueForLevel, MidpointRounding.AwayFromZero));
				m_character.ChangeHP(num2);
				suffixEffectEntry = new EnchantmentEffectEventArgs(p_suffix, num2);
				DamageEventArgs p_eventArgs = new DamageEventArgs(new AttackResult
				{
					Result = EResultType.HEAL,
					DamageResults = 
					{
						new DamageResult(EDamageType.HEAL, num2, 0, 1f)
					}
				});
				LegacyLogic.Instance.EventManager.InvokeEvent(m_character, EEventType.CHARACTER_HEALS, p_eventArgs);
			}
			else
			{
				Int32 num3 = Math.Max(1, (Int32)Math.Round(num * valueForLevel, MidpointRounding.AwayFromZero));
				m_character.ChangeMP(num3);
				suffixEffectEntry = new EnchantmentEffectEventArgs(p_suffix, num3);
			}
			m_character.FightHandler.FeedActionLog(suffixEffectEntry);
		}

		private void RemoveAllBlockAttempts(SuffixStaticData p_suffix)
		{
			m_target.CurrentGeneralBlockAttempts = 0;
			m_target.CurrentMeleeBlockAttempts = 0;
			EnchantmentEffectEventArgs suffixEffectEntry = new EnchantmentEffectEventArgs(p_suffix);
			m_character.FightHandler.FeedActionLog(suffixEffectEntry);
		}

		private void TargetDealMagicDamageOnTile(SuffixStaticData p_suffix)
		{
			Int32 suffixLevel = m_equipment.SuffixLevel;
			Int32 damageValue = (Int32)Math.Round(p_suffix.GetValueForLevel(suffixLevel, m_equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND), MidpointRounding.AwayFromZero);
			EnchantmentEffectEventArgs suffixEffectEntry = new EnchantmentEffectEventArgs(p_suffix);
			m_character.FightHandler.FeedActionLog(suffixEffectEntry);
			m_character.FightHandler.AttackAllMonstersOnTargetsTile(m_target, damageValue, p_suffix.MagicSchool, m_counterAttack);
		}

		public void AddElementalDamage(PrefixStaticData p_prefix, Int32 p_level, EEquipSlots p_attackHand)
		{
			EDamageType school = p_prefix.School;
			Int32 num = (Int32)Math.Round(p_prefix.GetValueForLevel(p_level), MidpointRounding.AwayFromZero);
			if (p_attackHand == EEquipSlots.MAIN_HAND)
			{
				DamageData damageData = m_fightValues.MainHandDamage[school];
				DamageData value = new DamageData(school, damageData.Minimum + num, damageData.Maximum + num);
				m_fightValues.MainHandDamage[school] = value;
			}
			else if (p_attackHand == EEquipSlots.OFF_HAND)
			{
				DamageData damageData2 = m_fightValues.OffHandDamage[school];
				DamageData value2 = new DamageData(school, damageData2.Minimum + num, damageData2.Maximum + num);
				m_fightValues.OffHandDamage[school] = value2;
			}
			else if (p_attackHand == EEquipSlots.RANGE_WEAPON)
			{
				DamageData damageData3 = m_fightValues.RangeDamage[school];
				DamageData value3 = new DamageData(school, damageData3.Minimum + num, damageData3.Maximum + num);
				m_fightValues.RangeDamage[school] = value3;
			}
		}

		public EDamageType MagicSchoolToDamageType(EMagicSchool p_school)
		{
			EDamageType result = EDamageType.PHYSICAL;
			String name = Enum.GetName(typeof(EMagicSchool), p_school);
			String[] names = Enum.GetNames(typeof(EDamageType));
			foreach (String text in names)
			{
				if (text == name)
				{
					result = (EDamageType)Enum.Parse(typeof(EDamageType), text);
				}
			}
			return result;
		}
	}
}
