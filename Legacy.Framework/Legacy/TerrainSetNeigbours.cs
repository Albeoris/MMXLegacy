using System;
using UnityEngine;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Map/Terrain Set Neigbours")]
	public class TerrainSetNeigbours : MonoBehaviour
	{
		public Terrain Left;

		public Terrain Top;

		public Terrain Right;

		public Terrain Bottom;

		private void Start()
		{
			Terrain component = GetComponent<Terrain>();
			if (component != null)
			{
				component.SetNeighbors(Left, Top, Right, Bottom);
			}
			enabled = false;
		}
	}
}
