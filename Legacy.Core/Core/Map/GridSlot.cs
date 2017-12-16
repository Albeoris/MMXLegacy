using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Pathfinding;

namespace Legacy.Core.Map
{
	[XmlType("Slot")]
	public class GridSlot : IPathNode<GridSlot>, IEquatable<GridSlot>
	{
		public const Int32 TRANSITION_COUNT = 4;

		private Position m_position;

		private Single m_height;

		private ETerrainType m_terrainType;

		private ETerrainSound m_terrainSound;

		private EMapArea m_mapArea;

		private GridTransition[] m_transitions;

		private List<Spawn> m_spawnObjects;

		private List<MovingEntity> m_Entities;

		private ReadOnlyCollection<MovingEntity> m_ReadOnlyEntities;

		private ESize m_EntitySpace;

		private GridSlot[] m_connections;

		private List<GridSlot> m_availableSlots;

		private List<InteractiveObject> m_objects;

		private Boolean m_TriggeredBark;

		public GridSlot()
		{
			m_transitions = new GridTransition[4];
			for (Int32 i = 0; i < m_transitions.Length; i++)
			{
				m_transitions[i] = new GridTransition();
			}
			m_Entities = new List<MovingEntity>();
			m_EntitySpace = ESize.BIG;
			m_spawnObjects = new List<Spawn>();
			m_objects = new List<InteractiveObject>();
			m_connections = new GridSlot[4];
			m_availableSlots = new List<GridSlot>(4);
		}

		Boolean IPathNode<GridSlot>.Invalid
		{
			get
			{
				if ((m_terrainType & ETerrainType.BLOCKED) != ETerrainType.NONE)
				{
					if ((m_terrainType & ETerrainType.FLY_THROUGH) == ETerrainType.NONE)
					{
						return true;
					}
					if (AStarHelper<GridSlot>.ForDistanceCalc && (m_terrainType & ETerrainType.SHOOT_THROUGH) == ETerrainType.NONE)
					{
						return true;
					}
				}
				return false;
			}
		}

		List<GridSlot> IPathNode<GridSlot>.GetConnections(Boolean checkDoors)
		{
			m_availableSlots.Clear();
			InitConnections(LegacyLogic.Instance.MapLoader.Grid);
			if (checkDoors)
			{
				for (Int32 i = 0; i < m_connections.Length; i++)
				{
					if (m_connections[i] != null && m_transitions[i].NextState == EGridTransitionType.OPEN)
					{
						if (CheckDoorsInLocation((EDirection)i))
						{
							m_availableSlots.Add(m_connections[i]);
						}
						else if (m_connections[i].CheckDoorsInLocation(EDirectionFunctions.GetOppositeDir((EDirection)i)))
						{
							m_availableSlots.Add(m_connections[i]);
						}
					}
				}
			}
			else
			{
				for (Int32 j = 0; j < m_connections.Length; j++)
				{
					if (m_connections[j] != null && m_transitions[j].NextState == EGridTransitionType.OPEN)
					{
						m_availableSlots.Add(m_connections[j]);
					}
				}
			}
			return m_availableSlots;
		}

		Boolean IPathNode<GridSlot>.IsPassableForEntity(MovingEntity p_entity, Boolean p_isForDistanceCalc, Boolean p_checkForSummons)
		{
			return IsPassable(p_entity, p_isForDistanceCalc, p_checkForSummons);
		}

		[XmlAttribute("Terrain")]
		public ETerrainType TerrainType
		{
			get => m_terrainType;
		    set => m_terrainType = value;
		}

		[XmlAttribute("TerrainSound")]
		public ETerrainSound TerrainSound
		{
			get => m_terrainSound;
		    set => m_terrainSound = value;
		}

		[XmlAttribute("MapArea")]
		public EMapArea MapArea
		{
			get => m_mapArea;
		    set => m_mapArea = value;
		}

		[XmlElement("Transition")]
		public GridTransition[] Transitions
		{
			get => m_transitions;
		    set => m_transitions = value;
		}

		[XmlElement("Trigger")]
		public List<Spawn> SpawnObjects
		{
			get => m_spawnObjects;
		    set => m_spawnObjects = value;
		}

		[XmlAttribute("Height")]
		public Single Height
		{
			get => m_height;
		    set => m_height = value;
		}

		[XmlElement("Position")]
		public Position Position
		{
			get => m_position;
		    set => m_position = value;
		}

		[XmlIgnore]
		public Boolean VisitedByParty { get; set; }

		public Boolean HasSpawners => m_spawnObjects.Count > 0;

