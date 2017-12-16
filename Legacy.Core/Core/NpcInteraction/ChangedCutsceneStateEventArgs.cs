using System;

namespace Legacy.Core.NpcInteraction
{
	public class ChangedCutsceneStateEventArgs : EventArgs
	{
		public ChangedCutsceneStateEventArgs(Int32 oldState, Int32 newState)
		{
			OldState = oldState;
			NewState = newState;
		}

		public Int32 OldState { get; private set; }

		public Int32 NewState { get; private set; }
	}
}
