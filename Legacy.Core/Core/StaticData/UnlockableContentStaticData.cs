using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class UnlockableContentStaticData : BaseStaticData
	{
		[CsvColumn("NameKey")]
		public String NameKey = String.Empty;

		[CsvColumn("InfoTextKey")]
		public String InfoTextKey = String.Empty;

		[CsvColumn("Image")]
		public String Image = String.Empty;

		[CsvColumn("IsBuyable")]
		public Boolean IsBuyable = true;

		[CsvColumn("PrivilegeId")]
		public Int32 PrivilegeId;
	}
}