	    public Boolean HasEntities => m_Entities.Count > 0;

	    public ESize EntitySpace => m_EntitySpace;

	    [XmlIgnore]
		public IList<MovingEntity> Entities
		{
			get
			{
				ReadOnlyCollection<MovingEntity> result;
				if ((result = m_ReadOnlyEntities) == null)
				{
					result = (m_ReadOnlyEntities = m_Entities.AsReadOnly());
				}
				return result;
			}
		}

		[XmlIgnore]
		public IList<InteractiveObject> InteractiveObjects => m_objects;

	    public Int32 CountEntityTypes(EObjectType type)
		{
			if (type != EObjectType.PARTY && type != EObjectType.MONSTER)
			{
				throw new NotSupportedException();
			}
			Int32 num = 0;
			if (m_Entities.Count > 0)
			{
				foreach (MovingEntity movingEntity in m_Entities)
				{
					if (movingEntity.Type == type)
					{
						num++;
					}
				}
			}
			return num;
		}

		public void SetSlotsForTransition()
		{
			for (Int32 i = 0; i < m_transitions.Length; i++)
			{
				m_transitions[i].Slot = this;
			}
		}

		public void UpdatedDynamicConnections()
		{
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UPDATE_AVAILABLE_ACTIONS, EventArgs.Empty);
			InitConnections(LegacyLogic.Instance.MapLoader.Grid);
		}

		public GridSlot GetNeighborSlot(Grid p_grid, EDirection p_dir)
		{
			switch (p_dir)
			{
			case EDirection.NORTH:
				return p_grid.GetSlot(m_position + Position.Up);
			case EDirection.EAST:
				return p_grid.GetSlot(m_position + Position.Right);
			case EDirection.SOUTH:
				return p_grid.GetSlot(m_position + Position.Down);
			case EDirection.WEST:
				return p_grid.GetSlot(m_position + Position.Left);
			default:
				return null;
			}
		}

		public GridSlot GetConnection(EDirection p_dir, Boolean p_lineOfSight)
		{
			return GetConnection(p_dir, p_lineOfSight, false);
		}

		public GridSlot GetConnectionForRanged(EDirection p_dir, Boolean p_lineOfSight)
		{
			InitConnections(LegacyLogic.Instance.MapLoader.Grid);
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(m_position + p_dir);
			GridTransition gridTransition = m_transitions[(Int32)p_dir];
			if (gridTransition == null)
			{
				return null;
			}
			if (gridTransition.NextState == EGridTransitionType.CLOSED)
			{
				return null;
			}
			if (p_lineOfSight)
			{
				if (slot != null && !slot.HasEntities && (slot.TerrainType & ETerrainType.BLOCKED) == ETerrainType.NONE)
				{
					return slot;
				}
				if (slot != null && !slot.HasEntities && (slot.TerrainType & ETerrainType.SHOOT_THROUGH) == ETerrainType.SHOOT_THROUGH)
				{
					return slot;
				}
			}
			else
			{
				if (slot != null && (slot.TerrainType & ETerrainType.BLOCKED) == ETerrainType.NONE)
				{
					return slot;
				}
				if (slot != null && (slot.TerrainType & ETerrainType.SHOOT_THROUGH) == ETerrainType.SHOOT_THROUGH)
				{
					return slot;
				}
			}
			return null;
		}

		public GridSlot GetConnection(EDirection p_dir, Boolean p_lineOfSight, Boolean p_flyThrought)
		{
			if (m_transitions[(Int32)p_dir].NextState == EGridTransitionType.OPEN)
			{
				InitConnections(LegacyLogic.Instance.MapLoader.Grid);
				GridSlot gridSlot = m_connections[(Int32)p_dir];
				if (p_lineOfSight)
				{
					if (gridSlot != null && !gridSlot.HasEntities && ((gridSlot.TerrainType & ETerrainType.BLOCKED) == ETerrainType.NONE || (p_flyThrought && (gridSlot.TerrainType & ETerrainType.FLY_THROUGH) != ETerrainType.NONE)))
					{
						return gridSlot;
					}
				}
				else if (gridSlot != null && ((gridSlot.TerrainType & ETerrainType.BLOCKED) == ETerrainType.NONE || (p_flyThrought && (gridSlot.TerrainType & ETerrainType.FLY_THROUGH) != ETerrainType.NONE)))
				{
					return gridSlot;
				}
			}
			return null;
		}

