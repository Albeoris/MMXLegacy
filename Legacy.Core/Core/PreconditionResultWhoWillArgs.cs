using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core
{
	public class PreconditionResultWhoWillArgs : EventArgs
	{
		public PreconditionResultWhoWillArgs(Character p_char, Boolean p_cancelled)
		{
			m_selectedCharacter = p_char;
			m_cancelled = p_cancelled;
		}

		public Character m_selectedCharacter { get; private set; }

		public Boolean m_cancelled { get; private set; }
	}
}
