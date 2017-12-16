using System;
using Legacy.Core.PartyManagement;

namespace Legacy.Core
{
	public class PreconditionEvaluateArgs : EventArgs
	{
		public PreconditionEvaluateArgs(Boolean p_passed, Boolean p_cancelled, Character p_character)
		{
			Passed = p_passed;
			Cancelled = p_cancelled;
			Character = p_character;
			ShowMessage = true;
		}

		public Boolean Passed { get; private set; }

		public Boolean Cancelled { get; private set; }

		public Character Character { get; private set; }

		public Boolean ShowMessage { get; set; }
	}
}
