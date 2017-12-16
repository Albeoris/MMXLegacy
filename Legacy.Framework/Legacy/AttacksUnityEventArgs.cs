using System;
using Legacy.Core.Combat;

namespace Legacy
{
	public class AttacksUnityEventArgs : UnityEventArgs
	{
		public AttacksUnityEventArgs(Object sender, AttackResult e, Boolean p_isMagical) : base(sender)
		{
			Result = e;
			IsMagical = p_isMagical;
		}

		public AttackResult Result { get; private set; }

		public Boolean IsMagical { get; private set; }
	}
}
