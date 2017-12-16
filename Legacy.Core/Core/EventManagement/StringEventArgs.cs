using System;

namespace Legacy.Core.EventManagement
{
	public class StringEventArgs : EventArgs
	{
		public StringEventArgs(String p_text)
		{
			text = p_text;
		}

		public StringEventArgs(String p_text, String p_caption)
		{
			text = p_text;
			caption = p_caption;
		}

		public String text { get; private set; }

		public String caption { get; private set; }
	}
}
