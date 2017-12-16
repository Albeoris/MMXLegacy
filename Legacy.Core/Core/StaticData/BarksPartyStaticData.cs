using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class BarksPartyStaticData : BarksCharacterStaticData
	{
		[CsvColumn("CoolDownMin")]
		public Int32 CoolDown;

		[CsvColumn("DayPhase")]
		public String DayPhase;

		[CsvColumn("GroupBark")]
		public Boolean GroupBark;
	}
}
