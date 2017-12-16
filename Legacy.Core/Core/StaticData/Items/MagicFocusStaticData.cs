using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData.Items
{
	public class MagicFocusStaticData : EquipmentStaticData
	{
		[CsvColumn("MinDamage")]
		protected Int32 m_minDamage;

		[CsvColumn("MaxDamage")]
		protected Int32 m_maxDamage;

		[CsvColumn("AddCritDamage")]
		protected Single m_addCritDamage;

		[CsvColumn("RequiredSkillID")]
		protected Int32 m_requiredSkillID;

		[CsvColumn("RequiredSkillTier")]
		protected ETier m_requiredSkillTier;

		public MagicFocusStaticData()
		{
			m_addCritDamage = 0f;
			m_type = EEquipmentType.MAGIC_FOCUS_ONEHANDED;
			m_requiredSkillID = 0;
			m_requiredSkillTier = ETier.NONE;
		}

		public Int32 MinDamage => m_minDamage;

	    public Int32 MaxDamage => m_maxDamage;

	    public Single AddCritDamage => m_addCritDamage;

	    public Int32 RequiredSkillID => m_requiredSkillID;

	    public ETier RequiredSkillTier => m_requiredSkillTier;
	}
}
