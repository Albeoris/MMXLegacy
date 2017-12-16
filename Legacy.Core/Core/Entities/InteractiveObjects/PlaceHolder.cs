using System;
using Legacy.Core.Map;
using Legacy.Utilities;

namespace Legacy.Core.Entities.InteractiveObjects
{
	public class PlaceHolder : InteractiveObject
	{
		public PlaceHolder(Int32 p_staticID, Int32 p_spawnID) : base(p_staticID, EObjectType.PLACEHOLDER, p_spawnID)
		{
		}

		public override void Execute(Grid p_grid)
		{
			if (Commands.Count > 0)
			{
				LegacyLogger.Log("Place-Holder cannot execute any Commands");
			}
		}
	}
}
