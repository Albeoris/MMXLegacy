using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Pathfinding;
using Legacy.Core.WorldMap;
using Legacy.Utilities;

namespace Legacy.Core.Map
{
	public class Grid
	{
		private EDungeonStyle m_style;

		private Int32 m_maxAssignedSpawnID;

		private GridSlot[][] m_slots;

		public Grid()
		{
		}

		public Grid(Int32 p_width, Int32 p_height, EMapType p_mapType)
		{
			Width = p_width;
			Height = p_height;
			Type = p_mapType;
			ETerrainType defaultTerrain = GetDefaultTerrain(Type);
			m_slots = new GridSlot[Height][];
			for (Int32 i = 0; i < Height; i++)
			{
				m_slots[i] = new GridSlot[Width];
				for (Int32 j = 0; j < Width; j++)
				{
					GridSlot gridSlot = new GridSlot();
					gridSlot.Position = new Position(j, i);
					gridSlot.TerrainType = defaultTerrain;
					gridSlot.InitTransitionTypes();
					m_slots[i][j] = gridSlot;
				}
			}
			InitConnections();
		}

		[XmlElement("Name")]
		public String Name { get; set; }

		[XmlElement("SceneName")]
		public String SceneName { get; set; }

		[XmlElement("MinimapName")]
		public String MinimapName { get; set; }

		[XmlElement("MinimapOffsetX")]
		public Single MinimapOffsetX { get; set; }

		[XmlElement("MinimapOffsetY")]
		public Single MinimapOffsetY { get; set; }

		[XmlElement("WorldMapPointID")]
		public Int32 WorldMapPointID { get; set; }

		[XmlElement("LocationLocaName")]
		public String LocationLocaName { get; set; }

		[XmlElement("Width")]
		public Int32 Width { get; set; }

		[XmlElement("Height")]
		public Int32 Height { get; set; }

		[XmlElement("Type")]
		public EMapType Type { get; set; }

		[XmlElement("Style")]
		public EDungeonStyle Style
		{
			get => m_style;
		    set
			{
				if (Type == EMapType.DUNGEON)
				{
					m_style = value;
				}
			}
		}

		[XmlArray("GridSlots")]
		[XmlArrayItem("Row")]
		[Obsolete("Only for XmlDump")]
		public GridSlot[][] Slots
		{
			get => m_slots;
		    set => m_slots = value;
		}

		[XmlElement("MinLevel")]
		public Int32 MinLevel { get; set; }

		[XmlElement("MaxLevel")]
		public Int32 MaxLevel { get; set; }

		[XmlElement("OffsetX")]
		public Single OffsetX { get; set; }

		[XmlElement("OffsetY")]
		public Single OffsetY { get; set; }

		[XmlElement("OffsetZ")]
		public Single OffsetZ { get; set; }

		[XmlElement("MusicAudioIDDay")]
		public String MusicAudioIDDay { get; set; }

		[XmlElement("MusicAudioIDNight")]
		public String MusicAudioIDNight { get; set; }

		[XmlElement("IsWithFightMusic")]
		public Boolean IsWithFightMusic { get; set; }

		public Int32 MaxAssignedSpawnID
		{
			get => m_maxAssignedSpawnID;
		    set => m_maxAssignedSpawnID = value;
		}

		public void InitConnections()
		{
			for (Int32 i = 0; i < Height; i++)
			{
				for (Int32 j = 0; j < Width; j++)
				{
					m_slots[i][j].SetSlotsForTransition();
					m_slots[i][j].InitConnections(this);
				}
			}
		}

		public GridSlot GetSlot(Position p_pos)
		{
			if (p_pos.X < 0 || p_pos.Y < 0 || p_pos.X >= Width || p_pos.Y >= Height)
			{
				return null;
			}
			return m_slots[p_pos.Y][p_pos.X];
		}

		public void SetSlot(Position p_pos, GridSlot p_slot)
		{
			if (p_pos.X < 0 || p_pos.Y < 0 || p_pos.X >= Width || p_pos.Y >= Height)
			{
				return;
			}
			m_slots[p_pos.Y][p_pos.X] = p_slot;
		}

