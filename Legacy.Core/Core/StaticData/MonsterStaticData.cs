using System;
using Dumper.Core;
using Legacy.Core.Abilities;
using Legacy.Core.Combat;
using Legacy.Core.Entities;
using Legacy.Core.Entities.AI.MonsterBehaviours;
using Legacy.Core.Entities.Items;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.StaticData
{
	public class MonsterStaticData : BaseStaticData
	{
		[CsvColumn("Prefab")]
		public String Prefab = String.Empty;

		[CsvColumn("PrefabAlt")]
		public String PrefabAlt = String.Empty;

		[CsvColumn("MovePrio")]
		public Int32 MovePrio;

		[CsvColumn("AttackRange")]
		public Int32 AttackRange;

		[CsvColumn("Size")]
		public ESize Size;

		[CsvColumn("AggroRange")]
		public Single AggroRange;

		[CsvColumn("AggroRangeDungeon")]
		public Single AggroRangeDungeon;

		[CsvColumn("SightAggroRange")]
		public Single SightAggroRange;

		[CsvColumn("SightAggroRangeDungeon")]
		public Single SightAggroRangeDungeon;

		[CsvColumn("AlwaysTriggerAggro")]
		public Boolean AlwaysTriggerAggro = true;

		[CsvColumn("NameKey")]
		public String NameKey = String.Empty;

		[CsvColumn("Gender")]
		public EGender Gender;

		[CsvColumn("Grade")]
		public EMonsterGrade Grade;

		[CsvColumn("Class")]
		public EMonsterClass Class;

		[CsvColumn("Type")]
		public EMonsterType Type;

		[CsvColumn("AccessibleTerrains")]
		public ETerrainType AccessibleTerrains;

		[CsvColumn("MeleeAttackStrikesAmount")]
		public Int32 MeleeAttackStrikesAmount;

		[CsvColumn("MeleeAttackValue")]
		public Single MeleeAttackValue;

		[CsvColumn("MeleeAttackDamageMin")]
		private Int32 m_meleeAttackDamageMin;

		[CsvColumn("MeleeAttackDamageMax")]
		private Int32 m_meleeAttackDamageMax;

		[CsvColumn("MeleeAttackElementalDamage")]
		public ExtraDamage m_meleeAttackElementalDamage;

		[CsvColumn("RangedAttackStrikesAmount")]
		public Int32 RangedAttackStrikesAmount;

		[CsvColumn("RangedAttackValue")]
		public Single RangedAttackValue;

		[CsvColumn("RangedAttackDamageMin")]
		private Int32 m_rangedAttackDamageMin;

		[CsvColumn("RangedAttackDamageMax")]
		private Int32 m_rangedAttackDamageMax;

		[CsvColumn("RangedAttackElementalDamage")]
		public ExtraDamage m_rangedAttackElementalDamage;

		[CsvColumn("CastSpellChance")]
		public Single CastSpellChance;

		[CsvColumn("Spells")]
		public SpellData[] Spells;

		[CsvColumn("CritialHitChanceMelee")]
		public Single CriticalHitChanceMelee;

		[CsvColumn("CriticalDamageMelee")]
		public Single CriticalDamageMelee;

		[CsvColumn("CriticalHitChanceRanged")]
		public Single CriticalHitChanceRanged;

		[CsvColumn("CriticalDamageRanged")]
		public Single CriticalDamageRanged;

		[CsvColumn("CriticalHitChanceSpells")]
		public Single CriticalHitChanceSpells;

		[CsvColumn("CriticalDamageSpells")]
		public Single CriticalDamageSpells;

		[CsvColumn("MaxHealthpoints")]
		public Int32 MaxHealthpoints;

		[CsvColumn("EvadeValue")]
		public Int32 EvadeValue;

		[CsvColumn("ArmorValue")]
		public Int32 ArmorValue;

		[CsvColumn("MeleeBlockAttemptsPerTurn")]
		public Int32 MeleeBlockAttemptsPerTurn;

		[CsvColumn("GeneralBlockAttemptsPerTurn")]
		public Int32 GeneralBlockAttemptsPerTurn;

		[CsvColumn("MagicResistances")]
		public Resistance[] MagicResistances;

		[CsvColumn("GeneralBlockChance")]
		public Single GeneralBlockChance;

		[CsvColumn("XpReward")]
		public Int32 XpReward;

		[CsvColumn("Abilities")]
		public MonsterAbilityID[] Abilities;

		[CsvColumn("MagicPower")]
		public Single MagicPower;

		[CsvColumn("SpellRanges")]
		public Int32 SpellRanges;

		[CsvColumn("SteadyLoot")]
		public SteadyLoot[] SteadyLoot;

		[CsvColumn("ItemModels")]
		public ModelProbability[] DropModelLevels;

		[CsvColumn("ItemDropChance")]
		public Single DropItemChance;

		[CsvColumn("PrefixChance")]
		public Single DropItemPrefixChance;

		[CsvColumn("PrefixProbabilities")]
		public EnchantmentProbability[] PrefixProbabilities;

		[CsvColumn("SuffixChance")]
		public Single DropItemSuffixChance;

		[CsvColumn("SuffixProbabilities")]
		public EnchantmentProbability[] SuffixProbabilities;

		[CsvColumn("ItemSpecificationList")]
		public EEquipmentType[] DropItemSpecificationList;

		[CsvColumn("GoldChance")]
		public Single DropGoldChance;

		[CsvColumn("GoldAmount")]
		public IntRange DropGoldAmount;

		[CsvColumn("TokenID")]
		public Int32[] DropTokenID;

		[CsvColumn("IsFightMusicForced")]
		public Boolean IsFightMusicForced;

		[CsvColumn("AIBehaviour")]
		public EMonsterAIBehaviour MonsterBehaviour;

		[CsvColumn("MenuPath")]
		public String MenuPath = String.Empty;

		[CsvColumn("BestiaryEntry")]
		public Boolean BestiaryEntry = true;

		[CsvColumn("ShowActionLog")]
		public Boolean ShowActionLog = true;

		[CsvColumn("BestiaryThresholds")]
		public Int32[] BestiaryThresholds;

		[CsvColumn("CanDie")]
		public Boolean CanDie = true;

		public Damage MeleeAttackDamage => new Damage(EDamageType.PHYSICAL, Random.Range(m_meleeAttackDamageMin, m_meleeAttackDamageMax), CriticalDamageMelee, 1f);

	    public Damage MeleeAttackElementalDamage => new Damage(m_meleeAttackElementalDamage.m_type, m_meleeAttackElementalDamage.m_damage, CriticalDamageMelee, 1f);

	    public Damage RangedAttackDamage => new Damage(EDamageType.PHYSICAL, Random.Range(m_rangedAttackDamageMin, m_rangedAttackDamageMax), CriticalDamageRanged, 1f);

	    public Damage RangedAttackElementalDamage => new Damage(m_rangedAttackElementalDamage.m_type, m_rangedAttackElementalDamage.m_damage, CriticalDamageRanged, 1f);

	    public Int32 MeleeAttackDamageMin => m_meleeAttackDamageMin;

	    public Int32 MeleeAttackDamageMax => m_meleeAttackDamageMax;

	    public Int32 RangedAttackDamageMin => m_rangedAttackDamageMin;

	    public Int32 RangedAttackDamageMax => m_rangedAttackDamageMax;

	    public struct SpellData
		{
			public readonly String AnimationClipName;

			public readonly Int32 SpellID;

			public readonly Int32 SpellProbability;

			public readonly Int32 Level;

			public SpellData(String p_animationClipName, Int32 p_spellID, Int32 p_spellProbability, Int32 p_level)
			{
				AnimationClipName = p_animationClipName;
				SpellID = p_spellID;
				SpellProbability = p_spellProbability;
				Level = p_level;
			}
		}

		public struct ExtraDamage
		{
			public readonly EDamageType m_type;

			public readonly Int32 m_damage;

			public ExtraDamage(EDamageType p_type, Int32 p_damage)
			{
				m_type = p_type;
				m_damage = p_damage;
			}

			public override String ToString()
			{
				return String.Format("[ExtraDamage: Type: {0} damage: {1}]", m_type, m_damage);
			}
		}
	}
}
