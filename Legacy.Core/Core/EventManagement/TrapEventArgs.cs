using System;
using Legacy.Core.Entities.InteractiveObjects;

namespace Legacy.Core.EventManagement
{
	public class TrapEventArgs : EventArgs
	{
		public TrapEventArgs(TrapEffectStaticData p_TrapEffect, InteractiveObject p_trap, BarkEventArgs[] p_BarkEventArgs)
		{
			TrapEffect = p_TrapEffect;
			Trap = p_trap;
			BarkEventArgs = p_BarkEventArgs;
		}

		public TrapEffectStaticData TrapEffect { get; private set; }

		public InteractiveObject Trap { get; private set; }

		public BarkEventArgs[] BarkEventArgs { get; private set; }

		public override String ToString()
		{
			return String.Format("[TrapEventArgs: TrapEffect={0}, Trap={1}]", TrapEffect, Trap);
		}
	}
}
