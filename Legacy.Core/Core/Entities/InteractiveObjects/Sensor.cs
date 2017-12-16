using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Sensor : InteractiveObject
	{
		public Sensor() : this(0, 0)
		{
		}

		public Sensor(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.SENSOR, p_spawnerID)
		{
		}
	}
}
