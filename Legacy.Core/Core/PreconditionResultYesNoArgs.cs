using System;

namespace Legacy.Core
{
	public class PreconditionResultYesNoArgs : EventArgs
	{
		public PreconditionResultYesNoArgs(Boolean p_accepted, Boolean p_cancelled)
		{
			m_accepted = p_accepted;
			m_cancelled = p_cancelled;
		}

		public Boolean m_accepted { get; private set; }

		public Boolean m_cancelled { get; private set; }
	}
}
