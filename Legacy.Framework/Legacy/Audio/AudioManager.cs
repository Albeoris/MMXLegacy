using System;
using System.Collections.Generic;
using AssetBundles.Core;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using UnityEngine;

namespace Legacy.Audio
{
	public class AudioManager : MonoBehaviour
	{
		private static AudioManager s_Instance;

		private Boolean m_mainControllerLoadError;

		private AudioController m_mainController;

		private Dictionary<String, AudioController> m_additionalControllers = new Dictionary<String, AudioController>();

		private Dictionary<String, Int32> m_controllerRefCountMap = new Dictionary<String, Int32>();

		private Int32 m_nextCheck;

		[SerializeField]
		private Boolean m_LoadFromLocalResource;

		[SerializeField]
		private String m_MainControllerName;

		[SerializeField]
		private AudioCategoryMap m_CategoryMap;

		public static Boolean ExistInstance => s_Instance != null;

	    public static AudioManager Instance => s_Instance;

	    public Boolean MainControllerIsLoaded => m_mainController != null || m_mainControllerLoadError;

	    public void RequestMainController()
		{
			if (m_mainController != null)
			{
				DestroyImmediate(m_mainController.gameObject);
			}
			m_mainController = null;
			m_mainControllerLoadError = false;
			m_additionalControllers.Remove(m_MainControllerName);
			Request(m_MainControllerName, 9999, false, null);
		}

		public AudioRequest RequestByAudioID(String audioID)
		{
			return RequestByAudioID(audioID, 0, false, null);
		}

		public AudioRequest RequestByAudioID(String audioID, Int32 priority)
		{
			return RequestByAudioID(audioID, priority, false, null);
		}

		public AudioRequest RequestByAudioID(String audioID, Int32 priority, Boolean volumeSetManually)
		{
			return RequestByAudioID(audioID, priority, volumeSetManually, null);
		}

		public AudioRequest RequestByAudioID(String audioID, Int32 priority, AudioControllerCallback finishCallback)
		{
			return RequestByAudioID(audioID, priority, false, finishCallback);
		}

		public AudioRequest RequestByAudioID(String audioID, Int32 priority, Boolean volumeSetManually, AudioControllerCallback finishCallback)
		{
			return Request(m_CategoryMap.FindControllerName(audioID), priority, volumeSetManually, finishCallback);
		}

		public AudioRequest Request(String categoryName)
		{
			return Request(categoryName, 0, false, null);
		}

		public AudioRequest Request(String categoryName, Int32 priority)
		{
			return Request(categoryName, priority, false, null);
		}

		public AudioRequest Request(String categoryName, Int32 priority, Boolean volumeSetManually)
		{
			return Request(categoryName, priority, volumeSetManually, null);
		}

		public AudioRequest Request(String categoryName, Int32 priority, AudioControllerCallback finishCallback)
		{
			return Request(categoryName, priority, false, finishCallback);
		}

		public AudioRequest Request(String categoryName, Int32 priority, Boolean volumeSetManually, AudioControllerCallback finishCallback)
		{
			AudioRequest audioRequest = new AudioRequest(this, categoryName, priority, volumeSetManually);
			if (categoryName == null)
			{
				ExecuteCallback(ref finishCallback, audioRequest);
				return audioRequest;
			}
			AudioController audioController;
			if (m_additionalControllers.TryGetValue(categoryName, out audioController) && audioController != null)
			{
				audioRequest._UpdateReady(audioController);
				ExecuteCallback(ref finishCallback, audioRequest);
				return audioRequest;
			}
			RequestData userToken = new RequestData
			{
				FinishCallback = finishCallback,
				UserRequest = audioRequest
			};
			AssetRequest assetRequest = null;
			if (CheckLoad(categoryName))
			{
				if (LegacyLogic.Instance.ModController.InModMode)
				{
					assetRequest = AssetBundleManagers.Instance.Mod.RequestAsset(categoryName, priority, new AssetRequestCallback(OnAudioBundleLoaded), userToken);
				}
				if (assetRequest == null)
				{
					assetRequest = AssetBundleManagers.Instance.Main.RequestAsset(categoryName, priority, new AssetRequestCallback(OnAudioBundleLoaded), userToken);
				}
			}
			else
			{
				Debug.Log("skip load audio category " + categoryName);
			}
			if (assetRequest == null)
			{
				Debug.LogError("Error load unknow assetbundle AudioController name?\n" + categoryName);
				ExecuteCallback(ref finishCallback, audioRequest);
			}
			return audioRequest;
		}

