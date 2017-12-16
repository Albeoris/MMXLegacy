using System;
using Legacy.Core.ActionLogging;

namespace Legacy.Core
{
	public class TokenRemovedEventArgs : LogEntryEventArgs
	{
		public TokenRemovedEventArgs(TokenStaticData p_token)
		{
			TokenID = p_token.StaticID;
			Name = p_token.Name;
		}

		public Int32 TokenID { get; private set; }

		public String Name { get; private set; }
	}
}
