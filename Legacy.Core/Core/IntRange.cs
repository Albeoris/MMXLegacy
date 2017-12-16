using System;

namespace Legacy.Core
{
	public struct IntRange
	{
		public static readonly IntRange Zero = new IntRange(0, 0);

		public Int32 Min;

		public Int32 Max;

		public IntRange(Int32 p_min, Int32 p_max)
		{
			Min = p_min;
			Max = p_max;
		}

		public Int32 Random()
		{
			return Legacy.Random.Range(Min, Max);
		}
	}
}
