using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.Quests;
using Legacy.Core.SaveGameManagement;
using Legacy.Utilities;

namespace Legacy.Core.MapLoading
{
	public sealed class MapLoader
	{
		internal const String MAP_DATA_FILE_EXTENSION = ".xml";

		private Grid m_grid;

		private EMapLoaderState m_state;

		private Boolean m_isSaveGame;

		private Boolean m_wasSaveGame;

		private String m_MapFolder;

		private Dictionary<String, GridInfo> m_GridInfos;

		private Single m_progress;

		private Single m_sceneLoaderProgress;

		private Single m_viewManagerProgress;

		internal MapLoader()
		{
			m_state = EMapLoaderState.IDLE;
		}

		public Boolean IsSaveGame
		{
			get => m_isSaveGame;
		    set => m_isSaveGame = value;
		}

		public String MapFolder
		{
			get => m_MapFolder;
		    set
			{
				if (m_MapFolder != value)
				{
					m_MapFolder = value;
					LoadGridInfos();
				}
			}
		}

		public Grid Grid
		{
			get => m_grid;
		    internal set => m_grid = value;
		}

		public String GridFileName { get; private set; }

		public Boolean IsLoading => m_state > EMapLoaderState.IDLE;

	    public EMapLoaderState State => m_state;

	    public Single SceneLoaderProgress
		{
			get => m_sceneLoaderProgress;
	        set => m_sceneLoaderProgress = value;
	    }

		public Single ViewManagerProgress
		{
			get => m_viewManagerProgress;
		    set => m_viewManagerProgress = value;
		}

		public GridInfo FindGridInfo(String mapName)
		{
			GridInfo result;
			if (m_GridInfos != null && mapName != null && m_GridInfos.TryGetValue(mapName, out result))
			{
				return result;
			}
			return null;
		}

		public void FinishedLoadScene()
		{
			if (m_state == EMapLoaderState.LOADING_SCENE)
			{
				m_state++;
			}
		}

