using System;
using UnityEngine;

namespace Legacy.Utilites
{
	[AddComponentMenu("MM Legacy/World/Mesh Combine")]
	public class CombineMeshes : MonoBehaviour
	{
		public Boolean combineSubMeshes = true;

		public Boolean useObjectTransforms = true;

		public Int32 vertexLimit = 65000;
	}
}
