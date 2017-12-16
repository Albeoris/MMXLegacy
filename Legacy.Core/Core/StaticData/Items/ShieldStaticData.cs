using System;
using Dumper.Core;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData.Items
{
	public class ShieldStaticData : EquipmentStaticData
	{
		[CsvColumn("AC")]
		protected Int32 m_AC;

		[CsvColumn("BreakChance")]
		protected Single m_breakChance;

		[CsvColumn("RequiredSkillID")]
		protected Int32 m_requiredSkillID;

		[CsvColumn("RequiredSkillTier")]
		protected ETier m_requiredSkillTier;

		public ShieldStaticData()
		{
			m_AC = 0;
			m_breakChance = 0f;
			m_requiredSkillID = 0;
			m_requiredSkillTier = ETier.NONE;
		}

		public Int32 AC => m_AC;

	    public Single BreakChance => m_breakChance;

	    public Int32 RequiredSkillID => m_requiredSkillID;

	    public ETier RequiredSkillTier => m_requiredSkillTier;
	}
}
