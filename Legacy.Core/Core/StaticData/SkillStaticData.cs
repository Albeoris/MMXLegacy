using System;
using Dumper.Core;
using Legacy.Core.Entities.Skills;

namespace Legacy.Core.StaticData
{
	public class SkillStaticData : BaseStaticData
	{
		[CsvColumn("Name")]
		protected String m_Name;

		[CsvColumn("Description")]
		protected String m_Description;

		[CsvColumn("Icon")]
		protected String m_Icon;

		[CsvColumn("Tier1Effects")]
		protected Int32[] m_Tier1Effects;

		[CsvColumn("Tier2Effects")]
		protected Int32[] m_Tier2Effects;

		[CsvColumn("Tier3Effects")]
		protected Int32[] m_Tier3Effects;

		[CsvColumn("Tier4Effects")]
		protected Int32[] m_Tier4Effects;

		[CsvColumn("Category")]
		protected ESkillCategory m_category;

		public String Name => m_Name;

	    public String Description => m_Description;

	    public String Icon => m_Icon;

	    public Int32[] Tier1Effects => m_Tier1Effects;

	    public Int32[] Tier2Effects => m_Tier2Effects;

	    public Int32[] Tier3Effects => m_Tier3Effects;

	    public Int32[] Tier4Effects => m_Tier4Effects;

	    public ESkillCategory Category => m_category;
	}
}
