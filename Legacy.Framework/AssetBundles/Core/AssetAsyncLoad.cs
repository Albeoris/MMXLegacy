using System;
using UnityEngine;

namespace AssetBundles.Core
{
    internal struct AssetAsyncLoad
    {
        private AssetBundleCache m_Origin;

        private AssetKey m_Key;

        private UnityEngine.AssetBundleRequest m_AsyncRequest;

        internal AssetAsyncLoad(AssetBundleCache origin, AssetKey key, UnityEngine.AssetBundleRequest asyncRequest)
        {
            m_Origin = origin;
            m_Key = key;
            m_AsyncRequest = asyncRequest;
        }

        public Boolean IsDone => m_AsyncRequest.isDone;

        public Single Progress => m_AsyncRequest.progress;

        public AssetKey Key => m_Key;

        public Boolean Update()
        {
            if (m_Origin.Status != EStatus.Ready || m_Origin.AssetBundleObject == null)
            {
                return true;
            }
            if (m_AsyncRequest.isDone)
            {
                m_Origin.AddAssetToCache(m_Key, m_Origin.AssetBundleObject.Load(m_Key.Name, m_Key.Type));
                return true;
            }
            return false;
        }
    }
}
