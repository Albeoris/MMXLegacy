using System;

namespace AssetBundles.Core.Loader
{
	public class LoadCompletedEventArgs : LoadEventArgs
	{
		internal LoadCompletedEventArgs(AssetBundleRequest request, Boolean cancelled, String error) : base(request)
		{
			Cancelled = cancelled;
			Error = error;
		}

		public Boolean Cancelled { get; private set; }

		public String Error { get; private set; }

		public Boolean IsError => !String.IsNullOrEmpty(Error);
	}
}
