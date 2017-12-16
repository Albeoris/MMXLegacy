using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData.Items
{
	public class ItemProbabilityStaticData : BaseItemStaticData
	{
		[CsvColumn("Type")]
		public EEquipmentType Type;

		[CsvColumn("Mercenary")]
		public Single Mercenary;

		[CsvColumn("Crusader")]
		public Single Crusader;

		[CsvColumn("Freemage")]
		public Single Freemage;

		[CsvColumn("Bladedancer")]
		public Single Bladedancer;

		[CsvColumn("Ranger")]
		public Single Ranger;

		[CsvColumn("Druid")]
		public Single Druid;

		[CsvColumn("Defender")]
		public Single Defender;

		[CsvColumn("Scout")]
		public Single Scout;

		[CsvColumn("Runepriest")]
		public Single Runepriest;

		[CsvColumn("Barbarian")]
		public Single Barbarian;

		[CsvColumn("Hunter")]
		public Single Hunter;

		[CsvColumn("Shaman")]
		public Single Shaman;
	}
}
