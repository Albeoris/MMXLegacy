using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData.Items
{
	public class PotionStaticData : BaseItemStaticData
	{
		[CsvColumn("TargetAttribute")]
		protected EPotionTarget m_targetAttribute;

		[CsvColumn("Operation")]
		protected EPotionOperation m_operation;

		[CsvColumn("Value")]
		protected Int32 m_value;

		[CsvColumn("Duration")]
		protected Int32 m_duration;

		[CsvColumn("ModelLevel")]
		protected Int32 m_modelLevel;

		[CsvColumn("Type")]
		protected EPotionType m_type;

		public PotionStaticData()
		{
			m_targetAttribute = EPotionTarget.HP;
			m_operation = EPotionOperation.INCREASE_ABS;
			m_value = 0;
			m_duration = 0;
		}

		public EPotionTarget TargetAttribute => m_targetAttribute;

	    public EPotionOperation Operation => m_operation;

	    public Int32 Value => m_value;

	    public Int32 Duration => m_duration;

	    public Int32 ModelLevel => m_modelLevel;

	    public EPotionType Type => m_type;
	}
}
