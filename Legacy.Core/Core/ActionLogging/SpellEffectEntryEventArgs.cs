using System;
using Legacy.Core.EventManagement;

namespace Legacy.Core.ActionLogging
{
	public class SpellEffectEntryEventArgs : LogEntryEventArgs
	{
		public SpellEffectEntryEventArgs(Object p_source, SpellEventArgs p_spellResult)
		{
			Source = p_source;
			SpellResult = p_spellResult;
		}

		public Object Source { get; private set; }

		public SpellEventArgs SpellResult { get; private set; }
	}
}
