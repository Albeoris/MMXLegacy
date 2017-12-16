using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class TrapHighlightGenericObjectFX : MonoBehaviour
	{
		private void Start()
		{
			if (transform.parent != null)
			{
				CombineAllMeshes();
			}
			else
			{
				Debug.LogError("TrapHighlightGenericObjectFX: Start: needs a parent!");
			}
		}

		private void CombineAllMeshes()
		{
			Transform transform = this.transform.parent.Find("HighlightFXGeometry");
			if (transform == null)
			{
				transform = this.transform.parent;
			}
			List<CombineInstance> list = new List<CombineInstance>();
			MeshFilter[] componentsInChildren = transform.gameObject.GetComponentsInChildren<MeshFilter>();
			Matrix4x4 worldToLocalMatrix = this.transform.worldToLocalMatrix;
			for (Int32 i = 0; i < componentsInChildren.Length; i++)
			{
				Mesh sharedMesh = componentsInChildren[i].sharedMesh;
				if (sharedMesh != null)
				{
					if (!sharedMesh.isReadable)
					{
						Debug.LogWarning("CombineMeshes\nMesh not readable! " + this, this);
					}
					else
					{
						for (Int32 j = 0; j < sharedMesh.subMeshCount; j++)
						{
							list.Add(new CombineInstance
							{
								mesh = sharedMesh,
								subMeshIndex = j,
								transform = worldToLocalMatrix * componentsInChildren[i].transform.localToWorldMatrix
							});
						}
					}
				}
			}
			Mesh mesh = new Mesh();
			mesh.CombineMeshes(list.ToArray());
			GetComponent<MeshFilter>().sharedMesh = mesh;
		}
	}
}
