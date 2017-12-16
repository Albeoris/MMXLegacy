using System;
using Dumper.Core;

namespace Legacy.Core.StaticData
{
	public class MonsterBuffStaticData : BaseStaticData
	{
		[CsvColumn("BuffValues1")]
		public Single[] BuffValues1;

		[CsvColumn("BuffValues2")]
		public Single[] BuffValues2;

		[CsvColumn("BuffValues3")]
		public Single[] BuffValues3;

		[CsvColumn("DestroyedAfterMonsterTurn")]
		public Boolean DestroyedAfterMonsterTurn;

		[CsvColumn("Stackable")]
		public Boolean Stackable;

		[CsvColumn("NameKey")]
		public String NameKey;

		[CsvColumn("IsDebuff")]
		public Boolean IsDebuff;

		[CsvColumn("Icon")]
		public String Icon = String.Empty;

		[CsvColumn("Gfx")]
		public String Gfx = String.Empty;

		[CsvColumn("Duration1")]
		public Int32 Duration1;

		[CsvColumn("Duration2")]
		public Int32 Duration2;

		[CsvColumn("Duration3")]
		public Int32 Duration3;

		public Single[] GetBuffValues(Int32 p_level)
		{
			if (p_level == 3)
			{
				return BuffValues3;
			}
			if (p_level == 2)
			{
				return BuffValues2;
			}
			return BuffValues1;
		}

		public Int32 GetDuration(Int32 p_level)
		{
			if (p_level == 3)
			{
				return Duration3;
			}
			if (p_level == 2)
			{
				return Duration2;
			}
			return Duration1;
		}
	}
}
