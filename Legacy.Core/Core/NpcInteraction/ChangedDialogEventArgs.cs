using System;

namespace Legacy.Core.NpcInteraction
{
	public class ChangedDialogEventArgs : EventArgs
	{
		public ChangedDialogEventArgs(Dialog p_dialog)
		{
			Dialog = p_dialog;
		}

		public Dialog Dialog { get; private set; }
	}
}
