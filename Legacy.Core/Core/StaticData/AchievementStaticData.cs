using System;
using Dumper.Core;
using Legacy.Core.Achievements;

namespace Legacy.Core.StaticData
{
	public class AchievementStaticData : BaseStaticData
	{
		[CsvColumn("KeyName")]
		public String NameKey;

		[CsvColumn("TriggerType")]
		public ETriggerType[] TriggerType;

		[CsvColumn("Counter")]
		public Int32 Count;

		[CsvColumn("ConditionID")]
		public EAchievementConditionType ConditionID;

		[CsvColumn("Parameter")]
		public String ConditionParameter;

		[CsvColumn("Icon")]
		public String Icon;

		[CsvColumn("DescriptionKey")]
		public String DescriptionKey;

		[CsvColumn("Global")]
		public Boolean Global;
	}
}