		public IEnumerable<GridSlot> SlotIterator()
		{
			for (Int32 y = 0; y < Height; y++)
			{
				for (Int32 x = 0; x < Width; x++)
				{
					yield return m_slots[y][x];
				}
			}
			yield break;
		}

		public IEnumerable<GridSlot> SlotIterator(Position p_position, Int32 p_width, Int32 p_height)
		{
			p_position.X = Math.Max(p_position.X, 0);
			p_position.Y = Math.Max(p_position.Y, 0);
			Int32 maxWidth = Math.Min(p_position.X + p_width, Width);
			Int32 maxHeight = Math.Min(p_position.Y + p_height, Height);
			for (Int32 y = p_position.Y; y < maxHeight; y++)
			{
				for (Int32 x = p_position.X; x < maxWidth; x++)
				{
					yield return m_slots[y][x];
				}
			}
			yield break;
		}

		public IEnumerable<GridSlot> SlotIteratorAround(Position p_center, Int32 p_radius)
		{
			p_center.X -= p_radius;
			p_center.Y -= p_radius;
			p_radius *= 2;
			return SlotIterator(p_center, p_radius, p_radius);
		}

		private static ETerrainType GetDefaultTerrain(EMapType p_mapType)
		{
			Boolean flag = p_mapType == EMapType.DUNGEON;
			return (!flag) ? ETerrainType.PASSABLE : ETerrainType.BLOCKED;
		}

		public Boolean AddMovingEntity(Position p_position, MovingEntity p_movingEntity)
		{
			GridSlot slot = GetSlot(p_position);
			return slot != null && slot.AddEntity(p_movingEntity);
		}

		public Boolean RemoveMovingEntity(MovingEntity p_movingEntity)
		{
			GridSlot slot = GetSlot(p_movingEntity.Position);
			return slot != null && slot.RemoveEntity(p_movingEntity);
		}

		public Boolean CanMoveEntity(MovingEntity p_entity, EDirection p_dir)
		{
			Position p_pos = p_entity.Position + p_dir;
			GridSlot slot = GetSlot(p_pos);
			if (slot == null)
			{
				return false;
			}
			if (p_entity is Party)
			{
				if (((Party)p_entity).IsPushed && (slot.TerrainType & ETerrainType.HAZARDOUS) != ETerrainType.NONE)
				{
					return true;
				}
				if (!((Party)p_entity).CanPassTerrain(slot.TerrainType))
				{
					return false;
				}
				if (slot.CountEntityTypes(EObjectType.MONSTER) > 0)
				{
					return false;
				}
			}
			else if (!p_entity.CanPassTerrain(slot.TerrainType))
			{
				return false;
			}
			GridSlot slot2 = GetSlot(p_entity.Position);
			EGridTransitionType egridTransitionType = (!(p_entity is Party)) ? slot2.GetTransition(p_dir).NextState : slot2.GetTransition(p_dir).TransitionType;
			return egridTransitionType != EGridTransitionType.CLOSED;
		}

		private void NotifyPlayer(ETerrainType p_type)
		{
			String text = String.Empty;
			if (p_type != ETerrainType.WATER)
			{
				if (p_type != ETerrainType.ROUGH)
				{
					if (p_type == ETerrainType.FOREST)
					{
						text = "TOKEN_BLESSING_ENTER_FOREST";
					}
				}
				else
				{
					text = "TOKEN_BLESSING_ENTER_ROUGH";
				}
			}
			else
			{
				text = "TOKEN_BLESSING_ENTER_WATER";
			}
			if (!String.IsNullOrEmpty(text))
			{
				GameMessageEventArgs gameMessageEventArgs = new GameMessageEventArgs("GAME_MESSAGE_BLESSING_IS_MISSING", text, 0f, true);
				gameMessageEventArgs.IsTerrainMessage = true;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.GAME_MESSAGE, gameMessageEventArgs);
			}
		}

