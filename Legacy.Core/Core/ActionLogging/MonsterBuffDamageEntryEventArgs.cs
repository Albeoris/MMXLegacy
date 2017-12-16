using System;
using Legacy.Core.Buffs;
using Legacy.Core.Combat;

namespace Legacy.Core.ActionLogging
{
	public class MonsterBuffDamageEntryEventArgs : LogEntryEventArgs
	{
		public MonsterBuffDamageEntryEventArgs(MonsterBuff p_buff, Object p_target, AttackResult p_attackResult)
		{
			Buff = p_buff;
			Target = p_target;
			AttackResult = p_attackResult;
		}

		public MonsterBuff Buff { get; private set; }

		public Object Target { get; private set; }

		public AttackResult AttackResult { get; private set; }
	}
}
