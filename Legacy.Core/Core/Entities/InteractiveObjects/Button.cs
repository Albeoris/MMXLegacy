using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Button : InteractiveObject
	{
		private Int32 m_turnsLeft;

		private EInteractiveObjectState m_State;

		public Button() : this(0, 0)
		{
		}

		public Button(Int32 p_staticID, Int32 p_spawnerID)
            :base(p_staticID, EObjectType.BUTTON, p_spawnerID)
		{
			m_State = EInteractiveObjectState.BUTTON_UP;
		}

		protected Button(Int32 p_staticID, EObjectType p_objectType, Int32 p_spawnerID)
            :base(p_staticID, p_objectType, p_spawnerID)
		{
			m_State = EInteractiveObjectState.BUTTON_UP;
		}

		public Int32 TurnsLeft => m_turnsLeft;

	    public override EInteractiveObjectState State
		{
			get => m_State;
	        set
			{
				if (value != EInteractiveObjectState.BUTTON_UP && value != EInteractiveObjectState.BUTTON_DOWN)
				{
					throw new ArgumentException("Invalid Button state: " + value);
				}
				m_State = value;
			}
		}

		public Boolean UpdateTimer()
		{
			if (m_turnsLeft > 0)
			{
				m_turnsLeft--;
				if (m_turnsLeft == 0)
				{
					if (State != EInteractiveObjectState.BUTTON_UP)
					{
						State = EInteractiveObjectState.BUTTON_UP;
						LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.OBJECT_STATE_CHANGED, new BaseObjectEventArgs(this, Position));
					}
					return true;
				}
			}
			return false;
		}

		public void ToggleState()
		{
			if (State == EInteractiveObjectState.BUTTON_UP)
			{
				State = EInteractiveObjectState.BUTTON_DOWN;
			}
			else if (State == EInteractiveObjectState.BUTTON_DOWN)
			{
				State = EInteractiveObjectState.BUTTON_UP;
				m_turnsLeft = 0;
			}
		}

		public void Down(Int32 p_time)
		{
			State = EInteractiveObjectState.BUTTON_DOWN;
			m_turnsLeft = p_time;
		}

		public void Up()
		{
			State = EInteractiveObjectState.BUTTON_UP;
			m_turnsLeft = 0;
		}

		public override void Load(SaveGameData p_data)
		{
			base.Load(p_data);
			m_turnsLeft = p_data.Get<Int32>("TurnsLeft", 0);
		}

		public override void Save(SaveGameData p_data)
		{
			base.Save(p_data);
			p_data.Set<Int32>("TurnsLeft", m_turnsLeft);
		}
	}
}
