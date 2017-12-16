using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Legacy.Core.ActionLogging;
using Legacy.Core.Combat;

namespace Legacy.Core.EventManagement
{
	public class AttacksEventArgs : EventArgs
	{
		public AttacksEventArgs(Boolean p_counterAttack)
		{
			Attacks = new List<AttackedTarget>();
			Counterattack = p_counterAttack;
			PushToParty = false;
			IsTryingToPushToParty = false;
		}

		public List<AttackedTarget> Attacks { get; private set; }

		public Boolean Counterattack { get; private set; }

		public Boolean PushToParty { get; set; }

		public Boolean IsTryingToPushToParty { get; set; }

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AttackedTarget
		{
			public AttackedTarget(Object p_attackTarget, AttackResult p_attackResult)
			{
				this = new AttackedTarget(p_attackTarget, p_attackResult, null, null);
			}

			public AttackedTarget(Object p_attackTarget, AttackResult p_attackResult, BarkEventArgs[] p_barkEventArgs, BloodMagicEventArgs p_bloodMagicArgs)
			{
				if (p_attackTarget == null)
				{
					throw new ArgumentNullException("p_attackTarget");
				}
				if (p_attackResult == null)
				{
					throw new ArgumentNullException("p_attakResults");
				}
				AttackTarget = p_attackTarget;
				AttackResult = p_attackResult;
				BarkEventArgs = p_barkEventArgs;
				BloodMagicEventArgs = p_bloodMagicArgs;
			}

			public Object AttackTarget { get; private set; }

			public AttackResult AttackResult { get; private set; }

			public Boolean IsCriticalAttack => AttackResult.Result == EResultType.CRITICAL_HIT;

		    public BarkEventArgs[] BarkEventArgs { get; private set; }

			public BloodMagicEventArgs BloodMagicEventArgs { get; private set; }
		}
	}
}
