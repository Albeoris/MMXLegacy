using System;
using Legacy.Core.NpcInteraction.Functions;

namespace Legacy.Core.EventManagement
{
	public class NPCSpellTradeEventArgs : EventArgs
	{
		public NPCSpellTradeEventArgs(SpellFunction p_spells)
		{
			SpellFunction = p_spells;
		}

		public SpellFunction SpellFunction { get; private set; }
	}
}
