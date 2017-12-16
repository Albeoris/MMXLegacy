using System;

namespace Legacy.Core.Map
{
	public static class EDirectionFunctions
	{
		public static EDirection Add(EDirection direction, Int32 addValue)
		{
			Int32 num = (Int32)(direction + addValue);
			if (num < 0)
			{
				num += 4;
			}
			if (num >= 4)
			{
				num -= 4;
			}
			return (EDirection)num;
		}

		public static Vector3D GetVector3Dir(EDirection direction)
		{
			if (direction == EDirection.NORTH)
			{
				return new Vector3D(0f, 0f, 1f);
			}
			if (direction == EDirection.EAST)
			{
				return new Vector3D(1f, 0f, 0f);
			}
			if (direction == EDirection.SOUTH)
			{
				return new Vector3D(0f, 0f, -1f);
			}
			if (direction == EDirection.WEST)
			{
				return new Vector3D(-1f, 0f, 0f);
			}
			return Vector3D.Zero;
		}

		public static EDirection GetOppositeDir(EDirection direction)
		{
			return (EDirection)(((Int32)direction + 2) % (Int32)EDirection.COUNT);
		}

		public static EDirection GetLineOfSightDirection(Position from, Position to)
		{
			return GetLineOfSightDirection(ref from, ref to);
		}

		public static EDirection GetLineOfSightDirection(ref Position from, ref Position to)
		{
			Position position = to - from;
			EDirection result = EDirection.COUNT;
			if (position.X == 0 && position.Y > 0)
			{
				result = EDirection.NORTH;
			}
			else if (position.X == 0 && position.Y < 0)
			{
				result = EDirection.SOUTH;
			}
			else if (position.X > 0 && position.Y == 0)
			{
				result = EDirection.EAST;
			}
			else if (position.X < 0 && position.Y == 0)
			{
				result = EDirection.WEST;
			}
			return result;
		}

		public static EDirection GetDirection(Position from, Position to)
		{
			return GetDirection(ref from, ref to);
		}

		public static EDirection GetDirection(ref Position from, ref Position to)
		{
			Position position = to - from;
			EDirection result = EDirection.COUNT;
			if (position.Y > 0 && position.Y >= Math.Abs(position.X))
			{
				result = EDirection.NORTH;
			}
			else if (position.Y < 0 && Math.Abs(position.Y) >= Math.Abs(position.X))
			{
				result = EDirection.SOUTH;
			}
			else if (position.X > 0)
			{
				result = EDirection.EAST;
			}
			else if (position.X < 0)
			{
				result = EDirection.WEST;
			}
			return result;
		}

		public static Int32 RotationCount(EDirection from, EDirection to)
		{
			if (from == to)
			{
				return 0;
			}
			Int32 num = 1;
			if (from == EDirection.WEST)
			{
				from -= 2;
				num = -1;
			}
			if (to == EDirection.WEST)
			{
				to -= 2;
				num = -1;
			}
			if (from == to)
			{
				return 2 * num;
			}
			return (to - from) * num;
		}

		public static EDirection Clamp(EDirection value)
		{
			if (value < EDirection.NORTH)
			{
				value += 4;
			}
			else if (value > EDirection.WEST)
			{
				value -= 4;
			}
			return value;
		}

		public static EDirection Random()
		{
			return (EDirection)Legacy.Random.Range(0, 4);
		}
	}
}
