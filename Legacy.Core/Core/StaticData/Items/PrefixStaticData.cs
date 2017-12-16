using System;
using Dumper.Core;
using Legacy.Core.Combat;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData.Items
{
	public class PrefixStaticData : BaseStaticData
	{
		[CsvColumn("Name")]
		private String m_name;

		[CsvColumn("Description")]
		private String m_description;

		[CsvColumn("Effect")]
		private EPrefixEffect m_effect;

		[CsvColumn("School")]
		private EDamageType m_school;

		[CsvColumn("ValuePerModel")]
		private Single[] m_valuePerModel;

		[CsvColumn("PricePerModel")]
		private Int32[] m_pricePerModel;

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

		public PrefixStaticData()
		{
			m_name = String.Empty;
			m_description = String.Empty;
			m_effect = EPrefixEffect.NONE;
			m_school = EDamageType.NONE;
			m_probabilityLightArmor = 0f;
			m_probabilityHeavyArmor = 0f;
			m_probabilityCloth = 0f;
			m_probabilityMartial = 0f;
			m_probabilityArcane = 0f;
			m_probabilityJewelry = 0f;
			m_probabilityShields = 0f;
			m_probabilityMelee = 0f;
			m_probabilityMagicFocus = 0f;
			m_probabilityRanged = 0f;
		}

		public String Name => m_name;

	    public String Description => m_description;

	    public EPrefixEffect Effect => m_effect;

	    public EDamageType School => m_school;

	    public Single ProbabilityCloth => m_probabilityCloth;

	    public Single ProbabilityLightArmor => m_probabilityLightArmor;

	    public Single ProbabilityHeavyArmor => m_probabilityHeavyArmor;

	    public Single ProbabilityArcane => m_probabilityArcane;

	    public Single ProbabilityMartial => m_probabilityMartial;

	    public Single ProbabilityJewelry => m_probabilityJewelry;

	    public Single ProbabilityShields => m_probabilityShields;

	    public Single ProbabilityMelee => m_probabilityMelee;

	    public Single ProbabilityMagicFocus => m_probabilityMagicFocus;

	    public Single ProbabilityRanged => m_probabilityRanged;

	    public Single GetValueForLevel(Int32 p_level)
		{
			if (p_level > 0 && p_level <= m_valuePerModel.Length)
			{
				return m_valuePerModel[p_level - 1];
			}
			return 0f;
		}

		public Int32 GetPriceForLevel(Int32 p_level)
		{
			if (p_level > 0 && p_level <= m_pricePerModel.Length)
			{
				return m_pricePerModel[p_level - 1];
			}
			return 0;
		}

		public Boolean IsIncreasingStats()
		{
			return Effect == EPrefixEffect.INCREASE_ELEMENTAL_PROTECTION;
		}
	}
}
