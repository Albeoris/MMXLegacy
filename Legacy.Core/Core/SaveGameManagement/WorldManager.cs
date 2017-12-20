using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Legacy.Core.Achievements;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.AI.MonsterGroups;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.Lorebook;
using Legacy.Core.Map;
using Legacy.Core.Map.Notes;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;
using Legacy.Core.WorldMap;
using Legacy.Utilities;

namespace Legacy.Core.SaveGameManagement
{
	public class WorldManager
	{
		private const Int32 DYNAMIC_SPAWN_ID_OFFSET = 1073741823;

		private const Int32 SAVEGAME_VERSION = 2;

		private Dictionary<String, MapData> m_mapData;

		private Party m_party;

		private Boolean m_checkPassableOnMovement;

		private String m_lastLoadedSaveGame = String.Empty;

		private EDifficulty m_difficulty;

		private Int32 m_dynamicSpawnIdCount;

		private NpcFactory m_npcFactory;

		private PartyCreator m_partyCreator;

		private WorldMapController m_worldMapController;

		private BestiaryHandler m_bestiaryHandler;

		private LoreBookHandler m_loreBookHandler;

		private HintManager m_hintManager;

		private MapNotesController m_mapNotesController;

		private AchievementManager m_achievementManager;

		private MonsterGroupHandler m_monsterGroupHandler;

		private ISaveGameManager m_saveGameManager;

		private SpiritBeaconController m_spiritBeaconController;

		private QuestHandler m_questHandler;

		public static String CurrentSaveGameFolder = String.Empty;

		private List<BaseObject> m_objects;

		private ReadOnlyCollection<BaseObject> m_publicObjects;

		private List<Int32> m_invalidSpawner;

		private String m_saveGameName = String.Empty;

		internal WorldManager(EventManager p_eventManager)
		{
			CurrentSaveGameFolder = GamePaths.UserGamePath;
			QuickSaveAllowed = true;
			m_mapData = new Dictionary<String, MapData>();
			m_objects = new List<BaseObject>();
			m_invalidSpawner = new List<Int32>();
			m_questHandler = new QuestHandler();
			m_loreBookHandler = new LoreBookHandler();
			m_bestiaryHandler = new BestiaryHandler();
			m_achievementManager = new AchievementManager();
			m_hintManager = new HintManager();
			m_mapNotesController = new MapNotesController(p_eventManager);
			m_npcFactory = new NpcFactory(p_eventManager);
			m_saveGameManager = new DefaultSaveGameManager();
			m_spiritBeaconController = new SpiritBeaconController(p_eventManager);
			m_worldMapController = new WorldMapController(p_eventManager);
		}

		public ISaveGameManager SaveGameManager
		{
			get => m_saveGameManager;
		    set => m_saveGameManager = value;
		}

		public String SaveGameName
		{
			get => m_saveGameName;
		    set => m_saveGameName = value;
		}

		public Int32 HighestSaveGameNumber { get; set; }

		public ESaveGameType CurrentSaveGameType { get; set; }

		public Boolean IsSaveGame { get; set; }

		public Boolean LoadedFromStartMenu { get; set; }

		public Boolean QuickSaveAllowed { get; set; }

		internal Dictionary<String, MapData> MapData => m_mapData;

	    public List<Int32> InvalidSpawner => m_invalidSpawner;

	    public Boolean IsShowingEndingSequences { get; set; }

		public Boolean IsShowingDLCEndingSequences { get; set; }

		public AchievementManager AchievementManager => m_achievementManager;

	    public Party Party
		{
			get
			{
				if (m_party == null)
				{
					m_party = PartyCreator.CurrentParty;
				}
				return m_party;
			}
		}

		public WorldMapController WorldMapController => m_worldMapController;

	    public NpcFactory NpcFactory => m_npcFactory;

	    public PartyCreator PartyCreator
		{
			get
			{
				if (m_partyCreator == null)
				{
					m_partyCreator = new PartyCreator();
				}
				return m_partyCreator;
			}
		}

