using System;

namespace Legacy.Core.EventManagement
{
	public class CustomPopupEventArgs : EventArgs
	{
		public CustomPopupEventArgs(String p_caption, String p_text)
		{
			caption = p_caption;
			text = p_text;
		}

		public String caption { get; private set; }

		public String text { get; private set; }
	}
}
