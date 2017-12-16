using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Door : InteractiveObject
	{
		private EInteractiveObjectState m_State = EInteractiveObjectState.DOOR_CLOSED;

		private Boolean m_openableByMonsters = true;

		public Door() : this(0, 0)
		{
		}

		public Door(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.DOOR, p_spawnerID)
		{
		}

		public override EInteractiveObjectState State
		{
			get => m_State;
		    set
			{
				if (value != EInteractiveObjectState.DOOR_OPEN && value != EInteractiveObjectState.DOOR_CLOSED)
				{
					throw new ArgumentException("Invalid Door state: " + value);
				}
				if (m_State != value)
				{
					m_State = value;
					UpdateNextState();
					DoorEntityEventArgs p_eventArgs = new DoorEntityEventArgs(this, false, m_State);
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.DOOR_STATE_CHANGED, p_eventArgs);
				}
			}
		}

		public void SetState(EInteractiveObjectState p_state)
		{
			m_State = p_state;
		}

		public Boolean OpenableByMonsters
		{
			get => m_openableByMonsters;
		    set => m_openableByMonsters = value;
		}

		public void ToggleState()
		{
			if (State == EInteractiveObjectState.DOOR_OPEN)
			{
				State = EInteractiveObjectState.DOOR_CLOSED;
			}
			else if (State == EInteractiveObjectState.DOOR_CLOSED)
			{
				State = EInteractiveObjectState.DOOR_OPEN;
			}
		}

		public void Open()
		{
			State = EInteractiveObjectState.DOOR_OPEN;
		}

		public void Close()
		{
			State = EInteractiveObjectState.DOOR_CLOSED;
		}

		public override void OnAddedToGrid()
		{
			UpdateState();
		}

		public void UpdateState()
		{
			GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(Position);
			GridSlot neighborSlot = slot.GetNeighborSlot(LegacyLogic.Instance.MapLoader.Grid, Location);
			if (State == EInteractiveObjectState.DOOR_OPEN)
			{
				slot.GetTransition(Location).TransitionType = EGridTransitionType.OPEN;
				if (neighborSlot != null)
				{
					neighborSlot.GetTransition(EDirectionFunctions.GetOppositeDir(Location)).TransitionType = EGridTransitionType.OPEN;
				}
			}
			else
			{
				slot.GetTransition(Location).TransitionType = EGridTransitionType.CLOSED;
				if (neighborSlot != null)
				{
					neighborSlot.GetTransition(EDirectionFunctions.GetOppositeDir(Location)).TransitionType = EGridTransitionType.CLOSED;
				}
			}
		}

		public void UpdateNextState()
		{
			UpdateNextState(State);
		}

		internal void UpdateNextState(EInteractiveObjectState p_targetState)
		{
			if (LegacyLogic.Instance.MapLoader.Grid != null)
			{
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(Position);
				GridSlot neighborSlot = slot.GetNeighborSlot(LegacyLogic.Instance.MapLoader.Grid, Location);
				if (p_targetState == EInteractiveObjectState.DOOR_OPEN)
				{
					slot.GetTransition(Location).NextState = EGridTransitionType.OPEN;
					if (neighborSlot != null)
					{
						neighborSlot.GetTransition(EDirectionFunctions.GetOppositeDir(Location)).NextState = EGridTransitionType.OPEN;
					}
				}
				else
				{
					slot.GetTransition(Location).NextState = EGridTransitionType.CLOSED;
					if (neighborSlot != null)
					{
						neighborSlot.GetTransition(EDirectionFunctions.GetOppositeDir(Location)).NextState = EGridTransitionType.CLOSED;
					}
				}
			}
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_openableByMonsters = p_data.Get<Boolean>("OpenableByMonsters", true);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<Boolean>("OpenableByMonsters", m_openableByMonsters);
		}
	}
}
