using System;
using System.Collections.Generic;
using System.Threading;
using AssetBundles.Core;
using Legacy.Core.Api;
using Legacy.NpcInteraction;
using UnityEngine;

namespace Legacy.Loading
{
	internal class IndoorSceneLoader : MonoBehaviour
	{
		public const Int32 MAX_CACHED_SCENES = 1;

		private String m_lastRequest;

		private List<IndoorCache> m_indoorScenes = new List<IndoorCache>(1);

		private AsyncOperation m_loads;

	    public event EventHandler<FinishLoadIndoorSceneEventArgs> FinishLoadIndoorScene;

		public static IndoorSceneLoader Instance { get; private set; }

		public String LoadingScene { get; private set; }

		public Single LoadingProgress => (m_loads == null) ? 0f : m_loads.progress;

	    public void RequestIndoorScene(String sceneName)
		{
			m_lastRequest = sceneName;
			if (LoadingScene != null)
			{
				return;
			}
			Int32 num = m_indoorScenes.FindIndex((IndoorCache a) => a.Key == sceneName);
			IndoorCache indoorCache = default(IndoorCache);
			if (num >= 0)
			{
				indoorCache = m_indoorScenes[num];
			}
			else if (m_indoorScenes.Count >= 1)
			{
				IndoorSceneRoot rootScene = m_indoorScenes[0].RootScene;
				Helper.DestroyGO<IndoorSceneRoot>(ref rootScene);
				Resources.UnloadUnusedAssets();
				m_indoorScenes.RemoveAt(0);
			}
			if (!indoorCache.IsLoading && indoorCache.RootScene == null)
			{
				if (num >= 0)
				{
					m_indoorScenes[num] = new IndoorCache(sceneName, null, true);
				}
				else
				{
					m_indoorScenes.Add(new IndoorCache(sceneName, null, true));
				}
				SceneRequest sceneRequest = null;
				if (LegacyLogic.Instance.ModController.InModMode)
				{
					sceneRequest = AssetBundleManagers.Instance.Mod.RequestScene(sceneName, 10, new SceneRequestCallback(OnSceneBundleLoaded), null);
				}
				if (sceneRequest == null)
				{
					sceneRequest = AssetBundleManagers.Instance.Main.RequestScene(sceneName, 10, new SceneRequestCallback(OnSceneBundleLoaded), null);
				}
				if (sceneRequest == null)
				{
					Debug.LogError("Fail load indoor scene '" + sceneName + "'");
				}
				LoadingScene = sceneName;
			}
			else if (FinishLoadIndoorScene != null && indoorCache.RootScene != null)
			{
				FinishLoadIndoorScene(this, new FinishLoadIndoorSceneEventArgs(indoorCache.RootScene));
			}
		}

		public void CancelRequestIndoorScene(String sceneName)
		{
			m_lastRequest = null;
		}

		internal void IndoorSceneLoaded(IndoorSceneRoot root)
		{
			Int32 num = m_indoorScenes.FindIndex((IndoorCache a) => a.Key == root.SceneName);
			if (num == -1)
			{
				String text = "IndoorScene name is invalid '" + root.SceneName + "'";
				foreach (IndoorCache indoorCache in m_indoorScenes)
				{
					if (indoorCache.IsLoading)
					{
						text = text + "\n expected '" + indoorCache.Key + "' ?";
					}
				}
				Debug.LogError(text);
				Destroy(root.gameObject);
				return;
			}
			root.transform.position = new Vector3(0f, -2000f, 0f);
			root.gameObject.SetActive(false);
			m_indoorScenes[num] = new IndoorCache(root.SceneName, root, false);
			if (m_lastRequest == root.SceneName)
			{
				LoadingScene = null;
				m_loads = null;
				if (FinishLoadIndoorScene != null)
				{
					FinishLoadIndoorScene(this, new FinishLoadIndoorSceneEventArgs(root));
				}
			}
			else
			{
				RequestIndoorScene(m_lastRequest);
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		private void OnSceneBundleLoaded(SceneRequest p_args)
		{
			m_loads = Application.LoadLevelAdditiveAsync(p_args.SceneName);
		}

		private struct IndoorCache
		{
			public String Key;

			public IndoorSceneRoot RootScene;

			public Boolean IsLoading;

			public IndoorCache(String p_key, IndoorSceneRoot p_root, Boolean p_isLoading)
			{
				Key = p_key;
				RootScene = p_root;
				IsLoading = p_isLoading;
			}
		}
	}
}
