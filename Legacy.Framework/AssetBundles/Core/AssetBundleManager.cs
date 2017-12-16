using System;
using System.Collections.Generic;
using AssetBundles.Core.Loader;
using AssetBundles.Core.Serialization;
using UnityEngine;
using Object = System.Object;

namespace AssetBundles.Core
{
	public class AssetBundleManager : MonoBehaviour
	{
		internal Dictionary<String, AssetBundleCache> m_AssetBundles;

		internal List<Request> m_RequestTasks = new List<Request>();

		internal List<AssetAsyncLoad> m_AssetAsyncTasks = new List<AssetAsyncLoad>();

		private AssetBundleDatabase m_Database;

		private Boolean m_CheckTaskFlag;

		private Boolean m_SortRequestFlag;

		[SerializeField]
		private TextAsset m_AssetBundleDatabase;

		[SerializeField]
		private AssetBundleLoader m_AssetBundleLoader;

		public Boolean IsReady => m_Database != null;

	    public AssetBundleLoader AssetBundleLoader => m_AssetBundleLoader;

	    public AssetBundleDatabase Database => m_Database;

	    public AssetRequest RequestAsset(String assetName, Type assetType, Int32 priority, AssetRequestCallback callback, Object userToken)
		{
			if (String.IsNullOrEmpty(assetName))
			{
				throw new ArgumentException("value is null or empty", "assetName");
			}
			if (assetType == null)
			{
				throw new ArgumentNullException("assetType");
			}
			if (!assetType.IsAssignableFrom(typeof(UnityEngine.Object)))
			{
				throw new ArgumentException("asset type is not a subclass of UnityEngine.Object", "assetType");
			}
			if (m_Database == null || m_AssetBundles == null)
			{
				throw new InvalidOperationException("Database not loaded!");
			}
			AssetBundleData assetBundleData = m_Database.FindBundleByAssetName(assetName);
			AssetBundleCache assetBundleCache;
			if (assetBundleData != null && m_AssetBundles.TryGetValue(assetBundleData.Name, out assetBundleCache))
			{
				AssetRequest assetRequest = new AssetRequest(this, priority, assetBundleCache, new AssetKey(assetName, assetType), callback);
				assetRequest.Tag = userToken;
				m_RequestTasks.Add(assetRequest);
				m_SortRequestFlag = true;
				m_CheckTaskFlag = true;
				return assetRequest;
			}
			return null;
		}

		public AssetRequest RequestAsset<T>(String assetName, Int32 priority, AssetRequestCallback callback, Object userToken) where T : UnityEngine.Object
		{
			return RequestAsset(assetName, typeof(T), priority, callback, userToken);
		}

		public AssetRequest RequestAsset(String assetName, Int32 priority, AssetRequestCallback callback, Object userToken)
		{
			return RequestAsset(assetName, typeof(UnityEngine.Object), priority, callback, userToken);
		}

		public AssetRequest RequestAsset(String assetName, Type assetType, Int32 priority)
		{
			return RequestAsset(assetName, assetType, priority, null, null);
		}

		public AssetRequest RequestAsset<T>(String assetName, Int32 priority) where T : UnityEngine.Object
		{
			return RequestAsset(assetName, typeof(T), priority, null, null);
		}

		public AssetRequest RequestAsset(String assetName, Int32 priority)
		{
			return RequestAsset(assetName, typeof(UnityEngine.Object), priority, null, null);
		}

		public AssetRequest RequestAsset(String assetName)
		{
			return RequestAsset(assetName, typeof(UnityEngine.Object), 0, null, null);
		}

		public AssetBundleRequest RequestAssetBundle(String bundleName, Int32 priority, AssetBundleRequestCallback callback, Object userToken)
		{
			if (String.IsNullOrEmpty(bundleName))
			{
				throw new ArgumentException("value is null or empty", "bundleName");
			}
			if (m_Database == null || m_AssetBundles == null)
			{
				throw new InvalidOperationException("Database not loaded!");
			}
			AssetBundleData assetBundleData = m_Database.FindBundleByBundleName(bundleName);
			AssetBundleCache assetBundleCache;
			if (assetBundleData != null && m_AssetBundles.TryGetValue(assetBundleData.Name, out assetBundleCache))
			{
				AssetBundleRequest assetBundleRequest = new AssetBundleRequest(this, priority, assetBundleCache, callback);
				assetBundleRequest.Tag = userToken;
				m_RequestTasks.Add(assetBundleRequest);
				m_SortRequestFlag = true;
				m_CheckTaskFlag = true;
				return assetBundleRequest;
			}
			return null;
		}

		public AssetBundleRequest RequestAssetBundle(String bundleName, Int32 priority)
		{
			return RequestAssetBundle(bundleName, priority, null, null);
		}

		public AssetBundleRequest RequestAssetBundle(String bundleName)
		{
			return RequestAssetBundle(bundleName, 0, null, null);
		}

