using System;
using Dumper.Core;
using Legacy.Core.Entities;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData
{
	public class ChestStaticData : BaseStaticData
	{
		[CsvColumn("Menupath")]
		public String Menupath = String.Empty;

		[CsvColumn("SteadyLoot")]
		public SteadyLoot[] DropSteadyLoot;

		[CsvColumn("ModelLevels")]
		public ModelProbability[] ModelLevels;

		[CsvColumn("ItemDropChance")]
		public Single DropItemChance;

		[CsvColumn("PrefixChance")]
		public Single DropItemPrefixChance;

		[CsvColumn("PrefixProbabilities")]
		public EnchantmentProbability[] DropItemPrefixProbabilities;

		[CsvColumn("SuffixChance")]
		public Single DropItemSuffixChance;

		[CsvColumn("SuffixProbabilities")]
		public EnchantmentProbability[] DropItemSuffixProbabilities;

		[CsvColumn("ItemSpecificationList")]
		public EEquipmentType[] DropItemSpecificationList;

		[CsvColumn("GoldChance")]
		public Single DropGoldChance;

		[CsvColumn("GoldAmount")]
		public IntRange DropGoldAmount;
	}
}
