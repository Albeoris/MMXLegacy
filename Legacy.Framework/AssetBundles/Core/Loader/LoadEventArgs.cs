using System;

namespace AssetBundles.Core.Loader
{
	public abstract class LoadEventArgs : EventArgs
	{
		internal LoadEventArgs(AssetBundleRequest request)
		{
			Request = request;
		}

		public AssetBundleRequest Request { get; private set; }

		public Object UserToken => Request.UserToken;
	}
}
