using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData.Items
{
	public class MeleeWeaponStaticData : EquipmentStaticData
	{
		[CsvColumn("MinDamage")]
		protected Int32 m_minDamage;

		[CsvColumn("MaxDamage")]
		protected Int32 m_maxDamage;

		[CsvColumn("CritDamage")]
		protected Single m_CritDamageFactor;

		[CsvColumn("Subtype")]
		protected EEquipmentType m_subtype;

		[CsvColumn("RequiredSkillID")]
		protected Int32 m_requiredSkillID;

		[CsvColumn("RequiredSkillTier")]
		protected ETier m_requiredSkillTier;

		[CsvColumn("BreakChance")]
		protected Single m_breakChance;

		public MeleeWeaponStaticData()
		{
			m_minDamage = 0;
			m_maxDamage = 0;
			m_CritDamageFactor = 0f;
			m_type = EEquipmentType.DAGGER;
			m_requiredSkillID = 0;
			m_breakChance = 0f;
			m_requiredSkillTier = ETier.NONE;
		}

		public Int32 MinDamage => m_minDamage;

	    public Int32 MaxDamage => m_maxDamage;

	    public Single CritDamageFactor => m_CritDamageFactor;

	    public EEquipmentType Subtype => m_subtype;

	    public Int32 RequiredSkillID => m_requiredSkillID;

	    public Single BreakChance => m_breakChance;

	    public ETier RequiredSkillTier => m_requiredSkillTier;
	}
}
