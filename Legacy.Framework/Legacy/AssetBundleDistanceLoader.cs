using System;
using AssetBundles.Core;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Map/Asset Bundle Distance Loader")]
	public class AssetBundleDistanceLoader : MonoBehaviour
	{
		public String AssetName = String.Empty;

		public Single VisibleDistance = 10f;

		public Single RequestDistance = 20f;

		public Single UnloadDistance = 30f;

		public Single QualityScaleFactor = 1.41f;

		private GameObject mInstantiatedAsset;

		private AssetRequest mRequest;

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnChangedPlayerMoved));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnChangedPlayerPosition));
		}

		private void OnChangedPlayerPosition(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			OnPositionChange(baseObjectEventArgs.Position);
		}

		private void OnChangedPlayerMoved(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			OnPositionChange(moveEntityEventArgs.Position);
		}

		private void OnPositionChange(Position p_playerPos)
		{
			Vector3 b = Helper.SlotLocalPosition(p_playerPos, LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_playerPos).Height) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			Single num = Vector3.Distance(transform.position, b);
			Single num2 = Mathf.Pow(QualityScaleFactor, (Single)GraphicsConfigManager.Settings.ViewDistance);
			num /= num2;
			if (num <= VisibleDistance && mInstantiatedAsset != null)
			{
				if (!mInstantiatedAsset.activeSelf)
				{
					mInstantiatedAsset.SetActive(true);
				}
			}
			else if (num <= RequestDistance)
			{
				if (mRequest == null)
				{
					RequestAsset();
				}
				if (mInstantiatedAsset != null && mInstantiatedAsset.activeSelf)
				{
					mInstantiatedAsset.SetActive(false);
				}
			}
			else if (num >= UnloadDistance)
			{
				if (mInstantiatedAsset != null)
				{
					Destroy(mInstantiatedAsset);
					mInstantiatedAsset = null;
					if (!IsInvoking("CleanUp"))
					{
						Invoke("CleanUp", Random.Range(1f, 3f));
					}
				}
				if (mRequest != null)
				{
					mRequest = null;
					if (!IsInvoking("CleanUp"))
					{
						Invoke("CleanUp", Random.Range(1f, 3f));
					}
				}
			}
		}

		private void CleanUp()
		{
		}

		private void RequestAsset()
		{
			mRequest = AssetBundleManagers.Instance.Main.RequestAsset(AssetName, 1, new AssetRequestCallback(OnAssetRequested), null);
			if (mRequest == null)
			{
				Debug.LogWarning("Prefab not found! " + AssetName);
			}
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
			if (this == null)
			{
				if (p_args.AssetBundleName != String.Empty)
				{
					AssetBundleManagers.Instance.Main.UnloadAssetBundle(p_args.AssetBundleName, true, true, false);
					Resources.UnloadUnusedAssets();
				}
				return;
			}
			mInstantiatedAsset = (GameObject)Instantiate(gameObject);
			mInstantiatedAsset.transform.parent = transform;
			mInstantiatedAsset.transform.localPosition = Vector3.zero;
			mInstantiatedAsset.transform.localEulerAngles = Vector3.zero;
			mInstantiatedAsset.transform.localScale = Vector3.one;
			mInstantiatedAsset.SetActive(false);
		}
	}
}
