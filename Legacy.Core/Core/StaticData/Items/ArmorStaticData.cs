using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData.Items
{
	public class ArmorStaticData : EquipmentStaticData
	{
		[CsvColumn("AC")]
		protected Int32 m_AC;

		[CsvColumn("Subtype")]
		protected EEquipmentType m_subtype;

		[CsvColumn("BreakChance")]
		protected Single m_breakChance;

		[CsvColumn("RequiredSkillID")]
		protected Int32 m_requiredSkillID;

		[CsvColumn("RequiredSkillTier")]
		protected ETier m_requiredSkillTier;

		public ArmorStaticData()
		{
			m_AC = 0;
			m_subtype = EEquipmentType.CLOTHING;
			m_breakChance = 0f;
			m_requiredSkillID = 0;
			m_requiredSkillTier = ETier.NONE;
		}

		public Int32 AC => m_AC;

	    public EEquipmentType Subtype => m_subtype;

	    public Single BreakChance => m_breakChance;

	    public Int32 RequiredSkillID => m_requiredSkillID;

	    public ETier RequiredSkillTier => m_requiredSkillTier;
	}
}
