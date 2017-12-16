using System;
using System.Collections.Generic;
using System.Text;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Core.UpdateLogic.Interactions;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class InteractiveObject : BaseObject, ISaveGameObject
	{
		private String m_prefab = String.Empty;

		private Position m_Position;

		private Position m_originalPosition;

		private Single m_originalHeight;

		private EDirection m_location;

		private Vector3D m_offsetPosition;

		private Vector3D m_objectRotation;

		private Boolean m_isSecret;

		private Boolean m_trapActive;

		private List<Int32> m_trapEffectSpawnerIDs;

		protected List<SpawnCommand> m_commands;

		protected List<SpawnQuestObjective> m_questObjectives;

		protected List<BaseInteraction> m_interactions;

		protected InteractiveObjectStaticData m_staticData;

		protected readonly StateMachine<EState> m_stateMachine;

		private Boolean m_interactLock;

		private Boolean m_isClickableViewState;

		private Boolean m_enabled = true;

		public InteractiveObject() : this(0, EObjectType.INTERACTIVE_OBJECT, 0)
		{
		}

		public InteractiveObject(Int32 p_staticID, Int32 p_spawnerID) : this(p_staticID, EObjectType.INTERACTIVE_OBJECT, p_spawnerID)
		{
		}

		protected InteractiveObject(Int32 p_staticID, EObjectType p_objectType, Int32 p_spawnerID) : base(p_staticID, p_objectType, p_spawnerID)
		{
			m_originalPosition = new Position(-1, -1);
			m_commands = new List<SpawnCommand>();
			m_interactions = new List<BaseInteraction>();
			m_questObjectives = new List<SpawnQuestObjective>();
			m_trapEffectSpawnerIDs = new List<Int32>();
			m_stateMachine = new StateMachine<EState>();
			m_stateMachine.AddState(new State<EState>(EState.IDLE));
			m_stateMachine.AddState(new State<EState>(EState.RUNNING));
			m_stateMachine.AddState(new State<EState>(EState.AFTER_RUNNING));
			m_stateMachine.AddState(new State<EState>(EState.FINISHED));
			m_stateMachine.ChangeState(EState.IDLE);
		}

		public Boolean SelfActive => m_staticData.SelfActive;

	    public Boolean IsClickableViewState
		{
			get => m_isClickableViewState;
	        set => m_isClickableViewState = value;
	    }

		public String Icon => m_staticData.Icon;

	    public virtual Boolean MinimapVisible => m_staticData.MinimapVisible;

	    public virtual Boolean MapVisible => m_staticData.MapVisible;

	    public String Prefab
		{
			get => m_prefab;
	        set => m_prefab = value;
	    }

		public Boolean IsSecret
		{
			get => m_isSecret;
		    set => m_isSecret = value;
		}

		public virtual EInteractiveObjectState State { get; set; }

		public virtual EDirection Location
		{
			get => m_location;
		    set
			{
				if (value == EDirection.CENTER && !m_staticData.Center)
				{
					throw new ArgumentException("trying to place a edge-only object into the center, spawnerId: " + SpawnerID);
				}
				if (value != EDirection.CENTER && !m_staticData.Edge)
				{
					throw new ArgumentException(String.Concat(new Object[]
					{
						"trying to place a center-only object into the edge: location=",
						m_location,
						", staticData.Edge=",
						m_staticData.Edge
					}));
				}
				m_location = value;
			}
		}

		public Position Position
		{
			get => m_Position;
		    set => m_Position = value;
		}

		public Position OriginalPosition
		{
			get => m_originalPosition;
		    set => m_originalPosition = value;
		}

		public Single OriginalHeight => m_originalHeight;

	    public Vector3D OffsetPosition
		{
			get => m_offsetPosition;
	        set => m_offsetPosition = value;
	    }

		public Vector3D ObjectRotation
		{
			get => m_objectRotation;
		    set => m_objectRotation = value;
		}

		public List<Int32> TrapEffectSpawnerIDs => m_trapEffectSpawnerIDs;

	    public Boolean TrapActive
		{
			get => AreTrapsActive();
	        set => SetTrapsActive(value);
	    }

		private Boolean AreTrapsActive()
		{
			Boolean flag = false;
			foreach (Int32 num in m_trapEffectSpawnerIDs)
			{
				if (num > 0)
				{
					InteractiveObject interactiveObject = LegacyLogic.Instance.MapLoader.Grid.FindInteractiveObject(num);
					if (interactiveObject != null && interactiveObject is TrapEffectContainer)
					{
						flag |= ((TrapEffectContainer)interactiveObject).IsTrapActive();
					}
				}
			}
			return flag;
		}

		private void SetTrapsActive(Boolean p_value)
		{
			foreach (Int32 num in m_trapEffectSpawnerIDs)
			{
				if (num > 0)
				{
					InteractiveObject interactiveObject = LegacyLogic.Instance.MapLoader.Grid.FindInteractiveObject(num);
					if (interactiveObject != null && interactiveObject is TrapEffectContainer)
					{
						((TrapEffectContainer)interactiveObject).SetTrapActive(p_value);
					}
				}
			}
		}

		public List<BaseInteraction> Interactions => m_interactions;

	    public List<SpawnCommand> Commands => m_commands;

	    public List<SpawnQuestObjective> QuestObjectives => m_questObjectives;

	    public EState TurnState => m_stateMachine.CurrentState.Id;

	    public Boolean InteractLock
		{
			get => m_interactLock;
	        set => m_interactLock = value;
	    }

		public Boolean Enabled
		{
			get => m_enabled;
		    set => m_enabled = value;
		}

		protected override void LoadStaticData()
		{
			if (StaticID == 0)
			{
				return;
			}
			m_staticData = StaticDataHandler.GetStaticData<InteractiveObjectStaticData>(EDataType.INTERACTIVE_OBJECT, StaticID);
			if (m_staticData == null)
			{
				throw new Exception("InteractiveObjectStaticData ID=" + StaticID + " not defined!");
			}
		}

		public Boolean IsInteractable(EDirection p_partyLocation, Boolean p_hasSpotSecrets, Boolean p_hasClairvoyance, Boolean p_isReturnExecutableOnly)
		{
			return Enabled && !SelfActive && Location == p_partyLocation && (p_hasSpotSecrets || !IsSecret) && (p_hasClairvoyance || !(this is Trap)) && (!p_isReturnExecutableOnly || (IsExecutable(LegacyLogic.Instance.MapLoader.Grid) && !(this is ConditionalTrigger)));
		}

		public virtual void SetData(EInteractiveObjectData p_key, String p_value)
		{
			if (p_key == EInteractiveObjectData.PREFAB)
			{
				Prefab = p_value;
			}
			else if (p_key == EInteractiveObjectData.IS_SECRET)
			{
				IsSecret = String.Equals(p_value, "true", StringComparison.InvariantCultureIgnoreCase);
			}
			else if (p_key == EInteractiveObjectData.STATE)
			{
				if (!Enum.IsDefined(typeof(EInteractiveObjectState), p_value))
				{
					throw new Exception("Invalid Object State in SetDataInteraction");
				}
				State = (EInteractiveObjectState)Enum.Parse(typeof(EInteractiveObjectState), p_value);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.OBJECT_STATE_CHANGED, EventArgs.Empty);
			}
			else if (p_key == EInteractiveObjectData.TRAP_DATA)
			{
				String[] array = p_value.Split(new Char[]
				{
					','
				});
				for (Int32 i = 0; i < array.Length; i++)
				{
					TrapEffectSpawnerIDs.Add(Int32.Parse(array[i]));
				}
				CheckActiveAfterSetData();
			}
		}

		public virtual void OnAfterCreate(Grid p_grid)
		{
			for (Int32 i = 0; i < m_commands.Count; i++)
			{
				SpawnCommand spawnCommand = m_commands[i];
				if (spawnCommand.Timing == EInteractionTiming.ON_SPAWN)
				{
					BaseInteraction baseInteraction = InteractionFactory.Create(this, spawnCommand, SpawnerID, i);
					if (baseInteraction.Valid)
					{
						baseInteraction.Execute();
					}
				}
			}
		}

		public void OnAfterSpawn(Grid p_grid)
		{
			if (m_commands.Count == 0 && m_questObjectives.Count > 0)
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.ObjectInteraction(this);
			}
			for (Int32 i = 0; i < m_commands.Count; i++)
			{
				SpawnCommand spawnCommand = m_commands[i];
				if (spawnCommand.Timing == EInteractionTiming.ON_AFTER_SPAWN)
				{
					BaseInteraction baseInteraction = InteractionFactory.Create(this, spawnCommand, SpawnerID, i);
					if (baseInteraction.Valid)
					{
						baseInteraction.Execute();
					}
				}
			}
		}

		public virtual void OnLevelLoaded(Grid p_grid)
		{
			if (m_commands.Count > 0)
			{
				for (Int32 i = 0; i < m_commands.Count; i++)
				{
					SpawnCommand spawnCommand = m_commands[i];
					if (spawnCommand.Timing == EInteractionTiming.ON_LEVEL_LOADED && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0))
					{
						BaseInteraction baseInteraction = InteractionFactory.Create(this, spawnCommand, SpawnerID, i);
						if (baseInteraction.Valid && (spawnCommand.RequiredState == EInteractiveObjectState.NONE || StateIsMatching(spawnCommand, baseInteraction)))
						{
							baseInteraction.Execute();
						}
					}
				}
			}
		}

		public virtual void OnPrewarm(Grid p_grid)
		{
			for (Int32 i = 0; i < m_commands.Count; i++)
			{
				SpawnCommand command = m_commands[i];
				BaseInteraction baseInteraction = InteractionFactory.Create(this, command, SpawnerID, i);
				if (baseInteraction.Valid)
				{
					baseInteraction.PrewarmAfterCreate();
				}
			}
		}

		public virtual void OnAddedToGrid()
		{
			if (m_originalPosition == new Position(-1, -1))
			{
				m_originalPosition = Position;
				m_originalHeight = LegacyLogic.Instance.MapLoader.Grid.GetSlot(Position).Height;
			}
		}

		public virtual void Execute(Grid p_grid)
		{
			for (Int32 i = 0; i < m_commands.Count; i++)
			{
				SpawnCommand spawnCommand = m_commands[i];
				if (spawnCommand.Timing == EInteractionTiming.ON_EXECUTE && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0))
				{
					BaseInteraction baseInteraction = InteractionFactory.Create(this, spawnCommand, SpawnerID, i);
					if (baseInteraction.Valid && (spawnCommand.RequiredState == EInteractiveObjectState.NONE || StateIsMatching(spawnCommand, baseInteraction)))
					{
						m_interactions.Add(baseInteraction);
					}
				}
				m_stateMachine.ChangeState(EState.IDLE);
			}
		}

		public Boolean IsExecutable(Grid p_grid)
		{
			Boolean flag = false;
			for (Int32 i = 0; i < m_commands.Count; i++)
			{
				SpawnCommand spawnCommand = m_commands[i];
				if (spawnCommand.Timing == EInteractionTiming.ON_EXECUTE && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0))
				{
					BaseInteraction baseInteraction = InteractionFactory.Create(this, spawnCommand, SpawnerID, i);
					if (baseInteraction.Valid && (spawnCommand.RequiredState == EInteractiveObjectState.NONE || StateIsMatching(spawnCommand, baseInteraction)))
					{
						if (baseInteraction is DisarmTrapInteraction)
						{
							DisarmTrapInteraction disarmTrapInteraction = (DisarmTrapInteraction)baseInteraction;
							flag = disarmTrapInteraction.IsExecutable();
						}
						else
						{
							flag = true;
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			return flag;
		}

		public void AddTrapEffectSpawner(Int32 p_id)
		{
			m_trapEffectSpawnerIDs.Add(p_id);
			CheckActiveAfterSetData();
		}

		private void CheckActiveAfterSetData()
		{
			if (TrapEffectSpawnerIDs.Count > 0)
			{
				m_trapActive = true;
			}
		}

		protected Boolean StateIsMatching(SpawnCommand sc, BaseInteraction b)
		{
			if (b is BaseDoorInteraction)
			{
				return ((BaseDoorInteraction)b).Target.State == sc.RequiredState;
			}
			if (b is BaseLeverInteraction)
			{
				return ((BaseLeverInteraction)b).Target.State == sc.RequiredState;
			}
			if (b is BaseButtonInteraction)
			{
				return ((BaseButtonInteraction)b).Target.State == sc.RequiredState;
			}
			if (b is SetStateInteraction)
			{
				return ((SetStateInteraction)b).Target.State == sc.RequiredState;
			}
			if (b is ViewSignInteraction)
			{
				return ((ViewSignInteraction)b).Target.State == sc.RequiredState;
			}
			return b is ExecuteInteractiveObjectInteraction && ((ExecuteInteractiveObjectInteraction)b).Target.State == sc.RequiredState;
		}

		public virtual void OnAfterExecute(EInteractionTiming p_success)
		{
			if (m_questObjectives.Count > 0)
			{
				LegacyLogic.Instance.WorldManager.QuestHandler.ObjectInteraction(this);
			}
			for (Int32 i = 0; i < m_commands.Count; i++)
			{
				SpawnCommand spawnCommand = m_commands[i];
				if (spawnCommand.Timing == p_success && (spawnCommand.ActivateCount == -1 || spawnCommand.ActivateCount > 0) && (spawnCommand.RequiredState == EInteractiveObjectState.NONE || State == spawnCommand.RequiredState))
				{
					BaseInteraction baseInteraction = InteractionFactory.Create(this, spawnCommand, SpawnerID, i);
					if (baseInteraction.Valid)
					{
						m_interactions.Add(baseInteraction);
						baseInteraction.Execute();
					}
				}
			}
			if (m_interactions.Count > 0)
			{
				m_stateMachine.ChangeState(EState.AFTER_RUNNING);
			}
			else
			{
				m_stateMachine.ChangeState(EState.FINISHED);
			}
		}

		public virtual void Update()
		{
			if (TurnState == EState.IDLE)
			{
				if (m_interactions.Count > 0)
				{
					foreach (BaseInteraction baseInteraction in m_interactions)
					{
						baseInteraction.Execute();
					}
				}
				m_stateMachine.ChangeState(EState.RUNNING);
			}
			else if (TurnState == EState.RUNNING)
			{
				Boolean flag = true;
				Boolean flag2 = true;
				Boolean flag3 = false;
				foreach (BaseInteraction baseInteraction2 in m_interactions)
				{
					if (baseInteraction2.State == 1)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					foreach (BaseInteraction baseInteraction3 in m_interactions)
					{
						if (baseInteraction3.State != 2)
						{
							flag2 = false;
							break;
						}
					}
					foreach (BaseInteraction baseInteraction4 in m_interactions)
					{
						if (baseInteraction4.State == 11)
						{
							flag3 = true;
							break;
						}
					}
					m_interactions.Clear();
					if (!flag3)
					{
						OnAfterExecute((!flag2) ? EInteractionTiming.ON_FAIL : EInteractionTiming.ON_SUCCESS);
					}
					else
					{
						m_stateMachine.ChangeState(EState.FINISHED);
					}
				}
			}
			else if (TurnState == EState.AFTER_RUNNING)
			{
				Boolean flag4 = true;
				foreach (BaseInteraction baseInteraction5 in m_interactions)
				{
					if (baseInteraction5.State == 1)
					{
						flag4 = false;
						break;
					}
				}
				if (flag4)
				{
					m_stateMachine.ChangeState(EState.FINISHED);
					m_interactions.Clear();
					for (Int32 i = m_commands.Count - 1; i > 0; i--)
					{
						SpawnCommand spawnCommand = m_commands[i];
						if (spawnCommand.ActivateCount == 0)
						{
							m_commands.RemoveAt(i);
						}
					}
				}
			}
			foreach (BaseInteraction baseInteraction6 in m_interactions)
			{
				baseInteraction6.Update();
			}
			m_stateMachine.Update();
		}

		public void ExecutionBreak(StringBuilder sb)
		{
			foreach (BaseInteraction baseInteraction in m_interactions)
			{
				sb.AppendLine("Interaction: " + baseInteraction.GetType().Name);
				sb.AppendLine("Interaction state: " + baseInteraction.State);
			}
			m_stateMachine.ChangeState(EState.FINISHED);
		}

		public void DecreaseActivate(Int32 idx)
		{
			SpawnCommand spawnCommand = m_commands[idx];
			spawnCommand.ActivateCount--;
		}

		public void ResolveTrapEffect(Grid p_grid, Party p_party)
		{
			foreach (Int32 num in TrapEffectSpawnerIDs)
			{
				if (num > 0)
				{
					InteractiveObject interactiveObject = p_grid.FindInteractiveObject(num);
					if (interactiveObject != null && interactiveObject is TrapEffectContainer)
					{
						((TrapEffectContainer)interactiveObject).Resolve(p_party);
					}
				}
			}
		}

		public void ClearInteractions()
		{
			m_interactions.Clear();
		}

		public override String ToString()
		{
			return String.Format("[InteractiveObject: SelfActive={0}, Location={1}, Icon={2}, MinimapVisible={3}, MapVisible={4}, Prefab={5}, IsSecret={6}, Position={7}, TurnState={8}, InteractLock={9}, ObjectType={10}]", new Object[]
			{
				SelfActive,
				Location,
				Icon,
				MinimapVisible,
				MapVisible,
				Prefab,
				IsSecret,
				Position,
				TurnState,
				InteractLock,
				Type
			});
		}

		public virtual void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("StaticID", StaticID);
			p_data.Set<Int32>("SpawnerID", SpawnerID);
			p_data.Set<EObjectType>("ObjectType", Type);
			p_data.Set<Int32>("Location", (Int32)Location);
			p_data.Set<String>("Prefab", m_prefab);
			p_data.Set<Boolean>("IsSecret", m_isSecret);
			p_data.Set<Int32>("State", (Int32)State);
			p_data.Set<Int32>("PositionX", m_Position.X);
			p_data.Set<Int32>("PositionY", m_Position.Y);
			p_data.Set<Int32>("OriginalPositionX", m_originalPosition.X);
			p_data.Set<Int32>("OriginalPositionY", m_originalPosition.Y);
			p_data.Set<Single>("OriginalHeight", m_originalHeight);
			p_data.Set<Single>("OffsetX", m_offsetPosition.X);
			p_data.Set<Single>("OffsetY", m_offsetPosition.Y);
			p_data.Set<Single>("OffsetZ", m_offsetPosition.Z);
			p_data.Set<Single>("RotationX", m_objectRotation.X);
			p_data.Set<Single>("RotationY", m_objectRotation.Y);
			p_data.Set<Single>("RotationZ", m_objectRotation.Z);
			p_data.Set<Boolean>("Enabled", m_enabled);
			p_data.Set<Int32>("TrapEffectSpawnerIDs", m_trapEffectSpawnerIDs.Count);
			for (Int32 i = 0; i < m_trapEffectSpawnerIDs.Count; i++)
			{
				p_data.Set<Int32>("TrapEffectSpawnerID" + i, m_trapEffectSpawnerIDs[i]);
			}
			p_data.Set<Boolean>("TrapActive", m_trapActive);
			p_data.Set<Int32>("StateMachineState", (Int32)m_stateMachine.CurrentState.Id);
			if (m_commands != null)
			{
				p_data.Set<Int32>("CommandsCount", m_commands.Count);
				SaveGameData saveGameData = new SaveGameData("SpawnCommands");
				for (Int32 j = 0; j < m_commands.Count; j++)
				{
					SaveGameData saveGameData2 = new SaveGameData("command" + j);
					m_commands[j].Save(saveGameData2);
					saveGameData.Set<SaveGameData>(saveGameData2.ID, saveGameData2);
				}
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
			p_data.Set<Int32>("QuestObjectiveCount", m_questObjectives.Count);
			for (Int32 k = 0; k < m_questObjectives.Count; k++)
			{
				p_data.Set<Int32>("QuestObjective" + k, m_questObjectives[k].ID);
			}
		}

		public virtual void Load(SaveGameData p_data)
		{
			m_location = (EDirection)p_data.Get<Int32>("Location", 0);
			m_prefab = p_data.Get<String>("Prefab", String.Empty);
			m_isSecret = p_data.Get<Boolean>("IsSecret", false);
			State = p_data.Get<EInteractiveObjectState>("State", EInteractiveObjectState.NONE);
			m_Position = new Position(p_data.Get<Int32>("PositionX", 0), p_data.Get<Int32>("PositionY", 0));
			m_originalPosition = new Position(p_data.Get<Int32>("OriginalPositionX", 0), p_data.Get<Int32>("OriginalPositionY", 0));
			m_originalHeight = p_data.Get<Single>("OriginalHeight", 0f);
			m_offsetPosition = new Vector3D(p_data.Get<Single>("OffsetX", 0f), p_data.Get<Single>("OffsetY", 0f), p_data.Get<Single>("OffsetZ", 0f));
			m_objectRotation = new Vector3D(p_data.Get<Single>("RotationX", 0f), p_data.Get<Single>("RotationY", 0f), p_data.Get<Single>("RotationZ", 0f));
			m_enabled = p_data.Get<Boolean>("Enabled", true);
			Int32 num = p_data.Get<Int32>("TrapEffectSpawnerIDs", 0);
			for (Int32 i = 0; i < num; i++)
			{
				m_trapEffectSpawnerIDs.Add(p_data.Get<Int32>("TrapEffectSpawnerID" + i, 0));
			}
			m_trapActive = p_data.Get<Boolean>("TrapActive", m_trapActive);
			Int32 p_newStateId = p_data.Get<Int32>("StateMachineState", 0);
			Int32 num2 = p_data.Get<Int32>("CommandsCount", 0);
			SaveGameData saveGameData = p_data.Get<SaveGameData>("SpawnCommands", null);
			if (saveGameData != null)
			{
				for (Int32 j = 0; j < num2; j++)
				{
					SaveGameData saveGameData2 = saveGameData.Get<SaveGameData>("command" + j, null);
					if (saveGameData2 != null)
					{
						SpawnCommand spawnCommand = new SpawnCommand();
						spawnCommand.Load(saveGameData2);
						m_commands.Add(spawnCommand);
					}
				}
			}
			Int32 num3 = p_data.Get<Int32>("QuestObjectiveCount", 0);
			for (Int32 k = 0; k < num3; k++)
			{
				SpawnQuestObjective spawnQuestObjective = new SpawnQuestObjective();
				spawnQuestObjective.ID = p_data.Get<Int32>("QuestObjective" + k, 0);
				m_questObjectives.Add(spawnQuestObjective);
			}
			m_stateMachine.ChangeState((EState)p_newStateId);
		}

		public enum EState
		{
			IDLE,
			RUNNING,
			AFTER_RUNNING,
			FINISHED = 10
		}
	}
}
