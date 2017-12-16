using System;

namespace Legacy.Core.Utilities
{
	public enum MessageBoxResult : uint
	{
		Ok = 1u,
		Cancel,
		Abort,
		Retry,
		Ignore,
		Yes,
		No,
		Close,
		Help,
		TryAgain,
		Continue,
		Timeout = 32000u
	}
}