		public MonsterGroupHandler MonsterGroupHandler
		{
			get
			{
				if (m_monsterGroupHandler == null)
				{
					m_monsterGroupHandler = new MonsterGroupHandler();
				}
				return m_monsterGroupHandler;
			}
		}

		public QuestHandler QuestHandler => m_questHandler;

	    public LoreBookHandler LoreBookHandler => m_loreBookHandler;

	    public BestiaryHandler BestiaryHandler => m_bestiaryHandler;

	    public HintManager HintManager => m_hintManager;

	    public MapNotesController MapNotesController => m_mapNotesController;

	    public SpiritBeaconController SpiritBeaconController => m_spiritBeaconController;

	    public ReadOnlyCollection<BaseObject> Objects
		{
			get
			{
				ReadOnlyCollection<BaseObject> result;
				if ((result = m_publicObjects) == null)
				{
					result = (m_publicObjects = m_objects.AsReadOnly());
				}
				return result;
			}
		}

		public EDifficulty Difficulty
		{
			get => m_difficulty;
		    set => m_difficulty = value;
		}

		public Single ItemResellMultiplicator
		{
			get
			{
				if (Difficulty == EDifficulty.NORMAL)
				{
					return ConfigManager.Instance.Game.ItemResellMultiplicator;
				}
				return ConfigManager.Instance.Game.ItemResellMultiplicatorHard;
			}
		}

		public Single DamageReceiveMultiplicator
		{
			get
			{
				if (Difficulty == EDifficulty.NORMAL)
				{
					return 1f;
				}
				return ConfigManager.Instance.Game.DamageReceiveFactor;
			}
		}

		public Boolean CheckPassableOnMovement
		{
			get => m_checkPassableOnMovement;
		    set => m_checkPassableOnMovement = value;
		}

		public Int32 GetNextDynamicSpawnID()
		{
			m_dynamicSpawnIdCount++;
			return 1073741823 + m_dynamicSpawnIdCount;
		}

		public void LoadLastLoadedSaveGame()
		{
			if (!String.IsNullOrEmpty(m_lastLoadedSaveGame))
			{
				Load(m_lastLoadedSaveGame);
			}
		}

		public void Load(String p_file)
		{
			m_party = null;
			m_mapData.Clear();
			m_objects.Clear();
			m_npcFactory.Clear();
			SaveGame saveGame = m_saveGameManager.LoadSaveGame(p_file);
			if (saveGame == null)
			{
				return;
			}
			m_lastLoadedSaveGame = p_file;
			SaveGameData saveGameData = saveGame["MainData"];
			Int32 num = saveGameData.Get<Int32>("Version", -1);
			String mapName = saveGameData.Get<String>("CurrentMapName", String.Empty);
			m_difficulty = (EDifficulty)saveGameData.Get<Int32>("Difficulty", 0);
			m_dynamicSpawnIdCount = saveGameData.Get<Int32>("DynamicSpawnIdCount", 0);
			m_checkPassableOnMovement = saveGameData.Get<Boolean>("CheckPassable", false);
			SaveGameData p_data = saveGame["Party"];
			SaveGameData p_data2 = saveGame["MapData"];
			SaveGameData p_data3 = saveGame["QuestData"];
			SaveGameData p_data4 = saveGame["GameTime"];
			SaveGameData p_data5 = saveGame["WorldMap"];
			SaveGameData p_data6 = saveGame["NpcFactory"];
			SaveGameData p_data7 = saveGame["SpiritBeaconPosition"];
			SaveGameData p_data8 = saveGame["Achievements"];
			SaveGameData p_data9 = saveGame["Lorebooks"];
			SaveGameData p_data10 = saveGame["Bestiary"];
			SaveGameData mapdata = saveGame["MapNotes"];
			LoadParty(p_data);
			m_npcFactory.Load(p_data6);
			LoadMapData(p_data2);
			LoadQuests(p_data3);
			LoadGameTime(p_data4);
			LoadWorldMap(p_data5);
			m_spiritBeaconController.Load(p_data7);
			LoadAchievements(p_data8);
			LoadLoreBooks(p_data9);
			LoadBestiary(p_data10);
			m_mapNotesController.Load(mapdata);
			m_hintManager.Load();
			m_questHandler.RepairLorebookQuest();
			m_questHandler.RepairObeliskQuest();
			m_questHandler.RemoveUnnecessaryTokens();
			m_questHandler.FixBattleOfKarthal();
			LoadGame(mapName);
			Party.CheckUnlockUPlayPrivilegesRewards();
			ItemFactory.InitItemProbabilities();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SAVEGAME_LOADED, EventArgs.Empty);
		}

