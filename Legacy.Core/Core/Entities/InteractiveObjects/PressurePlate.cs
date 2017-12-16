using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class PressurePlate : Button
	{
		public PressurePlate() : this(0, 0)
		{
		}

		public PressurePlate(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.PRESSURE_PLATE, p_spawnerID)
		{
		}
	}
}
