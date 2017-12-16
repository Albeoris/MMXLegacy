using System;

namespace AssetBundles.Core.Loader
{
	public class LoadProgressChangedEventArgs : LoadEventArgs
	{
		internal LoadProgressChangedEventArgs(AssetBundleRequest request) : base(request)
		{
			ProgressPercentage = (Int32)(100f * request.Progress);
			ReceivedBytes = request.ReceivedBytes;
			TotalBytesToReceive = request.TotalBytes;
		}

		public Int32 ProgressPercentage { get; private set; }

		public Int64 ReceivedBytes { get; private set; }

		public Int64 TotalBytesToReceive { get; private set; }
	}
}
