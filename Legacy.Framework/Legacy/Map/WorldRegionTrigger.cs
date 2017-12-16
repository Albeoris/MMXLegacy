using System;
using AssetBundles.Core;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Map
{
	[AddComponentMenu("MM Legacy/Map/Regions/WorldRegion Trigger")]
	public class WorldRegionTrigger : MonoBehaviour
	{
		public BoxCollider[] RegionAreas;

		public Terrain RegionMainTerrain;

		public GameObject[] WaterTiles;

		public WorldRegion_LODObject[] LODObjects;

		public String RegionGlobalPrefabPath = "none";

		public GameObject RegionGlobalDetails;

		public GameObject RegionGlobalLowLOD;

		public GameObject[] SharedGlobalDetails;

		public String SoundAssetBundleName = String.Empty;

		private GameObject mLoadedRegionGlobals;

		private Boolean mRequested;

		private String mAssetBundleName = String.Empty;

		private void Awake()
		{
			if (RegionGlobalDetails != null)
			{
				RegionGlobalDetails.SetActive(false);
			}
			if (RegionGlobalLowLOD != null)
			{
				RegionGlobalLowLOD.SetActive(true);
			}
			foreach (BoxCollider boxCollider in RegionAreas)
			{
				boxCollider.isTrigger = true;
			}
		}

		public void LoadRegion()
		{
			SetWorldHighLods(true);
			RegionMainTerrain.editorRenderFlags = TerrainRenderFlags.all;
			if (mLoadedRegionGlobals != null && !mLoadedRegionGlobals.activeSelf)
			{
				mLoadedRegionGlobals.SetActive(true);
			}
			else if (!mRequested)
			{
				RequestAsset();
			}
			foreach (WorldRegion_LODObject worldRegion_LODObject in LODObjects)
			{
				worldRegion_LODObject.LoadHighLOD();
			}
		}

		public void SetSharedObjects(Boolean p_enabled)
		{
			foreach (GameObject gameObject in WaterTiles)
			{
				gameObject.SetActive(p_enabled);
			}
			foreach (GameObject gameObject2 in SharedGlobalDetails)
			{
				gameObject2.SetActive(p_enabled);
			}
		}

		public void SetWorldHighLods(Boolean p_enabled)
		{
			if (RegionGlobalDetails != null)
			{
				RegionGlobalDetails.SetActive(p_enabled);
			}
			if (RegionGlobalLowLOD != null)
			{
				RegionGlobalLowLOD.SetActive(!p_enabled);
			}
		}

		public void UnloadRegion()
		{
			RegionMainTerrain.editorRenderFlags = (TerrainRenderFlags)3;
			Destroy(mLoadedRegionGlobals);
			mLoadedRegionGlobals = null;
			mRequested = false;
			foreach (WorldRegion_LODObject worldRegion_LODObject in LODObjects)
			{
				worldRegion_LODObject.UnloadHighLOD();
			}
			if (!String.IsNullOrEmpty(mAssetBundleName))
			{
				AssetBundleManagers.Instance.Main.UnloadAssetBundle(mAssetBundleName, true, true, false);
			}
			if (!String.IsNullOrEmpty(SoundAssetBundleName))
			{
				AssetBundleManagers.Instance.Main.UnloadAssetBundle(SoundAssetBundleName, true, true, true);
			}
			Resources.UnloadUnusedAssets();
			SetWorldHighLods(false);
		}

		public void DisableRegion()
		{
			RegionMainTerrain.editorRenderFlags = (TerrainRenderFlags)3;
			mLoadedRegionGlobals.SetActive(false);
			foreach (WorldRegion_LODObject worldRegion_LODObject in LODObjects)
			{
				worldRegion_LODObject.DisableHighLOD();
			}
			SetWorldHighLods(false);
		}

		private void RequestAsset()
		{
			AssetRequest assetRequest = AssetBundleManagers.Instance.Main.RequestAsset(RegionGlobalPrefabPath, 1, new AssetRequestCallback(OnAssetRequested), null);
			if (assetRequest == null)
			{
				Debug.LogWarning("Prefab not found! " + RegionGlobalPrefabPath);
			}
			else
			{
				mAssetBundleName = assetRequest.AssetBundleName;
			}
			mRequested = true;
		}

		private void OnAssetRequested(AssetRequest p_args)
		{
			GameObject gameObject = (GameObject)p_args.Asset;
			if (p_args.Status != ERequestStatus.Done)
			{
				Debug.LogError(String.Concat(new Object[]
				{
					"Error load asset. ",
					p_args.Status,
					"\nAssetName: ",
					p_args.AssetName
				}));
				return;
			}
			if (gameObject == null)
			{
				Debug.LogError("Error load asset.");
				return;
			}
			Transform transform = this.transform.FindChild(name + "_GLOBALDETAIL");
			if (transform != null)
			{
				Destroy(transform);
			}
			mLoadedRegionGlobals = (GameObject)Instantiate(gameObject);
			mLoadedRegionGlobals.name = name + "_GLOBALDETAIL";
			mLoadedRegionGlobals.transform.parent = this.transform;
			mLoadedRegionGlobals.transform.localPosition = Vector3.zero;
			mLoadedRegionGlobals.transform.localEulerAngles = Vector3.zero;
			mLoadedRegionGlobals.transform.localScale = Vector3.one;
		}
	}
}
