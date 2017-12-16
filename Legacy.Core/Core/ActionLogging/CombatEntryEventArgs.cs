using System;
using Legacy.Core.Combat;

namespace Legacy.Core.ActionLogging
{
	public class CombatEntryEventArgs : LogEntryEventArgs
	{
		public CombatEntryEventArgs(Object p_source, Object p_target, AttackResult p_result, BloodMagicEventArgs p_bmArgs)
		{
			Source = p_source;
			Target = p_target;
			Result = p_result;
			BloodMagicEventArgs = p_bmArgs;
		}

		public Object Source { get; private set; }

		public Object Target { get; private set; }

		public AttackResult Result { get; private set; }

		public BloodMagicEventArgs BloodMagicEventArgs { get; private set; }
	}
}
