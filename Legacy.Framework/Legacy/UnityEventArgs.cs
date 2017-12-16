using System;

namespace Legacy
{
	public class UnityEventArgs
	{
		public UnityEventArgs(Object sender, EventArgs eventArgs)
		{
			Sender = sender;
			EventArgs = eventArgs;
		}

		public UnityEventArgs(Object sender)
		{
			Sender = sender;
			EventArgs = EventArgs.Empty;
		}

		public Object Sender { get; protected set; }

		public EventArgs EventArgs { get; protected set; }
	}
}
