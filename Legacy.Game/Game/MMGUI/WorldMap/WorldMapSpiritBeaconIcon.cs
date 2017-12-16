using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.WorldMap;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.WorldMap
{
	[AddComponentMenu("MM Legacy/MMGUI/WorldMap spirit beacon icon")]
	internal class WorldMapSpiritBeaconIcon : MonoBehaviour
	{
		[SerializeField]
		private WorldMapUserMapNoteController m_UserMapNoteController;

		private void Awake()
		{
			OnSpiritBeaconUpdate(null, null);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SPIRIT_BEACON_UPDATE, new EventHandler(OnSpiritBeaconUpdate));
		}

		private void OnDestroy()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SPIRIT_BEACON_UPDATE, new EventHandler(OnSpiritBeaconUpdate));
		}

		private void OnEnable()
		{
			OnSpiritBeaconUpdate(null, null);
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnSpiritBeaconUpdate(Object sender, EventArgs e)
		{
			gameObject.SetActive(LegacyLogic.Instance.WorldManager.SpiritBeaconController.Existent);
			SpiritBeaconPosition spiritBeacon = LegacyLogic.Instance.WorldManager.SpiritBeaconController.SpiritBeacon;
			Vector3 worldMapPosition;
			if (spiritBeacon.MapPointID != 0)
			{
				WorldMapPoint worldMapPoint = LegacyLogic.Instance.WorldManager.WorldMapController.FindWorldMapPoint(spiritBeacon.MapPointID);
				if (worldMapPoint == null)
				{
					Debug.LogError("Beacon mappoint position not found! ID: " + spiritBeacon.MapPointID);
					gameObject.SetActive(false);
					return;
				}
				worldMapPosition = WorldMapController.GetWorldMapPosition(worldMapPoint.StaticData.Position);
			}
			else
			{
				worldMapPosition = WorldMapController.GetWorldMapPosition(spiritBeacon.Position);
			}
			worldMapPosition.z = -1f;
			transform.localPosition = worldMapPosition;
		}

		private void OnClick()
		{
			m_UserMapNoteController.OnClick();
		}

		private void OnTooltip(Boolean show)
		{
			if (show)
			{
				String text = LocaManager.GetText(LegacyLogic.Instance.WorldManager.SpiritBeaconController.SpiritBeacon.LocalizedMapnameKey);
				if (text.LastIndexOf('@') != -1)
				{
					text = text.Remove(text.LastIndexOf('@'));
				}
				String p_mapObjectNote = LocaManager.GetText("WORLDMAP_SPIRIT_BEACON") + " - " + text;
				TooltipManager.Instance.Show(this, p_mapObjectNote, m_UserMapNoteController.GetUserMapNoteText(Input.mousePosition), transform.position, new Vector3(24f, 24f, 0f));
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
