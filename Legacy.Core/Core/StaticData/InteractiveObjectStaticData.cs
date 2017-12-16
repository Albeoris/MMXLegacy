using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class InteractiveObjectStaticData : BaseStaticData
	{
		[CsvColumn("SelfActive")]
		public Boolean SelfActive;

		[CsvColumn("Center")]
		public Boolean Center;

		[CsvColumn("Edge")]
		public Boolean Edge;

		[CsvColumn("Icon")]
		public String Icon;

		[CsvColumn("MinimapVisible")]
		public Boolean MinimapVisible;

		[CsvColumn("MapVisible")]
		public Boolean MapVisible;
	}
}
