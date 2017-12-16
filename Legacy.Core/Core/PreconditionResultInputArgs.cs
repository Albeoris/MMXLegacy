using System;

namespace Legacy.Core
{
	public class PreconditionResultInputArgs : EventArgs
	{
		public PreconditionResultInputArgs(String p_text, Boolean p_cancelled)
		{
			m_textInput = p_text;
			m_cancelled = p_cancelled;
		}

		public String m_textInput { get; private set; }

		public Boolean m_cancelled { get; private set; }
	}
}
