using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Legacy.Configuration;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.Mods;
using Legacy.Core.PartyManagement;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI.Tooltip;
using Legacy.MMGUI;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/MinimapView")]
	public class MinimapView : MonoBehaviour
	{
		public const Int32 SLOT_WIDTH = 24;

		public const Int32 SLOT_HEIGHT = 24;

		private Dictionary<BaseObject, SymbolView> m_SymbolViews = new Dictionary<BaseObject, SymbolView>();

		private RenderTexture m_MinimpaRenderTex;

		private String m_actionColorHex;

		private Texture2D m_ModMinimapTexture;

		private Boolean m_LastDangerSense;

		private Boolean m_IsMouseHover;

		private SpiritBeaconSymbolView m_SpiritBeacon;

		private static MinimapView s_Instance;

		[SerializeField]
		private UICameraCustom m_MapCamera;

		[SerializeField]
		private AreaCameraView m_AreaCamera;

		[SerializeField]
		private GameObject m_arrowIndicator;

		[SerializeField]
		private Transform m_MapMaskLayer;

		[SerializeField]
		private UITexture m_MapLayer;

		[SerializeField]
		private Transform m_GridLayer;

		[SerializeField]
		private Transform m_SymbolLayer;

		[SerializeField]
		private PlayerSymbolView m_SymbolPlayer;

		[SerializeField]
		private UIButton m_ingameMenuBtn;

		[SerializeField]
		private UIButton m_mapBtn;

		[SerializeField]
		private Color m_actionColor = new Color(0.75f, 1f, 1f);

		[SerializeField]
		private SymbolPool m_SymbolMonsterPrefab;

		[SerializeField]
		private SymbolPool m_SymbolDoorPrefab;

		[SerializeField]
		private SymbolPool m_SymbolBeaconPrefab;

		[SerializeField]
		private SymbolPool m_SymbolEntrancePrefab;

		[SerializeField]
		private SymbolPool m_SymbolNpcPrefab;

		[SerializeField]
		private SymbolPool m_SymbolTeleportPrefab;

		[SerializeField]
		private SymbolPool m_SymbolStairsPrefab;

		[SerializeField]
		private SymbolPool m_SymbolTrapPrefab;

		public static MinimapView Instance => s_Instance;

	    public AreaCameraView AreaCamera => m_AreaCamera;

	    public static String GetLocalizedSymbolTooltipText(Position gridPosition)
		{
			if (s_Instance != null)
			{
				foreach (SymbolView symbolView in s_Instance.m_SymbolViews.Values)
				{
					if (symbolView.MyControllerGridPosition == gridPosition && symbolView is SimpleSymbolView)
					{
						return ((SimpleSymbolView)symbolView).GetLocalizedTooltipText();
					}
				}
			}
			return null;
		}

		public void HideAllTooltips()
		{
			HideTooltip();
			foreach (SymbolView symbolView in m_SymbolViews.Values)
			{
				if (symbolView is SimpleSymbolView)
				{
					SimpleSymbolView simpleSymbolView = (SimpleSymbolView)symbolView;
					simpleSymbolView.HideTooltip();
				}
			}
		}

		private void Awake()
		{
			s_Instance = this;
			m_MinimpaRenderTex = new RenderTexture(320, 320, 8, RenderTextureFormat.ARGB32);
			m_MinimpaRenderTex.name = "MinimapRT";
			m_MinimpaRenderTex.hideFlags = HideFlags.DontSave;
			Camera component = m_MapCamera.GetComponent<Camera>();
			component.targetTexture = m_MinimpaRenderTex;
			component.rect = new Rect(0f, 0f, 1f, 1f);
			m_actionColorHex = "[" + NGUITools.EncodeColor(m_actionColor) + "]";
			ReadOnlyCollection<BaseObject> objects = LegacyLogic.Instance.WorldManager.Objects;
			foreach (BaseObject p_baseObj in objects)
			{
				CreateSymbolView(p_baseObj);
			}
			OnSpiritBeaconUpdate(null, null);
			OnOptionsChanged(null, null);
			if (!LegacyLogic.Instance.MapLoader.IsLoading && LegacyLogic.Instance.MapLoader.Grid != null)
			{
				OnFinishLoadScene(null, null);
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnSpawnBaseObject));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnDestroyBaseObject));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishLoadScene));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPIRIT_BEACON_UPDATE, new EventHandler(OnSpiritBeaconUpdate));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnOptionsChanged));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPAWN_BASEOBJECT, new EventHandler(OnSpawnBaseObject));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnDestroyBaseObject));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnFinishLoadScene));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPIRIT_BEACON_UPDATE, new EventHandler(OnSpiritBeaconUpdate));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnOptionsChanged));
			Helper.DestroyImmediate<RenderTexture>(ref m_MinimpaRenderTex);
			Helper.DestroyImmediate<Texture2D>(ref m_ModMinimapTexture);
		}

		private void OnHover(Boolean isOver)
		{
			m_IsMouseHover = isOver;
		}

		private void OnPress(Boolean p_isDown)
		{
			if (p_isDown)
			{
				IngameController.Instance.ToggleAreaMap(this, EventArgs.Empty);
			}
		}

		private void OnSpawnBaseObject(Object p_sender, EventArgs p_args)
		{
			BaseObject @object = ((BaseObjectEventArgs)p_args).Object;
			CreateSymbolView(@object);
		}

		private void OnDestroyBaseObject(Object p_sender, EventArgs p_args)
		{
			BaseObject @object = ((BaseObjectEventArgs)p_args).Object;
			RecycleSymbolView(@object);
		}

		private void OnStartSceneLoad(Object p_sender, EventArgs p_args)
		{
			UnloadSpiritBeacon();
			while (m_SymbolViews.Count > 0)
			{
				IEnumerator<BaseObject> enumerator = m_SymbolViews.Keys.GetEnumerator();
				if (enumerator.MoveNext())
				{
					RecycleSymbolView(enumerator.Current);
				}
			}
			m_SymbolPlayer.InitializeView(null);
			m_AreaCamera.InitializeView(null);
		}

		private void OnSpiritBeaconUpdate(Object p_sender, EventArgs p_args)
		{
			UnloadSpiritBeacon();
			if (LegacyLogic.Instance.WorldManager.SpiritBeaconController.Existent)
			{
				SpiritBeaconPosition spiritBeacon = LegacyLogic.Instance.WorldManager.SpiritBeaconController.SpiritBeacon;
				SpiritBeaconSymbolView spiritBeaconSymbolView = m_SpiritBeacon = (SpiritBeaconSymbolView)m_SymbolBeaconPrefab.Get();
				spiritBeaconSymbolView.transform.parent = m_SymbolLayer;
				spiritBeaconSymbolView.transform.localPosition = new Vector3(spiritBeacon.Position.X * 24, spiritBeacon.Position.Y * 24, 0f);
				spiritBeaconSymbolView.ControllerGridPosition = spiritBeacon.Position;
				spiritBeaconSymbolView.ControllerGridDirection = EDirection.NORTH;
				spiritBeaconSymbolView.InitializeView(null);
				spiritBeaconSymbolView.MakePixelPerfect();
				spiritBeaconSymbolView.CheckVisibility(true);
			}
		}

		private void OnOptionsChanged(Object p_sender, EventArgs p_args)
		{
			m_SymbolPlayer.RotateAnchor = ConfigManager.Instance.Options.ViewAlignedMinimap;
			m_GridLayer.GetComponent<UITexture>().alpha = ConfigManager.Instance.Options.MinimapGirdOpacity;
			Camera component = m_MapCamera.GetComponent<Camera>();
			UITexture component2 = m_MapMaskLayer.GetComponent<UITexture>();
			if (ConfigManager.Instance.Options.ShowMinimap)
			{
				m_arrowIndicator.SetActive(true);
				component.enabled = true;
				Texture mainTexture = component2.mainTexture;
				Texture minimpaRenderTex = m_MinimpaRenderTex;
				if (minimpaRenderTex != mainTexture)
				{
					component2.mainTexture = minimpaRenderTex;
					mainTexture.UnloadAsset();
				}
			}
			else
			{
				m_arrowIndicator.SetActive(false);
				component.enabled = false;
				component2.mainTexture = Helper.ResourcesLoad<Texture>("MinimapMaps/GUI_minimap_overlay");
			}
		}

		private void UnloadSpiritBeacon()
		{
			if (m_SpiritBeacon != null)
			{
				m_SpiritBeacon.InitializeView(null);
				m_SymbolBeaconPrefab.Recycle(m_SpiritBeacon);
			}
		}

		private void OnFinishLoadScene(Object sender, EventArgs e)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Texture2D texture2D = null;
			String minimapName = grid.MinimapName;
			if (LegacyLogic.Instance.ModController.InModMode && !String.IsNullOrEmpty(minimapName))
			{
				ModController.ModInfo currentMod = LegacyLogic.Instance.ModController.CurrentMod;
				String path = Path.Combine(currentMod.AssetFolder, minimapName + ".png");
				if (File.Exists(path))
				{
					if (m_ModMinimapTexture == null)
					{
						m_ModMinimapTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
						m_ModMinimapTexture.hideFlags = HideFlags.DontSave;
					}
					Byte[] data = File.ReadAllBytes(path);
					if (m_ModMinimapTexture.LoadImage(data))
					{
						texture2D = m_ModMinimapTexture;
					}
				}
			}
			if (texture2D == null)
			{
				Texture mainTexture = m_MapLayer.mainTexture;
				texture2D = Helper.ResourcesLoad<Texture2D>(minimapName, false);
				if (texture2D != mainTexture)
				{
					m_MapLayer.mainTexture = null;
					if (mainTexture != m_ModMinimapTexture)
					{
						mainTexture.UnloadAsset();
					}
				}
			}
			m_MapLayer.mainTexture = texture2D;
			m_MapLayer.MakePixelPerfect();
			Vector3 localPosition = m_MapLayer.transform.localPosition;
			localPosition.x = -12f + grid.MinimapOffsetX;
			localPosition.y = -12f + grid.MinimapOffsetY;
			m_MapLayer.transform.localPosition = localPosition;
			localPosition = m_GridLayer.transform.localPosition;
			localPosition.x = -12f;
			localPosition.y = -12f;
			m_GridLayer.transform.localPosition = localPosition;
			m_GridLayer.localScale = new Vector3(grid.Width * 24, grid.Height * 24, 1f);
			m_GridLayer.GetComponent<UITexture>().uvRect = new Rect(0f, 0f, grid.Width, grid.Height);
			Camera cachedCamera = m_MapCamera.cachedCamera;
			Color backgroundColor = (!(texture2D == null)) ? Color.black : Color.magenta;
			m_AreaCamera.camera.backgroundColor = backgroundColor;
			cachedCamera.backgroundColor = backgroundColor;
			OnSpiritBeaconUpdate(null, null);
		}

		private void OnWorldMapClicked()
		{
			IngameController.Instance.ToggleWorldMap(this, EventArgs.Empty);
		}

		private void OnGameMenuClicked()
		{
			if (IngameController.Instance.IsAnyScreenOpen() && !IngameController.Instance.IsIngameMenuOpen())
			{
				IngameController.Instance.CloseAllScreens();
			}
			IngameController.Instance.ToggleIngameMenu(this, EventArgs.Empty);
		}

		private void MenuBtnHover()
		{
			Vector3 position = m_ingameMenuBtn.transform.position;
			Vector3 p_offset = new Vector3(24f, 24f, 0f);
			String text = LocaManager.GetText("TOOLTIP_OPEN_MENU");
			Hotkey hotkey = KeyConfigManager.KeyBindings[EHotkeyType.OPEN_CLOSE_MENU];
			KeyCode keyCode = hotkey.Key1;
			if (keyCode == KeyCode.None)
			{
				keyCode = hotkey.AltKey1;
			}
			if (keyCode != KeyCode.None)
			{
				String text2 = "[" + LocaManager.GetText("OPTIONS_INPUT_KEYNAME_" + keyCode.ToString().ToUpper()) + "]";
				String text3 = text;
				text = String.Concat(new String[]
				{
					text3,
					" ",
					m_actionColorHex,
					text2,
					"[-]"
				});
			}
			TooltipManager.Instance.Show(this, text, position, p_offset);
		}

		private void MapBtnHover()
		{
			Vector3 position = m_mapBtn.transform.position;
			Vector3 p_offset = new Vector3(24f, 24f, 0f);
			String text = LocaManager.GetText("TOOLTIP_OPEN_MAP");
			Hotkey hotkey = KeyConfigManager.KeyBindings[EHotkeyType.OPEN_MAP];
			KeyCode keyCode = hotkey.Key1;
			if (keyCode == KeyCode.None)
			{
				keyCode = hotkey.AltKey1;
			}
			if (keyCode != KeyCode.None)
			{
				String text2 = "[" + LocaManager.GetText("OPTIONS_INPUT_KEYNAME_" + keyCode.ToString().ToUpper()) + "]";
				String text3 = text;
				text = String.Concat(new String[]
				{
					text3,
					" ",
					m_actionColorHex,
					text2,
					"[-]"
				});
			}
			TooltipManager.Instance.Show(this, text, position, p_offset);
		}

		private void HideTooltip()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void Update()
		{
			if (m_IsMouseHover)
			{
				Vector3 position = GUIMainCamera.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
				position = m_MapMaskLayer.InverseTransformPoint(position);
				Vector2 mousePosition = new Vector2(position.x * 320f, 320f - -position.y * 320f);
				m_MapCamera.ProcessEvents(mousePosition);
			}
			CheckDangerSense();
		}

		private void CheckDangerSense()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party != null)
			{
				Boolean flag = party.HasDangerSense();
				if (m_LastDangerSense != flag)
				{
					m_LastDangerSense = flag;
					foreach (SymbolView symbolView in m_SymbolViews.Values)
					{
						symbolView.CheckVisibility(false);
					}
				}
			}
		}

		private void CreateSymbolView(BaseObject p_baseObj)
		{
			SymbolView symbolView = null;
			if (p_baseObj is Party)
			{
				m_SymbolPlayer.InitializeView(p_baseObj);
				m_AreaCamera.InitializeView(p_baseObj);
				return;
			}
			if (p_baseObj is Monster)
			{
				symbolView = m_SymbolMonsterPrefab.Get();
			}
			else if (p_baseObj is InteractiveObject)
			{
				InteractiveObject interactiveObject = (InteractiveObject)p_baseObj;
				if (interactiveObject.IsSecret)
				{
					return;
				}
				if (interactiveObject.MinimapVisible)
				{
					switch (interactiveObject.Type)
					{
					case EObjectType.DOOR:
						if (String.IsNullOrEmpty(interactiveObject.Prefab))
						{
							return;
						}
						symbolView = m_SymbolDoorPrefab.Get();
						break;
					case EObjectType.ENTRANCE:
						symbolView = m_SymbolEntrancePrefab.Get();
						break;
					case EObjectType.TELEPORTER:
						symbolView = m_SymbolTeleportPrefab.Get();
						break;
					case EObjectType.NPC_CONTAINER:
						symbolView = m_SymbolNpcPrefab.Get();
						break;
					case EObjectType.TRAP:
						symbolView = m_SymbolTrapPrefab.Get();
						break;
					}
				}
			}
			if (symbolView == null)
			{
				return;
			}
			symbolView.transform.parent = m_SymbolLayer;
			symbolView.InitializeView(p_baseObj);
			symbolView.MakePixelPerfect();
			symbolView.CheckVisibility(true);
			m_SymbolViews.Add(p_baseObj, symbolView);
		}

		private void RecycleSymbolView(BaseObject p_baseObj)
		{
			SymbolView symbolView;
			if (!m_SymbolViews.TryGetValue(p_baseObj, out symbolView))
			{
				return;
			}
			SymbolPool symbolPool = null;
			if (p_baseObj is Monster)
			{
				symbolPool = m_SymbolMonsterPrefab;
			}
			else if (p_baseObj is InteractiveObject && ((InteractiveObject)p_baseObj).MinimapVisible)
			{
				switch (((InteractiveObject)p_baseObj).Type)
				{
				case EObjectType.DOOR:
					symbolPool = m_SymbolDoorPrefab;
					break;
				case EObjectType.ENTRANCE:
					symbolPool = m_SymbolEntrancePrefab;
					break;
				case EObjectType.TELEPORTER:
					symbolPool = m_SymbolTeleportPrefab;
					break;
				case EObjectType.NPC_CONTAINER:
					symbolPool = m_SymbolNpcPrefab;
					break;
				case EObjectType.TRAP:
					symbolPool = m_SymbolTrapPrefab;
					break;
				}
			}
			if (symbolPool == null)
			{
				return;
			}
			symbolView.InitializeView(null);
			symbolPool.Recycle(symbolView);
			m_SymbolViews.Remove(p_baseObj);
		}

		[Serializable]
		public class SymbolPool
		{
			private Queue<SymbolView> m_Pool = new Queue<SymbolView>();

			[SerializeField]
			private SymbolView m_PrefabSymbolView;

			public SymbolView Get()
			{
				SymbolView symbolView;
				if (m_Pool.Count > 0)
				{
					symbolView = m_Pool.Dequeue();
					symbolView.gameObject.SetActive(true);
				}
				else
				{
					symbolView = Helper.Instantiate<SymbolView>(m_PrefabSymbolView);
				}
				return symbolView;
			}

			public void Recycle(SymbolView obj)
			{
				obj.gameObject.SetActive(false);
				m_Pool.Enqueue(obj);
			}
		}
	}
}
