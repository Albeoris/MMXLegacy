using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class CharacterSkillHandler : ISaveGameObject
	{
		private Character m_character;

		private List<Skill> m_availableSkills;

		private List<SkillEffectStaticData> m_tempSkillEffects;

		private List<ESkillEffectType> m_criticalStrikeCounter;

		private Boolean m_firstMeleeStrikeBlocked;

		private Boolean m_firstMeleeStrikeEvaded;

		public CharacterSkillHandler(Character p_character)
		{
			if (p_character == null)
			{
				throw new ArgumentNullException("p_character");
			}
			m_availableSkills = new List<Skill>();
			m_tempSkillEffects = new List<SkillEffectStaticData>();
			m_criticalStrikeCounter = new List<ESkillEffectType>();
			m_firstMeleeStrikeBlocked = false;
			m_firstMeleeStrikeEvaded = false;
			m_character = p_character;
		}

		public Character Character => m_character;

	    public List<Skill> AvailableSkills => m_availableSkills;

	    public Boolean CanSpendSkillpoints => m_character.CanSpendSkillpoints;

	    public void Init()
		{
			if (m_availableSkills.Count > 0)
			{
				m_availableSkills.Clear();
			}
			CharacterClass @class = m_character.Class;
			InitSkills(m_availableSkills, @class.GetSkillIDs(ETier.GRAND_MASTER), ETier.GRAND_MASTER);
			InitSkills(m_availableSkills, @class.GetSkillIDs(ETier.MASTER), ETier.MASTER);
			InitSkills(m_availableSkills, @class.GetSkillIDs(ETier.EXPERT), ETier.EXPERT);
		    InitRacialSkills(m_availableSkills, @class.GetRacialSkills());
		}

	    private static void InitSkills(List<Skill> skillList, Int32[] p_skillIDs, ETier p_maxTier)
		{
			for (Int32 i = 0; i < p_skillIDs.Length; i++)
			{
				Skill item = new Skill(p_skillIDs[i], p_maxTier);
				skillList.Add(item);
			}
		}

	    private void InitRacialSkills(List<Skill> skillList, List<RacialSkillsStaticData> racialSkills)
	    {
	        foreach (RacialSkillsStaticData skillData in racialSkills)
            {
                Skill item = new Skill(skillData.Value, skillData.Tier);
                skillList.Add(item);
            }
	    }

	    public void SpendSkillPoint()
		{
			if (m_character.SkillPoints > 0)
			{
				m_character.SkillPoints--;
			}
		}

		public void RefundSkillPoint()
		{
			m_character.SkillPoints++;
		}

		public Boolean HasTemporarySpendPoints()
		{
			foreach (Skill skill in m_availableSkills)
			{
				if (skill.TemporaryLevel > 0)
				{
					return true;
				}
			}
			return false;
		}

		public void FinalizeTemporarySpendPoints()
		{
			foreach (Skill skill in m_availableSkills)
			{
				if (skill.TemporaryLevel > 0 && skill.Level - skill.TemporaryLevel == 0)
				{
					LegacyLogic.Instance.TrackingManager.TrackSkillTrained(skill, m_character);
				}
				skill.TemporaryLevel = 0;
			}
		}

		public void IncreaseSkillLevel(Int32 p_skillStaticID)
		{
			Skill skill = FindSkill(p_skillStaticID);
			if (skill != null && skill.Level < skill.MaxLevel)
			{
				ETier tier = skill.Tier;
				skill.Level++;
				if (skill.Tier != tier)
				{
					CheckTierIncreaseEffects(skill);
				}
				m_character.CalculateCurrentAttributes();
				LegacyLogic.Instance.EventManager.InvokeEvent(skill, EEventType.CHARACTER_SKILL_CHANGED, EventArgs.Empty);
			}
		}

		public void DecreaseSkillLevel(Int32 p_skillStaticID)
		{
			Skill skill = FindSkill(p_skillStaticID);
			if (skill != null && skill.Level > 0)
			{
				ETier tier = skill.Tier;
				skill.Level--;
				if (skill.Tier != tier)
				{
					RevertTierIncreaseEffects(skill, tier);
				}
				m_character.CalculateCurrentAttributes();
				LegacyLogic.Instance.EventManager.InvokeEvent(skill, EEventType.CHARACTER_SKILL_CHANGED, EventArgs.Empty);
			}
		}

		public void ResetVirtualSkillLevels()
		{
			foreach (Skill skill in m_availableSkills)
			{
				skill.VirtualSkillLevel = 0;
			}
		}

		public void IncreaseVirtualSkillLevel(ESkillID p_skillID, Int32 p_value)
		{
			Skill skill = FindSkill((Int32)p_skillID);
			if (skill != null)
			{
				skill.VirtualSkillLevel += p_value;
			}
		}

		public void SetMaxSkillLevel(Int32 p_skillStaticID)
		{
			Skill skill = FindSkill(p_skillStaticID);
			if (skill != null)
			{
				skill.Tier = skill.MaxTier;
				skill.Level = skill.MaxLevel;
				m_character.CalculateCurrentAttributes();
			}
		}

		public void SetSkillTier(Int32 p_skillStaticID, ETier p_skillTier)
		{
			Skill skill = FindSkill(p_skillStaticID);
			if (skill != null)
			{
				ETier tier = skill.Tier;
				skill.Tier = p_skillTier;
				CheckTierIncreaseEffects(skill);
				m_character.CalculateCurrentAttributes();
				LegacyLogic.Instance.TrackingManager.TrackSkillTrained(skill, m_character);
				if (tier != p_skillTier)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(skill, EEventType.CHARACTER_SKILL_TIER_CHANGED, new SkillTierChangedEventArgs(skill, m_character));
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(skill, EEventType.CHARACTER_SKILL_CHANGED, EventArgs.Empty);
			}
		}

		private void CheckTierIncreaseEffects(Skill p_skill)
		{
			foreach (SkillEffectStaticData skillEffectStaticData in p_skill.CurrentlyAvailableEffects)
			{
				if (skillEffectStaticData.Type == ESkillEffectType.LEARN_SPELL)
				{
					m_character.SpellHandler.AddSpell((ECharacterSpell)skillEffectStaticData.Value);
				}
			}
		}

		private void RevertTierIncreaseEffects(Skill p_skill, ETier p_tier)
		{
			List<SkillEffectStaticData> list;
			switch (p_tier)
			{
			case ETier.EXPERT:
				list = p_skill.Tier2Effects;
				break;
			case ETier.MASTER:
				list = p_skill.Tier3Effects;
				break;
			case ETier.GRAND_MASTER:
				list = p_skill.Tier4Effects;
				break;
			default:
				list = p_skill.Tier1Effects;
				break;
			}
			foreach (SkillEffectStaticData skillEffectStaticData in list)
			{
				if (skillEffectStaticData.Type == ESkillEffectType.LEARN_SPELL)
				{
					m_character.SpellHandler.RemoveSpell((ECharacterSpell)skillEffectStaticData.Value);
				}
			}
		}

		public Boolean HasRequiredSkill(Int32 p_skillStaticID)
		{
			Skill skill = FindSkill(p_skillStaticID);
			return skill != null && skill.Tier >= ETier.NOVICE;
		}

		public Boolean HasRequiredSkillLevel(Int32 p_skillStaticID, Int32 p_skillLevel)
		{
			Skill skill = FindSkill(p_skillStaticID);
			return skill != null && skill.Level >= p_skillLevel;
		}

		public Boolean HasRequiredSkillTier(Int32 p_skillStaticID, ETier p_skillTier)
		{
			Skill skill = FindSkill(p_skillStaticID);
			return skill != null && skill.Tier >= p_skillTier;
		}

		public Skill FindSkill(Int32 p_skillStaticID)
		{
			foreach (Skill skill in m_availableSkills)
			{
				if (skill.StaticID == p_skillStaticID)
				{
					return skill;
				}
			}
			return null;
		}

		private Skill GetRequiredSkill(BaseItem p_equipment)
		{
			Int32 requiredSkillID = GetRequiredSkillID(p_equipment);
			return FindSkill(requiredSkillID);
		}

		public Boolean IsSkillRequirementFulfilled(Equipment p_equip, EEquipSlots p_slot)
		{
			Int32 requiredSkillID = GetRequiredSkillID(p_equip);
			ETier p_skillTier = GetRequiredSkillTier(p_equip);
			Int32 skillId = 0;
			MeleeWeapon meleeWeapon = p_equip as MeleeWeapon;
			if (meleeWeapon != null)
			{
				if (meleeWeapon.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND)
				{
					if (meleeWeapon.GetWeaponType() != EEquipmentType.SPEAR)
					{
						skillId = 8;
					}
				}
				else if (p_slot == EEquipSlots.OFF_HAND)
				{
					skillId = 10;
				}
			}
			MagicFocus magicFocus = p_equip as MagicFocus;
			if (magicFocus != null && p_slot == EEquipSlots.OFF_HAND)
			{
				p_skillTier = ETier.MASTER;
			}
			return (requiredSkillID == 0 || HasRequiredSkillTier(requiredSkillID, p_skillTier)) && (skillId == 0 || HasRequiredSkillTier(skillId, ETier.NOVICE));
		}

		public static ETier GetRequiredSkillTier(BaseItem p_equip)
		{
			ISkillDependant skillDependant = p_equip as ISkillDependant;
			if (skillDependant != null)
			{
				return skillDependant.GetRequiredSkillTier();
			}
			return ETier.NONE;
		}

		public void EndTurn()
		{
			m_criticalStrikeCounter.Clear();
			m_firstMeleeStrikeBlocked = false;
			m_firstMeleeStrikeEvaded = false;
		}

		public Int32 GetRangedAttackRange()
		{
			Int32 num = 0;
			BaseItem itemAt = m_character.Equipment.GetItemAt(EEquipSlots.RANGE_WEAPON);
			List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
			foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
			{
				if (skillEffectStaticData.Type == ESkillEffectType.SET_ATTACK_RANGE && skillEffectStaticData.Value > num)
				{
					num = (Int32)skillEffectStaticData.Value;
				}
			}
			if (m_character.Class.IsPathfinder())
			{
				num += 2;
			}
			return num;
		}

		public Int32 GetAdditionalStrikeCount(EEquipSlots p_attackHand)
		{
			Int32 num = 0;
			BaseItem itemAt = m_character.Equipment.GetItemAt(p_attackHand);
			List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
			foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
			{
				if (skillEffectStaticData.Type == ESkillEffectType.ADDITIONAL_STRIKE)
				{
					num++;
				}
			}
			if (p_attackHand == EEquipSlots.OFF_HAND && HasRequiredSkill(10))
			{
				Skill skill = FindSkill(10);
				foreach (SkillEffectStaticData skillEffectStaticData2 in skill.CurrentlyAvailableEffects)
				{
					if (skillEffectStaticData2.Type == ESkillEffectType.ADDITIONAL_STRIKE)
					{
						num++;
					}
				}
			}
			return num;
		}

		public void ResolveCombatEffects(AttackResult p_result, EEquipSlots p_attackHand, Monster p_target)
		{
			if (p_result.Result == EResultType.CRITICAL_HIT)
			{
				BaseItem itemAt = m_character.Equipment.GetItemAt(p_attackHand);
				List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
				foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
				{
					if (skillEffectStaticData.Condition == ESkillEffectCondition.CRITICAL_HIT)
					{
						if (skillEffectStaticData.Type == ESkillEffectType.STUN_MONSTER)
						{
							MonsterBuffMaceStun p_buff = (MonsterBuffMaceStun)BuffFactory.CreateMonsterBuff(EMonsterBuffType.MACE_STUN, 0f);
							p_target.AddBuff(p_buff);
							m_character.FightHandler.FeedActionLog(skillEffectStaticData);
						}
					}
					else if (skillEffectStaticData.Condition == ESkillEffectCondition.FIRST_CRITICAL_HIT && skillEffectStaticData.Type == ESkillEffectType.APPLY_GASH && !m_criticalStrikeCounter.Contains(skillEffectStaticData.Type))
					{
						MonsterBuffGash p_buff2 = (MonsterBuffGash)BuffFactory.CreateMonsterBuff(EMonsterBuffType.GASH, p_result.DamageDone);
						p_target.AddBuff(p_buff2);
						m_character.FightHandler.FeedActionLog(skillEffectStaticData);
						m_criticalStrikeCounter.Add(skillEffectStaticData.Type);
					}
				}
			}
		}

		public void ResolveStrikeCombatEffects(Boolean p_ranged, Monster p_target)
		{
			if (p_ranged)
			{
				BaseItem itemAt = m_character.Equipment.GetItemAt(EEquipSlots.RANGE_WEAPON);
				List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
				foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
				{
					if (skillEffectStaticData.Type == ESkillEffectType.PIERCE_ATTACK)
					{
						m_character.FightHandler.ExecuteRangedPierceAttack(p_target);
					}
				}
			}
		}

		public List<Attack> GetAdditionalStrikesFromResult(AttackResult p_result, EEquipSlots p_attackHand)
		{
			if (p_result.Result == EResultType.CRITICAL_HIT)
			{
				return GetAdditionalStrikesFromCriticalHit(p_attackHand);
			}
			return null;
		}

		private List<Attack> GetAdditionalStrikesFromCriticalHit(EEquipSlots p_attackHand)
		{
			List<Attack> list = new List<Attack>();
			BaseItem itemAt = m_character.Equipment.GetItemAt(p_attackHand);
			List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
			foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
			{
				if (skillEffectStaticData.Condition == ESkillEffectCondition.FIRST_CRITICAL_HIT && skillEffectStaticData.Type == ESkillEffectType.ADDITIONAL_MAIN_HAND_STRIKE && !m_criticalStrikeCounter.Contains(skillEffectStaticData.Type))
				{
					Attack meleeAttack = m_character.FightHandler.GetMeleeAttack(EEquipSlots.MAIN_HAND);
					if (meleeAttack != null)
					{
						list.Add(meleeAttack);
						m_character.FightHandler.FeedActionLog(skillEffectStaticData);
					}
					m_criticalStrikeCounter.Add(skillEffectStaticData.Type);
					return list;
				}
			}
			return list;
		}

		private List<SkillEffectStaticData> GetRequiredSkillEffects(BaseItem p_item)
		{
			if (p_item != null)
			{
				Skill requiredSkill = GetRequiredSkill(p_item);
				if (requiredSkill != null)
				{
					return requiredSkill.CurrentlyAvailableEffects;
				}
			}
			return m_tempSkillEffects;
		}

		public Int32 GetResistanceIgnoreValue(ESkillID p_magicSchool)
		{
			Int32 num = 0;
			Skill skill = FindSkill((Int32)p_magicSchool);
			if (skill != null)
			{
				List<SkillEffectStaticData> currentlyAvailableEffects = skill.CurrentlyAvailableEffects;
				foreach (SkillEffectStaticData skillEffectStaticData in currentlyAvailableEffects)
				{
					if (skillEffectStaticData.Type == ESkillEffectType.IGNORE_RESISTANCE)
					{
						num = (Int32)ResolveValue(num, skillEffectStaticData, skill);
					}
				}
			}
			return num;
		}

		public Single GetArmorIgnoreValuePercent(EEquipSlots p_attackHand)
		{
			Single num = 0f;
			BaseItem itemAt = m_character.Equipment.GetItemAt(p_attackHand);
			Skill requiredSkill = GetRequiredSkill(itemAt);
			List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
			foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
			{
				if (skillEffectStaticData.Type == ESkillEffectType.IGNORE_ARMOR_VALUE)
				{
					num = ResolveValue(num, skillEffectStaticData, requiredSkill);
				}
			}
			return num;
		}

		public void AddHealthMana(ref Attributes p_attributes)
		{
			if (HasRequiredSkill(15))
			{
				IncreaseMaximumHealthPoints(ref p_attributes);
			}
			if (HasRequiredSkill(18) || HasRequiredSkill(11))
			{
				IncreaseMaximumManaPoints(ref p_attributes);
			}
		}

		private void IncreaseMaximumHealthPoints(ref Attributes p_attributes)
		{
			Single num = 1f;
			foreach (Skill skill in m_availableSkills)
			{
				foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
				{
					if (skillEffectStaticData.Type == ESkillEffectType.INCREASE_MAXIMUM_HEALTH)
					{
						if (skillEffectStaticData.Mode == ESkillEffectMode.PER_SKILL_LEVEL)
						{
							p_attributes.HealthPoints = (Int32)Math.Round(ResolveValue(p_attributes.HealthPoints, skillEffectStaticData, skill), MidpointRounding.AwayFromZero);
						}
						else
						{
							num += skillEffectStaticData.Value;
						}
					}
				}
			}
			p_attributes.HealthPoints = (Int32)Math.Round(num * p_attributes.HealthPoints, MidpointRounding.AwayFromZero);
		}

		private void IncreaseMaximumManaPoints(ref Attributes p_attributes)
		{
			Single num = 1f;
			foreach (Skill skill in m_availableSkills)
			{
				foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
				{
					if (skillEffectStaticData.Type == ESkillEffectType.INCREASE_MAXIMUM_MANA)
					{
						if (skillEffectStaticData.Mode == ESkillEffectMode.PER_SKILL_LEVEL)
						{
							p_attributes.ManaPoints = (Int32)Math.Round(ResolveValue(p_attributes.ManaPoints, skillEffectStaticData, skill), MidpointRounding.AwayFromZero);
						}
						else
						{
							num += skillEffectStaticData.Value;
						}
					}
				}
			}
			p_attributes.ManaPoints = (Int32)Math.Round(num * p_attributes.ManaPoints, MidpointRounding.AwayFromZero);
		}

        public Single GetReducePriceDifferenceCoefficient()
        {
            Single result = 0;

            foreach (Skill skill in m_availableSkills)
            {
                foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
                {
                    ESkillEffectType type = skillEffectStaticData.Type;
                    if (type == ESkillEffectType.ReducePriceDifference)
                        result = ResolveValue(result, skillEffectStaticData, skill);
                }
            }

            return result;
        }

		public void AddFightValues(FightValues p_fightValues)
		{
			AddEquipmentDependentFightValues(p_fightValues);
			foreach (Skill skill in m_availableSkills)
			{
				foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
				{
					ESkillEffectType type = skillEffectStaticData.Type;
					switch (type)
					{
					case ESkillEffectType.INCREASE_MAGIC_RESISTANCE:
						IncreaseMagicResistance(p_fightValues, skillEffectStaticData, skill);
						break;
					default:
						if (type == ESkillEffectType.INCREASE_MAGIC_FACTOR_SKILL_BONUS)
						{
							IncreaseMagicSkillBonus(p_fightValues, skillEffectStaticData, skill);
						}
						break;
					case ESkillEffectType.INCREASE_EVADE_VALUE:
						p_fightValues.EvadeValue = (Int32)Math.Round(ResolveValue(p_fightValues.EvadeValue, skillEffectStaticData, skill), MidpointRounding.AwayFromZero);
						break;
					}
				}
			}
		}

		private void AddEquipmentDependentFightValues(FightValues p_fightValues)
		{
			BaseItem itemAt = m_character.Equipment.GetItemAt(EEquipSlots.MAIN_HAND);
			BaseItem itemAt2 = m_character.Equipment.GetItemAt(EEquipSlots.OFF_HAND);
			BaseItem itemAt3 = m_character.Equipment.GetItemAt(EEquipSlots.RANGE_WEAPON);
			BaseItem itemAt4 = m_character.Equipment.GetItemAt(EEquipSlots.BODY);
			AddFightValues(p_fightValues, itemAt, EEquipSlots.MAIN_HAND);
			if (itemAt != itemAt2)
			{
				AddFightValues(p_fightValues, itemAt2, EEquipSlots.OFF_HAND);
			}
			AddFightValues(p_fightValues, itemAt3, EEquipSlots.RANGE_WEAPON);
			AddFightValues(p_fightValues, itemAt4, EEquipSlots.BODY);
		}

		private void AddFightValues(FightValues p_fightValues, BaseItem p_equipment, EEquipSlots p_slot)
		{
			if (p_equipment is MagicFocus && p_slot == EEquipSlots.OFF_HAND && m_character.Equipment.GetItemAt(EEquipSlots.MAIN_HAND) is MagicFocus)
			{
				return;
			}
			Skill requiredSkill = GetRequiredSkill(p_equipment);
			if (requiredSkill != null && p_equipment is Equipment)
			{
				AddSkillEffects(p_fightValues, (Equipment)p_equipment, p_slot, requiredSkill);
			}
			MeleeWeapon meleeWeapon = p_equipment as MeleeWeapon;
			MeleeWeapon meleeWeapon2 = m_character.Equipment.GetItemAt(EEquipSlots.MAIN_HAND) as MeleeWeapon;
			if (meleeWeapon2 != null && meleeWeapon != null && p_slot == EEquipSlots.OFF_HAND)
			{
				Skill skill = FindSkill(10);
				if (skill != null)
				{
					AddSkillEffects(p_fightValues, meleeWeapon, p_slot, skill);
				}
			}
			if (meleeWeapon != null && meleeWeapon.GetSubType() == EEquipmentType.TWOHANDED)
			{
				Skill skill2 = FindSkill(8);
				if (skill2 != null)
				{
					AddSkillEffects(p_fightValues, meleeWeapon, p_slot, skill2);
				}
			}
		}

		private void AddSkillEffects(FightValues p_fightValues, Equipment p_equipment, EEquipSlots p_slot, Skill p_skill)
		{
			List<SkillEffectStaticData> currentlyAvailableEffects = p_skill.CurrentlyAvailableEffects;
			foreach (SkillEffectStaticData skillEffectStaticData in currentlyAvailableEffects)
			{
				ESkillEffectType type = skillEffectStaticData.Type;
				switch (type)
				{
				case ESkillEffectType.INCREASE_ATTACK_VALUE:
					IncreaseAttackValue(p_fightValues, skillEffectStaticData, p_skill, p_slot);
					break;
				case ESkillEffectType.INCREASE_DAMAGE_FACTOR:
					IncreaseDamageFactor(p_fightValues, skillEffectStaticData, p_skill, p_slot);
					break;
				case ESkillEffectType.INCREASE_DAMAGE_SKILL_BONUS:
					IncreaseDamageSkillBonus(p_fightValues, skillEffectStaticData, p_skill, p_slot);
					break;
				default:
					if (type == ESkillEffectType.INCREASE_ARMOR_VALUE)
					{
						IncreaseArmorValue(p_fightValues, skillEffectStaticData, p_skill);
					}
					break;
				case ESkillEffectType.INCREASE_CRITICAL_HIT_CHANCE:
					IncreaseCriticalHitChances(p_fightValues, skillEffectStaticData, p_skill, p_slot);
					break;
				case ESkillEffectType.INCREASE_CRITICAL_DAMAGE:
					IncreaseCriticalDamageFactor(p_fightValues, skillEffectStaticData, p_skill, p_slot);
					break;
				case ESkillEffectType.INCREASE_CRITICAL_MAGIC_DAMAGE:
					IncreaseCriticalMagicDamageFactor(p_fightValues, skillEffectStaticData, p_skill);
					break;
				case ESkillEffectType.INCREASE_CRITICAL_MAGIC_HIT_CHANCE:
					IncreaseMagicalCriticalHitChances(p_fightValues, skillEffectStaticData, p_skill);
					break;
				case ESkillEffectType.ADDITIONAL_BLOCK_ATTEMPT:
					IncreaseBlockAttempts(p_fightValues, skillEffectStaticData, p_skill);
					break;
				case ESkillEffectType.ADDITIONAL_MELEE_BLOCK_ATTEMPT:
					IncreaseMeleeBlockAttempts(p_fightValues, skillEffectStaticData, p_skill);
					break;
				case ESkillEffectType.INCREASE_CRITICAL_HIT_DESTINY_MULTIPLIER:
					IncreaseCriticalHitDestinyMultiplier(p_fightValues, p_slot, skillEffectStaticData, p_skill);
					break;
				case ESkillEffectType.DECREASE_DAMAGE_PENALTY:
					DecreaseDamageValuePenalty(p_fightValues, p_slot, skillEffectStaticData, p_skill);
					break;
				case ESkillEffectType.DECREASE_ATTACK_VALUE_PENALTY:
					DecreaseAttackValuePenalty(p_fightValues, skillEffectStaticData, p_skill);
					break;
				case ESkillEffectType.INCREASE_BLOCK_CHANCE:
					IncreaseBlockChance(p_fightValues, skillEffectStaticData, p_skill);
					break;
				}
			}
		}

		private void IncreaseMagicResistance(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			ResistanceCollection resistance = p_fightValues.Resistance;
			resistance.Set(new Resistance(EDamageType.AIR, (Int32)ResolveValue(resistance[EDamageType.AIR].Value, p_effect, p_skill)));
			resistance.Set(new Resistance(EDamageType.EARTH, (Int32)ResolveValue(resistance[EDamageType.EARTH].Value, p_effect, p_skill)));
			resistance.Set(new Resistance(EDamageType.FIRE, (Int32)ResolveValue(resistance[EDamageType.FIRE].Value, p_effect, p_skill)));
			resistance.Set(new Resistance(EDamageType.WATER, (Int32)ResolveValue(resistance[EDamageType.WATER].Value, p_effect, p_skill)));
			resistance.Set(new Resistance(EDamageType.DARK, (Int32)ResolveValue(resistance[EDamageType.DARK].Value, p_effect, p_skill)));
			resistance.Set(new Resistance(EDamageType.LIGHT, (Int32)ResolveValue(resistance[EDamageType.LIGHT].Value, p_effect, p_skill)));
			resistance.Set(new Resistance(EDamageType.PRIMORDIAL, (Int32)ResolveValue(resistance[EDamageType.PRIMORDIAL].Value, p_effect, p_skill)));
		}

		private void IncreaseCriticalHitChances(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill, EEquipSlots p_slot)
		{
			if (p_effect.Condition == ESkillEffectCondition.FOR_EACH_EQUIPMENT)
			{
				p_fightValues.CriticalMainHandHitChance = ResolveValue(p_fightValues.CriticalMainHandHitChance, p_effect, p_skill);
				p_fightValues.CriticalOffHandHitChance = ResolveValue(p_fightValues.CriticalOffHandHitChance, p_effect, p_skill);
			}
			else if (p_slot == EEquipSlots.RANGE_WEAPON)
			{
				p_fightValues.CriticalRangeHitChance = ResolveValue(p_fightValues.CriticalRangeHitChance, p_effect, p_skill);
			}
			else if (p_slot == EEquipSlots.MAIN_HAND)
			{
				p_fightValues.CriticalMainHandHitChance = ResolveValue(p_fightValues.CriticalMainHandHitChance, p_effect, p_skill);
			}
			else if (p_slot == EEquipSlots.OFF_HAND)
			{
				p_fightValues.CriticalOffHandHitChance = ResolveValue(p_fightValues.CriticalOffHandHitChance, p_effect, p_skill);
			}
		}

		private void IncreaseMagicalCriticalHitChances(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			p_fightValues.CriticalMagicHitChance = ResolveValue(p_fightValues.CriticalMagicHitChance, p_effect, p_skill);
		}

		private void IncreaseDamageFactor(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill, EEquipSlots p_slot)
		{
			if (p_effect.Condition == ESkillEffectCondition.IS_EQUIPED)
			{
				if (p_slot == EEquipSlots.MAIN_HAND)
				{
					p_fightValues.MainHandDamageFactor = ResolveValue(p_fightValues.MainHandDamageFactor, p_effect, p_skill);
				}
				else if (p_slot == EEquipSlots.OFF_HAND)
				{
					p_fightValues.OffHandDamageFactor = ResolveValue(p_fightValues.OffHandDamageFactor, p_effect, p_skill);
				}
			}
		}

		private void IncreaseCriticalDamageFactor(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill, EEquipSlots p_slot)
		{
			if (p_effect.Condition == ESkillEffectCondition.IS_EQUIPED)
			{
				if (p_slot == EEquipSlots.RANGE_WEAPON)
				{
					p_fightValues.RangeCriticalDamageMod = ResolveValue(p_fightValues.RangeCriticalDamageMod, p_effect, p_skill);
				}
				else if (p_slot == EEquipSlots.MAIN_HAND)
				{
					p_fightValues.MainHandCriticalDamageMod = ResolveValue(p_fightValues.MainHandCriticalDamageMod, p_effect, p_skill);
				}
				else if (p_slot == EEquipSlots.OFF_HAND)
				{
					p_fightValues.OffHandCriticalDamageMod = ResolveValue(p_fightValues.OffHandCriticalDamageMod, p_effect, p_skill);
				}
			}
		}

		private void IncreaseCriticalMagicDamageFactor(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			if (p_effect.Condition == ESkillEffectCondition.IS_EQUIPED)
			{
				p_fightValues.MagicalCriticalDamageMod = ResolveValue(p_fightValues.MagicalCriticalDamageMod, p_effect, p_skill);
			}
		}

		private void IncreaseDamageSkillBonus(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill, EEquipSlots p_slot)
		{
			if (p_effect.Condition == ESkillEffectCondition.IS_EQUIPED)
			{
				if (p_slot == EEquipSlots.MAIN_HAND)
				{
					p_fightValues.MainHandSkillLevelBonus = ResolveValue(p_fightValues.MainHandSkillLevelBonus, p_effect, p_skill);
				}
				else if (p_slot == EEquipSlots.OFF_HAND)
				{
					p_fightValues.OffHandSkillLevelBonus = ResolveValue(p_fightValues.OffHandSkillLevelBonus, p_effect, p_skill);
				}
				else if (p_slot == EEquipSlots.RANGE_WEAPON)
				{
					p_fightValues.RangedSkillLevelBonus = ResolveValue(p_fightValues.RangedSkillLevelBonus, p_effect, p_skill);
				}
			}
		}

		private void IncreaseMagicSkillBonus(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			p_fightValues.MagicFactorSkillBonus[(ESkillID)p_skill.StaticID] = ResolveValue(p_fightValues.MagicFactorSkillBonus[(ESkillID)p_skill.StaticID], p_effect, p_skill);
		}

		private void IncreaseArmorValue(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			if (p_effect.Condition == ESkillEffectCondition.IS_EQUIPED)
			{
				p_fightValues.ArmorValue = (Int32)ResolveValue(p_fightValues.ArmorValue, p_effect, p_skill);
			}
		}

		private void IncreaseAttackValue(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill, EEquipSlots p_slot)
		{
			if (p_slot == EEquipSlots.MAIN_HAND)
			{
				p_fightValues.MainHandAttackValue = ResolveValue(p_fightValues.MainHandAttackValue, p_effect, p_skill);
			}
			else if (p_slot == EEquipSlots.OFF_HAND)
			{
				p_fightValues.OffHandAttackValue = ResolveValue(p_fightValues.OffHandAttackValue, p_effect, p_skill);
			}
			else if (p_slot == EEquipSlots.RANGE_WEAPON)
			{
				p_fightValues.RangedAttackValue = ResolveValue(p_fightValues.RangedAttackValue, p_effect, p_skill);
			}
		}

		private void IncreaseBlockChance(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			p_fightValues.GeneralBlockChance = ResolveValue(p_fightValues.GeneralBlockChance, p_effect, p_skill);
		}

		private void IncreaseBlockAttempts(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			p_fightValues.GeneralBlockAttempts += (Int32)p_effect.Value;
		}

		private void IncreaseMeleeBlockAttempts(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			p_fightValues.MeleeBlockAttempts += (Int32)p_effect.Value;
		}

		private void DecreaseAttackValuePenalty(FightValues p_fightValues, SkillEffectStaticData p_effect, Skill p_skill)
		{
			p_fightValues.AttackValuePenaltyReduction = (Int32)ResolveValue(p_fightValues.AttackValuePenaltyReduction, p_effect, p_skill);
		}

		private void DecreaseDamageValuePenalty(FightValues p_fightValues, EEquipSlots p_slot, SkillEffectStaticData p_effect, Skill p_skill)
		{
			if (p_slot == EEquipSlots.OFF_HAND)
			{
				p_fightValues.DualWieldDamageBonus = ResolveValue(p_fightValues.DualWieldDamageBonus, p_effect, p_skill);
			}
		}

		private void IncreaseCriticalHitDestinyMultiplier(FightValues p_fightValues, EEquipSlots p_slot, SkillEffectStaticData p_effect, Skill p_skill)
		{
			Int32 destiny = m_character.CurrentAttributes.Destiny;
			Single num = p_effect.Value * destiny;
			if (p_slot == EEquipSlots.OFF_HAND)
			{
				p_fightValues.OffHandCriticalHitDestinyMultiplier += num;
			}
			else if (p_slot == EEquipSlots.MAIN_HAND)
			{
				p_fightValues.MainHandCriticalHitDestinyMultiplier += num;
			}
			else if (p_slot == EEquipSlots.RANGE_WEAPON)
			{
				p_fightValues.RangedCriticalHitDestinyMultiplier += num;
			}
		}

		public Single GetIncomingCriticalDamageReduction()
		{
			Single num = 0f;
			BaseItem itemAt = m_character.Equipment.GetItemAt(EEquipSlots.BODY);
			List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
			foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
			{
				if (skillEffectStaticData.Type == ESkillEffectType.REDUCE_INCOMING_CRITICAL_DAMAGE)
				{
					num += skillEffectStaticData.Value;
				}
			}
			return num;
		}

		public void OnCombatBegin(Attack p_attack, Boolean p_isMelee, EDamageType p_damageType)
		{
		}

		public void OnCombatEnd(AttackResult p_result, Boolean p_isMelee, EDamageType p_damageType)
		{
			foreach (Skill skill in m_availableSkills)
			{
				foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
				{
					ESkillEffectType type = skillEffectStaticData.Type;
					if (type == ESkillEffectType.RESTORE_MANA_ON_RESIST_MAGIC)
					{
						if (p_result.Result == EResultType.EVADE && p_damageType != EDamageType.PHYSICAL)
						{
							Int32 num = (Int32)ResolveValue(m_character.ManaPoints, skillEffectStaticData, skill);
							m_character.ChangeMP(num - m_character.ManaPoints);
						}
					}
				}
			}
		}

		public void OnCombatReceivedDamage(ref Damage damage, ref Resistance resistance, ref DamageResult damageResult, Boolean p_isCritical, Boolean p_isMelee, EDamageType p_damageType)
		{
			foreach (Skill skill in m_availableSkills)
			{
				foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
				{
					ESkillEffectType type = skillEffectStaticData.Type;
					if (type == ESkillEffectType.REDUCE_INCOMING_CRITICAL_MAGIC_DAMAGE)
					{
						if (p_isCritical)
						{
							damageResult.EffectiveValue = (Int32)Math.Round(damageResult.EffectiveValue * (1f - skillEffectStaticData.Value), MidpointRounding.AwayFromZero);
						}
					}
				}
			}
		}

		public void OnMonsterStrikeMelee(Monster p_monster, AttackResult p_result)
		{
			if (p_result.Result == EResultType.BLOCK)
			{
				if (m_character.ConditionHandler.CantDoAnything())
				{
					return;
				}
				BaseItem itemAt = m_character.Equipment.GetItemAt(EEquipSlots.OFF_HAND);
				List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
				foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
				{
					if (skillEffectStaticData.Condition == ESkillEffectCondition.FIRST_MELEE_STRIKE_BLOCKED && !m_firstMeleeStrikeBlocked && skillEffectStaticData.Type == ESkillEffectType.COUNTER_ATTACK)
					{
						m_firstMeleeStrikeBlocked = true;
						Skill requiredSkill = GetRequiredSkill(itemAt);
						m_character.FightHandler.FeedActionLog(new SkillTierBonusEventArgs(requiredSkill.Tier, m_character, requiredSkill.Name));
						m_character.FightHandler.ExecuteCounterAttack(p_monster);
						break;
					}
				}
			}
			else if (p_result.Result == EResultType.EVADE)
			{
				Skill skill = FindSkill(14);
				if (skill != null)
				{
					List<SkillEffectStaticData> currentlyAvailableEffects = skill.CurrentlyAvailableEffects;
					foreach (SkillEffectStaticData skillEffectStaticData2 in currentlyAvailableEffects)
					{
						if (skillEffectStaticData2.Condition == ESkillEffectCondition.EVADE_MELEE_ATTACK && !m_firstMeleeStrikeEvaded && skillEffectStaticData2.Type == ESkillEffectType.COUNTER_ATTACK)
						{
							m_firstMeleeStrikeEvaded = true;
							m_character.FightHandler.FeedActionLog(new SkillTierBonusEventArgs(skill.Tier, m_character, skill.Name));
							m_character.FightHandler.ExecuteCounterAttack(p_monster);
							break;
						}
					}
				}
			}
		}

		public Boolean CanSubstituteBlock()
		{
			BaseItem itemAt = m_character.Equipment.GetItemAt(EEquipSlots.OFF_HAND);
			List<SkillEffectStaticData> requiredSkillEffects = GetRequiredSkillEffects(itemAt);
			foreach (SkillEffectStaticData skillEffectStaticData in requiredSkillEffects)
			{
				if (skillEffectStaticData.Type == ESkillEffectType.BLOCK_PARTY_CHARACTER_ATTACK)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean IgnoreMonsterBlocks(Attack p_attack, Boolean p_melee)
		{
			if (p_melee)
			{
				MeleeWeapon meleeWeapon = (MeleeWeapon)m_character.Equipment.GetItemAt(p_attack.AttackHand);
				if (meleeWeapon != null && meleeWeapon.GetSubType() == EEquipmentType.TWOHANDED)
				{
					Skill skill = FindSkill(8);
					if (skill != null)
					{
						foreach (SkillEffectStaticData skillEffectStaticData in skill.CurrentlyAvailableEffects)
						{
							if (skillEffectStaticData.Type == ESkillEffectType.IGNORE_BLOCKS)
							{
								return true;
							}
						}
						return false;
					}
				}
			}
			return false;
		}

		public void Load(SaveGameData p_data)
		{
			foreach (Skill skill in m_availableSkills)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("Skill" + skill.StaticID, null);
				if (saveGameData != null)
				{
					skill.Load(saveGameData);
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			foreach (Skill skill in m_availableSkills)
			{
				SaveGameData saveGameData = new SaveGameData("Skill" + skill.StaticID);
				skill.Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
		}

		private Single ResolveValue(Single p_baseValue, SkillEffectStaticData p_effect, Skill p_skill)
		{
			switch (p_effect.Mode)
			{
			case ESkillEffectMode.FIXED:
				return p_baseValue + p_effect.Value;
			case ESkillEffectMode.PER_SKILL_LEVEL:
				return p_baseValue + p_effect.Value * (p_skill.Level + p_skill.VirtualSkillLevel);
			case ESkillEffectMode.PERCENT:
				return p_baseValue * (1f + p_effect.Value);
			case ESkillEffectMode.PER_DESTINY:
				return p_baseValue + p_effect.Value * m_character.CurrentAttributes.Destiny;
			default:
				return p_baseValue;
			}
		}

		public static Int32 GetRequiredSkillID(BaseItem p_equip)
		{
			ISkillDependant skillDependant = p_equip as ISkillDependant;
			if (skillDependant != null)
			{
				return skillDependant.GetRequiredSkillID();
			}
			return 0;
		}

		public void Respec()
		{
			foreach (Skill skill in m_availableSkills)
			{
				if (m_character.Class.IsStartSkill(skill.StaticID))
				{
					while (skill.Level > 1)
					{
						DecreaseSkillLevelSilent(skill.StaticID);
					}
				}
				else
				{
					while (skill.Level > 0)
					{
						DecreaseSkillLevelSilent(skill.StaticID);
					}
				}
			}
		}

		public void DecreaseSkillLevelSilent(Int32 p_skillStaticID)
		{
			Skill skill = FindSkill(p_skillStaticID);
			if (skill != null && skill.Level > 0)
			{
				ETier tier = skill.Tier;
				skill.Level--;
				if (skill.Tier != tier)
				{
					RevertTierIncreaseEffects(skill, tier);
				}
			}
		}
	}
}
