using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class BloodMagicEventArgs : LogEntryEventArgs
	{
		public BloodMagicEventArgs(Character p_character, Int32 p_value)
		{
			Character = p_character;
			Value = p_value;
		}

		public Character Character { get; private set; }

		public Int32 Value { get; private set; }
	}
}
