using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.EventManagement
{
	public class XpGainEventArgs : EventArgs
	{
		public XpGainEventArgs(Character p_character, Int32 p_xpWon, Single p_percentXPFilled, Int32 p_levelUp)
		{
			Character = p_character;
			XpWon = p_xpWon;
			PercentXPFilled = p_percentXPFilled;
			LevelUp = p_levelUp;
		}

		public Character Character { get; private set; }

		public Int32 XpWon { get; private set; }

		public Single PercentXPFilled { get; private set; }

		public Int32 LevelUp { get; private set; }
	}
}
