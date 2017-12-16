using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class PotionUsedEntryEventArgs : LogEntryEventArgs
	{
		public PotionUsedEntryEventArgs(Character p_character, Potion p_potion, Int32 p_value)
		{
			Character = p_character;
			Potion = p_potion;
			Value = p_value;
		}

		public Character Character { get; private set; }

		public Potion Potion { get; private set; }

		public Int32 Value { get; private set; }
	}
}
