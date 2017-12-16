using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class InteractiveObjectTooltipStaticData : BaseStaticData
	{
		[CsvColumn("PrefabFolder")]
		public String PrefabFolder;

		[CsvColumn("LocaKey")]
		public String LocaKey;
	}
}
