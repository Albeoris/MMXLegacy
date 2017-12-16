using System;

namespace Legacy
{
	public static class Random
	{
		private static System.Random sRandom = new System.Random();

		public static Single Value => (Single)sRandom.NextDouble();

	    public static Int32 Range(Int32 min, Int32 max)
		{
			return sRandom.Next(min, max);
		}

		public static Single Range(Single min, Single max)
		{
			if (min > max)
			{
				throw new ArgumentOutOfRangeException("min", "min > max");
			}
			Double num = sRandom.NextDouble() * (max - min);
			num += min;
			return (Single)num;
		}
	}
}
