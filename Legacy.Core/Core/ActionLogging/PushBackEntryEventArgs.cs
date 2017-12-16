using System;
using Legacy.Core.Entities;

namespace Legacy.Core.ActionLogging
{
	public class PushBackEntryEventArgs : LogEntryEventArgs
	{
		public PushBackEntryEventArgs(Monster p_target, Boolean p_wasPushed)
		{
			Target = p_target;
			PushedBack = p_wasPushed;
		}

		public Monster Target { get; private set; }

		public Boolean PushedBack { get; private set; }
	}
}
