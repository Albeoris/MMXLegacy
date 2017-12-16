using System;
using UnityEngine;

namespace AssetBundles.Core
{
	public sealed class AssetRequest : Request
	{
		private AssetBundleCache m_AssetBundleCache;

		private AssetKey m_AssetKey;

		private AssetRequestCallback m_Callback;

		private Int32 m_Priority;

		internal AssetRequest(AssetBundleManager manager, Int32 priority, AssetBundleCache assetBundleCache, AssetKey assetKey, AssetRequestCallback callback) : base(manager)
		{
			m_Priority = priority;
			m_AssetBundleCache = assetBundleCache;
			m_AssetKey = assetKey;
			m_Callback = callback;
		}

		public Type AssetType => m_AssetKey.Type;

	    public String AssetName => m_AssetKey.Name;

	    public UnityEngine.Object Asset { get; internal set; }

		public override String Name => m_AssetKey.Name;

	    public override String AssetBundleName => m_AssetBundleCache.Data.Name;

	    public override Int32 Priority => m_Priority;

	    internal override void Update()
		{
			if (Manager.CheckAndLoadBundles(m_AssetBundleCache, Priority))
			{
				switch (m_AssetBundleCache.GetCachedAssetStatus(m_AssetKey))
				{
				case EStatus.None:
				{
					AssetAsyncLoad item;
					if (m_AssetBundleCache.LoadAsset(m_AssetKey, out item) != EStatus.Load)
					{
						Debug.LogError("Error load asset " + m_AssetKey);
						IsDone = true;
						Progress = 1f;
						Status = ERequestStatus.Error;
						ExecuteCallback();
						return;
					}
					Manager.m_AssetAsyncTasks.Add(item);
					return;
				}
				case EStatus.Load:
					return;
				case EStatus.Ready:
					IsDone = true;
					Progress = 1f;
					Status = ERequestStatus.Done;
					Asset = m_AssetBundleCache.GetCachedAsset(m_AssetKey);
					ExecuteCallback();
					return;
				}
				Debug.LogError("Error load Asset " + m_AssetKey);
				IsDone = true;
				Progress = 1f;
				Status = ERequestStatus.Error;
				ErrorText = m_AssetBundleCache.ErrorText;
				ExecuteCallback();
			}
		}

		private void ExecuteCallback()
		{
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
