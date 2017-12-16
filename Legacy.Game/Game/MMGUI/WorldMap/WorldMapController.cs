using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Map;
using Legacy.Core.WorldMap;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.WorldMap
{
	[AddComponentMenu("MM Legacy/MMGUI/WorldMap Controller")]
	public class WorldMapController : MonoBehaviour
	{
		public const Int32 MAP_GRID_WIDTH = 145;

		public const Int32 MAP_GRID_HEIGHT = 105;

		public const Int32 MAP_PIXEL_WIDTH = 1340;

		public const Int32 MAP_PIXEL_HEIGHT = 971;

		[SerializeField]
		private GameObject m_IconOrigin;

		[SerializeField]
		private WorldMapIcon m_IconPrefab;

		[SerializeField]
		private WorldMapUserMapNoteController m_UserMapNoteController;

		private List<WorldMapIcon> m_Icons = new List<WorldMapIcon>();

		public Boolean IsOpen => gameObject.activeSelf;

	    public static Vector3 GetWorldMapPosition(Position position)
		{
			position.X = Mathf.Clamp(position.X, 0, 138);
			position.Y = Mathf.Clamp(position.Y, 0, 99);
			Vector3 result;
			result.x = (Int32)(position.X / 145f * 1340f);
			result.y = (Int32)(position.Y / 105f * 971f);
			result.z = 0f;
			return result;
		}

		public static Position GetWorldMapGridPosition(Vector3 position)
		{
			position.x = Mathf.Clamp(position.x, 0f, 1340f);
			position.y = Mathf.Clamp(position.y, 0f, 971f);
			Position result;
			result.X = (Int32)(position.x * 145f / 1340f);
			result.Y = (Int32)(position.y * 105f / 971f);
			return result;
		}

		private void Awake()
		{
			LegacyLogic.Instance.WorldManager.WorldMapController.MapPointsLoaded += WorldMapLoaded;
			LegacyLogic.Instance.WorldManager.WorldMapController.MapPointUpdated += WorldMapLoaded;
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.WorldManager.WorldMapController.MapPointsLoaded -= WorldMapLoaded;
			LegacyLogic.Instance.WorldManager.WorldMapController.MapPointUpdated -= WorldMapLoaded;
		}

		private void OnEnable()
		{
			if (m_Icons.Count == 0)
			{
				GenerateIconViews();
			}
			else
			{
				UpdateIconViews();
			}
		}

		private void WorldMapLoaded(Object sender, EventArgs e)
		{
			if (gameObject.activeInHierarchy)
			{
				if (m_Icons.Count == 0)
				{
					GenerateIconViews();
					return;
				}
				MapPointVisibleEventArgs mapPointVisibleEventArgs = e as MapPointVisibleEventArgs;
				if (e != null)
				{
					if (mapPointVisibleEventArgs != null && mapPointVisibleEventArgs.Point != null)
					{
						UpdateIconViews(mapPointVisibleEventArgs.Point);
					}
				}
				else
				{
					GenerateIconViews();
				}
			}
			else if (e == EventArgs.Empty)
			{
				GenerateIconViews();
			}
		}

		private void GenerateIconViews()
		{
			EventHandler value = new EventHandler(MapIcon_MouseClick);
			EventHandler<XEventArgs<Boolean>> value2 = new EventHandler<XEventArgs<Boolean>>(MapIcon_MouseTooltip);
			foreach (WorldMapIcon worldMapIcon in m_Icons)
			{
				if (worldMapIcon != null)
				{
					worldMapIcon.MouseClick -= value;
					worldMapIcon.MouseTooltip -= value2;
					Destroy(worldMapIcon.gameObject);
				}
			}
			m_Icons.Clear();
			IEnumerable<WorldMapPoint> enumerable = LegacyLogic.Instance.WorldManager.WorldMapController.WorldMapPointsIterator();
			foreach (WorldMapPoint worldMapPoint in enumerable)
			{
				WorldMapIcon worldMapIcon2 = Helper.Instantiate<WorldMapIcon>(m_IconPrefab);
				worldMapIcon2.transform.parent = m_IconOrigin.transform;
				worldMapIcon2.transform.localRotation = Quaternion.identity;
				worldMapIcon2.transform.localScale = Vector3.one;
				worldMapIcon2.transform.localPosition = GetWorldMapPosition(worldMapPoint.StaticData.Position);
				worldMapIcon2.MouseClick += value;
				worldMapIcon2.MouseTooltip += value2;
				worldMapIcon2.Initialize(this, worldMapPoint);
				m_Icons.Add(worldMapIcon2);
			}
		}

		private void MapIcon_MouseClick(Object sender, EventArgs e)
		{
			m_UserMapNoteController.OnClick();
		}

		private void MapIcon_MouseTooltip(Object sender, XEventArgs<Boolean> e)
		{
			WorldMapIcon worldMapIcon = (WorldMapIcon)sender;
			if (e.Value)
			{
				TooltipManager.Instance.Show(this, LocaManager.GetText(worldMapIcon.Data.StaticData.NameKey), m_UserMapNoteController.GetUserMapNoteText(Input.mousePosition), worldMapIcon.transform.position, new Vector3(24f, 24f, 0f));
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		private void UpdateIconViews()
		{
			foreach (WorldMapIcon worldMapIcon in m_Icons)
			{
				if (worldMapIcon != null)
				{
					worldMapIcon.UpdateIcon();
				}
			}
		}

		private void UpdateIconViews(WorldMapPoint data)
		{
			foreach (WorldMapIcon worldMapIcon in m_Icons)
			{
				if (worldMapIcon != null && worldMapIcon.Data == data)
				{
					worldMapIcon.UpdateIcon();
					break;
				}
			}
		}
	}
}
