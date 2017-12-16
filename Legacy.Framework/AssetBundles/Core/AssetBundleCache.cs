using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundles.Core
{
	internal class AssetBundleCache
	{
		internal Boolean m_Foldout;

		private Dictionary<AssetKey, AssetCache> m_CachedAssetMap;

		private AssetBundle m_CachedAssetBundleObject;

		public readonly AssetBundleData Data;

		public readonly AssetBundleCache[] Dependencies;

		public AssetBundleCache(AssetBundleData data)
		{
			Data = data;
			if (data.BundleDependency != null && data.BundleDependency.Length > 0)
			{
				Dependencies = new AssetBundleCache[data.BundleDependency.Length];
			}
		}

		public EStatus Status { get; private set; }

		public String ErrorText { get; private set; }

		public AssetBundle AssetBundleObject => m_CachedAssetBundleObject;

	    public EStatus GetCachedAssetStatus(AssetKey key)
		{
			AssetCache assetCache;
			if (m_CachedAssetMap != null && m_CachedAssetMap.TryGetValue(key, out assetCache))
			{
				return assetCache.Status;
			}
			return EStatus.None;
		}

		public UnityEngine.Object GetCachedAsset(AssetKey key)
		{
			AssetCache assetCache;
			if (m_CachedAssetMap != null && m_CachedAssetMap.TryGetValue(key, out assetCache))
			{
				return assetCache.AssetObject;
			}
			return null;
		}

		public void AddAssetToCache(AssetKey key, UnityEngine.Object asset)
		{
			if (m_CachedAssetBundleObject != null && m_CachedAssetBundleObject.Contains(key.Name))
			{
				SetCachedAssetMap(key, EStatus.Ready, asset);
			}
		}

		public EStatus LoadAsset(AssetKey key, out AssetAsyncLoad assetLoader)
		{
			if (m_CachedAssetBundleObject != null && m_CachedAssetBundleObject.Contains(key.Name))
			{
				SetCachedAssetMap(key, EStatus.Load, null);
			    UnityEngine.AssetBundleRequest asyncRequest = m_CachedAssetBundleObject.LoadAsync(key.Name, key.Type);
				assetLoader = new AssetAsyncLoad(this, key, asyncRequest);
				return EStatus.Load;
			}
			assetLoader = default(AssetAsyncLoad);
			return EStatus.Error;
		}

		public void Unload(Boolean unloadAllLoadedObjects)
		{
			if (m_CachedAssetMap != null)
			{
				m_CachedAssetMap.Clear();
				m_CachedAssetMap = null;
			}
			if (m_CachedAssetBundleObject != null)
			{
				m_CachedAssetBundleObject.Unload(unloadAllLoadedObjects);
				m_CachedAssetBundleObject = null;
				Status = EStatus.None;
			}
		}

		public void UpdateStatus(AssetBundle bundle)
		{
			m_CachedAssetBundleObject = bundle;
			Status = EStatus.Ready;
		}

		public void UpdateStatus(String errorText)
		{
			ErrorText = errorText;
			m_CachedAssetBundleObject = null;
			Status = EStatus.Error;
		}

		public void UpdateStatusLoad()
		{
			Status = EStatus.Load;
		}

		private void SetCachedAssetMap(AssetKey key, EStatus status, UnityEngine.Object asset)
		{
			if (m_CachedAssetMap == null)
			{
				m_CachedAssetMap = new Dictionary<AssetKey, AssetCache>(Data.Assets.Length);
			}
			m_CachedAssetMap[key] = new AssetCache(status, asset);
		}

		private struct AssetCache
		{
			public EStatus Status;

			public UnityEngine.Object AssetObject;

			public AssetCache(EStatus status, UnityEngine.Object assetObject)
			{
				Status = status;
				AssetObject = assetObject;
			}
		}
	}
}