		private Boolean CheckLoad(String categoryName)
		{
			AudioCategoryMap.ECategoryTag controllerTag = m_CategoryMap.GetControllerTag(categoryName);
			return controllerTag == AudioCategoryMap.ECategoryTag.None || (ConfigManager.Instance.Options.TriggerBarks && controllerTag == AudioCategoryMap.ECategoryTag.Bark) || (SoundConfigManager.Settings.EnableAmbientSounds && controllerTag == AudioCategoryMap.ECategoryTag.Ambient) || (SoundConfigManager.Settings.EnableMonsterSounds && controllerTag == AudioCategoryMap.ECategoryTag.Monster);
		}

		public Boolean UnloadByAudioID(String audioID)
		{
			return audioID != null && Unload(m_CategoryMap.FindControllerName(audioID));
		}

		public Boolean Unload(String categoryName)
		{
			if (categoryName == m_MainControllerName)
			{
				Debug.LogError("Skip unload! Try to unload the main audio controller!");
				return false;
			}
			AudioController audioController;
			if (categoryName != null && m_additionalControllers.TryGetValue(categoryName, out audioController))
			{
				m_additionalControllers.Remove(categoryName);
				m_controllerRefCountMap.Remove(categoryName);
				if (audioController != null)
				{
					DestroyImmediate(audioController.gameObject);
				}
				UnloadAudioBundle(categoryName, true);
				return true;
			}
			return false;
		}

		public Boolean IsCategoryLoaded(String categoryName)
		{
			return FindCategory(categoryName) != null;
		}

		public Boolean IsAudioIDLoaded(String audioID)
		{
			if (audioID != null)
			{
				String text = m_CategoryMap.FindControllerName(audioID);
				return text == null || FindCategory(text) != null;
			}
			return false;
		}

		public AudioController FindCategory(String categoryName)
		{
			AudioController audioController;
			if (categoryName == null || (m_additionalControllers.TryGetValue(categoryName, out audioController) && audioController == null))
			{
				return null;
			}
			return audioController;
		}

		public Boolean IsValidCategory(String categoryName)
		{
			return categoryName != null && m_CategoryMap.HasControllerName(categoryName);
		}

		public Boolean IsValidAudioID(String audioID)
		{
			return audioID != null && m_CategoryMap.HasAudioID(audioID);
		}

		public String FindCategoryNameByAudioID(String audioID)
		{
			return m_CategoryMap.FindControllerName(audioID);
		}

		public Boolean InAudioRange(Vector3 worldPosition, String audioID)
		{
			if (audioID != null)
			{
				Int32 num = m_CategoryMap.FindAudioRange(audioID);
				return num == -1 || num >= Vector3.Distance(worldPosition, AudioController.GetCurrentAudioListenerTransform().position);
			}
			return false;
		}

		public Boolean AddRefController(String categoryName)
		{
			return RefCountController(categoryName, 1);
		}

		public Boolean ReleaseRefController(String categoryName)
		{
			return RefCountController(categoryName, -1);
		}

		private Boolean RefCountController(String categoryName, Int32 mod)
		{
			if (IsValidCategory(categoryName))
			{
				Int32 num;
				m_controllerRefCountMap.TryGetValue(categoryName, out num);
				num += mod;
				m_controllerRefCountMap[categoryName] = ((num >= 0) ? num : 0);
				return true;
			}
			return false;
		}

		public void RequestPlayAudioID(String audioID, Int32 requestPriority)
		{
			AudioListener currentAudioListener = AudioController.GetCurrentAudioListener();
			if (currentAudioListener == null)
			{
				Debug.LogWarning("No AudioListener found in the scene");
				return;
			}
			Transform transform = currentAudioListener.transform;
			RequestPlayAudioID(audioID, requestPriority, -1f, transform.position + transform.forward, null, 1f, 0f, 0f, null);
		}

