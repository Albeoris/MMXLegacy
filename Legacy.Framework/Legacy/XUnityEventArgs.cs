using System;

namespace Legacy
{
	public class XUnityEventArgs<T> : UnityEventArgs<XEventArgs<T>>
	{
		public XUnityEventArgs(Object sender, XEventArgs<T> eventArgs) : base(sender, eventArgs)
		{
		}
	}
}
