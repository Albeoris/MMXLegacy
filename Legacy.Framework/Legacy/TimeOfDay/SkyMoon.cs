using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public class SkyMoon : SkySatellite
	{
		private Color m_MoonHaloColor;

		private Single m_MoonPhase;

		private Material m_MoonMaterial;

		private Material m_MoonHaloMaterial;

		public Color MoonHaloColor
		{
			get => m_MoonHaloColor;
		    set
			{
				if (m_MoonHaloMaterial != null)
				{
					m_MoonHaloColor = value;
					m_MoonHaloMaterial.SetColor("_Color", value);
				}
			}
		}

		public Single MoonPhase
		{
			get => m_MoonPhase;
		    set
			{
				if (m_MoonMaterial != null)
				{
					m_MoonPhase = value;
					m_MoonMaterial.SetFloat("_Phase", value);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (m_RendererSource != null)
			{
				Material[] sharedMaterials = m_RendererSource.sharedMaterials;
				if (sharedMaterials.Length > 0 && sharedMaterials[0] != null)
				{
					m_MoonMaterial = sharedMaterials[0];
					m_MoonPhase = m_MoonMaterial.GetFloat("_Phase");
				}
				if (sharedMaterials.Length > 1 && sharedMaterials[1] != null)
				{
					m_MoonHaloMaterial = sharedMaterials[1];
					m_MoonHaloColor = m_MoonHaloMaterial.GetColor("_Color");
				}
			}
		}
	}
}
