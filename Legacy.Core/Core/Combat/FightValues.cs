using System;
using System.Collections.Generic;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.Combat
{
	public class FightValues
	{
		public FightValues()
		{
			MainHandDamage = new DamageCollection();
			OffHandDamage = new DamageCollection();
			RangeDamage = new DamageCollection();
			MagicPowers = new Dictionary<ESkillID, Single>
			{
				{
					ESkillID.SKILL_AIR_MAGIC,
					0f
				},
				{
					ESkillID.SKILL_FIRE_MAGIC,
					0f
				},
				{
					ESkillID.SKILL_WATER_MAGIC,
					0f
				},
				{
					ESkillID.SKILL_EARTH_MAGIC,
					0f
				},
				{
					ESkillID.SKILL_LIGHT_MAGIC,
					0f
				},
				{
					ESkillID.SKILL_DARK_MAGIC,
					0f
				},
				{
					ESkillID.SKILL_PRIMORDIAL_MAGIC,
					0f
				}
			};
			MagicPowerKeys = new List<ESkillID>(MagicPowers.Keys);
			MagicFactorSkillBonus = new Dictionary<ESkillID, Single>();
			foreach (ESkillID key in MagicPowerKeys)
			{
				MagicFactorSkillBonus.Add(key, 0f);
			}
			Resistance = new ResistanceCollection();
		}

		public DamageCollection MainHandDamage { get; private set; }

		public Single MainHandCriticalDamageMod { get; set; }

		public Single MainHandAttackValue { get; set; }

		public DamageCollection OffHandDamage { get; private set; }

		public Single OffHandCriticalDamageMod { get; set; }

		public Single OffHandAttackValue { get; set; }

		public Single DualWieldDamageBonus { get; set; }

		public DamageCollection RangeDamage { get; private set; }

		public Single RangeCriticalDamageMod { get; set; }

		public Single RangedAttackValue { get; set; }

		public Dictionary<ESkillID, Single> MagicPowers { get; private set; }

		public Single MagicalCriticalDamageMod { get; set; }

		public ResistanceCollection Resistance { get; private set; }

		public Int32 ArmorValue
		{
			get => Resistance[EDamageType.PHYSICAL].Value;
		    set => Resistance.Set(new Resistance(EDamageType.PHYSICAL, value));
		}

		public Single CriticalMainHandHitChance { get; set; }

		public Single CriticalOffHandHitChance { get; set; }

		public Single CriticalRangeHitChance { get; set; }

		public Single CriticalMagicHitChance { get; set; }

		public Single EvadeValue { get; set; }

		public Single EvadeValueRangedBonus { get; set; }

		public Int32 MeleeBlockAttempts { get; set; }

		public Int32 GeneralBlockAttempts { get; set; }

		public Single GeneralBlockChance { get; set; }

		public Int32 RangedAttackRange { get; set; }

		public Int32 AttackValuePenaltyReduction { get; set; }

		public Single ConditionProtectionParalysis { get; set; }

		public Single ConditionProtectionKnockOut { get; set; }

		public Single ConditionProtectionSleep { get; set; }

		public Single ConditionProtectionPoison { get; set; }

		public Single ConditionProtectionConfusion { get; set; }

		public Single ConditionProtectionWeakness { get; set; }

		public Single ConditionProtectionCurses { get; set; }

		public Single MainHandDamageFactor { get; set; }

		public Single OffHandDamageFactor { get; set; }

		public Single MainHandCriticalHitDestinyMultiplier { get; set; }

		public Single OffHandCriticalHitDestinyMultiplier { get; set; }

		public Single RangedCriticalHitDestinyMultiplier { get; set; }

		public Single MainHandSkillLevelBonus { get; set; }

		public Single OffHandSkillLevelBonus { get; set; }

		public Single RangedSkillLevelBonus { get; set; }

		public Dictionary<ESkillID, Single> MagicFactorSkillBonus { get; private set; }

		public List<ESkillID> MagicPowerKeys { get; private set; }

		public void ResetValues(Int32 p_baseEvade)
		{
			MainHandDamage.Clear();
			MainHandCriticalDamageMod = 0f;
			MainHandAttackValue = 0f;
			MainHandSkillLevelBonus = 0f;
			MainHandDamageFactor = 1f;
			CriticalMainHandHitChance = 0f;
			OffHandDamage.Clear();
			OffHandCriticalDamageMod = 0f;
			OffHandAttackValue = 0f;
			OffHandSkillLevelBonus = 0f;
			OffHandDamageFactor = 1f;
			CriticalOffHandHitChance = 0f;
			DualWieldDamageBonus = 0f;
			MainHandCriticalHitDestinyMultiplier = 0f;
			OffHandCriticalHitDestinyMultiplier = 0f;
			RangedCriticalHitDestinyMultiplier = 0f;
			RangeDamage.Clear();
			RangeCriticalDamageMod = 0f;
			RangedAttackRange = 0;
			RangedSkillLevelBonus = 0f;
			RangedAttackValue = 0f;
			CriticalRangeHitChance = 0f;
			foreach (ESkillID key in MagicPowerKeys)
			{
				MagicPowers[key] = 0f;
				MagicFactorSkillBonus[key] = 0f;
			}
			MagicalCriticalDamageMod = 0f;
			CriticalMagicHitChance = 0f;
			Resistance.Clear();
			ArmorValue = 0;
			EvadeValue = p_baseEvade;
			EvadeValueRangedBonus = 0f;
			MeleeBlockAttempts = 0;
			GeneralBlockAttempts = 0;
			GeneralBlockChance = 0f;
			AttackValuePenaltyReduction = 0;
			ConditionProtectionParalysis = 0f;
			ConditionProtectionKnockOut = 0f;
			ConditionProtectionSleep = 0f;
			ConditionProtectionPoison = 0f;
			ConditionProtectionConfusion = 0f;
			ConditionProtectionWeakness = 0f;
			ConditionProtectionCurses = 0f;
		}

		public List<Damage> GetMeleeDamage(Boolean p_offHand, Single p_armorIgnoreValue)
		{
			DamageCollection damageCollection = (!p_offHand) ? MainHandDamage : OffHandDamage;
			Single p_criticalBonusMod = (!p_offHand) ? MainHandCriticalDamageMod : OffHandCriticalDamageMod;
			List<Damage> list = new List<Damage>();
			for (EDamageType edamageType = EDamageType.PHYSICAL; edamageType < EDamageType._MAX_; edamageType++)
			{
				DamageData p_data = damageCollection[edamageType];
				if (p_data.Maximum > 0)
				{
					Damage item = Damage.Create(p_data, p_criticalBonusMod);
					if (edamageType == EDamageType.PHYSICAL)
					{
						item.IgnoreResistancePercent = p_armorIgnoreValue;
					}
					list.Add(item);
				}
			}
			return list;
		}

		public List<Damage> GetRangedDamage(Boolean p_isMelee)
		{
			DamageCollection rangeDamage = RangeDamage;
			Single rangeCriticalDamageMod = RangeCriticalDamageMod;
			List<Damage> list = new List<Damage>();
			for (EDamageType edamageType = EDamageType.PHYSICAL; edamageType < EDamageType._MAX_; edamageType++)
			{
				DamageData p_data = rangeDamage[edamageType];
				if (p_isMelee)
				{
					Int32 p_minimum = (Int32)(p_data.Minimum * ConfigManager.Instance.Game.RangedAttackMeleeMalus + 0.5f);
					Int32 p_maximum = (Int32)(p_data.Maximum * ConfigManager.Instance.Game.RangedAttackMeleeMalus + 0.5f);
					p_data = new DamageData(p_data.Type, p_minimum, p_maximum);
				}
				if (p_data.Maximum > 0)
				{
					list.Add(Damage.Create(p_data, rangeCriticalDamageMod));
				}
			}
			return list;
		}
	}
}
