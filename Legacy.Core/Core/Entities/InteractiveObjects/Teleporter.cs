using System;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class Teleporter : InteractiveObject
	{
		public Teleporter() : this(0, 0)
		{
		}

		public Teleporter(Int32 p_staticID, Int32 p_spawnerID) : base(p_staticID, EObjectType.TELEPORTER, p_spawnerID)
		{
		}

		public override Boolean MinimapVisible
		{
			get
			{
				foreach (SpawnCommand spawnCommand in m_commands)
				{
					if (spawnCommand.Type == EInteraction.TELEPORT)
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
