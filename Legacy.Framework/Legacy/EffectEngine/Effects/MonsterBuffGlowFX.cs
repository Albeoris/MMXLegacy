using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class MonsterBuffGlowFX : MonoBehaviour
	{
		private const Single WIDTH_INCREASE = 0.12f;

		private const Single TIME_RETURN_WIDTH_TO_NORMAL = 0.25f;

		private const String MESH_NAME_SUFFIX = "_outlined";

		private Renderer[] m_renderers;

		private Boolean m_isOutlineShown;

		public String Materialpath = "FXMaterials/Surendering";

		public Renderer[] GetRenderers()
		{
			return m_renderers;
		}

		public void ShowOutline()
		{
			if (!m_isOutlineShown)
			{
				CollectRenderers();
				foreach (Renderer renderer in m_renderers)
				{
					if (renderer != null)
					{
						Mesh sharedMesh;
						if (renderer is MeshRenderer)
						{
							sharedMesh = renderer.GetComponent<MeshFilter>().sharedMesh;
						}
						else
						{
							sharedMesh = ((SkinnedMeshRenderer)renderer).sharedMesh;
						}
						renderer.bounds.Expand(3f);
						if (sharedMesh.subMeshCount > 1)
						{
							AddSubMesh(sharedMesh);
						}
						AddOutlineMaterial(renderer);
					}
				}
				m_isOutlineShown = true;
			}
		}

		public void HideOutline()
		{
			if (m_isOutlineShown)
			{
				foreach (Renderer renderer in m_renderers)
				{
					if (renderer != null && IsOutlineMaterialAdded(renderer))
					{
						RemoveOutlineMaterial(renderer);
					}
				}
				m_isOutlineShown = false;
			}
		}

		private void CollectRenderers()
		{
			if (m_renderers == null)
			{
				List<Renderer> list = new List<Renderer>(GetComponentsInChildren<Renderer>());
				for (Int32 i = list.Count - 1; i >= 0; i--)
				{
					Renderer renderer = list[i];
					if (!(renderer is MeshRenderer) && !(renderer is SkinnedMeshRenderer))
					{
						list.RemoveAt(i);
					}
				}
				m_renderers = list.ToArray();
			}
		}

		private void AddOutlineMaterial(Renderer pSmr)
		{
			Material[] sharedMaterials = pSmr.sharedMaterials;
			Material[] array = new Material[sharedMaterials.Length + 1];
			Array.Copy(sharedMaterials, array, sharedMaterials.Length);
			Material material = Helper.Instantiate<Material>(Helper.ResourcesLoad<Material>(Materialpath));
			array[array.Length - 1] = material;
			pSmr.sharedMaterials = array;
		}

		private void RemoveOutlineMaterial(Renderer pSmr)
		{
			Material[] sharedMaterials = pSmr.sharedMaterials;
			Material[] array = new Material[sharedMaterials.Length - 1];
			Array.Copy(sharedMaterials, array, array.Length);
			pSmr.sharedMaterials = array;
			Destroy(sharedMaterials[sharedMaterials.Length - 1]);
		}

		private Boolean IsOutlineMaterialAdded(Renderer pSmr)
		{
			return pSmr.sharedMaterials[pSmr.sharedMaterials.Length - 1].shader.name == "ShurikenMagic/TransparentRim";
		}

		private void AddSubMesh(Mesh p_Mesh)
		{
			if (!p_Mesh.name.EndsWith("_outlined"))
			{
				p_Mesh.name += "_outlined";
				Int32 num = p_Mesh.subMeshCount + 1;
				p_Mesh.subMeshCount = num;
				p_Mesh.SetTriangles(p_Mesh.triangles, num - 1);
			}
		}
	}
}
