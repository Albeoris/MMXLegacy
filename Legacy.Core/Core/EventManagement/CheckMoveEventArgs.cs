using System;

namespace Legacy.Core.EventManagement
{
	public class CheckMoveEventArgs : EventArgs
	{
		public CheckMoveEventArgs(Boolean p_forward, Boolean p_backward, Boolean p_left, Boolean p_right)
		{
			Forward = p_forward;
			Backward = p_backward;
			Left = p_left;
			Right = p_right;
		}

		public Boolean Forward { get; private set; }

		public Boolean Backward { get; private set; }

		public Boolean Left { get; private set; }

		public Boolean Right { get; private set; }
	}
}
