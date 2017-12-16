using System;
using Legacy.Core.Combat;

namespace Legacy.Core
{
	public class DamageEventArgs : EventArgs
	{
		public DamageEventArgs(AttackResult p_attackResult)
		{
			AttackResult = p_attackResult;
		}

		public AttackResult AttackResult { get; private set; }
	}
}
