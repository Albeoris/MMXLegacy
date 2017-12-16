using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class BarksCharacterStaticData : BaseStaticData
	{
		[CsvColumn("TriggerCondition")]
		public EBarks TriggerCondition;

		[CsvColumn("Probability")]
		public Single Probability;

		[CsvColumn("Clipname")]
		public String Clipname;

		[CsvColumn("Priority")]
		public Int32 Priority;

		[CsvColumn("OnRecieve")]
		public Boolean OnRecieve;
	}
}
