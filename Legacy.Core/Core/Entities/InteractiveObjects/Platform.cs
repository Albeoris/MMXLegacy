using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Platform : InteractiveObject
	{
		public Platform() : this(0, 0)
		{
		}

		public Platform(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.PLATFORM, p_spawnerID)
		{
		}
	}
}
