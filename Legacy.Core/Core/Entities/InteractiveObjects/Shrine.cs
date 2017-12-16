using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Shrine : InteractiveObject
	{
		public Shrine() : this(0, 0)
		{
		}

		public Shrine(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.SHRINE, p_spawnerID)
		{
			State = EInteractiveObjectState.OFF;
		}
	}
}
