using System;

namespace Legacy.Core.ActionLogging
{
	public class BuffRemovedEventArgs : LogEntryEventArgs
	{
		public BuffRemovedEventArgs(Object p_lostBy, Object p_buff)
		{
			LostBy = p_lostBy;
			Buff = p_buff;
		}

		public Object LostBy { get; private set; }

		public Object Buff { get; private set; }
	}
}
