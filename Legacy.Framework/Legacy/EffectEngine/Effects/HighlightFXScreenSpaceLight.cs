using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class HighlightFXScreenSpaceLight : HighlightFX
	{
		private Material m_Material;

		private Single m_OriginalAlphaColor = 1f;

		private Single m_OriginalAlphaAmbient = 1f;

		public override void ResetToInvisible()
		{
			if (m_Material != null)
			{
				Color color = m_Material.color;
				color.a = 0f;
				m_Material.color = color;
				color = m_Material.GetColor("_Ambient");
				color.a = 0f;
				m_Material.SetColor("_Ambient", color);
			}
		}

		protected override void Start()
		{
			m_Material = Helper.Instantiate<Material>(renderer.sharedMaterial);
			Color color = m_Material.color;
			m_OriginalAlphaColor = color.a;
			color.a = 0f;
			m_Material.color = color;
			color = m_Material.GetColor("_Ambient");
			m_OriginalAlphaAmbient = color.a;
			color.a = 0f;
			m_Material.SetColor("_Ambient", color);
			renderer.material = m_Material;
			m_Material = renderer.material;
		}

		protected override void Update()
		{
			Boolean flag = true;
			if (m_AlphaSpeed != 0f)
			{
				Color color = m_Material.color;
				Color color2 = m_Material.GetColor("_Ambient");
				color.a += m_AlphaSpeed * Time.deltaTime;
				if (m_AlphaSpeed > 0f && color.a >= m_OriginalAlphaColor)
				{
					color.a = m_OriginalAlphaColor;
				}
				else if (m_AlphaSpeed < 0f && color.a <= 0f)
				{
					color.a = 0f;
				}
				else
				{
					flag = false;
				}
				color2.a = color.a / m_OriginalAlphaColor * m_OriginalAlphaAmbient;
				m_Material.color = color;
				m_Material.SetColor("_Ambient", color2);
			}
			if (flag)
			{
				m_AlphaSpeed = 0f;
			}
		}
	}
}
