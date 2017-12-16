using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Trap : InteractiveObject
	{
		public Trap() : this(0, 0)
		{
		}

		public Trap(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.TRAP, p_spawnerID)
		{
		}
	}
}
