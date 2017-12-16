using System;
using AssetBundles.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Loading
{
	public class SceneLoader : MonoBehaviour
	{
		private ELoadingState m_LoadState;

		private AsyncOperation m_CurrentOperation;

		private String m_LastTargetScene;

		private String m_TargetScene;

		private Single m_DebugLoadTime;

		public static SceneLoader Instance { get; set; }

		public Single LoadProgress { get; private set; }

		public String TargetScene => m_TargetScene;

	    public Boolean IsDone { get; private set; }

		private void Awake()
		{
			Instance = this;
			IsDone = true;
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnLoadScene));
			enabled = false;
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnLoadScene));
		}

		private void Update()
		{
			if (m_CurrentOperation != null)
			{
				if (!m_CurrentOperation.isDone)
				{
					AsyncOperationProcess();
					return;
				}
				AsyncOperationDone();
				m_CurrentOperation = null;
				NextLoadState();
			}
			switch (m_LoadState)
			{
			case ELoadingState.Initiated:
				NextLoadState();
				break;
			case ELoadingState.LoadEmptyScene:
				Application.LoadLevel("Empty");
				NextLoadState();
				break;
			case ELoadingState.Skip:
				if (!String.IsNullOrEmpty(m_LastTargetScene))
				{
					AssetBundleManagers.Instance.Main.UnloadAssetBundleByAssetName(m_LastTargetScene, true, false, true);
					AssetBundleManagers.Instance.Mod.UnloadAssetBundleByAssetName(m_LastTargetScene, true, false, true);
				}
				AssetBundleManagers.Instance.Main.UnloadManager(false);
				AssetBundleManagers.Instance.Mod.UnloadManager(false);
				NextLoadState();
				break;
			case ELoadingState.GarbageCollecting:
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				NextLoadState();
				break;
			case ELoadingState.UnloadUnusedAssets:
				m_CurrentOperation = Resources.UnloadUnusedAssets();
				break;
			case ELoadingState.LoadTargetSceneAssetBundle:
			{
				SceneRequest sceneRequest = null;
				if (LegacyLogic.Instance.ModController.InModMode)
				{
					sceneRequest = AssetBundleManagers.Instance.Mod.RequestScene(m_TargetScene, 0, new SceneRequestCallback(OnSceneBundleLoaded), null);
				}
				if (sceneRequest == null)
				{
					sceneRequest = AssetBundleManagers.Instance.Main.RequestScene(m_TargetScene, 0, new SceneRequestCallback(OnSceneBundleLoaded), null);
				}
				if (sceneRequest == null)
				{
					Debug.LogError("Scene not in a assetbundle defined! \n" + m_TargetScene);
					m_LoadState = ELoadingState.UnloadBundles;
				}
				else
				{
					NextLoadState();
				}
				break;
			}
			case ELoadingState.LoadTargetScene:
				m_CurrentOperation = Application.LoadLevelAsync(m_TargetScene);
				if (m_CurrentOperation == null)
				{
					Debug.LogError("Error load scene! \n" + m_TargetScene);
					NextLoadState();
				}
				break;
			case ELoadingState.Skip2:
				NextLoadState();
				break;
			case ELoadingState.UnloadUnusedAssets2:
				NextLoadState();
				break;
			case ELoadingState.UnloadBundles:
			{
				Camera[] array = (Camera[])FindObjectsOfType(typeof(Camera));
				for (Int32 i = 0; i < array.Length; i++)
				{
					if (!array[i].CompareTag("UICamera"))
					{
						array[i].enabled = false;
					}
				}
				GameObject gameObject = new GameObject();
				Camera camera = gameObject.AddComponent<Camera>();
				camera.fieldOfView = 180f;
				camera.nearClipPlane = 0f;
				camera.farClipPlane = 10000f;
				camera.useOcclusionCulling = false;
				RenderTexture temporary = RenderTexture.GetTemporary(1, 1, 0);
				temporary.isCubemap = true;
				camera.RenderToCubemap(temporary);
				temporary.Release();
				RenderTexture.ReleaseTemporary(temporary);
				Destroy(gameObject);
				m_CurrentOperation = Resources.UnloadUnusedAssets();
				break;
			}
			case ELoadingState.SceneLoaded:
				NextLoadState();
				SceneLoadFinish();
				break;
			}
			LegacyLogic.Instance.MapLoader.SceneLoaderProgress = (1f - (ELoadingState.Finish - m_LoadState) / 12f) * 0.5f;
		}

		private void AsyncOperationDone()
		{
			LoadProgress = 1f;
		}

		private void AsyncOperationProcess()
		{
			if (m_LoadState == ELoadingState.LoadTargetScene)
			{
				LoadProgress = m_CurrentOperation.progress;
			}
		}

		private void SceneLoadFinish()
		{
			IsDone = true;
			LoadProgress = 1f;
			enabled = false;
			LegacyLogic.Instance.MapLoader.FinishedLoadScene();
			Debug.Log(String.Concat(new Object[]
			{
				"Finish load scene: '",
				m_TargetScene,
				"' Time: ",
				TimeSpan.FromSeconds(Time.realtimeSinceStartup - m_DebugLoadTime)
			}));
		}

		private void OnLoadScene(Object p_sender, EventArgs p_args)
		{
			StartSceneLoadEventArgs startSceneLoadEventArgs = (StartSceneLoadEventArgs)p_args;
			if (String.IsNullOrEmpty(startSceneLoadEventArgs.SceneName))
			{
				throw new ArgumentNullException("p_sceneName");
			}
			Debug.Log("Start load scene: '" + startSceneLoadEventArgs.SceneName + "'");
			m_DebugLoadTime = Time.realtimeSinceStartup;
			m_LastTargetScene = m_TargetScene;
			m_TargetScene = startSceneLoadEventArgs.SceneName;
			LoadProgress = 0f;
			IsDone = false;
			enabled = true;
			m_LoadState = ELoadingState.Initiated;
		}

		private void OnSceneBundleLoaded(SceneRequest p_args)
		{
			NextLoadState();
		}

		private void NextLoadState()
		{
			m_LoadState++;
		}

		public enum ELoadingState
		{
			Initiated,
			LoadEmptyScene,
			Skip,
			GarbageCollecting,
			UnloadUnusedAssets,
			LoadTargetSceneAssetBundle,
			WaitSceneAssetBundle,
			LoadTargetScene,
			Skip2,
			UnloadUnusedAssets2,
			UnloadBundles,
			SceneLoaded,
			Finish
		}
	}
}
