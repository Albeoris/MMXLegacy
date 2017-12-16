using System;
using Legacy.Core.Map;

namespace Legacy.Core.EventManagement
{
	public class ReboundEntityEventArgs : EventArgs
	{
		public ReboundEntityEventArgs(Position p_oldPosition, Position p_position)
		{
			ReboundDirection = p_position - p_oldPosition;
		}

		public Position ReboundDirection { get; private set; }
	}
}
