using System;

namespace AssetBundles.Core
{
	public class UnityEventArgs
	{
		public static readonly UnityEventArgs Empty = new UnityEventArgs(null, EventArgs.Empty);

		public UnityEventArgs(Object sender, EventArgs eventArgs)
		{
			Sender = sender;
			EventArgs = eventArgs;
		}

		public Object Sender { get; private set; }

		public EventArgs EventArgs { get; private set; }
	}
}