		public Boolean MoveEntity(MovingEntity p_entity, EDirection p_dir)
		{
			if (!CanMoveEntity(p_entity, p_dir))
			{
				return false;
			}
			Position position = p_entity.Position + p_dir;
			GridSlot slot = GetSlot(p_entity.Position);
			Position position2 = p_entity.Position;
			if (!GetSlot(position).AddEntity(p_entity))
			{
				return false;
			}
			CheckPassable(p_entity, position);
			if (p_entity is Party && LegacyLogic.Instance.WorldManager.CheckPassableOnMovement)
			{
				GridSlot slot2 = GetSlot(position);
				for (EDirection edirection = EDirection.NORTH; edirection <= EDirection.WEST; edirection++)
				{
					GridSlot slot3 = GetSlot(position + edirection);
					if (slot3 != null)
					{
						Single num = Math.Abs(slot3.Height - slot2.Height);
						if (num <= 2f)
						{
							slot2.GetTransition(edirection).Open();
							slot3.GetTransition(EDirectionFunctions.GetOppositeDir(edirection)).Open();
						}
						else
						{
							slot2.GetTransition(edirection).Close();
							slot3.GetTransition(EDirectionFunctions.GetOppositeDir(edirection)).Close();
						}
					}
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.REEVALUATE_PASSABLE, EventArgs.Empty);
			}
			slot.RemoveEntity(p_entity);
			MoveEntityEventArgs p_eventArgs = new MoveEntityEventArgs(position2, position);
			LegacyLogic.Instance.EventManager.InvokeEvent(p_entity, EEventType.MOVE_ENTITY, p_eventArgs);
			return true;
		}

