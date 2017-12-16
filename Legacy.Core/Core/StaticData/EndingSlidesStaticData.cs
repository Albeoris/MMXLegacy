using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class EndingSlidesStaticData : BaseStaticData
	{
		[CsvColumn("TextKey")]
		public String TextKey;

		[CsvColumn("VoiceFile")]
		public String VoiceFile;

		[CsvColumn("Image")]
		public String Image;

		[CsvColumn("Quest")]
		public Int32 Quest;

		[CsvColumn("Tokens")]
		public Int32[] Tokens;

		[CsvColumn("Result")]
		public Boolean Result;

		[CsvColumn("DLC")]
		public Boolean DLC;
	}
}