		public void SaveCurrentMapData()
		{
			MapData mapData = CollectData();
			m_mapData[mapData.Name] = mapData;
		}

		public void Save(String p_file, Byte[] p_screenshot)
		{
			if (m_party.InCombat)
			{
				return;
			}
			String name = LegacyLogic.Instance.MapLoader.Grid.Name;
			m_lastLoadedSaveGame = p_file;
			MapData mapData = CollectData();
			m_mapData[mapData.Name] = mapData;
			SaveGameMeta p_meta = new SaveGameMeta(DateTime.Now, new TimeSpan(LegacyLogic.Instance.GameTime.Time.Days, LegacyLogic.Instance.GameTime.Time.Hours, 0, 0), HighestSaveGameNumber, CurrentSaveGameType, m_difficulty);
			SaveGame saveGame = new SaveGame(p_meta);
			SaveGameData saveGameData = new SaveGameData("MainData");
			saveGameData.Set<Int32>("Version", 2);
			saveGameData.Set<String>("CurrentMapName", name);
			saveGameData.Set<Int32>("Difficulty", (Int32)m_difficulty);
			saveGameData.Set<Int32>("DynamicSpawnIdCount", m_dynamicSpawnIdCount);
			saveGameData.Set<Boolean>("CheckPassable", m_checkPassableOnMovement);
			saveGame.Add(saveGameData);
			SaveGameData p_data = new SaveGameData("Party");
			SaveGameData p_data2 = new SaveGameData("MapData");
			SaveGameData p_data3 = new SaveGameData("QuestData");
			SaveGameData p_data4 = new SaveGameData("GameTime");
			SaveGameData p_data5 = new SaveGameData("WorldMap");
			SaveGameData p_data6 = new SaveGameData("NpcFactory");
			SaveGameData p_data7 = new SaveGameData("SpiritBeaconPosition");
			SaveGameData p_data8 = new SaveGameData("Achievements");
			SaveGameData p_data9 = new SaveGameData("Lorebooks");
			SaveGameData p_data10 = new SaveGameData("Bestiary");
			SaveGameData saveGameData2 = new SaveGameData("MapNotes");
			m_party.Save(p_data);
			m_npcFactory.Save(p_data6);
			SaveMapData(p_data2);
			m_questHandler.Save(p_data3);
			LegacyLogic.Instance.GameTime.Save(p_data4);
			m_worldMapController.Save(p_data5);
			m_spiritBeaconController.Save(p_data7);
			m_achievementManager.Save(p_data8);
			m_loreBookHandler.Save(p_data9);
			m_bestiaryHandler.Save(p_data10);
			m_mapNotesController.Save(saveGameData2);
			saveGame.Add(p_data);
			saveGame.Add(p_data2);
			saveGame.Add(p_data3);
			saveGame.Add(p_data4);
			saveGame.Add(p_data5);
			saveGame.Add(p_data6);
			saveGame.Add(p_data7);
			saveGame.Add(p_data8);
			saveGame.Add(p_data9);
			saveGame.Add(p_data10);
			saveGame.Add(saveGameData2);
			m_saveGameManager.SaveSaveGame(saveGame, p_file, p_screenshot);
			m_hintManager.Save();
		}

		public static Boolean HasCheatsCheckFile()
		{
			return File.Exists(Path.Combine(GamePaths.UserGamePath, "global3.lsg"));
		}

