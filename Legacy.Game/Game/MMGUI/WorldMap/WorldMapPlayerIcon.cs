using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.WorldMap;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.WorldMap
{
	[AddComponentMenu("MM Legacy/MMGUI/WorldMap player icon")]
	internal class WorldMapPlayerIcon : MonoBehaviour
	{
		[SerializeField]
		private Single m_MoveTime = 0.2f;

		[SerializeField]
		private WorldMapUserMapNoteController m_UserMapNoteController;

		[SerializeField]
		private String m_TooltipLocaKey;

		private void Awake()
		{
			UpdateIconPosition(true);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnEntityChangePosition));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SET_ENTITY_POSITION, new EventHandler(OnEntityChangePosition));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.TELEPORT_ENTITY, new EventHandler(OnEntityChangePosition));
		}

		private void OnEnable()
		{
			UpdateIconPosition(true);
		}

		private void OnEntityChangePosition(Object sender, EventArgs e)
		{
			if (e is BaseObjectEventArgs)
			{
				sender = ((BaseObjectEventArgs)e).Object;
			}
			if (sender is Party)
			{
				UpdateIconPosition(false);
			}
		}

		private void OnClick()
		{
			m_UserMapNoteController.OnClick();
		}

		private void OnTooltip(Boolean show)
		{
			if (show)
			{
				TooltipManager.Instance.Show(this, LocaManager.GetText(m_TooltipLocaKey), m_UserMapNoteController.GetUserMapNoteText(Input.mousePosition), transform.position, new Vector3(24f, 24f, 0f));
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		private void UpdateIconPosition(Boolean skipAnimation)
		{
			Position position;
			if (GetPlayerPosition(out position))
			{
				gameObject.SetActive(true);
				Vector3 worldMapPosition = WorldMapController.GetWorldMapPosition(position);
				worldMapPosition.z = -2f;
				if (skipAnimation)
				{
					TweenPosition.Begin(gameObject, 0f, worldMapPosition);
				}
				else
				{
					TweenPosition.Begin(gameObject, m_MoveTime, worldMapPosition);
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		private static Boolean GetPlayerPosition(out Position partyPosition)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (grid != null)
			{
				if (grid.Type == EMapType.OUTDOOR)
				{
					Party party = LegacyLogic.Instance.WorldManager.Party;
					if (party != null)
					{
						partyPosition = party.Position;
						return true;
					}
				}
				else
				{
					WorldMapPoint worldMapPoint = LegacyLogic.Instance.WorldManager.WorldMapController.FindWorldMapPoint(grid.WorldMapPointID);
					if (worldMapPoint != null)
					{
						partyPosition = worldMapPoint.StaticData.Position;
						return true;
					}
					Debug.LogError("Grid mappoint ID not found! ID: " + grid.WorldMapPointID);
				}
			}
			partyPosition = default(Position);
			return false;
		}
	}
}