		private Boolean CheckDoorsInLocation(EDirection p_dir)
		{
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				Door door = interactiveObject as Door;
				if (door != null && door.Location == p_dir && door.OpenableByMonsters)
				{
					return true;
				}
			}
			return m_transitions[(Int32)p_dir].NextState != EGridTransitionType.CLOSED;
		}

		public Boolean IsPassable(MovingEntity p_entity, Boolean p_isForDistanceCalc)
		{
			return IsPassable(p_entity, p_isForDistanceCalc, false);
		}

		public Boolean IsPassable(MovingEntity p_entity, Boolean p_isForDistanceCalc, Boolean p_checkForSummons)
		{
			if (p_isForDistanceCalc)
			{
				if (p_entity != null && p_entity is Monster && ((Monster)p_entity).AiHandler.CalculatesRanged)
				{
					return p_entity.CanPassTerrain(m_terrainType) || (m_terrainType & ETerrainType.SHOOT_THROUGH) != ETerrainType.NONE;
				}
				return p_entity != null && p_entity.CanPassTerrain(m_terrainType);
			}
			else
			{
				if (!p_checkForSummons)
				{
					return p_entity != null && p_entity.CanPassTerrain(m_terrainType) && p_entity.Size <= m_EntitySpace;
				}
				return p_entity != null && p_entity.CanPassTerrain(m_terrainType) && p_entity.Size <= m_EntitySpace && !CheckForSummons();
			}
		}

		public GridTransition GetTransition(EDirection p_dir)
		{
			return m_transitions[(Int32)p_dir];
		}

		internal void InitConnections(Grid p_grid)
		{
			m_connections[0] = CheckConnection(p_grid.GetSlot(m_position + Position.Up), m_transitions[0]);
			m_connections[1] = CheckConnection(p_grid.GetSlot(m_position + Position.Right), m_transitions[1]);
			m_connections[2] = CheckConnection(p_grid.GetSlot(m_position + Position.Down), m_transitions[2]);
			m_connections[3] = CheckConnection(p_grid.GetSlot(m_position + Position.Left), m_transitions[3]);
		}

		private static GridSlot CheckConnection(GridSlot targetSlot, GridTransition transition)
		{
			if (targetSlot != null && (targetSlot.TerrainType & ETerrainType.BLOCKED) == ETerrainType.NONE)
			{
				if (!transition.IsDynamic && transition.NextState == EGridTransitionType.CLOSED)
				{
					return null;
				}
				return targetSlot;
			}
			else
			{
				if (targetSlot == null || (targetSlot.TerrainType & ETerrainType.FLY_THROUGH) == ETerrainType.NONE)
				{
					return null;
				}
				if (!transition.IsDynamic && transition.NextState == EGridTransitionType.CLOSED)
				{
					return null;
				}
				return targetSlot;
			}
		}

		internal void InitTransitionTypes()
		{
			EGridTransitionType transitionType = ((m_terrainType & ETerrainType.BLOCKED) == ETerrainType.NONE) ? EGridTransitionType.OPEN : EGridTransitionType.CLOSED;
			for (Int32 i = 0; i < m_transitions.Length; i++)
			{
				m_transitions[i].TransitionType = transitionType;
			}
		}

		public Boolean AddEntity(MovingEntity p_entity)
		{
			if (!IsPassable(p_entity, false) && (!p_entity.IsPushed || (m_terrainType & ETerrainType.HAZARDOUS) == ETerrainType.NONE))
			{
				return false;
			}
			if (p_entity.IsPushed && (m_terrainType & ETerrainType.HAZARDOUS) != ETerrainType.NONE && p_entity is Party)
			{
				((Party)p_entity).PushedOntoHazardousTerrain();
			}
			m_EntitySpace -= (Int32)p_entity.Size;
			m_Entities.Add(p_entity);
			if (p_entity is Party)
			{
				((Party)p_entity).CurrentMapArea = m_mapArea;
				if (!VisitedByParty)
				{
					VisitedByParty = true;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.UNCOVERED_TILES, EventArgs.Empty);
				}
			}
			p_entity.Position = Position;
			p_entity.Height = Height;
			return true;
		}

		public Boolean RemoveEntity(MovingEntity p_movingEntity)
		{
			if (p_movingEntity != null && m_Entities.Remove(p_movingEntity))
			{
				m_EntitySpace += (Int32)p_movingEntity.Size;
				return true;
			}
			return false;
		}

		public void AddInteractiveObject(InteractiveObject p_obj)
		{
			if (p_obj == null)
			{
				return;
			}
			m_objects.Add(p_obj);
			p_obj.OnAddedToGrid();
		}

		public void RemoveInteractiveObject(InteractiveObject p_obj)
		{
			if (p_obj == null)
			{
				return;
			}
			m_objects.Remove(p_obj);
		}

		public List<InteractiveObject> GetPassiveInteractiveObjects(EDirection p_location, Boolean p_hasSpotSecrets, Boolean p_hasClairvoyance, Boolean p_isReturnExecutableOnly)
		{
			List<InteractiveObject> list = new List<InteractiveObject>();
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if ((!(interactiveObject is Trap) || ((Trap)interactiveObject).TrapActive) && interactiveObject.IsInteractable(p_location, p_hasSpotSecrets, p_hasClairvoyance, p_isReturnExecutableOnly))
				{
					list.Add(interactiveObject);
				}
			}
			return list;
		}

		public List<InteractiveObject> GetActiveInteractiveObjects(EDirection p_location)
		{
			List<InteractiveObject> list = new List<InteractiveObject>();
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if (interactiveObject.Enabled && interactiveObject.Location == p_location && interactiveObject.SelfActive && !(interactiveObject is ConditionalTrigger) && !(interactiveObject is PrefabContainer))
				{
					list.Add(interactiveObject);
				}
			}
			return list;
		}

		public IEnumerable<InteractiveObject> GetInteractiveObjectIterator()
		{
			return m_objects;
		}

		public List<InteractiveObject> GetInteractiveObjectsWithClickableViewState()
		{
			List<InteractiveObject> list = new List<InteractiveObject>();
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if (interactiveObject.IsClickableViewState)
				{
					list.Add(interactiveObject);
				}
			}
			return list;
		}

		public List<InteractiveObject> GetDoors()
		{
			List<InteractiveObject> list = new List<InteractiveObject>();
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if (interactiveObject is Door)
				{
					list.Add(interactiveObject);
				}
			}
			return list;
		}

		public List<Trap> GetActiveTraps(EDirection p_location)
		{
			List<Trap> list = new List<Trap>();
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if (interactiveObject.Location == p_location && interactiveObject is Trap && ((Trap)interactiveObject).TrapActive)
				{
					list.Add((Trap)interactiveObject);
				}
			}
			return list;
		}

		public InteractiveObject FindInteractiveObject(Int32 p_spawnerID)
		{
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if (interactiveObject.SpawnerID == p_spawnerID)
				{
					return interactiveObject;
				}
			}
			return null;
		}

		public Spawn FindSpawn(Int32 p_spawnerID)
		{
			foreach (Spawn spawn in m_spawnObjects)
			{
				if (spawn.ID == p_spawnerID)
				{
					return spawn;
				}
			}
			return null;
		}

		public NpcContainer FindNpcContainer(Int32 p_npcStaticID)
		{
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				NpcContainer npcContainer = interactiveObject as NpcContainer;
				if (npcContainer != null && npcContainer.Contains(p_npcStaticID))
				{
					return npcContainer;
				}
			}
			return null;
		}

		public void SpotSecrets()
		{
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if (interactiveObject.IsSecret && interactiveObject.IsExecutable(LegacyLogic.Instance.MapLoader.Grid))
				{
					if (!(interactiveObject is Door) || ((Door)interactiveObject).State != EInteractiveObjectState.DOOR_OPEN)
					{
						BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(interactiveObject, interactiveObject.Position);
						LegacyLogic.Instance.EventManager.InvokeEvent(interactiveObject, EEventType.HIGHLIGHT_SECRET, p_eventArgs);
						if (!m_TriggeredBark)
						{
							m_TriggeredBark = true;
							LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.SECRET);
						}
					}
				}
			}
		}

		public void SpotTraps()
		{
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				if (interactiveObject.TrapActive)
				{
					BaseObjectEventArgs p_eventArgs = new BaseObjectEventArgs(interactiveObject, interactiveObject.Position);
					LegacyLogic.Instance.EventManager.InvokeEvent(interactiveObject, EEventType.HIGHLIGHT_TRAP, p_eventArgs);
					if (!m_TriggeredBark)
					{
						m_TriggeredBark = true;
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.TRAP);
					}
				}
			}
		}

		internal Boolean CheckForSummons()
		{
			return false;
		}

		public Boolean Equals(GridSlot other)
		{
			return other != null && m_position == other.m_position;
		}

		public override Int32 GetHashCode()
		{
			return m_position.GetHashCode();
		}

		public override String ToString()
		{
			String text = String.Empty;
			foreach (InteractiveObject interactiveObject in m_objects)
			{
				text = text + interactiveObject.ToString() + ",";
			}
			return text;
		}
	}
}
