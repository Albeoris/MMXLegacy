using System;

namespace Legacy.Core.Combat
{
	public struct Resistance
	{
		public EDamageType Type;

		public Int32 Value;

		public Resistance(EDamageType p_type, Int32 p_value)
		{
			Type = p_type;
			Value = p_value;
		}

		public static Resistance operator +(Resistance left, Resistance right)
		{
			if (left.Type != right.Type)
			{
				throw new InvalidOperationException();
			}
			return new Resistance(left.Type, left.Value + right.Value);
		}
	}
}
