using System;
using Legacy.Core.Api;
using Legacy.Core.Combat;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.Utilities.StateManagement;

namespace Legacy.Core.Entities
{
	public abstract class MovingEntity : BaseObject
	{
		private Position m_position;

		private Single m_height;

		private EDirection m_direction;

		private Boolean m_isPushed;

		protected TriggerHelper m_movementDone = new TriggerHelper();

		protected TriggerHelper m_rotationDone = new TriggerHelper();

		protected TriggerHelper m_attackingDone = new TriggerHelper();

		protected TriggerHelper m_vanishAnimationDone = new TriggerHelper();

		protected TriggerHelper m_appearAnimationDone = new TriggerHelper();

		public MovingEntity(Int32 p_staticID, EObjectType p_type, Int32 p_spawnerID) : base(p_staticID, p_type, p_spawnerID)
		{
		}

		public TriggerHelper MovementDone => m_movementDone;

	    public TriggerHelper RotationDone => m_rotationDone;

	    public TriggerHelper AttackingDone => m_attackingDone;

	    public TriggerHelper VanishAnimationDone => m_vanishAnimationDone;

	    public TriggerHelper AppearAnimationDone => m_appearAnimationDone;

	    public Position Position
		{
			get => m_position;
	        set => m_position = value;
	    }

		public Single Height
		{
			get => m_height;
		    set => m_height = value;
		}

		public EDirection Direction
		{
			get => m_direction;
		    set
			{
				if (value < EDirection.NORTH || value > EDirection.WEST)
				{
					throw new ArgumentOutOfRangeException("value", value, "value < EDirection.NORTH || value > EDirection.WEST");
				}
				m_direction = value;
			}
		}

		public Boolean IsPushed
		{
			get => m_isPushed;
		    set => m_isPushed = value;
		}

		public abstract ESize Size { get; }

		public Boolean IsFinishTurn { get; set; }

		public virtual String Prefab { get; set; }

		public virtual String PrefabAlt { get; set; }

		public abstract void ApplyDamages(AttackResult p_result, Object p_source);

		public virtual void BeginTurn()
		{
			IsFinishTurn = false;
			m_movementDone.Reset();
			m_rotationDone.Reset();
			m_attackingDone.Reset();
		}

		public virtual void UpdateTurn()
		{
			IsFinishTurn = true;
		}

		public virtual void EndTurn()
		{
		}

		public virtual Boolean CanPassTerrain(ETerrainType p_type)
		{
			return true;
		}

		public Boolean Move(Position newPosition, Boolean immediate = false)
		{
			return Move(newPosition, m_direction, immediate);
		}

		public Boolean Move(Position newPosition, EDirection newDirection, Boolean immediate = false)
		{
			Grid grid = LegacyLogic.Instance.MapLoader.Grid;
			if (Position != newPosition || m_direction != newDirection)
			{
				m_direction = newDirection;
				Position position = Position;
				if (grid.AddMovingEntity(newPosition, this))
				{
					grid.GetSlot(position).RemoveEntity(this);
				}
				if (immediate)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SET_ENTITY_POSITION, new BaseObjectEventArgs(this, m_position));
				}
				else
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.MOVE_ENTITY, new MoveEntityEventArgs(position, Position));
				}
				return true;
			}
			return false;
		}

		public Boolean Rotate(Int32 turnDirection, Boolean immediate = false)
		{
			if (turnDirection == 0)
			{
				return false;
			}
			EDirection newDirection = EDirectionFunctions.Add(m_direction, turnDirection);
			return Rotate(newDirection, immediate);
		}

		public Boolean Rotate(EDirection newDirection, Boolean immediate = false)
		{
			if (newDirection != m_direction)
			{
				m_direction = newDirection;
				if (immediate)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.SET_ENTITY_POSITION, new BaseObjectEventArgs(this, m_position));
				}
				else
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.ROTATE_ENTITY, new BaseObjectEventArgs(this, m_position));
				}
				return true;
			}
			return false;
		}
	}
}
