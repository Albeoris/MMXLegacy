using System;

namespace Legacy.Core.EventManagement
{
	public class BestiaryEntryEventArgs : EventArgs
	{
		public BestiaryEntryEventArgs(String p_monsterName, EEntryState p_entryState)
		{
			MonsterNameKey = p_monsterName;
			EntryState = p_entryState;
		}

		public String MonsterNameKey { get; private set; }

		public EEntryState EntryState { get; private set; }

		public enum EEntryState
		{
			ENTRY_NEW,
			ENRTY_UPDATED
		}
	}
}
