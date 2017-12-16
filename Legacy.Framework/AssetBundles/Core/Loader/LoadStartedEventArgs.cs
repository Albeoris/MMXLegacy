using System;

namespace AssetBundles.Core.Loader
{
	public class LoadStartedEventArgs : LoadEventArgs
	{
		public LoadStartedEventArgs(AssetBundleRequest request) : base(request)
		{
		}
	}
}
