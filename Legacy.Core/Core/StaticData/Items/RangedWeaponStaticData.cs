using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData.Items
{
	public class RangedWeaponStaticData : EquipmentStaticData
	{
		[CsvColumn("MinDamage")]
		protected Int32 m_minDamage;

		[CsvColumn("MaxDamage")]
		protected Int32 m_maxDamage;

		[CsvColumn("CritDamage")]
		protected Single m_critDamageFactor;

		[CsvColumn("RequiredSkillID")]
		protected Int32 m_requiredSkillID;

		[CsvColumn("RequiredSkillTier")]
		protected ETier m_requiredSkillTier;

		public RangedWeaponStaticData()
		{
			m_minDamage = 0;
			m_maxDamage = 0;
			m_critDamageFactor = 0f;
			m_type = EEquipmentType.BOW;
			m_requiredSkillID = 0;
			m_requiredSkillTier = ETier.NONE;
		}

		public Int32 MinDamage => m_minDamage;

	    public Int32 MaxDamage => m_maxDamage;

	    public Single CritDamageFactor => m_critDamageFactor;

	    public Int32 RequiredSkillID => m_requiredSkillID;

	    public ETier RequiredSkillTier => m_requiredSkillTier;
	}
}
