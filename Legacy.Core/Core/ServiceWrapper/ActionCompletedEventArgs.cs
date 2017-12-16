using System;

namespace Legacy.Core.ServiceWrapper
{
	public class ActionCompletedEventArgs : EventArgs
	{
		public ActionCompletedEventArgs(Int32 p_action, Boolean p_online)
		{
			ActionID = p_action;
			Online = p_online;
		}

		public Int32 ActionID { get; private set; }

		public Boolean Online { get; private set; }
	}
}
