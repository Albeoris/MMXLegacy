using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class HighlightFX : MonoBehaviour
	{
		[SerializeField]
		private Int32 m_MaterialArraySize = 2;

		[SerializeField]
		private Single m_UVSpeed = 1f;

		[SerializeField]
		protected Single m_FadeSpeed = 1f;

		private Material[] m_Materials;

		private Vector2[] m_MaterialSpeeds;

		private Single m_OriginalAlpha = 1f;

		protected Single m_AlphaSpeed;

		public virtual void ResetToInvisible()
		{
			if (m_Materials != null && m_Materials.Length == m_MaterialArraySize)
			{
				for (Int32 i = 0; i < m_MaterialArraySize; i++)
				{
					Color color = m_Materials[i].color;
					color.a = 0f;
					m_Materials[i].color = color;
				}
			}
		}

		public void FadeIn()
		{
			m_AlphaSpeed = m_FadeSpeed;
		}

		public void FadeOut()
		{
			m_AlphaSpeed = -m_FadeSpeed;
		}

		protected virtual void Start()
		{
			m_Materials = new Material[m_MaterialArraySize];
			m_MaterialSpeeds = new Vector2[m_MaterialArraySize];
			for (Int32 i = 0; i < m_MaterialArraySize; i++)
			{
				Material material = Helper.Instantiate<Material>(renderer.sharedMaterial);
				Color color = material.color;
				m_OriginalAlpha = color.a;
				color.a = 0f;
				material.color = color;
				m_Materials[i] = material;
				m_MaterialSpeeds[i] = UnityEngine.Random.insideUnitCircle.normalized * m_UVSpeed;
			}
			renderer.materials = m_Materials;
			m_Materials = renderer.materials;
		}

		protected virtual void Update()
		{
			Boolean flag = true;
			for (Int32 i = 0; i < m_MaterialArraySize; i++)
			{
				Vector2 vector = m_MaterialSpeeds[i] * Time.deltaTime;
				vector.x += m_Materials[i].GetFloat("_OffsetX");
				vector.y += m_Materials[i].GetFloat("_OffsetY");
				m_Materials[i].SetFloat("_OffsetX", vector.x);
				m_Materials[i].SetFloat("_OffsetY", vector.y);
				if (m_AlphaSpeed != 0f)
				{
					Color color = m_Materials[i].color;
					color.a += m_AlphaSpeed * Time.deltaTime;
					if (m_AlphaSpeed > 0f && color.a >= m_OriginalAlpha)
					{
						color.a = m_OriginalAlpha;
					}
					else if (m_AlphaSpeed < 0f && color.a <= 0f)
					{
						color.a = 0f;
					}
					else
					{
						flag = false;
					}
					m_Materials[i].color = color;
				}
			}
			if (flag)
			{
				m_AlphaSpeed = 0f;
			}
		}
	}
}
