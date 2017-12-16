using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace AssetBundles.Core
{
	public class AssetBundleManagerDebugGUI : MonoBehaviour
	{
		private AssetBundleManager m_Manager;

		private void OnEnable()
		{
			m_Manager = GetComponent<AssetBundleManager>();
			if (m_Manager == null)
			{
				enabled = false;
				Debug.LogError("AssetBundleManager not found!", this);
			}
		}

		private void OnGUI()
		{
			List<Request> requestTasks = m_Manager.m_RequestTasks;
			for (Int32 i = 0; i < requestTasks.Count; i++)
			{
				Request request = requestTasks[i];
				GUILayout.Label(String.Concat(new Object[]
				{
					"Request: '",
					request.Name,
					"' ",
					request.IsDone,
					" ",
					request.Progress.ToString("0.0")
				}), new GUILayoutOption[0]);
			}
			List<AssetAsyncLoad> assetAsyncTasks = m_Manager.m_AssetAsyncTasks;
			for (Int32 j = 0; j < assetAsyncTasks.Count; j++)
			{
				AssetAsyncLoad assetAsyncLoad = assetAsyncTasks[j];
				GUILayout.Label(String.Concat(new Object[]
				{
					"AssetRequest: '",
					assetAsyncLoad.Key.Name,
					"' ",
					assetAsyncLoad.IsDone,
					" ",
					assetAsyncLoad.Progress.ToString("0.0")
				}), new GUILayoutOption[0]);
			}
		}
	}
}
