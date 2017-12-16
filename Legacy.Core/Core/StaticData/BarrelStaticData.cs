using System;
using Dumper.Core;
using Legacy.Core.Entities.Items;

namespace Legacy.Core.StaticData
{
	public class BarrelStaticData : BaseStaticData
	{
		[CsvColumn("TargetAttribute")]
		public EPotionTarget TargetAttribute;

		[CsvColumn("Value")]
		public Int32 Value;

		[CsvColumn("Prefab")]
		public String Prefab;

		[CsvColumn("MenuPath")]
		public String MenuPath;
	}
}
