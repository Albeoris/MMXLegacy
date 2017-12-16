using System;
using System.Collections.Generic;
using Legacy.Core.Entities;
using Legacy.Core.Map;

namespace Legacy.Core.Pathfinding
{
	public interface IPathNode<T> : IEquatable<T>
	{
		Position Position { get; }

		Boolean Invalid { get; }

		List<T> GetConnections(Boolean checkDoors);

		Boolean IsPassableForEntity(MovingEntity p_monster, Boolean p_isForDistanceCalc, Boolean p_checkForSummons);
	}
}
