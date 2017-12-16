using System;
using System.Collections;

namespace AssetBundles.Core
{
	public abstract class Request : IEnumerator
	{
		internal Request(AssetBundleManager manager)
		{
			Manager = manager;
		}

		Object IEnumerator.Current => null;

	    Boolean IEnumerator.MoveNext()
		{
			return !IsDone;
		}

		void IEnumerator.Reset()
		{
		}

		public AssetBundleManager Manager { get; private set; }

		public ERequestStatus Status { get; internal set; }

		public String ErrorText { get; internal set; }

		public Boolean IsDone { get; internal set; }

		public Single Progress { get; internal set; }

		public abstract String Name { get; }

		public abstract String AssetBundleName { get; }

		public abstract Int32 Priority { get; }

		public Object Tag { get; set; }

		internal abstract void Update();

		internal void Cancel()
		{
			if (Status != ERequestStatus.Done)
			{
				Status = ERequestStatus.Error;
				IsDone = true;
				Progress = 1f;
			}
		}
	}
}
