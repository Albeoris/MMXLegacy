using System;

namespace Legacy.Core
{
	public struct FloatRange
	{
		public static readonly FloatRange Zero = new FloatRange(0f, 0f);

		public Single Min;

		public Single Max;

		public FloatRange(Single p_min, Single p_max)
		{
			Min = p_min;
			Max = p_max;
		}

		public Single Random()
		{
			return Legacy.Random.Range(Min, Max);
		}
	}
}
