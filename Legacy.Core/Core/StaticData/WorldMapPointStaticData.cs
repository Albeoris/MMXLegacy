using System;
using Dumper.Core;
using Legacy.Core.Map;
using Legacy.Core.WorldMap;

namespace Legacy.Core.StaticData
{
	public class WorldMapPointStaticData : BaseStaticData
	{
		[CsvColumn("InitialState")]
		public EWorldMapPointState InitialState;

		[CsvColumn("NameKey")]
		public String NameKey;

		[CsvColumn("InfoKey")]
		public String InfoKey;

		[CsvColumn("StateChangeType")]
		public EWorldMapPointStateChangeType StateChangeType;

		[CsvColumn("QuestChangedType")]
		public QuestChangedEventArgs.Type QuestChangedType;

		[CsvColumn("QuestID")]
		public Int32 QuestID;

		[CsvColumn("TokenID")]
		public Int32 TokenID;

		[CsvColumn("GridPosition")]
		public Position Position;

		[CsvColumn("Icon")]
		public String Icon;
	}
}
