using System;
using UnityEngine;

namespace AssetBundles.Core
{
	public sealed class SceneRequest : Request
	{
		private AssetBundleCache m_AssetBundleCache;

		private AssetKey m_AssetKey;

		private SceneRequestCallback m_Callback;

		private Int32 m_Priority;

		internal SceneRequest(AssetBundleManager manager, Int32 priority, AssetBundleCache assetBundleCache, AssetKey assetKey, SceneRequestCallback callback) : base(manager)
		{
			m_Priority = priority;
			m_AssetBundleCache = assetBundleCache;
			m_AssetKey = assetKey;
			m_Callback = callback;
		}

		public String SceneName => m_AssetKey.Name;

	    public override String Name => m_AssetKey.Name;

	    public override String AssetBundleName => m_AssetBundleCache.Data.Name;

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
						Debug.LogError("Error load SceneAssetBundle\nBundle: " + m_AssetBundleCache.Data.Name + "\nSceneName: " + SceneName);
						IsDone = true;
						Progress = 1f;
						Status = ERequestStatus.Error;
						ErrorText = m_AssetBundleCache.ErrorText;
					}
				}
				else
				{
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
