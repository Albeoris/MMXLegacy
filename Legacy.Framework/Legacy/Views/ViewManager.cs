using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AssetBundles.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;
using Legacy.Map;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/View Manager")]
	public class ViewManager : MonoBehaviour
	{
		[SerializeField]
		private PositioningTemplate m_PositioningTemplates;

		private static ViewManager s_Instance;

		private Int32 m_ViewCounter;

		private Int32 m_StartViewCounter;

		private Boolean m_BeginLoadScene;

		private Transform m_GridOrigin;

		private Dictionary<BaseObject, GameObject> m_ControllerViewMap = new Dictionary<BaseObject, GameObject>();

		private Dictionary<Position, EntityPositioning> m_PositioningViewMap = new Dictionary<Position, EntityPositioning>();

		public static ViewManager Instance => s_Instance;

	    public Transform GridOrigin => FindGridOrigin();

	    public Vector3 GridOffset => FindGridOrigin().position;

	    public Single Progress
		{
			get
			{
				if (m_BeginLoadScene && m_StartViewCounter == 0)
				{
					return 0f;
				}
				return (m_ViewCounter != 0 || m_StartViewCounter != 0) ? (1f - m_ViewCounter / (Single)m_StartViewCounter) : 1f;
			}
		}

		public GameObject FindView(BaseObject p_controller)
		{
			GameObject gameObject = null;
			if (p_controller != null && m_ControllerViewMap.TryGetValue(p_controller, out gameObject) && gameObject == null)
			{
				gameObject = null;
				m_ControllerViewMap.Remove(p_controller);
			}
			return gameObject;
		}

		public GameObject FindView(Int32 p_spawnerID)
		{
			for (;;)
			{
				IL_00:
				foreach (KeyValuePair<BaseObject, GameObject> keyValuePair in m_ControllerViewMap)
				{
					if (keyValuePair.Key.SpawnerID == p_spawnerID)
					{
						if (keyValuePair.Value == null)
						{
							m_ControllerViewMap.Remove(keyValuePair.Key);
							goto IL_00;
						}
						return keyValuePair.Value;
					}
				}
				break;
			}
			return null;
		}

		public T FindViewAndGetComponent<T>(BaseObject p_controller) where T : Component
		{
			GameObject gameObject = FindView(p_controller);
			if (gameObject != null)
			{
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
			return null;
		}

		public EntityPositioning GetEntityPositioning(Position p_position)
		{
			EntityPositioning entityPositioning;
			if (!m_PositioningViewMap.TryGetValue(p_position, out entityPositioning))
			{
				entityPositioning = new EntityPositioning(LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_position), m_PositioningTemplates);
				m_PositioningViewMap.Add(p_position, entityPositioning);
			}
			return entityPositioning;
		}

		public GameObject GetSlotOrigin(Position p_position)
		{
			EntityPositioning entityPositioning = GetEntityPositioning(p_position);
			if (entityPositioning.m_SlotOrigin == null)
			{
				entityPositioning.m_SlotOrigin = new GameObject("Origin " + p_position);
				entityPositioning.m_SlotOrigin.transform.parent = FindGridOrigin();
				Vector3 localPosition = Helper.SlotLocalPosition(p_position, LegacyLogic.Instance.MapLoader.Grid.GetSlot(p_position).Height);
				entityPositioning.m_SlotOrigin.transform.localPosition = localPosition;
			}
			return entityPositioning.m_SlotOrigin;
		}

		public static void GetSlotDatas(Position origin, Position target, out Vector3 originPosition, out Vector3 forward, out Vector3 left, out Vector3 targetPosition)
		{
			originPosition = Helper.SlotLocalPosition(origin, LegacyLogic.Instance.MapLoader.Grid.GetSlot(origin).Height) + Instance.GridOffset;
			targetPosition = Helper.SlotLocalPosition(target, LegacyLogic.Instance.MapLoader.Grid.GetSlot(target).Height) + Instance.GridOffset;
			forward = (targetPosition - originPosition).normalized;
			left = Vector3.Cross(Vector3.up, forward);
		}

		internal static void DestroyView(BaseObject p_controller)
		{
			if (p_controller == null || s_Instance == null)
			{
				return;
			}
			s_Instance.m_ControllerViewMap.Remove(p_controller);
		}

		private void Update()
		{
			LegacyLogic.Instance.MapLoader.ViewManagerProgress = Progress * 0.5f;
		}

		private void Awake()
		{
			if (s_Instance != null)
			{
				Destroy(this);
				throw new Exception("ViewManager\nInstance already set! by -> " + s_Instance);
			}
			DontDestroyOnLoad(gameObject);
			s_Instance = this;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishedSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnSpawnBaseObject));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.ANNOUNCE_LATE_MONSTER_SPAWN, new EventHandler(OnAnnounceLateMonsterSpawn));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CHANGE_MAP, new EventHandler(OnChangeMap));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishedSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnSpawnBaseObject));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.ANNOUNCE_LATE_MONSTER_SPAWN, new EventHandler(OnAnnounceLateMonsterSpawn));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CHANGE_MAP, new EventHandler(OnChangeMap));
		}

		private void OnStartSceneLoad(Object sender, EventArgs e)
		{
			m_BeginLoadScene = true;
		}

		private void OnFinishedSceneLoad(Object p_sender, EventArgs p_args)
		{
			m_BeginLoadScene = false;
			ReadOnlyCollection<BaseObject> objects = LegacyLogic.Instance.WorldManager.Objects;
			m_ViewCounter = objects.Count;
			m_StartViewCounter = objects.Count;
			for (Int32 i = 0; i < objects.Count; i++)
			{
				if (SpawnBaseObject(objects[i]))
				{
					m_ViewCounter--;
				}
			}
			FinishedLoadViews();
		}

		private void OnSpawnBaseObject(Object p_sender, EventArgs p_args)
		{
			if (m_BeginLoadScene)
			{
				return;
			}
			BaseObject @object = ((BaseObjectEventArgs)p_args).Object;
			SpawnBaseObject(@object);
		}

		private void OnAnnounceLateMonsterSpawn(Object p_sender, EventArgs p_args)
		{
			Int32 monsterID = ((AnnounceLateMonsterSpawnArgs)p_args).MonsterID;
			MonsterStaticData staticData = StaticDataHandler.GetStaticData<MonsterStaticData>(EDataType.MONSTER, monsterID);
			if (staticData != null)
			{
				LoadPrefab(staticData.Prefab, null, null);
			}
		}

		private Boolean SpawnBaseObject(BaseObject baseObj)
		{
			if (m_ControllerViewMap.ContainsKey(baseObj))
			{
				Debug.LogError(String.Concat(new Object[]
				{
					"ViewManager: View already created! ",
					baseObj.Type,
					" ",
					baseObj.StaticID
				}));
				return true;
			}
			m_ControllerViewMap[baseObj] = null;
			if (baseObj is Party)
			{
				GameObject asset = Helper.ResourcesLoad<GameObject>("Prefabs/Player");
				AssetLoaded(baseObj, asset);
				return false;
			}
			String text;
			if (baseObj is MovingEntity)
			{
				MovingEntity movingEntity = (MovingEntity)baseObj;
				if (ConfigManager.Instance.Options.ShowAlternativeMonsterModel && !String.IsNullOrEmpty(movingEntity.PrefabAlt))
				{
					text = movingEntity.PrefabAlt;
				}
				else
				{
					text = movingEntity.Prefab;
				}
			}
			else
			{
				if (!(baseObj is InteractiveObject))
				{
					Debug.LogError("TrySpawn unknow type: " + ((baseObj == null) ? "null" : baseObj.GetType().ToString()));
					return true;
				}
				InteractiveObject interactiveObject = (InteractiveObject)baseObj;
				text = interactiveObject.Prefab;
			}
			if (baseObj is TrapEffectContainer)
			{
				GameObject asset2 = Helper.ResourcesLoad<GameObject>("Prefabs/TrapEffectContainerHACK");
				AssetLoaded(baseObj, asset2);
				return false;
			}
			return String.IsNullOrEmpty(text) || LoadPrefab(text, new AssetRequestCallback(OnAssetRequested), baseObj);
		}

		private static Boolean LoadPrefab(String assetName, AssetRequestCallback callback, Object userToken)
		{
			AssetRequest assetRequest = null;
			if (LegacyLogic.Instance.ModController.InModMode)
			{
				assetRequest = AssetBundleManagers.Instance.Mod.RequestAsset(assetName, 0, callback, userToken);
			}
			if (assetRequest == null)
			{
				assetRequest = AssetBundleManagers.Instance.Main.RequestAsset(assetName, 0, callback, userToken);
			}
			if (assetRequest == null)
			{
				Debug.LogError("Prefab not found! " + assetName);
				return true;
			}
			return assetRequest.Status >= ERequestStatus.Done;
		}

		private void OnChangeMap(Object sender, EventArgs e)
		{
			m_PositioningViewMap.Clear();
			m_ControllerViewMap.Clear();
		}

		private void OnAssetRequested(AssetRequest p_args)
		{
			GameObject gameObject = p_args.Asset as GameObject;
			if (p_args.Status != ERequestStatus.Done || gameObject == null)
			{
				m_ViewCounter--;
				FinishedLoadViews();
				return;
			}
			BaseObject entity = (BaseObject)p_args.Tag;
			AssetLoaded(entity, gameObject);
		}

		private void AssetLoaded(BaseObject entity, GameObject asset)
		{
			Transform parent = FindGridOrigin();
			asset = Helper.Instantiate<GameObject>(asset);
			asset.transform.parent = parent;
			asset.transform.localPosition = Vector3.zero;
			if (entity is InteractiveObject)
			{
				InteractiveObject interactiveObject = (InteractiveObject)entity;
				Vector3 vector = Helper.SlotLocalPosition(interactiveObject.Position, LegacyLogic.Instance.MapLoader.Grid.GetSlot(interactiveObject.Position).Height);
				vector += new Vector3(interactiveObject.OffsetPosition.X, interactiveObject.OffsetPosition.Y, interactiveObject.OffsetPosition.Z);
				asset.transform.localPosition = vector;
				Quaternion quaternion = (interactiveObject.Location == EDirection.CENTER) ? Quaternion.identity : Helper.GridDirectionToQuaternion(interactiveObject.Location);
				quaternion = Quaternion.Euler(new Vector3(interactiveObject.ObjectRotation.X, interactiveObject.ObjectRotation.Y, interactiveObject.ObjectRotation.Z)) * quaternion;
				asset.transform.localRotation = quaternion;
			}
			InitAllViews(asset, entity);
			m_ControllerViewMap[entity] = asset;
			m_ViewCounter--;
			FinishedLoadViews();
		}

		private void FinishedLoadViews()
		{
			if (m_ViewCounter <= 0 && !m_BeginLoadScene)
			{
				LegacyLogic.Instance.MapLoader.FinishedLoadViews();
			}
		}

		private Transform FindGridOrigin()
		{
			if (m_GridOrigin == null)
			{
				GameObject gameObject = GameObject.FindGameObjectWithTag("Grid Origin");
				if (gameObject == null)
				{
					gameObject = new GameObject();
					gameObject.tag = "Grid Origin";
					gameObject.AddComponent<GridOrigin>();
				}
				m_GridOrigin = gameObject.transform;
			}
			m_GridOrigin.position = LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			return m_GridOrigin;
		}

		private static void InitAllViews(GameObject p_Asset, BaseObject p_Controller)
		{
			BaseView[] components = p_Asset.GetComponents<BaseView>();
			if (components.Length == 0)
			{
				Debug.LogError(String.Concat(new Object[]
				{
					"No views attached! SpawnerID=",
					p_Controller.SpawnerID,
					"\n",
					p_Asset.name
				}));
			}
			else
			{
				foreach (BaseView baseView in components)
				{
					try
					{
						baseView.InitializeView(p_Controller);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception, baseView);
					}
				}
			}
		}
	}
}
