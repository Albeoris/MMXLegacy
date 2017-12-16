using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.WorldMap
{
	public class WorldMapController : ISaveGameObject
	{
		private Dictionary<Int32, WorldMapPoint> m_worldMapPoints = new Dictionary<Int32, WorldMapPoint>();

		internal WorldMapController(EventManager p_eventManager)
		{
			p_eventManager.RegisterEvent(EEventType.QUESTLOG_CHANGED, new EventHandler(OnQuestChanged));
			p_eventManager.RegisterEvent(EEventType.TOKEN_ADDED, new EventHandler(OnTokenAdded));
			p_eventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
		}

		public event EventHandler MapPointsLoaded;

		public event EventHandler MapPointUpdated;

		internal void Cleanup()
		{
			m_worldMapPoints.Clear();
			LoadWorlMapPoints();
		}

		public WorldMapPoint FindWorldMapPoint(Int32 id)
		{
			WorldMapPoint result;
			m_worldMapPoints.TryGetValue(id, out result);
			return result;
		}

		public IEnumerable<WorldMapPoint> WorldMapPointsIterator()
		{
			return m_worldMapPoints.Values;
		}

		private void LoadWorlMapPoints()
		{
			IEnumerable<WorldMapPointStaticData> iterator = StaticDataHandler.GetIterator<WorldMapPointStaticData>(EDataType.WORLD_MAP);
			foreach (WorldMapPointStaticData worldMapPointStaticData in iterator)
			{
				m_worldMapPoints.Add(worldMapPointStaticData.StaticID, new WorldMapPoint(worldMapPointStaticData));
			}
		}

		internal void OnQuestChanged(Object p_sender, EventArgs p_e)
		{
			QuestChangedEventArgs questChangedEventArgs = (QuestChangedEventArgs)p_e;
			if (questChangedEventArgs.QuestStep != null)
			{
				foreach (WorldMapPoint worldMapPoint in m_worldMapPoints.Values)
				{
					if (worldMapPoint.StaticData.QuestChangedType == questChangedEventArgs.ChangeType && worldMapPoint.StaticData.QuestID == questChangedEventArgs.QuestStep.StaticData.StaticID)
					{
						SetVisible(worldMapPoint);
					}
				}
			}
		}

		internal void OnTokenAdded(Object p_sender, EventArgs p_args)
		{
			Int32 tokenID = ((TokenEventArgs)p_args).TokenID;
			foreach (WorldMapPoint worldMapPoint in m_worldMapPoints.Values)
			{
				if (worldMapPoint.StaticData.StateChangeType == EWorldMapPointStateChangeType.TOKEN_ADDED && worldMapPoint.StaticData.TokenID == tokenID)
				{
					SetVisible(worldMapPoint);
				}
			}
		}

		internal void OnMoveEntity(Object p_sender, EventArgs p_e)
		{
			Party party = p_sender as Party;
			if (party != null && LegacyLogic.Instance.MapLoader.Grid.Type == EMapType.OUTDOOR)
			{
				foreach (WorldMapPoint worldMapPoint in m_worldMapPoints.Values)
				{
					if (worldMapPoint.StaticData.StateChangeType == EWorldMapPointStateChangeType.PARTY_POSITION_CHANGED && Position.Distance(party.Position, worldMapPoint.StaticData.Position) <= party.ExploreRange)
					{
						SetVisible(worldMapPoint);
					}
				}
			}
		}

		internal void SetVisible(WorldMapPoint point)
		{
			if (point.CurrentState != EWorldMapPointState.VISIBLE)
			{
				point.CurrentState = EWorldMapPointState.VISIBLE;
				MapPointVisibleEventArgs mapPointVisibleEventArgs = new MapPointVisibleEventArgs(point);
				if (MapPointUpdated != null)
				{
					MapPointUpdated(this, mapPointVisibleEventArgs);
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.WORLDMAP_LOCATION_ADDED, mapPointVisibleEventArgs);
				MapPointAddedEventArgs p_args = new MapPointAddedEventArgs(point);
				LegacyLogic.Instance.ActionLog.PushEntry(p_args);
			}
		}

		public void Load(SaveGameData p_data)
		{
			m_worldMapPoints.Clear();
			Int32 num = p_data.Get<Int32>("WorldMapCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("WorldMapPoint" + i, null);
				if (saveGameData != null)
				{
					Int32 num2 = saveGameData.Get<Int32>("StaticID", 0);
					WorldMapPointStaticData staticData = StaticDataHandler.GetStaticData<WorldMapPointStaticData>(EDataType.WORLD_MAP, num2);
					if (staticData != null)
					{
						WorldMapPoint worldMapPoint = new WorldMapPoint(staticData);
						worldMapPoint.Load(saveGameData);
						m_worldMapPoints.Add(num2, worldMapPoint);
					}
				}
			}
			if (MapPointsLoaded != null)
			{
				MapPointsLoaded(this, EventArgs.Empty);
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("WorldMapCount", m_worldMapPoints.Count);
			Int32 num = 0;
			foreach (WorldMapPoint worldMapPoint in m_worldMapPoints.Values)
			{
				SaveGameData saveGameData = new SaveGameData("WorldMapPoint" + num);
				worldMapPoint.Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
				num++;
			}
		}
	}
}
