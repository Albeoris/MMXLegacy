using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class CommandContainer : InteractiveObject
	{
		public CommandContainer(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.COMMAND_CONTAINER, p_spawnerID)
		{
		}
	}
}
