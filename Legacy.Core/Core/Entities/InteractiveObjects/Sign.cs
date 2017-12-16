using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Sign : InteractiveObject
	{
		public Sign() : this(0, 0)
		{
		}

		public Sign(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.SIGN, p_spawnerID)
		{
		}
	}
}
