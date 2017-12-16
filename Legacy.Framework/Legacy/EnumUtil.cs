using System;

namespace Legacy
{
	public static class EnumUtil<T>
	{
		public static readonly T[] Values = (T[])Enum.GetValues(typeof(T));

		public static readonly Int32 Length = Values.Length;
	}
}
