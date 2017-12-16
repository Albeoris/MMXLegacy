using System;
using Dumper.Core;
using Legacy.Core.Spells;

namespace Legacy.Core.StaticData
{
	public class MonsterAbilityStaticData : BaseStaticData
	{
		[CsvColumn("KeyName")]
		private String m_nameKey;

		[CsvColumn("Icon")]
		private String m_icon;

		[CsvColumn("GeneralValues1")]
		private Single[] m_values1;

		[CsvColumn("GeneralValues2")]
		private Single[] m_values2;

		[CsvColumn("GeneralValues3")]
		private Single[] m_values3;

		[CsvColumn("IsPercentValue")]
		private Boolean m_isPercentValue;

		[CsvColumn("Gfx")]
		private String m_Gfx;

		[CsvColumn("Animation")]
		private String m_Animation;

		[CsvColumn("ActiveAbility")]
		private Boolean m_ActiveAbility;

		[CsvColumn("TargetType")]
		private ETargetType m_TargetType;

		public MonsterAbilityStaticData()
		{
			m_nameKey = String.Empty;
			m_icon = String.Empty;
			m_values1 = new Single[0];
			m_values2 = new Single[0];
			m_values3 = new Single[0];
			m_isPercentValue = false;
			m_Gfx = String.Empty;
			m_Animation = ",";
			m_ActiveAbility = false;
			m_TargetType = ETargetType.NONE;
		}

		public Boolean IsPercentValue => m_isPercentValue;

	    public String Icon
		{
			get => m_icon;
	        set => m_icon = value;
	    }

		public String NameKey => m_nameKey;

	    public String Gfx => m_Gfx;

	    public String Animation => m_Animation;

	    public Boolean ActiveAbility => m_ActiveAbility;

	    public ETargetType TargetType => m_TargetType;

	    public Single[] GetValues(Int32 p_level)
		{
			if (p_level == 3)
			{
				return m_values3;
			}
			if (p_level == 2)
			{
				return m_values2;
			}
			return m_values1;
		}
	}
}
