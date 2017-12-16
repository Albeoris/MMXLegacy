using System;
using Legacy.Core.Combat;
using Legacy.Core.Entities.TrapEffects;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.ActionLogging
{
	public class TrapDamageEventArgs : LogEntryEventArgs
	{
		public TrapDamageEventArgs(BaseTrapEffect p_source, Character p_target, AttackResult p_result, ECondition p_conditionReceived)
		{
			Source = p_source;
			Target = p_target;
			Result = p_result;
			ConditionReceived = p_conditionReceived;
		}

		public BaseTrapEffect Source { get; private set; }

		public Character Target { get; private set; }

		public AttackResult Result { get; private set; }

		public ECondition ConditionReceived { get; private set; }
	}
}
