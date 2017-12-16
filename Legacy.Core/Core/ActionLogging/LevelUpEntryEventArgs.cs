using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class LevelUpEntryEventArgs : LogEntryEventArgs
	{
		public LevelUpEntryEventArgs(Character p_character)
		{
			Character = p_character;
		}

		public Character Character { get; private set; }
	}
}
