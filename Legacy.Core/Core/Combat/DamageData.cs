using System;

namespace Legacy.Core.Combat
{
	public struct DamageData
	{
		public readonly EDamageType Type;

		public readonly Int32 Minimum;

		public readonly Int32 Maximum;

		public DamageData(EDamageType p_type, Int32 p_minimum, Int32 p_maximum)
		{
			if (p_minimum < 0)
			{
				throw new ArgumentOutOfRangeException("p_minimum", "p_minimum < 0");
			}
			if (p_maximum < 0)
			{
				throw new ArgumentOutOfRangeException("p_maximum", "p_maximum < 0");
			}
			if (p_minimum > p_maximum)
			{
				throw new ArgumentOutOfRangeException("p_minimum", "p_minimum > p_maximum");
			}
			Type = p_type;
			Minimum = p_minimum;
			Maximum = p_maximum;
		}

		public static DamageData Add(DamageData left, DamageData right)
		{
			Add(ref left, ref right, out left);
			return left;
		}

		public static void Add(ref DamageData left, ref DamageData right, out DamageData result)
		{
			if (left.Type != right.Type)
			{
				throw new InvalidOperationException();
			}
			result = new DamageData(left.Type, left.Minimum + right.Minimum, left.Maximum + right.Maximum);
		}

		public static DamageData Scale(DamageData value, Single factor)
		{
			Scale(ref value, factor, out value);
			return value;
		}

		public static void Scale(ref DamageData value, Single factor, out DamageData result)
		{
			result = new DamageData(value.Type, (Int32)(value.Minimum * factor + 0.5f), (Int32)(value.Maximum * factor + 0.5f));
		}

		public override String ToString()
		{
			return String.Format("[DamageData Type:{0} Minimum:{1} Maximum:{2}]", Type, Minimum, Maximum);
		}

		public static DamageData operator +(DamageData left, DamageData right)
		{
			if (left.Type != right.Type)
			{
				throw new InvalidOperationException();
			}
			return new DamageData(left.Type, left.Minimum + right.Minimum, left.Maximum + right.Maximum);
		}

		public static DamageData operator *(DamageData value, Single factor)
		{
			Scale(ref value, factor, out value);
			return value;
		}
	}
}
