using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Map;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities
{
	public class Spawn
	{
		private Int32 m_id;

		private Int32 m_objectID;

		private Int32 m_monsterGroupID;

		private EObjectType m_objectType;

		private ESpawnTime m_spawnTime;

		private Boolean m_canSpawn;

		private Boolean m_touched;

		private Position m_position;

		private EDirection m_direction = EDirection.CENTER;

		private Vector3D m_offsetPosition;

		private Vector3D m_objectRotation;

		private List<SpawnCommand> m_commands;

		private List<SpawnCommand> m_objectTypeCommands;

		private List<SpawnQuestObjective> m_questObjectives;

		private Boolean m_enabled = true;

		private Boolean m_isOpenableByMonsters;

		private EInteractiveObjectState m_initialState;

		private StateMachine<EState> m_stateMachine;

		private TriggerHelper m_turnIdle = new TriggerHelper();

		public Spawn()
		{
			InitStateMachine();
			m_commands = new List<SpawnCommand>();
			m_questObjectives = new List<SpawnQuestObjective>();
			m_canSpawn = true;
			m_touched = false;
		}

		[XmlAttribute("ID")]
		public Int32 ID
		{
			get => m_id;
		    set => m_id = value;
		}

		[XmlElement("SpawnStaticID")]
		public Int32 ObjectID
		{
			get => m_objectID;
		    set => m_objectID = value;
		}

		[XmlElement("MonsterGroupID")]
		public Int32 MonsterGroupID
		{
			get => m_monsterGroupID;
		    set => m_monsterGroupID = value;
		}

		[XmlElement("SpawnObjectType")]
		public EObjectType ObjectType
		{
			get => m_objectType;
		    set => m_objectType = value;
		}

		[XmlElement("SpawnTime")]
		public ESpawnTime SpawnTime
		{
			get => m_spawnTime;
		    set => m_spawnTime = value;
		}

		[XmlElement("Position")]
		public Position Position
		{
			get => m_position;
		    set => m_position = value;
		}

		[XmlElement("SpawnDirection")]
		public EDirection Direction
		{
			get => m_direction;
		    set => m_direction = value;
		}

		[XmlElement("OffsetPosition")]
		public Vector3D OffsetPosition
		{
			get => m_offsetPosition;
		    set => m_offsetPosition = value;
		}

		[XmlElement("ObjectRotation")]
		public Vector3D ObjectRotation
		{
			get => m_objectRotation;
		    set => m_objectRotation = value;
		}

		[XmlElement("Command")]
		public List<SpawnCommand> Commands
		{
			get => m_commands;
		    set => m_commands = value;
		}

		[XmlElement("ObjectTypeCommand")]
		public List<SpawnCommand> ObjectTypeCommands
		{
			get => m_objectTypeCommands;
		    set => m_objectTypeCommands = value;
		}

		[XmlElement("Objective")]
		public List<SpawnQuestObjective> QuestObjectives
		{
			get => m_questObjectives;
		    set => m_questObjectives = value;
		}

		[XmlElement("Enabled")]
		public Boolean Enabled
		{
			get => m_enabled;
		    set => m_enabled = value;
		}

		[XmlElement("OpenableByMonsters")]
		public Boolean IsOpenableByMonsters
		{
			get => m_isOpenableByMonsters;
		    set => m_isOpenableByMonsters = value;
		}

		[XmlElement("InitialState")]
		public EInteractiveObjectState InitialState
		{
			get => m_initialState;
		    set => m_initialState = value;
		}

		public TriggerHelper TurnIdle => m_turnIdle;

	    public EState State => m_stateMachine.CurrentState.Id;

	    public Boolean CanSpawn
		{
			get => m_canSpawn;
	        set => m_canSpawn = value;
	    }

		private void InitStateMachine()
		{
			m_stateMachine = new StateMachine<EState>();
			m_stateMachine.AddState(new State<EState>(EState.IDLE));
			State<EState> state = new State<EState>(EState.ACTION_FINISHED);
			state.AddTransition(new Transition<EState>(EState.IDLE, m_turnIdle));
			m_stateMachine.AddState(state);
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public virtual void Update()
		{
			m_stateMachine.Update();
		}

		public BaseObject SpawnObject()
		{
			BaseObject baseObject = null;
			if (m_objectType == EObjectType.MONSTER)
			{
				if (!InSpawnRange(LegacyLogic.Instance.WorldManager.Party.Position))
				{
					m_touched = false;
					return null;
				}
				if (!m_touched)
				{
					m_touched = true;
					Boolean flag = false;
					if (m_spawnTime == ESpawnTime.EVERYTIME || (m_spawnTime == ESpawnTime.ENABLED && m_enabled) || (m_spawnTime == ESpawnTime.ACT4 && LegacyLogic.Instance.WorldManager.Party.TokenHandler.GetTokens(9) > 0) || (LegacyLogic.Instance.GameTime.DayState == EDayState.DAY && m_spawnTime == ESpawnTime.DAY) || (LegacyLogic.Instance.GameTime.DayState == EDayState.NIGHT && m_spawnTime == ESpawnTime.NIGHT))
					{
						flag = true;
					}
					if (!flag || !m_canSpawn || (LegacyLogic.Instance.WorldManager.IsSaveGame && InVisiblityRange(LegacyLogic.Instance.WorldManager.Party.Position) && LegacyLogic.Instance.MapLoader.Grid.Type == EMapType.OUTDOOR))
					{
						if ((m_spawnTime == ESpawnTime.ENABLED && !m_enabled) || m_spawnTime == ESpawnTime.ACT4)
						{
							m_touched = false;
						}
						return null;
					}
					baseObject = EntityFactory.Create(m_objectType, m_objectID, m_id);
					Monster monster = (Monster)baseObject;
					monster.Position = m_position;
					monster.Direction = m_direction;
					monster.MonsterGroupID = m_monsterGroupID;
					if (m_initialState != EInteractiveObjectState.NONE)
					{
						monster.SpawnAnim = 1;
					}
					m_canSpawn = false;
					if (m_spawnTime == ESpawnTime.ENABLED)
					{
						LegacyLogic.Instance.UpdateManager.SkipPartyTurn = true;
						monster.m_stateMachine.ChangeState(Monster.EState.SPAWNING);
					}
				}
			}
			else
			{
				baseObject = EntityFactory.Create(m_objectType, m_objectID, m_id);
				if (baseObject is InteractiveObject)
				{
					InteractiveObject interactiveObject = (InteractiveObject)baseObject;
					interactiveObject.Position = m_position;
					interactiveObject.Location = m_direction;
					interactiveObject.ObjectRotation = m_objectRotation;
					interactiveObject.OffsetPosition = m_offsetPosition;
					interactiveObject.Commands.AddRange(m_commands);
					interactiveObject.Commands.AddRange(m_objectTypeCommands);
					interactiveObject.QuestObjectives.AddRange(m_questObjectives);
					interactiveObject.Enabled = m_enabled;
					m_canSpawn = false;
					if (interactiveObject is Door)
					{
						Door door = (Door)interactiveObject;
						door.OpenableByMonsters = m_isOpenableByMonsters;
					}
					if (m_initialState != EInteractiveObjectState.NONE)
					{
						interactiveObject.State = m_initialState;
					}
				}
			}
			return baseObject;
		}

		public Boolean ShouldRespawn()
		{
			return m_canSpawn;
		}

		public void SetCanRespawn()
		{
			m_canSpawn = true;
		}

		public void EndTurn()
		{
			m_stateMachine.ChangeState(EState.ACTION_FINISHED);
		}

		public Boolean InSpawnRange(Position p_position)
		{
			Single num = Position.Distance(p_position, m_position);
			return num < ConfigManager.Instance.Game.MonsterSpawnRange;
		}

		public Boolean InVisiblityRange(Position p_position)
		{
			Single num = Position.Distance(p_position, m_position);
			return num < ConfigManager.Instance.Game.MonsterVisibilityRange;
		}

		public enum EState
		{
			IDLE,
			ACTION_FINISHED = 10
		}
	}
}
