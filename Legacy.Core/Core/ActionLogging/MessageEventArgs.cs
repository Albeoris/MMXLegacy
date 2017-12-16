using System;

namespace Legacy.Core.ActionLogging
{
	public class MessageEventArgs : LogEntryEventArgs
	{
		public MessageEventArgs(String p_message)
		{
			Message = p_message;
			IsLocalized = false;
		}

		public MessageEventArgs(String p_message, Boolean p_isLocalized)
		{
			Message = p_message;
			IsLocalized = p_isLocalized;
		}

		public String Message { get; private set; }

		public Boolean IsLocalized { get; private set; }
	}
}
