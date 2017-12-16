using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData
{
	public class GeneratedConsumableStaticData : BaseStaticData
	{
		[CsvColumn("ModelRange")]
		private IntRange m_modelRange;

		[CsvColumn("SpecificationList")]
		private EOfferConsumableType[] m_specificationList;

		[CsvColumn("Fire")]
		private Single m_fire;

		[CsvColumn("Water")]
		private Single m_water;

		[CsvColumn("Air")]
		private Single m_air;

		[CsvColumn("Earth")]
		private Single m_earth;

		[CsvColumn("Light")]
		private Single m_light;

		[CsvColumn("Dark")]
		private Single m_dark;

		[CsvColumn("Primordial")]
		private Single m_primordial;

		public Single Fire => m_fire;

	    public Single Water => m_water;

	    public Single Air => m_air;

	    public Single Earth => m_earth;

	    public Single Light => m_light;

	    public Single Dark => m_dark;

	    public Single Primordial => m_primordial;

	    public IntRange ModelRange => m_modelRange;

	    public EOfferConsumableType[] SpecificationList => m_specificationList;
	}
}
