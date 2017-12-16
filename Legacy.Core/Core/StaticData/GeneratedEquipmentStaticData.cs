using System;
using Dumper.Core;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData
{
	public class GeneratedEquipmentStaticData : BaseStaticData
	{
		[CsvColumn("ModelLevels")]
		public ModelProbability[] ModelLevels;

		[CsvColumn("PrefixChance")]
		public Single PrefixChance;

		[CsvColumn("PrefixProbabilities")]
		public EnchantmentProbability[] PrefixProbabilities;

		[CsvColumn("SuffixChance")]
		public Single SuffixChance;

		[CsvColumn("SuffixProbabilities")]
		public EnchantmentProbability[] SuffixProbabilities;

		[CsvColumn("SpecificationList")]
		public EEquipmentType[] SpecificationList;

		[CsvColumn("Fire")]
		public Single Fire;

		[CsvColumn("Water")]
		public Single Water;

		[CsvColumn("Air")]
		public Single Air;

		[CsvColumn("Earth")]
		public Single Earth;

		[CsvColumn("Light")]
		public Single Light;

		[CsvColumn("Dark")]
		public Single Dark;

		[CsvColumn("Primordial")]
		public Single Primordial;

		[CsvColumn("None")]
		public Single None;
	}
}
