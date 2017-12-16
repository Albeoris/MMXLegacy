using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class DungeonEntryStaticData : BaseStaticData
	{
		[CsvColumn("DungeonEntryID")]
		public String DungeonEntryID;

		[CsvColumn("PartyAverageMinLevel")]
		public Single PartyAverageMinLevel;

		[CsvColumn("PartyAverageMaxLevel")]
		public Single PartyAverageMaxLevel;
	}
}
