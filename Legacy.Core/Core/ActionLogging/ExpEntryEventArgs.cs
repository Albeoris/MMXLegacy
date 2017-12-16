using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class ExpEntryEventArgs : LogEntryEventArgs
	{
		public ExpEntryEventArgs(Character p_character, Int32 p_exp)
		{
			Character = p_character;
			Exp = p_exp;
		}

		public Character Character { get; private set; }

		public Int32 Exp { get; private set; }
	}
}
