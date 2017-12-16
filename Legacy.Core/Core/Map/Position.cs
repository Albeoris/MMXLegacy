using System;
using System.Globalization;

namespace Legacy.Core.Map
{
	public struct Position : IEquatable<Position>
	{
		public static readonly Position Zero = default(Position);

		public static readonly Position Right = new Position(1, 0);

		public static readonly Position Left = new Position(-1, 0);

		public static readonly Position Up = new Position(0, 1);

		public static readonly Position Down = new Position(0, -1);

		public Int32 X;

		public Int32 Y;

		public Position(Int32 x, Int32 y)
		{
			X = x;
			Y = y;
		}

		public Int32 this[Int32 index]
		{
			get
			{
				if (index == 0)
				{
					return X;
				}
				if (index != 1)
				{
					throw new ArgumentOutOfRangeException("index", "Indices for Position run from 0 to 1, inclusive.");
				}
				return Y;
			}
			set
			{
				if (index != 0)
				{
					if (index != 1)
					{
						throw new ArgumentOutOfRangeException("index", "Indices for Position run from 0 to 1, inclusive.");
					}
					Y = value;
				}
				else
				{
					X = value;
				}
			}
		}

		public Single Length()
		{
			return (Single)Math.Sqrt(X * X + Y * Y);
		}

		public Single LengthSquared()
		{
			return X * X + Y * Y;
		}

		public static void Distance(ref Position value1, ref Position value2, out Single result)
		{
			Single num = value1.X - value2.X;
			Single num2 = value1.Y - value2.Y;
			result = (Single)Math.Sqrt(num * num + num2 * num2);
		}

		public static Single Distance(Position value1, Position value2)
		{
			Single num = value1.X - value2.X;
			Single num2 = value1.Y - value2.Y;
			return (Single)Math.Sqrt(num * num + num2 * num2);
		}

		public static void DistanceSquared(ref Position value1, ref Position value2, out Single result)
		{
			Single num = value1.X - value2.X;
			Single num2 = value1.Y - value2.Y;
			result = num * num + num2 * num2;
		}

		public static Single DistanceSquared(Position value1, Position value2)
		{
			Single num = value1.X - value2.X;
			Single num2 = value1.Y - value2.Y;
			return num * num + num2 * num2;
		}

		public override Int32 GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode();
		}

		public Boolean Equals(Position other)
		{
			return other.X == X && other.Y == Y;
		}

		public override Boolean Equals(Object value)
		{
			return value != null && ReferenceEquals(value.GetType(), typeof(Position)) && Equals((Position)value);
		}

		public override String ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", new Object[]
			{
				X,
				Y
			});
		}

		public String ToString(String format)
		{
			if (format == null)
			{
				return ToString();
			}
			return String.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", new Object[]
			{
				X.ToString(format, CultureInfo.CurrentCulture),
				Y.ToString(format, CultureInfo.CurrentCulture)
			});
		}

		public static Position operator +(Position left, Position right)
		{
			return new Position(left.X + right.X, left.Y + right.Y);
		}

		public static implicit operator Position(GridPosition p_pos)
		{
			return new Position((Int32)p_pos.X, (Int32)p_pos.Y);
		}

		public static Position operator +(Position pos, EDirection p_dir)
		{
			switch (p_dir)
			{
			case EDirection.NORTH:
				return pos + Up;
			case EDirection.EAST:
				return pos + Right;
			case EDirection.SOUTH:
				return pos + Down;
			case EDirection.WEST:
				return pos + Left;
			default:
				return pos;
			}
		}

		public static Position operator *(Position left, Position right)
		{
			return new Position(left.X * right.X, left.Y * right.Y);
		}

		public static Position operator *(Position left, Int32 value)
		{
			return new Position(left.X * value, left.Y * value);
		}

		public static Position operator +(Position value)
		{
			return value;
		}

		public static Position operator -(Position left, Position right)
		{
			return new Position(left.X - right.X, left.Y - right.Y);
		}

		public static Position operator -(Position value)
		{
			return new Position(-value.X, -value.Y);
		}

		public static Boolean operator ==(Position left, Position right)
		{
			return left.Equals(right);
		}

		public static Boolean operator !=(Position left, Position right)
		{
			return !left.Equals(right);
		}
	}
}