		public void CheckPassable(MovingEntity p_entity, Position target)
		{
			if (p_entity is Party && LegacyLogic.Instance.WorldManager.CheckPassableOnMovement)
			{
				GridSlot slot = GetSlot(target);
				for (EDirection edirection = EDirection.NORTH; edirection <= EDirection.WEST; edirection++)
				{
					GridSlot slot2 = GetSlot(target + edirection);
					if (slot2 != null)
					{
						Single num = Math.Abs(slot2.Height - slot.Height);
						if (num <= 2f)
						{
							slot.GetTransition(edirection).Open();
							slot2.GetTransition(EDirectionFunctions.GetOppositeDir(edirection)).Open();
						}
						else
						{
							slot.GetTransition(edirection).Close();
							slot2.GetTransition(EDirectionFunctions.GetOppositeDir(edirection)).Close();
						}
					}
				}
				LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.REEVALUATE_PASSABLE, EventArgs.Empty);
			}
		}

		public void MoveObject(InteractiveObject p_target, Position p_newPos)
		{
			GridSlot slot = GetSlot(p_target.Position);
			GetSlot(p_newPos).AddInteractiveObject(p_target);
			slot.RemoveInteractiveObject(p_target);
		}

		public void ReboundOffWall(MovingEntity p_entity, EDirection p_dir)
		{
			Position position = p_entity.Position + p_dir;
			ReboundEntityEventArgs p_eventArgs = new ReboundEntityEventArgs(p_entity.Position, position);
			LegacyLogic.Instance.EventManager.InvokeEvent(p_entity, EEventType.REBOUND_ENTITY, p_eventArgs);
			if (p_entity is Party && GetSlot(position) != null)
			{
				GridSlot slot = GetSlot(p_entity.Position);
				if (slot != null && slot.GetTransition(p_dir).TransitionType == EGridTransitionType.OPEN)
				{
					NotifyPlayer(GetSlot(position).TerrainType);
				}
			}
		}

		public void AddInteractiveObject(InteractiveObject p_obj)
		{
			GridSlot slot = GetSlot(p_obj.Position);
			if (slot == null)
			{
				throw new ArgumentOutOfRangeException("p_obj", String.Concat(new Object[]
				{
					"Object Position out of range: ",
					p_obj.Position.ToString(),
					" ",
					p_obj.Type,
					" ",
					p_obj.SpawnerID
				}));
			}
			slot.AddInteractiveObject(p_obj);
		}

		public InteractiveObject FindInteractiveObject(Int32 p_spawnerID)
		{
			return FindInteractiveObject<InteractiveObject>(p_spawnerID);
		}

		public Spawn FindSpawn(Int32 p_spawnerID)
		{
			for (Int32 i = 0; i < Height; i++)
			{
				for (Int32 j = 0; j < Width; j++)
				{
					Spawn result;
					if ((result = m_slots[i][j].FindSpawn(p_spawnerID)) != null)
					{
						return result;
					}
				}
			}
			return null;
		}

		public T FindInteractiveObject<T>(Int32 p_spawnerID) where T : InteractiveObject
		{
			for (Int32 i = 0; i < Height; i++)
			{
				for (Int32 j = 0; j < Width; j++)
				{
					InteractiveObject interactiveObject;
					if ((interactiveObject = m_slots[i][j].FindInteractiveObject(p_spawnerID)) != null)
					{
						return interactiveObject as T;
					}
				}
			}
			return null;
		}

		public Boolean CheckInCombat(Position p_pos)
		{
			GridSlot slot = GetSlot(p_pos);
			if (slot != null)
			{
				for (EDirection edirection = EDirection.NORTH; edirection < EDirection.COUNT; edirection++)
				{
					if (slot.GetTransition(edirection).NextState == EGridTransitionType.OPEN)
					{
						GridSlot slot2 = GetSlot(p_pos + edirection);
						if (CheckSlotForEntities(slot2))
						{
							return true;
						}
					}
					else if (!slot.GetTransition(edirection).IsDynamic)
					{
						GridSlot neighborSlot = slot.GetNeighborSlot(this, edirection);
						if (neighborSlot != null && (neighborSlot.TerrainType & ETerrainType.FLY_THROUGH) != ETerrainType.NONE && CheckSlotForEntities(neighborSlot))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private Boolean CheckSlotForEntities(GridSlot n)
		{
			if (n != null && n.HasEntities)
			{
				IList<MovingEntity> entities = n.Entities;
				for (Int32 i = 0; i < entities.Count; i++)
				{
					Monster monster = entities[i] as Monster;
					if (monster != null && monster.CurrentHealth > 0 && monster.AlwaysTriggerAggro)
					{
						return true;
					}
				}
			}
			return false;
		}

		public Boolean LineOfSight(Position p_start, Position p_end)
		{
			return LineOfSight(p_start, p_end, false);
		}

		public Boolean LineOfSight(Position p_start, Position p_end, Boolean p_forRanged)
		{
			if ((p_start != p_end && p_start.X == p_end.X) || p_start.Y == p_end.Y)
			{
				GridSlot slot = GetSlot(p_start);
				GridSlot slot2 = GetSlot(p_end);
				EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(ref p_start, ref p_end);
				if (TestLineOfSight(slot, slot2, lineOfSightDirection, p_forRanged))
				{
					return true;
				}
			}
			return false;
		}

		private static Boolean TestLineOfSight(GridSlot p_start, GridSlot p_end, EDirection p_dir, Boolean p_forRanged)
		{
			GridSlot gridSlot = p_start;
			Boolean flag = true;
			while (flag)
			{
				GridSlot gridSlot2 = gridSlot;
				if (!p_forRanged)
				{
					gridSlot = gridSlot.GetConnection(p_dir, true);
				}
				else
				{
					gridSlot = gridSlot.GetConnectionForRanged(p_dir, true);
				}
				if (gridSlot == null)
				{
					GridSlot gridSlot3;
					if (!p_forRanged)
					{
						gridSlot3 = gridSlot2.GetConnection(p_dir, false);
					}
					else
					{
						gridSlot3 = gridSlot2.GetConnectionForRanged(p_dir, false);
					}
					if (gridSlot3 == p_end)
					{
						return true;
					}
					flag = false;
				}
				else if (gridSlot == p_end)
				{
					return true;
				}
			}
			return false;
		}

		public GridSlot GetVisitedSlotInDirection(GridSlot p_slot, EDirection p_direction, Boolean p_forRangedAttack)
		{
			GridSlot gridSlot = null;
			if (p_forRangedAttack)
			{
				gridSlot = p_slot.GetConnectionForRanged(p_direction, false);
			}
			else
			{
				gridSlot = p_slot.GetConnection(p_direction, false);
			}
			Int32 num = 10;
			Int32 num2 = 0;
			while (gridSlot != null)
			{
				IList<MovingEntity> entities = gridSlot.Entities;
				foreach (MovingEntity movingEntity in entities)
				{
					if (movingEntity is Monster)
					{
						return gridSlot;
					}
				}
				if (p_forRangedAttack)
				{
					gridSlot = gridSlot.GetConnectionForRanged(p_direction, false);
				}
				else
				{
					gridSlot = gridSlot.GetConnection(p_direction, false);
				}
				num2++;
				if (num2 > num)
				{
					break;
				}
			}
			return null;
		}

		public GridSlot GetFreeSlotInDirection(GridSlot p_slot, EDirection p_direction, Int32 p_maxSteps)
		{
			GridSlot gridSlot = null;
			GridSlot connection = p_slot.GetConnection(p_direction, false);
			Int32 num = 0;
			while (connection != null)
			{
				if ((connection.TerrainType & ETerrainType.PASSABLE) != ETerrainType.NONE || (connection.TerrainType & ETerrainType.ROUGH) != ETerrainType.NONE)
				{
					gridSlot = connection;
				}
				connection = connection.GetConnection(p_direction, false);
				num++;
				if (num > p_maxSteps)
				{
					break;
				}
				if (gridSlot.HasEntities)
				{
					Boolean flag = false;
					foreach (MovingEntity movingEntity in gridSlot.Entities)
					{
						if (movingEntity is Monster)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return gridSlot;
		}

		public Monster GetRandomMonsterInDirection(Position p_origin, EDirection p_direction, Int32 p_range)
		{
			Boolean flag = p_range > 1;
			GridSlot gridSlot = GetSlot(p_origin);
			for (Int32 i = 0; i <= p_range; i++)
			{
				if (gridSlot == null)
				{
					break;
				}
				if (!flag && (gridSlot.TerrainType & ETerrainType.BLOCKED) != ETerrainType.NONE)
				{
					break;
				}
				if (flag && (gridSlot.TerrainType & ETerrainType.BLOCKED) == ETerrainType.BLOCKED && (gridSlot.TerrainType & ETerrainType.SHOOT_THROUGH) == ETerrainType.NONE)
				{
					break;
				}
				IList<MovingEntity> entities = gridSlot.Entities;
				for (Int32 j = 0; j < entities.Count; j++)
				{
					Int32 index = Random.Range(0, entities.Count);
					if (entities[index] is Monster)
					{
						return (Monster)entities[index];
					}
				}
				gridSlot = (flag ? gridSlot.GetConnectionForRanged(p_direction, false) : gridSlot.GetConnection(p_direction, false));
			}
			return null;
		}

		public Int32 GetMonstersOnFirstSlot(Position p_origin, EDirection p_direction, Int32 p_range, ICollection<Object> p_buffer)
		{
			Boolean flag = p_range > 1;
			Int32 num = 0;
			GridSlot slot = GetSlot(p_origin);
			for (Int32 i = 0; i <= p_range; i++)
			{
				if (num > 0 || slot == null)
				{
					break;
				}
				if (!flag && (slot.TerrainType & ETerrainType.BLOCKED) != ETerrainType.NONE && (slot.TerrainType & ETerrainType.SHOOT_THROUGH) == ETerrainType.NONE)
				{
					break;
				}
				if (flag && (slot.TerrainType & ETerrainType.BLOCKED) != ETerrainType.NONE && (slot.TerrainType & ETerrainType.SHOOT_THROUGH) == ETerrainType.NONE)
				{
					break;
				}
				IList<MovingEntity> entities = slot.Entities;
				for (Int32 j = 0; j < entities.Count; j++)
				{
					if (entities[j] is Monster)
					{
						p_buffer.Add(entities[j]);
						num++;
					}
				}
				if (slot.GetTransition(p_direction).NextState != EGridTransitionType.OPEN)
				{
					break;
				}
				slot = GetSlot(slot.Position + p_direction);
			}
			return num;
		}

		public Int32 GetMonstersInDirection(Position p_origin, EDirection p_direction, Int32 p_range, ICollection<Object> p_buffer)
		{
			Boolean flag = p_range > 1;
			Int32 num = 0;
			GridSlot slot = GetSlot(p_origin);
			for (Int32 i = 0; i <= p_range; i++)
			{
				if (slot == null)
				{
					break;
				}
				if (!flag && (slot.TerrainType & ETerrainType.BLOCKED) != ETerrainType.NONE)
				{
					break;
				}
				if (flag && (slot.TerrainType & ETerrainType.BLOCKED) != ETerrainType.NONE && (slot.TerrainType & ETerrainType.SHOOT_THROUGH) == ETerrainType.NONE)
				{
					break;
				}
				IList<MovingEntity> entities = slot.Entities;
				for (Int32 j = 0; j < entities.Count; j++)
				{
					if (entities[j] is Monster)
					{
						p_buffer.Add(entities[j]);
						num++;
					}
				}
				if (slot.GetTransition(p_direction).NextState != EGridTransitionType.OPEN)
				{
					break;
				}
				slot = GetSlot(slot.Position + p_direction);
			}
			return num;
		}

		public List<Object> GetOtherMonstersOnSlot(Monster p_monster)
		{
			Position position = p_monster.Position;
			GridSlot slot = GetSlot(position);
			IList<MovingEntity> entities = slot.Entities;
			List<Object> list = new List<Object>(entities.Count);
			for (Int32 i = 0; i < entities.Count; i++)
			{
				if (entities[i] is Monster && entities[i] != p_monster)
				{
					list.Add(entities[i]);
				}
			}
			return list;
		}

		public void SpotSecrets(Position p_pos, EDirection p_dir)
		{
			foreach (Position p_pos2 in new List<Position>
			{
				p_pos,
				p_pos + p_dir,
				p_pos + EDirectionFunctions.Add(p_dir, -1),
				p_pos + EDirectionFunctions.Add(p_dir, 1),
				p_pos + p_dir + EDirectionFunctions.Add(p_dir, -1),
				p_pos + p_dir + EDirectionFunctions.Add(p_dir, 1)
			})
			{
				GridSlot slot = GetSlot(p_pos2);
				if (slot != null)
				{
					slot.SpotSecrets();
				}
			}
		}

		public void SpotTraps(Position p_pos, EDirection p_dir)
		{
			foreach (Position p_pos2 in new List<Position>
			{
				p_pos,
				p_pos + EDirectionFunctions.Add(p_dir, -1),
				p_pos + EDirectionFunctions.Add(p_dir, 1),
				p_pos + p_dir,
				p_pos + p_dir + EDirectionFunctions.Add(p_dir, -1),
				p_pos + p_dir + EDirectionFunctions.Add(p_dir, 1),
				p_pos + p_dir + p_dir,
				p_pos + p_dir + p_dir + EDirectionFunctions.Add(p_dir, -1),
				p_pos + p_dir + p_dir + EDirectionFunctions.Add(p_dir, 1)
			})
			{
				GridSlot slot = GetSlot(p_pos2);
				if (slot != null)
				{
					slot.SpotTraps();
				}
			}
		}

		public List<GridSlot> GetGridSlotsOnLine(GridPosition p_start, GridPosition p_target)
		{
			List<GridSlot> list = new List<GridSlot>();
			Boolean flag = Math.Abs(p_start.X - p_target.X) == Math.Abs(p_start.Y - p_target.Y);
			p_start.X += 0.5f;
			p_start.Y += 0.5f;
			p_target.X += 0.5f;
			p_target.Y += 0.5f;
			Single num = (p_start.X >= p_target.X) ? p_target.X : p_start.X;
			Single num2 = (p_start.X <= p_target.X) ? p_target.X : p_start.X;
			Single num3 = (p_start.Y >= p_target.Y) ? p_target.Y : p_start.Y;
			Single num4 = (p_start.Y <= p_target.Y) ? p_target.Y : p_start.Y;
			if (LineOfSight(p_start.ToPosition(), p_target.ToPosition(), true))
			{
				EDirection lineOfSightDirection = EDirectionFunctions.GetLineOfSightDirection(p_start.ToPosition(), p_target.ToPosition());
				if (lineOfSightDirection != EDirection.COUNT)
				{
					GridSlot gridSlot = GetSlot(p_start);
					while (gridSlot != null && gridSlot.Position != p_target)
					{
						gridSlot = gridSlot.GetNeighborSlot(this, lineOfSightDirection);
						if (gridSlot == null)
						{
							list.Clear();
							break;
						}
						list.Add(gridSlot);
					}
					return list;
				}
			}
			Boolean flag2 = false;
			Int32 num5 = (Int32)num;
			while (num5 <= num2)
			{
				GridPosition gridPosition = Intersect(p_start, p_target, new GridPosition(num5, num3), new GridPosition(num5, num4));
				if (gridPosition != null)
				{
					if (gridPosition.X == gridPosition.Y)
					{
						GridSlot slot = GetSlot(new Position((Int32)(gridPosition.X - 0.5f), (Int32)(gridPosition.Y - 0.5f)));
						if (slot.Transitions[1].TransitionType == EGridTransitionType.CLOSED)
						{
							flag2 = true;
							break;
						}
						CheckAndAdd(list, slot);
						GridSlot slot2 = GetSlot(new Position((Int32)gridPosition.X, (Int32)gridPosition.Y));
						if (slot2.Transitions[3].TransitionType == EGridTransitionType.CLOSED)
						{
							flag2 = true;
							break;
						}
						CheckAndAdd(list, slot2);
						GridSlot slot3 = GetSlot(new Position((Int32)gridPosition.X, (Int32)(gridPosition.Y - 0.5f)));
						if (slot3.Transitions[2].TransitionType == EGridTransitionType.CLOSED)
						{
							flag2 = true;
							break;
						}
						CheckAndAdd(list, slot3);
						GridSlot slot4 = GetSlot(new Position((Int32)(gridPosition.X - 0.5f), (Int32)gridPosition.Y));
						if (slot4.Transitions[0].TransitionType == EGridTransitionType.CLOSED)
						{
							flag2 = true;
							break;
						}
						CheckAndAdd(list, slot4);
					}
					else if (gridPosition.X == num5)
					{
						GridSlot slot = GetSlot(new Position((Int32)(gridPosition.X - 0.5f), (Int32)gridPosition.Y));
						if (slot.Transitions[1].TransitionType == EGridTransitionType.CLOSED)
						{
							flag2 = true;
							break;
						}
						CheckAndAdd(list, slot);
						GridSlot slot2 = GetSlot(new Position((Int32)(gridPosition.X + 0.5f), (Int32)gridPosition.Y));
						if (slot2.Transitions[3].TransitionType == EGridTransitionType.CLOSED)
						{
							flag2 = true;
							break;
						}
						CheckAndAdd(list, slot2);
					}
				}
				num5++;
			}
			if (!flag2)
			{
				Int32 num6 = (Int32)num3;
				while (num6 <= num4)
				{
					GridPosition gridPosition2 = Intersect(p_start, p_target, new GridPosition(num, num6), new GridPosition(num2, num6));
					if (gridPosition2 != null)
					{
						if (gridPosition2.X == gridPosition2.Y)
						{
							GridSlot slot = GetSlot(new Position((Int32)(gridPosition2.X - 0.5f), (Int32)(gridPosition2.Y - 0.5f)));
							if (slot.Transitions[1].TransitionType == EGridTransitionType.CLOSED)
							{
								flag2 = true;
								break;
							}
							CheckAndAdd(list, slot);
							GridSlot slot2 = GetSlot(new Position((Int32)gridPosition2.X, (Int32)gridPosition2.Y));
							if (slot2.Transitions[3].TransitionType == EGridTransitionType.CLOSED)
							{
								flag2 = true;
								break;
							}
							CheckAndAdd(list, slot2);
							GridSlot slot3 = GetSlot(new Position((Int32)gridPosition2.X, (Int32)(gridPosition2.Y - 0.5f)));
							if (slot3.Transitions[0].TransitionType == EGridTransitionType.CLOSED)
							{
								flag2 = true;
								break;
							}
							CheckAndAdd(list, slot3);
							GridSlot slot4 = GetSlot(new Position((Int32)(gridPosition2.X - 0.5f), (Int32)gridPosition2.Y));
							if (slot4.Transitions[2].TransitionType == EGridTransitionType.CLOSED)
							{
								flag2 = true;
								break;
							}
							CheckAndAdd(list, slot4);
						}
						else if (gridPosition2.Y == num6)
						{
							GridSlot slot = GetSlot(new Position((Int32)gridPosition2.X, (Int32)(gridPosition2.Y - 0.5f)));
							if (slot.Transitions[0].TransitionType == EGridTransitionType.CLOSED)
							{
								flag2 = true;
								break;
							}
							CheckAndAdd(list, slot);
							GridSlot slot2 = GetSlot(new Position((Int32)gridPosition2.X, (Int32)(gridPosition2.Y + 0.5f)));
							if (slot2.Transitions[2].TransitionType == EGridTransitionType.CLOSED)
							{
								flag2 = true;
								break;
							}
							CheckAndAdd(list, slot2);
						}
					}
					num6++;
				}
			}
			if (flag2)
			{
				list.Clear();
			}
			return list;
		}

		private void CheckAndAdd(List<GridSlot> p_slots, GridSlot p_item)
		{
			if (p_item != null && !p_slots.Contains(p_item))
			{
				p_slots.Add(p_item);
			}
		}

		public static GridPosition Intersect(GridPosition a1, GridPosition a2, GridPosition b1, GridPosition b2)
		{
			GridPosition gridPosition = new GridPosition(a2.X - a1.X, a2.Y - a1.Y);
			GridPosition gridPosition2 = new GridPosition(b2.X - b1.X, b2.Y - b1.Y);
			Single num = gridPosition2.X * gridPosition.Y - gridPosition.X * gridPosition2.Y;
			if (Math.Abs(num) >= 1E-10f)
			{
				Single num2 = (-gridPosition2.Y * (b1.X - a1.X) + gridPosition2.X * (b1.Y - a1.Y)) / num;
				Single num3 = (-gridPosition.Y * (b1.X - a1.X) + gridPosition.X * (b1.Y - a1.Y)) / num;
				if (0f <= num2 && 0f <= num3 && num3 <= 1f)
				{
					return new GridPosition(a1.X + num2 * gridPosition.X, a1.Y + num2 * gridPosition.Y);
				}
			}
			return null;
		}

		public Int32 CalculateMovablePath(MovingEntity p_controller, Position p_target, List<GridSlot> p_path)
		{
			return AStarHelper<GridSlot>.Calculate(GetSlot(p_controller.Position), GetSlot(p_target), 10, p_controller, false, p_path);
		}

	    public Boolean GetPlayerPosition(out Position partyPosition)
	    {
	        if (Type == EMapType.OUTDOOR)
	        {
	            Party party = LegacyLogic.Instance.WorldManager.Party;
	            if (party != null)
	            {
	                partyPosition = party.Position;
	                return true;
	            }
	        }
            else if (Type == EMapType.CITY)
            {
                Party party = LegacyLogic.Instance.WorldManager.Party;
                if (party != null)
                {
                    partyPosition = party.Position;
                    return true;
                }
            }
	        else
	        {
	            WorldMapPoint worldMapPoint = LegacyLogic.Instance.WorldManager.WorldMapController.FindWorldMapPoint(WorldMapPointID);
	            if (worldMapPoint != null)
	            {
	                partyPosition = worldMapPoint.StaticData.Position;
	                return true;
	            }
                LegacyLogger.LogError("Grid mappoint ID not found! ID: " + WorldMapPointID);
	        }

	        partyPosition = default(Position);
            return false;
	    }
	}
}