		private void SaveMapData(SaveGameData p_data)
		{
			p_data.Set<Int32>("MapDataCount", m_mapData.Count);
			Int32 num = 0;
			foreach (MapData mapData in m_mapData.Values)
			{
				SaveGameData saveGameData = new SaveGameData("MapData" + num);
				mapData.Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
				num++;
			}
		}

		private void LoadParty(SaveGameData p_data)
		{
			if (m_party != null)
			{
				m_party.Destroy();
			}
			m_party = new Party();
			m_party.Load(p_data);
		}

		private void LoadMapData(SaveGameData p_data)
		{
			Int32 num = p_data.Get<Int32>("MapDataCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				SaveGameData saveGameData = p_data.Get<SaveGameData>("MapData" + i, null);
				if (saveGameData != null)
				{
					MapData mapData = new MapData();
					mapData.Load(saveGameData);
					if (!m_mapData.ContainsKey(mapData.Name))
					{
						m_mapData.Add(mapData.Name, mapData);
					}
					else
					{
						LegacyLogger.Log("A Map Data name conflict! " + mapData.Name);
					}
				}
				else
				{
					LegacyLogger.Log("A Map Data was not found in SaveGame");
				}
			}
		}

		private void LoadQuests(SaveGameData p_data)
		{
			if (m_questHandler != null)
			{
				m_questHandler.Cleanup();
			}
			m_questHandler = new QuestHandler();
			m_questHandler.Initialize();
			m_questHandler.Load(p_data);
		}

		private void LoadGameTime(SaveGameData p_data)
		{
			LegacyLogic.Instance.GameTime.Load(p_data);
		}

		private void LoadWorldMap(SaveGameData p_data)
		{
			WorldMapController.Load(p_data);
		}

		private void LoadLoreBooks(SaveGameData p_data)
		{
			m_loreBookHandler.Cleanup();
			m_loreBookHandler.Load(p_data);
		}

		private void LoadBestiary(SaveGameData p_data)
		{
			if (m_bestiaryHandler != null)
			{
				m_bestiaryHandler.Cleanup();
			}
			m_bestiaryHandler = new BestiaryHandler();
			m_bestiaryHandler.Initialize();
			m_bestiaryHandler.Load(p_data);
		}

		private void LoadAchievements(SaveGameData p_data)
		{
			m_achievementManager.Destroy();
			m_achievementManager.Initialize();
			m_achievementManager.Load(p_data);
		}

		internal MapData CollectData()
		{
			MapData mapData = new MapData(LegacyLogic.Instance.MapLoader.Grid.Name);
			GetObjectsByType<InteractiveObject>(mapData.Objects);
			for (Int32 i = 0; i < mapData.Objects.Count; i++)
			{
				mapData.Objects[i].ClearInteractions();
			}
			GetObjectsByType<Monster>(mapData.Monster);
			mapData.InvalidSpawns.AddRange(m_invalidSpawner);
			mapData.SaveTerrainDataMatrix(LegacyLogic.Instance.MapLoader.Grid);
			return mapData;
		}

		private void LoadGame(String mapName)
		{
			for (Int32 i = 0; i < m_objects.Count; i++)
			{
				m_objects[i].Destroy();
			}
			m_objects.Clear();
			m_invalidSpawner.Clear();
			LegacyLogic.Instance.MapLoader.IsSaveGame = true;
			HighestSaveGameNumber = 0;
			Dictionary<String, SaveGameMeta> allSaveGames = m_saveGameManager.GetAllSaveGames(false);
			foreach (SaveGameMeta saveGameMeta in allSaveGames.Values)
			{
				HighestSaveGameNumber = Math.Max(HighestSaveGameNumber, saveGameMeta.SaveNumber);
			}
			LegacyLogic.Instance.MapLoader.LoadMap(mapName + ".xml");
			IsSaveGame = false;
		}

		public void CheatToLevel(String p_mapFile)
		{
			LegacyLogic.Instance.MapLoader.LoadMap(p_mapFile);
		}