		public SceneRequest RequestScene(String sceneName, Int32 priority, SceneRequestCallback callback, Object userToken)
		{
			if (String.IsNullOrEmpty(sceneName))
			{
				throw new ArgumentException("value is null or empty", "sceneName");
			}
			if (m_Database == null || m_AssetBundles == null)
			{
				throw new InvalidOperationException("Database not loaded!");
			}
			AssetBundleData assetBundleData = m_Database.FindBundleByAssetName(sceneName);
			AssetBundleCache assetBundleCache;
			if (assetBundleData != null && m_AssetBundles.TryGetValue(assetBundleData.Name, out assetBundleCache))
			{
				SceneRequest sceneRequest = new SceneRequest(this, priority, assetBundleCache, new AssetKey(sceneName, typeof(UnityEngine.Object)), callback);
				sceneRequest.Tag = userToken;
				m_RequestTasks.Add(sceneRequest);
				m_SortRequestFlag = true;
				m_CheckTaskFlag = true;
				return sceneRequest;
			}
			return null;
		}

		public SceneRequest RequestScene(String sceneName, Int32 priority)
		{
			return RequestScene(sceneName, priority, null, null);
		}

		public SceneRequest RequestScene(String sceneName)
		{
			return RequestScene(sceneName, 0, null, null);
		}

		public String FindBundleNameByAssetName(String assetName)
		{
			if (String.IsNullOrEmpty(assetName))
			{
				throw new ArgumentException("value is null or empty", "assetName");
			}
			if (m_Database == null)
			{
				throw new InvalidOperationException("Database not loaded!");
			}
			AssetBundleData assetBundleData = m_Database.FindBundleByAssetName(assetName);
			return (assetBundleData == null) ? null : assetBundleData.Name;
		}

		public void UnloadManager(Boolean unloadAllLoadedObjects)
		{
			m_AssetBundleLoader.Abort();
			m_AssetAsyncTasks.Clear();
			foreach (Request request in m_RequestTasks)
			{
				request.Cancel();
			}
			m_CheckTaskFlag = true;
			if (m_AssetBundles != null)
			{
				foreach (AssetBundleCache assetBundleCache in m_AssetBundles.Values)
				{
					assetBundleCache.Unload(unloadAllLoadedObjects);
				}
			}
		}

		public Boolean UnloadAssetBundle(String bundleName, Boolean unloadAllLoadedObjectsInMainBundle, Boolean unloadSharedBundle, Boolean unloadAllLoadedObjectsInSharedBundle)
		{
			if (String.IsNullOrEmpty(bundleName))
			{
				throw new ArgumentException("value is null or empty", "bundleName");
			}
			if (m_Database == null || m_AssetBundles == null)
			{
				return true;
			}
			AssetBundleData assetBundleData = m_Database.FindBundleByBundleName(bundleName);
			AssetBundleCache assetBundleCache;
			if (assetBundleData != null && m_AssetBundles.TryGetValue(assetBundleData.Name, out assetBundleCache))
			{
				AssetBundleData[] bundleDependency = assetBundleCache.Data.BundleDependency;
				if (bundleDependency != null && unloadSharedBundle)
				{
					foreach (AssetBundleData assetBundleData2 in bundleDependency)
					{
						AssetBundleCache assetBundleCache2;
						if (m_AssetBundles.TryGetValue(assetBundleData2.Name, out assetBundleCache2))
						{
							assetBundleCache2.Unload(unloadAllLoadedObjectsInSharedBundle);
						}
					}
				}
				assetBundleCache.Unload(unloadAllLoadedObjectsInMainBundle);
				return true;
			}
			return false;
		}

		public Boolean UnloadAssetBundleByAssetName(String assetName, Boolean unloadAllLoadedObjectsInMainBundle, Boolean unloadSharedBundle, Boolean unloadAllLoadedObjectsInSharedBundle)
		{
			if (String.IsNullOrEmpty(assetName))
			{
				throw new ArgumentException("value is null or empty", "assetName");
			}
			if (m_Database == null)
			{
				return true;
			}
			AssetBundleData assetBundleData = m_Database.FindBundleByAssetName(assetName);
			return assetBundleData != null && UnloadAssetBundle(assetBundleData.Name, unloadAllLoadedObjectsInMainBundle, unloadSharedBundle, unloadAllLoadedObjectsInSharedBundle);
		}

		public Boolean LoadDatabase(TextAsset dbFile)
		{
			try
			{
				DatabaseDeserializer databaseDeserializer = new DatabaseDeserializer();
				m_Database = databaseDeserializer.Deserialize(dbFile);
				InitDatabase();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return true;
			}
			return false;
		}

		public Boolean LoadDatabase(String dbFilePath)
		{
			try
			{
				DatabaseDeserializer databaseDeserializer = new DatabaseDeserializer();
				m_Database = databaseDeserializer.Deserialize(dbFilePath);
				InitDatabase();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return true;
			}
			return false;
		}

