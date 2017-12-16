using System;
using Dumper.Core;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData.Items
{
	public class SuffixStaticData : BaseStaticData
	{
		[CsvColumn("Name")]
		private String m_name;

		[CsvColumn("Description")]
		private String m_description;

		[CsvColumn("CountableDescription")]
		private Boolean m_countableDescription;

		[CsvColumn("Chance")]
		private Single[] m_chance;

		[CsvColumn("ChanceTwoHanded")]
		private Single[] m_chanceTwoHanded;

		[CsvColumn("ConditionDayTime")]
		private EDayTimeCondition m_conditionDayTime;

		[CsvColumn("ConditionTargetedBySource")]
		private EAttackTypeCondition m_conditionTargetedBySource;

		[CsvColumn("ConditionHitBySource")]
		private EAttackTypeCondition m_conditionHitBySource;

		[CsvColumn("ConditionHitByMagic")]
		private EMagicAttackTypeCondition m_conditionHitByMagic;

		[CsvColumn("ConditionHitByType")]
		private EHitTypeCondition m_conditionHitByType;

		[CsvColumn("ConditionCastSpell")]
		private EMagicAttackTypeCondition m_conditionCastSpell;

		[CsvColumn("ConditionAttacksMonster")]
		private EMonsterClassTypeCondition m_conditionAttacksMonster;

		[CsvColumn("ConditionHits")]
		private EHitTypeCondition m_conditionHits;

		[CsvColumn("Effect")]
		private ESuffixEffect m_effect;

		[CsvColumn("MagicSchool")]
		private EDamageType m_magicSchool;

		[CsvColumn("ValuePerModel")]
		private Single[] m_valuePerModel;

		[CsvColumn("ValuePerModelTwoHanded")]
		private Single[] m_valuePerModelTwoHanded;

		[CsvColumn("ValueMode")]
		private EValueMode m_valueMode;

		[CsvColumn("PricePerModel")]
		private Int32[] m_pricePerLevel;

		[CsvColumn("PricePerModelTwoHanded")]
		private Int32[] m_pricePerLevelTwoHanded;

		[CsvColumn("ProbabilityCloth")]
		private Single m_probabilityCloth;

		[CsvColumn("ProbabilityLightArmor")]
		private Single m_probabilityLightArmor;

		[CsvColumn("ProbabilityHeavyArmor")]
		private Single m_probabilityHeavyArmor;

		[CsvColumn("ProbabilityArcane")]
		private Single m_probabilityArcane;

		[CsvColumn("ProbabilityMartial")]
		private Single m_probabilityMartial;

		[CsvColumn("ProbabilityJewelry")]
		private Single m_probabilityJewelry;

		[CsvColumn("ProbabilityMagicFocus")]
		private Single m_probabilityMagicFocus;

		[CsvColumn("ProbabilityShields")]
		private Single m_probabilityShields;

		[CsvColumn("ProbabilityMelee")]
		private Single m_probabilityMelee;

		[CsvColumn("ProbabilityRanged")]
		private Single m_probabilityRanged;

		private String[,] m_buffValues;

		public SuffixStaticData()
		{
			m_name = String.Empty;
			m_description = String.Empty;
			m_conditionDayTime = EDayTimeCondition.NONE;
			m_conditionTargetedBySource = EAttackTypeCondition.NONE;
			m_conditionHitBySource = EAttackTypeCondition.NONE;
			m_conditionHitByMagic = EMagicAttackTypeCondition.NONE;
			m_conditionHitByType = EHitTypeCondition.NONE;
			m_conditionCastSpell = EMagicAttackTypeCondition.NONE;
			m_conditionAttacksMonster = EMonsterClassTypeCondition.NONE;
			m_conditionHits = EHitTypeCondition.NONE;
			m_effect = ESuffixEffect.NONE;
			m_probabilityCloth = 0f;
			m_probabilityLightArmor = 0f;
			m_probabilityHeavyArmor = 0f;
			m_probabilityArcane = 0f;
			m_probabilityMartial = 0f;
			m_probabilityJewelry = 0f;
			m_probabilityMagicFocus = 0f;
			m_probabilityShields = 0f;
			m_probabilityMelee = 0f;
			m_probabilityRanged = 0f;
			m_valueMode = EValueMode.NONE;
		}

		public String Name => m_name;

	    public String Description => m_description;

	    public Boolean CountableDescription => m_countableDescription;

	    public EDayTimeCondition ConditionDayTime => m_conditionDayTime;

	    public EAttackTypeCondition ConditionTargetedBySource => m_conditionTargetedBySource;

	    public EAttackTypeCondition ConditionHitBySource => m_conditionHitBySource;

	    public EMagicAttackTypeCondition ConditionHitByMagic => m_conditionHitByMagic;

	    public EHitTypeCondition ConditionHitByType => m_conditionHitByType;

	    public EMagicAttackTypeCondition ConditionCastSpell => m_conditionCastSpell;

	    public EMonsterClassTypeCondition ConditionAttacksMonster => m_conditionAttacksMonster;

	    public EHitTypeCondition ConditionHits => m_conditionHits;

	    public ESuffixEffect Effect => m_effect;

	    public EDamageType MagicSchool => m_magicSchool;

	    public Single ProbabilityCloth => m_probabilityCloth;

	    public Single ProbabilityLightArmor => m_probabilityLightArmor;

	    public Single ProbabilityHeavyArmor => m_probabilityHeavyArmor;

	    public Single ProbabilityArcane => m_probabilityArcane;

	    public Single ProbabilityMartial => m_probabilityMartial;

	    public Single ProbabilityJewelry => m_probabilityJewelry;

	    public Single ProbabilityMagicFocus => m_probabilityMagicFocus;

	    public Single ProbabilityShields => m_probabilityShields;

	    public Single ProbabilityMelee => m_probabilityMelee;

	    public Single ProbabilityRanged => m_probabilityRanged;

	    public EValueMode ValueMode => m_valueMode;

	    public Boolean IsIncreasingStats()
		{
			Boolean flag = Effect == ESuffixEffect.INCREASE_MIGHT;
			Boolean flag2 = Effect == ESuffixEffect.INCREASE_MAGIC;
			Boolean flag3 = Effect == ESuffixEffect.INCREASE_PERCEPTION;
			Boolean flag4 = Effect == ESuffixEffect.INCREASE_DESTINY;
			Boolean flag5 = Effect == ESuffixEffect.INCREASE_HP;
			Boolean flag6 = Effect == ESuffixEffect.INCREASE_ARMOR_VALUE;
			return flag || flag2 || flag3 || flag4 || flag5 || flag6;
		}

		public Single GetValueForLevel(Int32 p_level, Boolean p_isTwoHanded)
		{
			if (!p_isTwoHanded)
			{
				if (p_level > 0 && p_level <= m_valuePerModel.Length)
				{
					return m_valuePerModel[p_level - 1];
				}
			}
			else if (m_valuePerModelTwoHanded != null && p_level > 0 && p_level <= m_valuePerModelTwoHanded.Length)
			{
				return m_valuePerModelTwoHanded[p_level - 1];
			}
			return 0f;
		}

		public Int32 GetPriceForLevel(Int32 p_level, Boolean p_isTwoHanded)
		{
			if (!p_isTwoHanded)
			{
				if (p_level > 0 && p_level <= m_pricePerLevel.Length)
				{
					return m_pricePerLevel[p_level - 1];
				}
			}
			else if (m_pricePerLevelTwoHanded != null && p_level > 0 && p_level <= m_pricePerLevelTwoHanded.Length)
			{
				return m_pricePerLevelTwoHanded[p_level - 1];
			}
			return 0;
		}

		public Single GetChanceForLevel(Int32 p_level, Boolean isTwoHanded)
		{
			if (!isTwoHanded)
			{
				if (p_level > 0 && p_level <= m_chance.Length)
				{
					return m_chance[p_level - 1];
				}
			}
			else if (m_chanceTwoHanded != null && p_level > 0 && p_level <= m_chanceTwoHanded.Length)
			{
				return m_chanceTwoHanded[p_level - 1];
			}
			return 0f;
		}

		public String GetBuffValueForLevel(Int32 p_Level, Int32 p_buffValueIndex)
		{
			if (m_buffValues == null)
			{
				InitBuffValues();
			}
			return m_buffValues[p_Level - 1, p_buffValueIndex];
		}

		private void InitBuffValues()
		{
			m_buffValues = new String[10, 3];
			switch (Effect)
			{
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_CHILLED:
				InitBuffValues(EMonsterBuffType.CHILLED);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_IMMOBILISED:
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_IMMOBILISED_GROUP:
				InitBuffValues(EMonsterBuffType.IMMOBILISED);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_MANASURGE:
				InitBuffValues(EMonsterBuffType.MANASURGE);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_MEMORYGAP:
				InitBuffValues(EMonsterBuffType.MEMORYGAP);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_POISON:
				InitBuffValues(EMonsterBuffType.POISONED);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_SLEEP:
				InitBuffValues(EMonsterBuffType.SLEEPING);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_IMPRISSONED:
				InitBuffValues(EMonsterBuffType.IMPRISONED);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_ACIDSPLASH:
				InitBuffValues(EMonsterBuffType.ACIDSPLASH);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_WEAKNESS:
				InitBuffValues(EMonsterBuffType.WEAKNESS);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_AGONY:
				InitBuffValues(EMonsterBuffType.AGONY);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_TERROR:
				InitBuffValues(EMonsterBuffType.TERROR);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_SUNDERING:
				InitBuffValues(EMonsterBuffType.SUNDERING);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_STUNNED:
				InitBuffValues(EMonsterBuffType.MACE_STUN);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_GASH:
				InitBuffValues(EMonsterBuffType.GASH);
				break;
			case ESuffixEffect.AFFLICT_MONSTER_DEBUFF_PROVOKE:
				InitBuffValues(EMonsterBuffType.PROVOKE);
				break;
			}
		}

		private void InitBuffValues(EMonsterBuffType p_buffType)
		{
			for (Int32 i = 0; i < 10; i++)
			{
				MonsterBuff monsterBuff = BuffFactory.CreateMonsterBuff(p_buffType, GetValueForLevel(i + 1, false));
				for (Int32 j = 0; j < 3; j++)
				{
					m_buffValues[i, j] = monsterBuff.GetBuffValueForTooltip(j);
				}
			}
		}

		public Boolean HasCondition()
		{
			Boolean flag = m_conditionDayTime != EDayTimeCondition.NONE;
			Boolean flag2 = m_conditionTargetedBySource != EAttackTypeCondition.NONE;
			Boolean flag3 = m_conditionHitBySource != EAttackTypeCondition.NONE;
			Boolean flag4 = m_conditionHitByMagic != EMagicAttackTypeCondition.NONE;
			Boolean flag5 = m_conditionHitByType != EHitTypeCondition.NONE;
			Boolean flag6 = m_conditionCastSpell != EMagicAttackTypeCondition.NONE;
			Boolean flag7 = m_conditionAttacksMonster != EMonsterClassTypeCondition.NONE;
			Boolean flag8 = m_conditionHits != EHitTypeCondition.NONE;
			return flag || flag2 || flag3 || flag4 || flag5 || flag6 || flag7 || flag8;
		}
	}
}