		public void ClearAndDestroy()
		{
			foreach (MapData mapData in m_mapData.Values)
			{
				mapData.Destroy();
			}
			m_mapData.Clear();
			if (m_party != null)
			{
				m_partyCreator.ClearAndDestroy();
				m_party = null;
			}
			for (Int32 i = 0; i < m_objects.Count; i++)
			{
				m_objects[i].Destroy();
			}
			ClearObjectsList();
			LegacyLogic.Instance.Reset();
			m_questHandler.ClearQuests();
			m_questHandler.Initialize();
			m_spiritBeaconController.Clear();
			m_npcFactory.Clear();
			if (m_achievementManager != null)
			{
				m_achievementManager.Destroy();
				m_achievementManager = new AchievementManager();
				m_achievementManager.Initialize();
			}
			if (m_bestiaryHandler != null)
			{
				m_bestiaryHandler.Cleanup();
				m_bestiaryHandler = new BestiaryHandler();
				m_bestiaryHandler.Initialize();
			}
			if (m_loreBookHandler != null)
			{
				m_loreBookHandler.Cleanup();
			}
			m_worldMapController.Cleanup();
			m_mapNotesController.Clear();
			DestroyMonsterGroups();
			LegacyLogic.Instance.MapLoader.Grid = null;
			LegacyLogic.Instance.UpdateManager.ClearAndDestroy();
		}

		internal Boolean SpawnObject(BaseObject p_obj, Position position)
		{
			if (!m_objects.Contains(p_obj))
			{
				m_objects.Add(p_obj);
				LegacyLogic.Instance.UpdateManager.AddEntity(p_obj);
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(p_obj, position);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SPAWN_BASEOBJECT, p_eventArgs);
				return true;
			}
			return false;
		}

		internal Boolean DestroyObject(BaseObject p_obj, Position position)
		{
			if (m_objects.Remove(p_obj))
			{
				LegacyLogic.Instance.UpdateManager.RemoveEntity(p_obj);
				MovingEntity movingEntity = p_obj as MovingEntity;
				if (movingEntity != null)
				{
					GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(movingEntity.Position);
					if (slot != null)
					{
						slot.RemoveEntity(movingEntity);
					}
				}
				BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(p_obj, position);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.DESTROY_BASEOBJECT, p_eventArgs);
				return true;
			}
			return false;
		}

		public BaseObject FindObject(Int32 spawnerId)
		{
			foreach (BaseObject baseObject in m_objects)
			{
				if (baseObject != null && baseObject.SpawnerID == spawnerId)
				{
					return baseObject;
				}
			}
			return null;
		}

		public T FindObjectBySpawnerId<T>(Int32 p_spawnerId) where T : BaseObject
		{
			for (Int32 i = 0; i < m_objects.Count; i++)
			{
				if (m_objects[i].SpawnerID == p_spawnerId && m_objects[i] is T)
				{
					return (T)m_objects[i];
				}
			}
			return null;
		}

		public BaseObject FindObjectBySpawnerId(Int32 p_spawnerId)
		{
			return FindObjectBySpawnerId<BaseObject>(p_spawnerId);
		}

		public List<T> GetObjectsByType<T>() where T : BaseObject
		{
			List<T> list = new List<T>();
			GetObjectsByType<T>(list);
			return list;
		}

		public Int32 GetObjectsByType<T>(List<T> p_results) where T : BaseObject
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_objects.Count; i++)
			{
				if (m_objects[i] is T)
				{
					p_results.Add((T)m_objects[i]);
					num++;
				}
			}
			return num;
		}

		public IEnumerable<T> GetObjectsByTypeIterator<T>() where T : BaseObject
		{
			for (Int32 i = 0; i < m_objects.Count; i++)
			{
				if (m_objects[i] is T)
				{
					yield return (T)m_objects[i];
				}
			}
			yield break;
		}

		public void ClearObjectsList()
		{
			m_objects.Clear();
			m_invalidSpawner.Clear();
			DestroyMonsterGroups();
		}

		internal void DestroyMonsterGroups()
		{
			if (m_monsterGroupHandler != null)
			{
				m_monsterGroupHandler.Clear();
			}
			m_monsterGroupHandler = null;
		}
	}
}
