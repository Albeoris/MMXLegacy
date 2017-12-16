using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Minimap;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/AreaMap Controller")]
	public class AreaMapController : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_mapName;

		[SerializeField]
		private UILabel m_coordinates;

		[SerializeField]
		private UITexture m_areaTextureView;

		[SerializeField]
		private Vector2 m_areaMapSize;

		private RenderTexture m_AreaRT;

		private void Awake()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnEntityChangePosition));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.START_SCENE_LOAD, new EventHandler(OnStartSceneLoad));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnEntityChangePosition));
			Helper.Destroy<RenderTexture>(ref m_AreaRT);
		}

		private void OnEnable()
		{
			if (m_AreaRT == null)
			{
				m_AreaRT = new RenderTexture((Int32)m_areaMapSize.x, (Int32)m_areaMapSize.y, 8, RenderTextureFormat.ARGB32);
				m_AreaRT.name = "AreaMapRT";
			}
			m_areaTextureView.mainTexture = m_AreaRT;
			Camera camera = MinimapView.Instance.AreaCamera.Camera;
			camera.enabled = true;
			camera.targetTexture = m_AreaRT;
			camera.rect = new Rect(0f, 0f, 1f, 1f);
			UpdateCoordinates();
			UpdateMapName();
		}

		private void OnDisable()
		{
			Camera camera = MinimapView.Instance.AreaCamera.Camera;
			camera.enabled = false;
		}

		private void OnStartSceneLoad(Object sender, EventArgs e)
		{
			UpdateMapName();
		}

		private void OnEntityChangePosition(Object p_sender, EventArgs p_args)
		{
			if (p_args is BaseObjectEventArgs)
			{
				p_sender = ((BaseObjectEventArgs)p_args).Object;
			}
			if (p_sender is Party)
			{
				UpdateCoordinates();
			}
		}

		private void UpdateMapName()
		{
			String text = null;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (grid != null)
			{
				GridInfo gridInfo = LegacyLogic.Instance.MapLoader.FindGridInfo(grid.Name);
				if (gridInfo != null)
				{
					text = LocaManager.GetText(gridInfo.LocationLocaName);
					text = text.Replace("@", ", ");
				}
				else
				{
					Debug.LogError("Grid Info not found " + grid.Name);
				}
			}
			m_mapName.text = text;
		}

		private void UpdateCoordinates()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party != null)
			{
				m_coordinates.text = LocaManager.GetText("GUI_AREAMAP_COORDINATES", party.Position.X, party.Position.Y);
			}
			else
			{
				m_coordinates.text = null;
			}
		}
	}
}
