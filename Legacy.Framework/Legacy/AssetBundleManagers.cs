using System;
using System.IO;
using AssetBundles.Core;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Mods;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	internal class AssetBundleManagers : MonoBehaviour
	{
		private static AssetBundleManagers s_Instance;

		[SerializeField]
		private AssetBundleManager m_Main;

		[SerializeField]
		private AssetBundleManager m_Mod;

		public static Boolean HasInstance => s_Instance != null;

	    public static AssetBundleManagers Instance => s_Instance;

	    public AssetBundleManager Main => m_Main;

	    public AssetBundleManager Mod => m_Mod;

	    private void Awake()
		{
			if (s_Instance != null)
			{
				UnityEngine.Object.Destroy(this);
				throw new Exception("AssetBundleManager\nInstance already set! by -> " + s_Instance);
			}
			s_Instance = this;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOD_LOADED, new EventHandler(OnModLoaded));
		}

		private void Destroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOD_LOADED, new EventHandler(OnModLoaded));
		}

		private void OnModLoaded(Object sender, EventArgs e)
		{
			ModController.ModInfo currentMod = LegacyLogic.Instance.ModController.CurrentMod;
			m_Mod.LoadDatabase(Path.Combine(currentMod.AssetFolder, "assets.db"));
			m_Mod.AssetBundleLoader.BaseAddress = currentMod.AssetFolder;
			Debug.Log("Mod loaded assetpath: " + currentMod.AssetFolder);
		}
	}
}
