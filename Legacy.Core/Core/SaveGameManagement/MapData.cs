using System;
using System.Collections.Generic;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Utilities;

namespace Legacy.Core.SaveGameManagement
{
	public class MapData : ISaveGameObject
	{
		private String m_name;

		private List<InteractiveObject> m_objects;

		private List<Monster> m_monster;

		private List<Int32> m_invalidSpawns;

		private Int32 m_terrainDataWidth;

		private Int32 m_terrainDataHeight;

		private TerrainTypeData[] m_terrainData;

		public MapData() : this(null)
		{
		}

		public MapData(String p_name)
		{
			m_name = p_name;
			m_objects = new List<InteractiveObject>();
			m_monster = new List<Monster>();
			m_invalidSpawns = new List<Int32>();
		}

		public String Name => m_name;

	    public List<InteractiveObject> Objects => m_objects;

	    public List<Monster> Monster => m_monster;

	    public List<Int32> InvalidSpawns => m_invalidSpawns;

	    public void Destroy()
		{
			for (Int32 i = 0; i < m_objects.Count; i++)
			{
				m_objects[i].Destroy();
			}
			m_objects.Clear();
			for (Int32 j = 0; j < m_monster.Count; j++)
			{
				m_monster[j].Destroy();
			}
			m_monster.Clear();
			m_name = null;
			m_invalidSpawns.Clear();
			m_terrainData = null;
		}

		public TerrainTypeData GetTerrainData(Position p_pos)
		{
			Int32 num = p_pos.Y * m_terrainDataWidth + p_pos.X;
			return m_terrainData[num];
		}

		public void SaveTerrainDataMatrix(Grid p_grid)
		{
			m_terrainDataWidth = p_grid.Width;
			m_terrainDataHeight = p_grid.Height;
			m_terrainData = new TerrainTypeData[p_grid.Width * p_grid.Height];
			for (Int32 i = 0; i < p_grid.Height; i++)
			{
				for (Int32 j = 0; j < p_grid.Width; j++)
				{
					GridSlot slot = p_grid.GetSlot(new Position(j, i));
					m_terrainData[i * p_grid.Width + j] = new TerrainTypeData(slot.TerrainType, slot.VisitedByParty, slot.Height);
				}
			}
		}

		public void LoadTerrainDataMatrix(Grid p_grid)
		{
			if (m_terrainData == null)
			{
				return;
			}
			Int32 num = 0;
			while (num < m_terrainDataHeight && num < p_grid.Height)
			{
				Int32 num2 = 0;
				while (num2 < m_terrainDataWidth && num2 < p_grid.Width)
				{
					GridSlot slot = p_grid.GetSlot(new Position(num2, num));
					TerrainTypeData terrainTypeData = m_terrainData[num * m_terrainDataWidth + num2];
					slot.VisitedByParty = terrainTypeData.Visited;
					slot.TerrainType = terrainTypeData.Type;
					slot.Height = ((terrainTypeData.Height != Single.MaxValue) ? terrainTypeData.Height : slot.Height);
					num2++;
				}
				num++;
			}
		}

		public void Load(SaveGameData p_data)
		{
			m_name = p_data.Get<String>("Name", null);
			Int32 num = p_data.Get<Int32>("ObjectCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("InteractiveObject" + i, null);
				if (saveGameData != null)
				{
					try
					{
						Int32 p_staticID = saveGameData.Get<Int32>("StaticID", 0);
						Int32 p_spawnID = saveGameData.Get<Int32>("SpawnerID", 0);
						EObjectType p_type = saveGameData.Get<EObjectType>("ObjectType", EObjectType.NONE);
						InteractiveObject interactiveObject = (InteractiveObject)EntityFactory.Create(p_type, p_staticID, p_spawnID);
						interactiveObject.Load(saveGameData);
						m_objects.Add(interactiveObject);
					}
					catch (Exception p_message)
					{
						LegacyLogger.LogError(p_message);
					}
				}
			}
			Int32 num2 = p_data.Get<Int32>("MonsterCount", 0);
			for (Int32 j = 0; j < num2; j++)
			{
				SaveGameData saveGameData2 = p_data.Get<SaveGameData>("Monster" + j, null);
				if (saveGameData2 != null)
				{
					try
					{
						Monster monster = new Monster();
						monster.Load(saveGameData2);
						m_monster.Add(monster);
					}
					catch (Exception p_message2)
					{
						LegacyLogger.LogError(p_message2);
					}
				}
			}
			SaveGameData saveGameData3 = p_data.Get<SaveGameData>("InvalidSpawns", null);
			if (saveGameData3 != null)
			{
				List<Int32> collection = SaveGame.CreateArrayFromData<Int32>(saveGameData3);
				m_invalidSpawns.AddRange(collection);
			}
			SaveGameData saveGameData4 = p_data.Get<SaveGameData>("TerrainMatrix", null);
			if (saveGameData4 != null)
			{
				m_terrainDataHeight = saveGameData4.Get<Int32>("MatrixHeight", 0);
				m_terrainDataWidth = saveGameData4.Get<Int32>("MatrixWidth", 0);
				m_terrainData = new TerrainTypeData[m_terrainDataWidth * m_terrainDataHeight];
				for (Int32 k = 0; k < m_terrainDataHeight; k++)
				{
					for (Int32 l = 0; l < m_terrainDataWidth; l++)
					{
						SaveGameData saveGameData5 = saveGameData4.Get<SaveGameData>(String.Concat(new Object[]
						{
							"terrain_",
							l,
							"_",
							k
						}), null);
						if (saveGameData5 != null)
						{
							TerrainTypeData terrainTypeData = default(TerrainTypeData);
							terrainTypeData.Load(saveGameData5);
							m_terrainData[k * m_terrainDataWidth + l] = terrainTypeData;
						}
					}
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<String>("Name", m_name);
			p_data.Set<Int32>("ObjectCount", m_objects.Count);
			for (Int32 i = 0; i < m_objects.Count; i++)
			{
				SaveGameData saveGameData = new SaveGameData("InteractiveObject" + i);
				m_objects[i].Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
			p_data.Set<Int32>("MonsterCount", m_monster.Count);
			for (Int32 j = 0; j < m_monster.Count; j++)
			{
				SaveGameData saveGameData2 = new SaveGameData("Monster" + j);
				m_monster[j].Save(saveGameData2);
				p_data.Set<SaveGameData>(saveGameData2.ID, saveGameData2);
			}
			SaveGameData saveGameData3 = SaveGame.CreateDataFromArray<Int32>("InvalidSpawns", m_invalidSpawns);
			p_data.Set<SaveGameData>(saveGameData3.ID, saveGameData3);
			SaveGameData saveGameData4 = new SaveGameData("TerrainMatrix");
			saveGameData4.Set<Int32>("MatrixHeight", m_terrainDataHeight);
			saveGameData4.Set<Int32>("MatrixWidth", m_terrainDataWidth);
			if (m_terrainData != null)
			{
				for (Int32 k = 0; k < m_terrainDataHeight; k++)
				{
					for (Int32 l = 0; l < m_terrainDataWidth; l++)
					{
						SaveGameData saveGameData5 = new SaveGameData(String.Concat(new Object[]
						{
							"terrain_",
							l,
							"_",
							k
						}));
						m_terrainData[k * m_terrainDataWidth + l].Save(saveGameData5);
						saveGameData4.Set<SaveGameData>(saveGameData5.ID, saveGameData5);
					}
				}
			}
			p_data.Set<SaveGameData>(saveGameData4.ID, saveGameData4);
		}
	}
}