		public void RequestPlayAudioID(String audioID, Int32 requestPriority, Single maxRequestDelay, PlayAudioCallback startPlayCallback)
		{
			AudioListener currentAudioListener = AudioController.GetCurrentAudioListener();
			if (currentAudioListener == null)
			{
				Debug.LogWarning("No AudioListener found in the scene");
				return;
			}
			Transform transform = currentAudioListener.transform;
			RequestPlayAudioID(audioID, requestPriority, maxRequestDelay, transform.position + transform.forward, null, 1f, 0f, 0f, startPlayCallback);
		}

		public void RequestPlayAudioID(String audioID, Int32 requestPriority, Single maxRequestDelay = -1f, Transform parentObj = null, Single volume = 1f, Single delay = 0f, Single startTime = 0f, PlayAudioCallback startPlayCallback = null)
		{
			Vector3 worldPosition = (!(parentObj == null)) ? parentObj.position : default(Vector3);
			RequestPlayAudioID(audioID, requestPriority, maxRequestDelay, worldPosition, parentObj, volume, delay, startTime, startPlayCallback);
		}

		public void RequestPlayAudioID(String audioID, Int32 requestPriority, Single maxRequestDelay = -1f, Vector3 worldPosition = default(Vector3), Transform parentObj = null, Single volume = 1f, Single delay = 0f, Single startTime = 0f, PlayAudioCallback startPlayCallback = null)
		{
			if (audioID == null)
			{
				ExecuteCallback(ref startPlayCallback, null);
				return;
			}
			String text = m_CategoryMap.FindControllerName(audioID);
			if (text == null)
			{
				Debug.LogError("Unknow AudioID: " + audioID);
				return;
			}
			if (FindCategory(text) != null)
			{
				AudioObject audioItem = AudioController.Play(audioID, worldPosition, parentObj, volume, delay, startTime);
				ExecuteCallback(ref startPlayCallback, audioItem);
				return;
			}
			AudioPlayRequest @object = new AudioPlayRequest
			{
				RequestStartTime = Time.time,
				MaxRequestDelay = maxRequestDelay,
				AudioID = audioID,
				WorldPosition = worldPosition,
				ParentObj = parentObj,
				Volume = volume,
				Delay = delay,
				StartTime = startTime,
				StartPlayCallback = startPlayCallback
			};
			Request(text, requestPriority, new AudioControllerCallback(@object.DoneLoad));
		}

		private void Awake()
		{
			if (s_Instance == null || s_Instance == this)
			{
				s_Instance = this;
				DontDestroyOnLoad(gameObject);
				return;
			}
			Debug.LogError("There are multiple " + GetType() + " in the scene!", gameObject);
			enabled = false;
			Destroy(gameObject);
		}

		private void Update()
		{
			m_nextCheck--;
			if (m_nextCheck < 0)
			{
				m_nextCheck = 100;
				IL_22:
				while (m_controllerRefCountMap.Count > 0)
				{
					foreach (KeyValuePair<String, Int32> keyValuePair in m_controllerRefCountMap)
					{
						if (keyValuePair.Value <= 0)
						{
							if (!Unload(keyValuePair.Key))
							{
								m_controllerRefCountMap.Remove(keyValuePair.Key);
							}
							goto IL_22;
						}
					}
					break;
				}
			}
		}

