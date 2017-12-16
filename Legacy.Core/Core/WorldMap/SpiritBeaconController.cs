using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Utilities;

namespace Legacy.Core.WorldMap
{
	public class SpiritBeaconController
	{
		private SpiritBeaconPosition m_SpiritBeacon;

		private Boolean m_Existent;

		public SpiritBeaconController(EventManager p_eventManager)
		{
			m_SpiritBeacon = default(SpiritBeaconPosition);
			m_Existent = false;
		}

		public SpiritBeaconPosition SpiritBeacon => m_SpiritBeacon;

	    public Boolean Existent => m_Existent;

	    public ESpiritBeaconAction Action { get; set; }

		internal void ExecuteAction()
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			ESpiritBeaconAction action = Action;
			if (action != ESpiritBeaconAction.TRAVEL)
			{
				if (action == ESpiritBeaconAction.SET_POINT)
				{
					if (party.Position != m_SpiritBeacon.Position || grid.Name != m_SpiritBeacon.Mapname || grid.LocationLocaName != m_SpiritBeacon.LocalizedMapnameKey)
					{
						m_SpiritBeacon = new SpiritBeaconPosition(party.Position, grid.Name, grid.LocationLocaName, grid.WorldMapPointID);
						m_Existent = true;
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SPIRIT_BEACON_UPDATE, EventArgs.Empty);
					}
				}
			}
			else if (m_Existent)
			{
				if (grid.Name != m_SpiritBeacon.Mapname)
				{
					party.SelectedInteractiveObject = null;
					LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
					LegacyLogic.Instance.WorldManager.HighestSaveGameNumber++;
					LegacyLogic.Instance.WorldManager.CurrentSaveGameType = ESaveGameType.AUTO;
					LegacyLogic.Instance.WorldManager.SaveGameName = Localization.Instance.GetText("SAVEGAMETYPE_AUTO");
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_STARTED_SAVE, EventArgs.Empty);
				}
				else if (party.Position != m_SpiritBeacon.Position)
				{
					OnPartyPositionSet(null, EventArgs.Empty);
				}
			}
			else
			{
				LegacyLogger.LogError("No beacon defined cannot travel!");
			}
			Action = ESpiritBeaconAction.None;
		}

		internal void Clear()
		{
			m_SpiritBeacon = default(SpiritBeaconPosition);
			m_Existent = false;
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SPIRIT_BEACON_UPDATE, EventArgs.Empty);
		}

		internal void Save(SaveGameData p_data)
		{
			p_data.Set<Boolean>("Existent", m_Existent);
			p_data.Set<Int32>("X", m_SpiritBeacon.Position.X);
			p_data.Set<Int32>("Y", m_SpiritBeacon.Position.Y);
			p_data.Set<String>("MapName", m_SpiritBeacon.Mapname ?? String.Empty);
			p_data.Set<String>("LocaMapName", m_SpiritBeacon.LocalizedMapnameKey ?? String.Empty);
			p_data.Set<Int32>("MapPointID", m_SpiritBeacon.MapPointID);
		}

		internal void Load(SaveGameData p_data)
		{
			m_Existent = p_data.Get<Boolean>("Existent", false);
			Int32 x = p_data.Get<Int32>("X", 0);
			Int32 y = p_data.Get<Int32>("Y", 0);
			String p_mapname = p_data.Get<String>("MapName", null);
			String p_locaKey = p_data.Get<String>("LocaMapName", null);
			Int32 p_worldPointID = p_data.Get<Int32>("MapPointID", 0);
			Position p_position = new Position(x, y);
			m_SpiritBeacon = new SpiritBeaconPosition(p_position, p_mapname, p_locaKey, p_worldPointID);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SPIRIT_BEACON_UPDATE, EventArgs.Empty);
		}

		private void OnGameSaved(Object sender, EventArgs e)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.SAVEGAME_SAVED, new EventHandler(OnGameSaved));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnPartyPositionSet));
			LegacyLogic.Instance.MapLoader.LoadMap(m_SpiritBeacon.Mapname + ".xml");
		}

		private void OnPartyPositionSet(Object p_sender, EventArgs p_args)
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.FINISH_SCENE_LOAD, new EventHandler(OnPartyPositionSet));
			Position position = m_SpiritBeacon.Position;
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Position position2 = party.Position;
			if (grid.AddMovingEntity(position, party))
			{
				Boolean flag = grid.GetSlot(position2).RemoveEntity(party);
				party.Position = position;
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(party, position);
				LegacyLogic.Instance.EventManager.InvokeEvent(party, EEventType.SET_ENTITY_POSITION, p_eventArgs);
			}
		}
	}
}
