using System;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.WorldMap
{
	public class WorldMapPoint : ISaveGameObject
	{
		private WorldMapPointStaticData m_staticData;

		private EWorldMapPointState m_currentState;

		public WorldMapPoint(WorldMapPointStaticData staticData)
		{
			m_staticData = staticData;
			m_currentState = staticData.InitialState;
		}

		public EWorldMapPointState CurrentState
		{
			get => m_currentState;
		    set => m_currentState = value;
		}

		public Int32 StaticID => m_staticData.StaticID;

	    public WorldMapPointStaticData StaticData => m_staticData;

	    public void Load(SaveGameData p_data)
		{
			m_currentState = (EWorldMapPointState)p_data.Get<Int32>("CurrentState", 0);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", m_staticData.StaticID);
			p_data.Set<Int32>("CurrentState", (Int32)m_currentState);
		}
	}
}
