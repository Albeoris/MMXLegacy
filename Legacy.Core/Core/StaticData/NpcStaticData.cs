using System;
using Dumper.Core;
using Legacy.Core.NpcInteraction;

namespace Legacy.Core.StaticData
{
	public class NpcStaticData : BaseStaticData
	{
		[CsvColumn("NameKey")]
		public String NameKey = String.Empty;

		[CsvColumn("HirelingProfession")]
		public String HirelingProfession = String.Empty;

		[CsvColumn("PortraitKey")]
		public String PortraitKey = String.Empty;

		[CsvColumn("ConversationKey")]
		public String ConversationKey = String.Empty;

		[CsvColumn("TravelStationID")]
		public Int32 TravelStationID;

		[CsvColumn("NpcEffects")]
		public NpcEffect[] NpcEffects;

		[CsvColumn("HirePrice")]
		public Int32 HirePrice;

		[CsvColumn("HireShare")]
		public Single HireShare;

		[CsvColumn("CanBeFired")]
		public Boolean CanBeFired = true;

		[CsvColumn("MinimapSymbol")]
		public ENpcMinimapSymbol MinimapSymbol;

		[CsvColumn("AllowItemSell")]
		public Boolean AllowItemSell;
	}
}
