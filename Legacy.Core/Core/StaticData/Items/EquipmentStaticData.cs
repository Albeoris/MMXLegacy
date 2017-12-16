using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData.Items
{
	public class EquipmentStaticData : BaseItemStaticData
	{
		[CsvColumn("Type")]
		protected EEquipmentType m_type;

		[CsvColumn("PrefixID")]
		protected Int32[] m_prefix;

		[CsvColumn("SuffixID")]
		protected Int32[] m_suffix;

		[CsvColumn("PrefixLevel")]
		protected Int32 m_prefixLevel;

		[CsvColumn("SuffixLevel")]
		protected Int32 m_suffixLevel;

		[CsvColumn("ModelLevel")]
		protected String m_modelLevelComplete;

		[CsvColumn("Level")]
		protected Int32 m_level;

		[CsvColumn("RequiredXP")]
		protected Int32 m_requiredXP;

		[CsvColumn("NextLevelItemID")]
		protected Int32 m_nextLevelItemID;

		[CsvColumn("Description")]
		protected String m_description;

		[CsvColumn("Identified")]
		protected Boolean m_identified;

		private Int32 m_modelLevel;

		private ESubModel m_subModel;

		public EquipmentStaticData()
		{
			m_type = EEquipmentType.HEADGEAR;
			m_modelLevel = 1;
			m_subModel = ESubModel.A;
			m_level = 0;
			m_requiredXP = 0;
			m_nextLevelItemID = 0;
			m_description = String.Empty;
		}

		public override void PostDeserialization()
		{
			if (m_modelLevelComplete.Length > 1)
			{
				Int32.TryParse(m_modelLevelComplete.Substring(0, 1), out m_modelLevel);
				m_subModel = (ESubModel)Enum.Parse(typeof(ESubModel), m_modelLevelComplete.Substring(1, 1), true);
			}
		}

		public EEquipmentType Type => m_type;

	    public Int32[] Prefix => m_prefix;

	    public Int32[] Suffix => m_suffix;

	    public Int32 PrefixLevel => m_prefixLevel;

	    public Int32 SuffixLevel => m_suffixLevel;

	    public Int32 ModelLevel => m_modelLevel;

	    public ESubModel SubModel => m_subModel;

	    public Int32 RelicLevel => m_level;

	    public Int32 RequiredXP => m_requiredXP;

	    public Int32 NextLevelItemID => m_nextLevelItemID;

	    public String Description => m_description;

	    public Boolean Identified => m_identified;
	}
}
