using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.PartyManagement;

namespace Legacy.Core
{
	public class ExtraAttackEventArgs : LogEntryEventArgs
	{
		public ExtraAttackEventArgs(Character p_character)
		{
			Character = p_character;
		}

		public Character Character { get; private set; }
	}
}
