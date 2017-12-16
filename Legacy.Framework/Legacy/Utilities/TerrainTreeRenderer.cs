using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Terrain Tree Only Renderer")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(Terrain))]
	public class TerrainTreeRenderer : MonoBehaviour
	{
		public TerrainRenderFlags renderFlag = TerrainRenderFlags.all;

		private Terrain mTerrain;

		private void Start()
		{
			mTerrain = gameObject.GetComponent<Terrain>();
			mTerrain.editorRenderFlags = renderFlag;
		}
	}
}
