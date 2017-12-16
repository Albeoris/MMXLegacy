using System;

namespace Legacy.Core.EventManagement
{
	public class TokenEventArgs : EventArgs
	{
		public TokenEventArgs(Int32 tokenID)
		{
			TokenID = tokenID;
		}

		public Int32 TokenID { get; private set; }
	}
}
