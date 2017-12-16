using System;
using UnityEngine;

namespace AssetBundles.Core
{
	public sealed class AssetBundleRequest : Request
	{
		private AssetBundleCache m_AssetBundleCache;

		private AssetBundleRequestCallback m_Callback;

		private Int32 m_Priority;

		internal AssetBundleRequest(AssetBundleManager manager, Int32 priority, AssetBundleCache assetBundleCache, AssetBundleRequestCallback callback) : base(manager)
		{
			m_Priority = priority;
			m_AssetBundleCache = assetBundleCache;
			m_Callback = callback;
		}

		public override String AssetBundleName => m_AssetBundleCache.Data.Name;

	    public AssetBundle AssetBundle { get; private set; }

		public override String Name => m_AssetBundleCache.Data.Name;

	    public override Int32 Priority => m_Priority;

	    internal override void Update()
		{
			if (Manager.CheckAndLoadBundles(m_AssetBundleCache, m_Priority))
			{
				EStatus status = m_AssetBundleCache.Status;
				if (status != EStatus.Ready)
				{
					if (status == EStatus.Error)
					{
						Debug.LogError("Error load AssetBundle: " + m_AssetBundleCache.Data.Name);
						IsDone = true;
						Progress = 1f;
						Status = ERequestStatus.Error;
						ErrorText = m_AssetBundleCache.ErrorText;
					}
				}
				else
				{
					AssetBundle = m_AssetBundleCache.AssetBundleObject;
					IsDone = true;
					Progress = 1f;
					Status = ERequestStatus.Done;
				}
				if (m_Callback != null)
				{
					try
					{
						m_Callback(this);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
					m_Callback = null;
				}
			}
		}
	}
}