		protected virtual void Awake()
		{
			m_AssetBundleLoader = (m_AssetBundleLoader ?? GetComponent<AssetBundleLoader>());
			if (m_AssetBundleLoader == null)
			{
				Debug.LogError("A AssetBundleLoader no found in the GameObject.", this);
				return;
			}
			m_AssetBundleLoader.LoadCompleted += AssetBundleLoader_LoadCompleted;
			if (m_AssetBundleDatabase != null)
			{
				LoadDatabase(m_AssetBundleDatabase);
			}
		}

		protected virtual void Update()
		{
			if (m_AssetAsyncTasks.Count > 0)
			{
				for (Int32 i = m_AssetAsyncTasks.Count - 1; i >= 0; i--)
				{
					if (m_AssetAsyncTasks[i].Update())
					{
						m_AssetAsyncTasks.RemoveAt(i);
						m_CheckTaskFlag = true;
					}
				}
			}
			if (m_CheckTaskFlag)
			{
				m_CheckTaskFlag = false;
				UpdateRequestTasks();
			}
		}

		protected virtual void OnDestroy()
		{
			if (m_AssetBundleLoader != null)
			{
				m_AssetBundleLoader.LoadCompleted += AssetBundleLoader_LoadCompleted;
			}
			UnloadManager(true);
		}

		private void InitDatabase()
		{
			if (m_Database != null)
			{
				UnloadManager(true);
				m_AssetBundles = new Dictionary<String, AssetBundleCache>(m_Database.AssetBundleCount, StringComparer.InvariantCultureIgnoreCase);
				foreach (AssetBundleData assetBundleData in m_Database)
				{
					AssetBundleCache value = new AssetBundleCache(assetBundleData);
					m_AssetBundles.Add(assetBundleData.Name, value);
				}
				foreach (AssetBundleCache assetBundleCache in m_AssetBundles.Values)
				{
					AssetBundleData[] bundleDependency = assetBundleCache.Data.BundleDependency;
					if (bundleDependency != null)
					{
						for (Int32 i = 0; i < bundleDependency.Length; i++)
						{
							String name = bundleDependency[i].Name;
							m_AssetBundles.TryGetValue(name, out assetBundleCache.Dependencies[i]);
						}
					}
				}
			}
		}

		internal Boolean CheckAndLoadBundles(AssetBundleCache bundle, Int32 priority)
		{
			Boolean flag = true;
			AssetBundleCache[] dependencies = bundle.Dependencies;
			if (dependencies != null)
			{
				foreach (AssetBundleCache assetBundleCache in dependencies)
				{
					flag &= (assetBundleCache.Status >= EStatus.Ready);
					if (assetBundleCache.Status == EStatus.None)
					{
						assetBundleCache.UpdateStatusLoad();
						LoadAssetBundle(assetBundleCache, priority);
					}
				}
			}
			if (flag)
			{
				flag = (bundle.Status >= EStatus.Ready);
				if (bundle.Status == EStatus.None)
				{
					bundle.UpdateStatusLoad();
					LoadAssetBundle(bundle, priority);
				}
			}
			return flag;
		}

		private void LoadAssetBundle(AssetBundleCache bundle, Int32 priority)
		{
			if (m_AssetBundleLoader == null)
			{
				Debug.LogError("Lost reference");
				return;
			}
			m_AssetBundleLoader.Load(bundle.Data.Path, priority, bundle.Data.Size, bundle.Data.Version, bundle.Data.CrcValue, bundle);
		}

		private void UpdateRequestTasks()
		{
			if (m_SortRequestFlag)
			{
				m_SortRequestFlag = false;
				m_RequestTasks.Sort(RequestTaskComparer.Default);
			}
			for (Int32 i = m_RequestTasks.Count - 1; i >= 0; i--)
			{
				Request request = m_RequestTasks[i];
				request.Update();
				if (request.IsDone)
				{
					m_RequestTasks.RemoveAt(i);
				}
			}
		}

		private void AssetBundleLoader_LoadCompleted(Object sender, LoadCompletedEventArgs e)
		{
			AssetBundleCache assetBundleCache = e.UserToken as AssetBundleCache;
			if (assetBundleCache != null && m_AssetBundles != null && m_AssetBundles.ContainsValue(assetBundleCache))
			{
				if (e.Cancelled)
				{
					assetBundleCache.UpdateStatus("Loading cancelled.");
				}
				else if (e.IsError)
				{
					assetBundleCache.UpdateStatus("Error: " + e.Error);
				}
				else
				{
					assetBundleCache.UpdateStatus(e.Request.GetAssetBundle());
				}
				UpdateRequestTasks();
			}
		}

		private class RequestTaskComparer : IComparer<Request>
		{
			public static RequestTaskComparer Default = new RequestTaskComparer();

			public Int32 Compare(Request x, Request y)
			{
				return y.Priority.CompareTo(x.Priority);
			}
		}
	}
}
