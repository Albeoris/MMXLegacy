using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class SunderingFX : MonoBehaviour
	{
		private const Single WIDTH_INCREASE = 0.12f;

		private const Single TIME_RETURN_WIDTH_TO_NORMAL = 0.25f;

		private const String MESH_NAME_SUFFIX = "_outlined";

		private SkinnedMeshRenderer[] m_renderers;

		private Boolean m_isOutlineShown;

		private Material m_material;

		private Single m_outlineWidthInitialValue;

		private Single m_transitionTime;

		private void Update()
		{
			ShowOutline();
			if (m_material != null)
			{
				Single num = 0f;
				Single num2 = 1f - (m_transitionTime - Time.time) / 0.25f;
				if (num2 >= 0f)
				{
					if (num2 <= 0.5f)
					{
						num = num2 * 0.12f;
					}
					else
					{
						num = (1f - num2) * 0.12f;
					}
				}
				if (num < 0f)
				{
					num = 0f;
				}
				m_material.SetFloat("_Outline", m_outlineWidthInitialValue + num);
			}
		}

		public void ShowOutline()
		{
			if (!m_isOutlineShown)
			{
				CollectRenderers();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in m_renderers)
				{
					if (skinnedMeshRenderer != null)
					{
						skinnedMeshRenderer.renderer.bounds.Expand(3f);
						if (skinnedMeshRenderer.sharedMesh.subMeshCount > 1)
						{
							AddSubMesh(skinnedMeshRenderer);
						}
						AddOutlineMaterial(skinnedMeshRenderer);
					}
				}
				m_isOutlineShown = true;
			}
		}

		public void HideOutline()
		{
			if (m_isOutlineShown)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in m_renderers)
				{
					if (skinnedMeshRenderer != null && IsOutlineMaterialAdded(skinnedMeshRenderer))
					{
						RemoveOutlineMaterial(skinnedMeshRenderer);
					}
				}
				m_isOutlineShown = false;
			}
		}

		private void CollectRenderers()
		{
			if (m_renderers == null)
			{
				m_renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			}
		}

		private void AddOutlineMaterial(SkinnedMeshRenderer pSmr)
		{
			Material[] sharedMaterials = pSmr.sharedMaterials;
			Material[] array = new Material[sharedMaterials.Length + 1];
			Array.Copy(sharedMaterials, array, sharedMaterials.Length);
			m_material = Helper.Instantiate<Material>(Helper.ResourcesLoad<Material>("FXMaterials/Surendering"));
			array[array.Length - 1] = m_material;
			m_outlineWidthInitialValue = m_material.GetFloat("_Outline");
			pSmr.sharedMaterials = array;
			m_transitionTime = Time.time + 0.25f;
		}

		private void RemoveOutlineMaterial(SkinnedMeshRenderer pSmr)
		{
			Material[] sharedMaterials = pSmr.sharedMaterials;
			Material[] array = new Material[sharedMaterials.Length - 1];
			Array.Copy(sharedMaterials, array, array.Length);
			pSmr.sharedMaterials = array;
			m_material = null;
		}

		private Boolean IsOutlineMaterialAdded(SkinnedMeshRenderer pSmr)
		{
			return pSmr.sharedMaterials[pSmr.sharedMaterials.Length - 1].shader.name == "Outline";
		}

		private void AddSubMesh(SkinnedMeshRenderer pSmr)
		{
			Mesh sharedMesh = pSmr.sharedMesh;
			if (!sharedMesh.name.EndsWith("_outlined"))
			{
				sharedMesh.name += "_outlined";
				Int32 num = sharedMesh.subMeshCount + 1;
				sharedMesh.subMeshCount = num;
				pSmr.sharedMesh.SetTriangles(sharedMesh.triangles, num - 1);
			}
		}
	}
}