		private void OnLevelWasLoaded(Int32 level)
		{
			List<String> list = new List<String>();
			foreach (KeyValuePair<String, AudioController> keyValuePair in m_additionalControllers)
			{
				if (keyValuePair.Value == null)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (String key in list)
			{
				m_additionalControllers.Remove(key);
				m_controllerRefCountMap.Remove(key);
			}
		}

		private void OnAudioBundleLoaded(AssetRequest request)
		{
			RequestData requestData = (RequestData)request.Tag;
			request.Tag = null;
			if (request.Status == ERequestStatus.Error)
			{
				if (request.Name == m_MainControllerName)
				{
					m_mainControllerLoadError = true;
				}
				Debug.LogError("Error load assetbundle AudioController\n" + request.Name);
				ExecuteCallback(ref requestData.FinishCallback, requestData.UserRequest);
				return;
			}
			GameObject gameObject = request.Asset as GameObject;
			if (gameObject == null)
			{
				if (request.Name == m_MainControllerName)
				{
					m_mainControllerLoadError = true;
				}
				Debug.LogError("Error load assetbundle AudioController, is not a GameObject\n" + request.Name);
				ExecuteCallback(ref requestData.FinishCallback, requestData.UserRequest);
				return;
			}
			if (requestData.UserRequest.State == EAudioRequestState.Abort)
			{
				ExecuteCallback(ref requestData.FinishCallback, requestData.UserRequest);
				return;
			}
			LoadController(gameObject, requestData.UserRequest, requestData.FinishCallback);
		}

		private void LoadController(GameObject prefab, AudioRequest request, AudioControllerCallback finishCallback)
		{
			AudioController component;
			if (m_additionalControllers.TryGetValue(request.CategoryName, out component) && component != null)
			{
				request._UpdateReady(component);
				if (request.CategoryName == m_MainControllerName)
				{
					m_mainController = component;
				}
				ExecuteCallback(ref finishCallback, request);
				return;
			}
			component = prefab.GetComponent<AudioController>();
			if (component == null)
			{
				if (request.CategoryName == m_MainControllerName)
				{
					m_mainControllerLoadError = true;
				}
				Debug.LogError("AudioManager: GameObject has no AudioController script attached!\n" + request.CategoryName);
				ExecuteCallback(ref finishCallback, request);
				return;
			}
			if (component.isSingletonObject && request.CategoryName != m_MainControllerName)
			{
				if (request.CategoryName == m_MainControllerName)
				{
					m_mainControllerLoadError = true;
				}
				Debug.LogError("AudioManager: AudioController is not set to be an additional AuidoController!\n" + request.CategoryName);
				ExecuteCallback(ref finishCallback, request);
				return;
			}
			if (component.Persistent && request.CategoryName != m_MainControllerName)
			{
				Debug.LogWarning("AudioManager: AudioController is set persistent, MEMORY LEAK?\n" + request.CategoryName);
			}
			GameObject gameObject = Helper.Instantiate<GameObject>(prefab);
			component = gameObject.GetComponent<AudioController>();
			request._UpdateReady(component);
			if (request.CategoryName == m_MainControllerName)
			{
				m_mainController = component;
			}
			if (!request.IsVolumeSetManually)
			{
				AudioHelper.SetVolumeForCtrl(SoundConfigManager.Settings.SFXVolume, SoundConfigManager.Settings.PartyBarkVolume, component);
			}
			m_additionalControllers[request.CategoryName] = component;
			ExecuteCallback(ref finishCallback, request);
			UnloadAudioBundle(request.CategoryName, false);
		}

		private void UnloadAudioBundle(String categoryName, Boolean unloadAllLoadedObjects)
		{
			if (AssetBundleManagers.HasInstance)
			{
				AssetBundleManagers.Instance.Main.UnloadAssetBundle(categoryName, unloadAllLoadedObjects, true, unloadAllLoadedObjects);
				AssetBundleManagers.Instance.Mod.UnloadAssetBundle(categoryName, unloadAllLoadedObjects, true, unloadAllLoadedObjects);
			}
		}

		private static void ExecuteCallback(ref AudioControllerCallback finishCallback, AudioRequest request)
		{
			if (finishCallback != null)
			{
				try
				{
					finishCallback(request);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				finishCallback = null;
			}
		}

		private static void ExecuteCallback(ref PlayAudioCallback startPlayCallback, AudioObject audioItem)
		{
			try
			{
				if (startPlayCallback != null)
				{
					startPlayCallback(audioItem);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			startPlayCallback = null;
		}

		private class AudioPlayRequest
		{
			public Single MaxRequestDelay;

			public Single RequestStartTime;

			public String AudioID;

			public Vector3 WorldPosition;

			public Transform ParentObj;

			public Single Volume;

			public Single Delay;

			public Single StartTime;

			public PlayAudioCallback StartPlayCallback;

			public void DoneLoad(AudioRequest request)
			{
				if (MaxRequestDelay != -1f && Time.time - RequestStartTime > MaxRequestDelay)
				{
					return;
				}
				AudioObject audioItem = AudioController.Play(AudioID, WorldPosition, ParentObj, Volume, Delay, StartTime);
				ExecuteCallback(ref StartPlayCallback, audioItem);
			}
		}

		private class RequestData
		{
			public AudioRequest UserRequest;

			public AudioControllerCallback FinishCallback;
		}
	}
}
