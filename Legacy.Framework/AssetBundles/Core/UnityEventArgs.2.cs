using System;

namespace AssetBundles.Core
{
	public class UnityEventArgs<TEventArgs> : UnityEventArgs where TEventArgs : EventArgs
	{
		public UnityEventArgs(Object sender, TEventArgs eventArgs) : base(sender, eventArgs)
		{
		}

		public new TEventArgs EventArgs => (TEventArgs)base.EventArgs;
	}
}
