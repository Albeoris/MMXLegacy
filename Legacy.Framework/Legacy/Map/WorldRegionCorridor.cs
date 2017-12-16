using System;
using UnityEngine;

namespace Legacy.Map
{
	[AddComponentMenu("MM Legacy/Map/Regions/WorldRegion Corridor")]
	public class WorldRegionCorridor : MonoBehaviour
	{
		public WorldRegionTrigger RegionA;

		public WorldRegionTrigger RegionB;

		public BoxCollider TransitionCollider;

		private void Awake()
		{
			TransitionCollider = (collider as BoxCollider);
			TransitionCollider.isTrigger = true;
		}
	}
}
