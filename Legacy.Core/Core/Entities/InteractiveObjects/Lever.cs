using System;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Lever : InteractiveObject
	{
		private EInteractiveObjectState m_State = EInteractiveObjectState.LEVER_UP;

		public Lever() : this(0, 0)
		{
		}

		public Lever(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.LEVER, p_spawnerID)
		{
		}

		public override EInteractiveObjectState State
		{
			get => m_State;
		    set
			{
				if (value != EInteractiveObjectState.LEVER_UP && value != EInteractiveObjectState.LEVER_DOWN)
				{
					throw new ArgumentException("Invalid Lever state: " + value);
				}
				m_State = value;
			}
		}

		public void ToggleState()
		{
			if (State == EInteractiveObjectState.LEVER_UP)
			{
				State = EInteractiveObjectState.LEVER_DOWN;
			}
			else if (State == EInteractiveObjectState.LEVER_DOWN)
			{
				State = EInteractiveObjectState.LEVER_UP;
			}
		}

		public void Up()
		{
			State = EInteractiveObjectState.LEVER_UP;
		}

		public void Down()
		{
			State = EInteractiveObjectState.LEVER_DOWN;
		}

		public override void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("LeverState", (Int32)m_State);
			base.Save(p_data);
		}

		public override void Load(SaveGameData p_data)
		{
			m_State = (EInteractiveObjectState)p_data.Get<Int32>("LeverState", 0);
			base.Load(p_data);
		}
	}
}
