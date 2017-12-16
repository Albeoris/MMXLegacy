using System;
using UnityEngine;
using Object = System.Object;

namespace AssetBundles.Core.Loader
{
	public abstract class AssetBundleRequest : IDisposable
	{
		public abstract Boolean IsStarted { get; }

		public abstract Single Progress { get; }

		public abstract Int32 TotalBytes { get; }

		public abstract Int32 ReceivedBytes { get; }

		public abstract String RequestAddress { get; }

		public abstract String ErrorText { get; }

		public virtual Boolean IsDone => Progress == 1f;

	    public Boolean IsError => !String.IsNullOrEmpty(ErrorText);

	    public Object UserToken { get; set; }

		public abstract Int32 Priority { get; }

		public abstract void Start();

		public abstract void Abort();

		public abstract Single Update();

		public abstract void Dispose();

		public abstract AssetBundle GetAssetBundle();
	}
}
