using System;
using System.Threading;

namespace Legacy.Core.ActionLogging
{
	public class ActionLog
	{
		public event EventHandler<LogEntryEventArgs> ReceivedLogEntry;

		public void PushEntry(LogEntryEventArgs p_args)
		{
			if (p_args == null)
			{
				throw new ArgumentNullException("p_args");
			}
			if (ReceivedLogEntry != null)
			{
				ReceivedLogEntry(this, p_args);
			}
		}
	}
}