		public void FinishedLoadViews()
		{
			if (m_state == EMapLoaderState.LOAD_VIEWS)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.FINISH_LOAD_VIEWS, EventArgs.Empty);
				EndLoading();
			}
		}

		public void OnLevelLoaded()
		{
			List<InteractiveObject> list = new List<InteractiveObject>();
			for (Int32 i = 0; i < m_grid.Width; i++)
			{
				for (Int32 j = 0; j < m_grid.Height; j++)
				{
					IList<InteractiveObject> interactiveObjects = m_grid.GetSlot(new Position(i, j)).InteractiveObjects;
					list.AddRange(interactiveObjects);
				}
			}
			for (Int32 k = 0; k < list.Count; k++)
			{
				list[k].OnLevelLoaded(m_grid);
			}
		}

		private void LoadGridInfos()
		{
			if (Directory.Exists(m_MapFolder))
			{
				if (m_GridInfos == null)
				{
					m_GridInfos = new Dictionary<String, GridInfo>(StringComparer.OrdinalIgnoreCase);
				}
				m_GridInfos.Clear();
				XmlRootAttribute root = new XmlRootAttribute("Grid");
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GridInfo), root);
				String[] files = Directory.GetFiles(m_MapFolder, "*.xml", SearchOption.TopDirectoryOnly);
				foreach (String text in files)
				{
					using (FileStream fileStream = File.OpenRead(text))
					{
						try
						{
							GridInfo gridInfo = (GridInfo)xmlSerializer.Deserialize(fileStream);
							if (gridInfo == null)
							{
								LegacyLogger.LogError("Error load GridInfo in " + text);
							}
							else if (!m_GridInfos.ContainsKey(gridInfo.Name))
							{
								m_GridInfos.Add(gridInfo.Name, gridInfo);
							}
							else
							{
								LegacyLogger.LogError("GridInfo name conflict " + gridInfo.Name + "\n" + text);
							}
						}
						catch (Exception p_message)
						{
							LegacyLogger.LogError(p_message);
						}
					}
				}
			}
		}

		internal void LoadStartMap()
		{
			String mapStart = ConfigManager.Instance.Game.MapStart;
			LoadMap(mapStart + ".xml");
		}

		internal void LoadEndMap()
		{
			String mapAfterEndingSequences = ConfigManager.Instance.Game.MapAfterEndingSequences;
			LegacyLogic.Instance.WorldManager.Party.StartSpawnerID = ConfigManager.Instance.Game.SpawnerAfterEndingSequences;
			LoadMap(mapAfterEndingSequences + ".xml");
		}

		internal void LoadMap(String p_fileName)
		{
			if (IsLoading)
			{
				return;
			}
			String text = Path.Combine(MapFolder, p_fileName);
			LegacyLogic.Instance.WorldManager.ClearObjectsList();
			LegacyLogic.Instance.UpdateManager.Clear();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHANGE_MAP, new ChangeMapEventArgs(text));
			m_grid = GridLoader.Load(text);
			if (m_grid == null)
			{
				LegacyLogger.LogError("Fail load Grid! '" + p_fileName + "'");
				return;
			}
			GridFileName = p_fileName;
			m_state = EMapLoaderState.LOADING_SCENE;
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.START_SCENE_LOAD, new StartSceneLoadEventArgs(m_grid.SceneName));
		}

		private void LoadDynamicObjects()
		{
			List<Int32> invalidSpawnList = null;
			MapData mapData;
			LegacyLogic.Instance.WorldManager.MapData.TryGetValue(m_grid.Name, out mapData);
			if (mapData != null)
			{
				invalidSpawnList = mapData.InvalidSpawns;
				LegacyLogic.Instance.WorldManager.InvalidSpawner.Clear();
				LegacyLogic.Instance.WorldManager.InvalidSpawner.AddRange(mapData.InvalidSpawns);
			}
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Position position;
			EDirection direction;
			HashSet<Int32> spawnerIds = LoadSpawnerFromGrid(invalidSpawnList, party.StartSpawnerID, out position, out direction);
			if (m_isSaveGame)
			{
				m_grid.RemoveMovingEntity(party);
				m_grid.AddMovingEntity(party.Position, party);
			}
			else
			{
				m_grid.RemoveMovingEntity(party);
				party.Position = position;
				party.Direction = direction;
				m_grid.AddMovingEntity(party.Position, party);
			}
			m_grid.CheckPassable(party, party.Position);
			party.StartSpawnerID = -1;
			LegacyLogic.Instance.WorldManager.SpawnObject(party, party.Position);
			if (mapData != null)
			{
				AddAlreadySpawnedObjectsToGrid(spawnerIds, mapData);
			}
			m_state++;
		}

		private void AddAlreadySpawnedObjectsToGrid(HashSet<Int32> spawnerIds, MapData p_currentMapData)
		{
			for (Int32 i = 0; i < p_currentMapData.Monster.Count; i++)
			{
				Monster monster = p_currentMapData.Monster[i];
				if (spawnerIds.Contains(monster.SpawnerID))
				{
					if (m_grid.AddMovingEntity(monster.Position, monster))
					{
						monster.SetAggro(false);
						monster.InitMonsterGroup();
						LegacyLogic.Instance.WorldManager.SpawnObject(monster, monster.Position);
					}
					else
					{
						LegacyLogger.LogError(String.Concat(new Object[]
						{
							"Cannot add object to GRID: ",
							monster.SpawnerID,
							" - staticID: ",
							monster.StaticID
						}));
					}
				}
			}
			for (Int32 i = 0; i < p_currentMapData.Objects.Count; i++)
			{
				InteractiveObject interactiveObject = p_currentMapData.Objects[i];
				if (spawnerIds.Contains(interactiveObject.SpawnerID))
				{
					m_grid.AddInteractiveObject(interactiveObject);
					LegacyLogic.Instance.WorldManager.SpawnObject(interactiveObject, interactiveObject.Position);
					if (interactiveObject is TrapEffectContainer)
					{
						((TrapEffectContainer)interactiveObject).NotifyLevelLoaded();
					}
					if (interactiveObject is Door)
					{
						((Door)interactiveObject).UpdateState();
					}
				}
			}
			for (Int32 i = 0; i < p_currentMapData.Objects.Count; i++)
			{
				InteractiveObject interactiveObject2 = p_currentMapData.Objects[i];
				if (spawnerIds.Contains(interactiveObject2.SpawnerID))
				{
					interactiveObject2.OnPrewarm(m_grid);
				}
			}
			p_currentMapData.LoadTerrainDataMatrix(m_grid);
			LegacyLogic.Instance.WorldManager.QuestHandler.Initialize();
			LegacyLogic.Instance.WorldManager.BestiaryHandler.Initialize();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.PARTY_BUFF_REMOVED, EventArgs.Empty);
		}

		private HashSet<Int32> LoadSpawnerFromGrid(List<Int32> invalidSpawnList, Int32 partySpawnerID, out Position partyPosition, out EDirection partyDirection)
		{
			Int32 width = m_grid.Width;
			Int32 height = m_grid.Height;
			partyPosition = Position.Zero;
			partyDirection = EDirection.NORTH;
			HashSet<Int32> hashSet = new HashSet<Int32>();
			Spawn spawn = null;
			for (Int32 i = 0; i < width; i++)
			{
				for (Int32 j = 0; j < height; j++)
				{
					GridSlot slot = m_grid.GetSlot(new Position(i, j));
					foreach (Spawn spawn2 in slot.SpawnObjects)
					{
						spawn2.Position = new Position(i, j);
						hashSet.Add(spawn2.ID);
						if (spawn2.ObjectType == EObjectType.PARTY)
						{
							if (partySpawnerID != -1 && partySpawnerID == spawn2.ID)
							{
								spawn = spawn2;
							}
							else if (spawn == null && partySpawnerID == -1 && spawn2.Enabled)
							{
								spawn = spawn2;
							}
						}
						else if (invalidSpawnList == null || !invalidSpawnList.Contains(spawn2.ID))
						{
							LegacyLogic.Instance.UpdateManager.SpawnTurnActor.AddSpawner(spawn2);
						}
					}
				}
			}
			if (spawn != null)
			{
				partyPosition = spawn.Position;
				partyDirection = spawn.Direction;
			}
			return hashSet;
		}

		private void CheckSensors()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			GridSlot slot = m_grid.GetSlot(party.Position);
			List<InteractiveObject> activeInteractiveObjects = slot.GetActiveInteractiveObjects(EDirection.CENTER);
			Boolean flag = false;
			foreach (InteractiveObject interactiveObject in activeInteractiveObjects)
			{
				if (interactiveObject is Sensor)
				{
					flag = true;
					LegacyLogic.Instance.UpdateManager.InteractionsActor.AddObject(interactiveObject);
				}
			}
			if (flag)
			{
				LegacyLogic.Instance.UpdateManager.SkipPartyTurn = true;
			}
			m_wasSaveGame = m_isSaveGame;
			LegacyLogic.Instance.UpdateManager.Update();
			m_state = EMapLoaderState.LOAD_VIEWS;
			if (m_isSaveGame)
			{
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAME_LOADED, EventArgs.Empty);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.FINISH_SCENE_LOAD, EventArgs.Empty);
			m_isSaveGame = false;
		}

		private void EndLoading()
		{
			LegacyLogic.Instance.CommandManager.ClearQueue();
			if (m_wasSaveGame || LegacyLogic.Instance.WorldManager.MapData.ContainsKey(m_grid.Name))
			{
				m_wasSaveGame = false;
				OnLevelLoaded();
			}
			else
			{
				LegacyLogic.Instance.UpdateManager.SpawnTurnActor.m_isFirstUpdate = true;
			}
			m_grid.CheckPassable(LegacyLogic.Instance.WorldManager.Party, LegacyLogic.Instance.WorldManager.Party.Position);
			m_state = EMapLoaderState.IDLE;
		}

		internal void Update()
		{
			if (!IsLoading)
			{
				return;
			}
			switch (m_state)
			{
			case EMapLoaderState.LOADING_DYNAMIC_OBJECTS:
				LoadDynamicObjects();
				break;
			case EMapLoaderState.REENABLE_MONSTER_SPAWNS:
				ReenableMonsterSpawns();
				FixLevel();
				TriggerDLC();
				break;
			case EMapLoaderState.CHECK_SENSORS:
				CheckSensors();
				break;
			}
		}

		private void ReenableMonsterSpawns()
		{
			if (m_grid.Name == "theworld")
			{
				QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(79);
				if (step != null && step.QuestState == EQuestState.ACTIVE)
				{
					EnableSpawn(1074);
					EnableSpawn(1075);
					EnableSpawn(1069);
					EnableSpawn(1079);
					EnableSpawn(1073);
				}
				step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(107);
				if (step != null && step.QuestState == EQuestState.ACTIVE)
				{
					EnableSpawn(1083);
					EnableSpawn(1076);
					EnableSpawn(1077);
					EnableSpawn(1080);
					EnableSpawn(1081);
					EnableSpawn(1082);
				}
				step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(108);
				if (step != null && step.QuestState == EQuestState.ACTIVE)
				{
					EnableSpawn(1084);
					EnableSpawn(1085);
				}
			}
			else if (m_grid.Name == "Fort_Laegaire_1" && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(798) > 0)
			{
				EnableSpawn(215);
				EnableSpawn(216);
				EnableSpawn(217);
				EnableSpawn(218);
				EnableSpawn(219);
				EnableSpawn(220);
				EnableSpawn(221);
				EnableSpawn(222);
				EnableSpawn(223);
				EnableSpawn(224);
				EnableSpawn(226);
				EnableSpawn(227);
				EnableSpawn(228);
				EnableSpawn(229);
				EnableSpawn(230);
				EnableSpawn(231);
				EnableSpawn(232);
				EnableSpawn(233);
			}
			m_state++;
		}

		private void EnableSpawn(Int32 p_id)
		{
			Spawn spawn = m_grid.FindSpawn(p_id);
			if (spawn != null)
			{
				spawn.Enabled = true;
			}
		}

		private void FixLevel()
		{
			if (m_grid.Name == "Karthal_Jail_1")
			{
				InteractiveObject interactiveObject = m_grid.FindInteractiveObject(57);
				if (interactiveObject != null && interactiveObject.State == EInteractiveObjectState.LEVER_DOWN)
				{
					OpenDoor(56);
				}
			}
			else if (m_grid.Name == "Palace_of_Light_1")
			{
				if (LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(664) > 0)
				{
					OpenDoor(170);
					OpenDoor(171);
					OpenDoor(172);
					Spawn spawn = m_grid.FindSpawn(177);
					if (spawn != null)
					{
						spawn.Enabled = false;
					}
				}
				else
				{
					if (LegacyLogic.Instance.WorldManager.Party.Position == new Position(8, 24))
					{
						return;
					}
					CloseDoor(170);
					CloseDoor(171);
					CloseDoor(172);
					Spawn spawn2 = m_grid.FindSpawn(177);
					if (spawn2 != null)
					{
						spawn2.Enabled = false;
					}
					if (LegacyLogic.Instance.WorldManager.Party.Position == new Position(0, 10))
					{
						LegacyLogic.Instance.WorldManager.Party.Position = new Position(1, 10);
					}
					else if (LegacyLogic.Instance.WorldManager.Party.Position == new Position(16, 10))
					{
						LegacyLogic.Instance.WorldManager.Party.Position = new Position(15, 10);
					}
				}
			}
			else if (m_grid.Name == "TheCrag")
			{
				QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(17);
				if (step != null && step.QuestState == EQuestState.ACTIVE)
				{
					step.Deactivate();
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.NEW_QUEST, step));
					step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(16);
					step.SetState(EQuestState.ACTIVE);
					QuestObjective objective = step.GetObjective(34);
					objective.Activate();
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.QUESTLOG_CHANGED, new QuestChangedEventArgs(QuestChangedEventArgs.Type.NEW_QUEST, step));
					LegacyLogic.Instance.WorldManager.Party.HirelingHandler.Fire(50);
				}
			}
			else if (m_grid.Name == "Ker_Thal_3")
			{
				InteractiveObject interactiveObject2 = m_grid.FindInteractiveObject(14);
				if (interactiveObject2 != null)
				{
					foreach (SpawnCommand spawnCommand in interactiveObject2.Commands)
					{
						if (spawnCommand.Type == EInteraction.TRIGGER_CUTSCENE)
						{
							spawnCommand.Precondition = "NONE";
						}
					}
				}
				interactiveObject2 = m_grid.FindInteractiveObject(12);
				if (interactiveObject2 != null)
				{
					foreach (SpawnCommand spawnCommand2 in interactiveObject2.Commands)
					{
						if (spawnCommand2.Type == EInteraction.EXECUTE_INTERACTIVE_OBJECT)
						{
							spawnCommand2.Precondition = "INPUT,TEXT_INPUT,RIDDLE_SOLAR_SIGIL_TEXT_3,RIDDLE_SOLAR_SIGIL_TEXT_4,RIDDLE_SOLAR_SIGIL_TEXT_5,RIDDLE_SOLAR_SIGIL_TEXT_ANSWER";
							spawnCommand2.ActivateCount = 1;
						}
					}
				}
			}
			else if (m_grid.Name == "Fort_Laegaire_1")
			{
				QuestStep step2 = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(111);
				if (step2.QuestState != EQuestState.INACTIVE)
				{
					OpenDoor(21);
				}
			}
		}

		private void OpenDoor(Int32 p_id)
		{
			InteractiveObject interactiveObject = m_grid.FindInteractiveObject(p_id);
			Door door = interactiveObject as Door;
			if (door != null && door.State == EInteractiveObjectState.DOOR_CLOSED)
			{
				door.SetState(EInteractiveObjectState.DOOR_OPEN);
				door.UpdateState();
			}
			else
			{
				Spawn spawn = m_grid.FindSpawn(p_id);
				if (spawn != null && spawn.InitialState == EInteractiveObjectState.DOOR_CLOSED)
				{
					spawn.InitialState = EInteractiveObjectState.DOOR_OPEN;
				}
			}
		}

		private void CloseDoor(Int32 p_id)
		{
			InteractiveObject interactiveObject = m_grid.FindInteractiveObject(p_id);
			Door door = interactiveObject as Door;
			if (door != null && door.State == EInteractiveObjectState.DOOR_OPEN)
			{
				door.SetState(EInteractiveObjectState.DOOR_CLOSED);
				door.UpdateState();
			}
			else
			{
				Spawn spawn = m_grid.FindSpawn(p_id);
				if (spawn != null && spawn.InitialState == EInteractiveObjectState.DOOR_OPEN)
				{
					spawn.InitialState = EInteractiveObjectState.DOOR_CLOSED;
				}
			}
		}

		private void TriggerDLC()
		{
			if (!LegacyLogic.Instance.ServiceWrapper.IsPrivilegeAvailable(1003))
			{
				return;
			}
			QuestStep step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(31);
			if (step.QuestState == EQuestState.SOLVED && m_grid.Name != "Karthal_Harbour")
			{
				step = LegacyLogic.Instance.WorldManager.QuestHandler.GetStep(109);
				if (step != null && step.QuestState == EQuestState.INACTIVE)
				{
					LegacyLogic.Instance.WorldManager.QuestHandler.ActivateQuest(109);
				}
			}
		}
	}
}
