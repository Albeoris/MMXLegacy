using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class AdvancedUVAnimatorAndScroller : MonoBehaviour
	{
		[SerializeField]
		private Int32 m_materialIndex;

		[SerializeField]
		private String[] m_textureNames;

		public Vector2 m_uvSpeed;

		[SerializeField]
		private Vector2 m_uvSpeedRandomity;

		private Material m_material;

		private Vector2 m_uv;

		public void Start()
		{
			if (renderer == null || renderer.sharedMaterials.Length - 1 < m_materialIndex || renderer.sharedMaterials.Length <= 0)
			{
				Debug.LogError("AdvancedUVAnimatorAndScroller: Start: no material found will destroy! " + name);
				Destroy(this);
				return;
			}
			if (m_textureNames == null || m_textureNames.Length <= 0)
			{
				Debug.LogError("AdvancedUVAnimatorAndScroller: Start: no texture names are set will destroy! " + name);
				Destroy(this);
				return;
			}
			Material[] materials = renderer.materials;
			m_material = materials[m_materialIndex];
			renderer.materials = materials;
			m_uv = m_material.GetTextureOffset(m_textureNames[0]);
		}

		public void Update()
		{
			Vector2 uvSpeedRandomity = m_uvSpeedRandomity;
			uvSpeedRandomity.Scale(UnityEngine.Random.insideUnitCircle);
			m_uv += (m_uvSpeed + uvSpeedRandomity) * Time.deltaTime;
			foreach (String propertyName in m_textureNames)
			{
				m_material.SetTextureOffset(propertyName, m_uv);
			}
		}
	}
}
