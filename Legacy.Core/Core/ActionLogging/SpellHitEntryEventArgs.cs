using System;
using Legacy.Core.Spells;

namespace Legacy.Core.ActionLogging
{
	public class SpellHitEntryEventArgs : LogEntryEventArgs
	{
		public SpellHitEntryEventArgs(Object p_target, Spell p_spell)
		{
			Spell = p_spell;
			Target = p_target;
		}

		public Object Target { get; private set; }

		public Spell Spell { get; private set; }
	}
}
