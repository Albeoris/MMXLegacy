using System;

namespace Legacy.Core.Map
{
	public class GridPosition
	{
		public Single X;

		public Single Y;

		public GridPosition(Single x, Single y)
		{
			X = x;
			Y = y;
		}

		public GridPosition()
		{
		}

		public Position ToPosition()
		{
			return new Position((Int32)X, (Int32)Y);
		}

		public static implicit operator GridPosition(Position p_pos)
		{
			return new GridPosition(p_pos.X, p_pos.Y);
		}
	}
}
