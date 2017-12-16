using System;
using AssetBundles.Core;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Map
{
	[AddComponentMenu("MM Legacy/Map/Regions/WorldRegion LOD Object")]
	public class WorldRegion_LODObject : MonoBehaviour
	{
		public GameObject BaseModel;

		public String HighModelPrefabPath = String.Empty;

		private Boolean mRequested;

		private GameObject mHighLODModel;

		public void LoadHighLOD()
		{
			if (mHighLODModel != null && !mHighLODModel.activeSelf)
			{
				mHighLODModel.SetActive(true);
				if (BaseModel != null)
				{
					BaseModel.SetActive(false);
				}
			}
			else if (!mRequested)
			{
				RequestAsset();
			}
		}

		public void UnloadHighLOD()
		{
			Destroy(mHighLODModel);
			mHighLODModel = null;
			mRequested = false;
			if (BaseModel != null)
			{
				BaseModel.SetActive(true);
			}
		}

		public void DisableHighLOD()
		{
			mHighLODModel.SetActive(false);
		}

		private void RequestAsset()
		{
			if (AssetBundleManagers.Instance.Main.RequestAsset(HighModelPrefabPath, 1, new AssetRequestCallback(OnAssetRequested), null) == null)
			{
				Debug.LogWarning("Prefab not found! " + HighModelPrefabPath);
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
			mHighLODModel = (GameObject)Instantiate(gameObject);
			mHighLODModel.transform.parent = transform;
			mHighLODModel.transform.localPosition = Vector3.zero;
			mHighLODModel.transform.localEulerAngles = Vector3.zero;
			mHighLODModel.transform.localScale = Vector3.one;
			if (BaseModel != null)
			{
				BaseModel.SetActive(false);
			}
		}
	}
}
