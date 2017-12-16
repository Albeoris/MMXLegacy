using System;
using Legacy.Core.Map;

namespace Legacy.Core.EventManagement
{
	public class MoveEntityEventArgs : EventArgs
	{
		public MoveEntityEventArgs(Position p_oldPosition, Position p_position)
		{
			OldPosition = p_oldPosition;
			Position = p_position;
		}

		public MoveEntityEventArgs(Position p_oldPosition, Position p_position, EDirection p_targetOrientation)
		{
			OldPosition = p_oldPosition;
			Position = p_position;
			TargetOrientation = p_targetOrientation;
		}

		public Position OldPosition { get; private set; }

		public Position Position { get; private set; }

		public EDirection TargetOrientation { get; private set; }
	}
}
