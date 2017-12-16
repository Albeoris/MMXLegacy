using System;
using UnityEngine;

namespace Legacy.EffectEngine.PostEffects
{
	[ExecuteInEditMode]
	public class BrightnessGammaTune : MonoBehaviour
	{
		private const String SHADER_NAME = "Hidden/BrightnessGammaTune";

		private const String DEF_BRIGHTNESS = "BRIGHTNESS";

		private const String DEF_GAMMA = "GAMMA";

		private static String[] tmp = new String[2];

		private Material m_CurrentMaterial;

		[SerializeField]
		private Single m_BrightnessAmount = 1f;

		[SerializeField]
		private Single m_GammaAmount = 1f;

		public Single BrightnessAmount
		{
			get => m_BrightnessAmount;
		    set
			{
				if (m_BrightnessAmount != value)
				{
					m_BrightnessAmount = value;
					UpdateMaterial();
				}
			}
		}

		public Single GammaAmount
		{
			get => m_GammaAmount;
		    set
			{
				if (m_GammaAmount != value)
				{
					m_GammaAmount = value;
					UpdateMaterial();
				}
			}
		}

		private Material CurrentMaterial
		{
			get
			{
				if (m_CurrentMaterial == null)
				{
					m_CurrentMaterial = new Material(Helper.FindShader("Hidden/BrightnessGammaTune"));
					m_CurrentMaterial.hideFlags = HideFlags.DontSave;
					UpdateMaterial();
				}
				return m_CurrentMaterial;
			}
		}

		private void Start()
		{
			if (!SystemInfo.supportsImageEffects)
			{
				enabled = false;
				return;
			}
		}

		private void OnDestroy()
		{
			Helper.DestroyImmediate<Material>(ref m_CurrentMaterial);
		}

		private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
		{
			Material currentMaterial = CurrentMaterial;
			if (currentMaterial != null)
			{
				Graphics.Blit(sourceTexture, destTexture, currentMaterial);
			}
			else
			{
				Graphics.Blit(sourceTexture, destTexture);
			}
		}

		private void UpdateMaterial()
		{
			if (m_CurrentMaterial == null)
			{
				return;
			}
			Material currentMaterial = m_CurrentMaterial;
			Int32 i = 0;
			m_GammaAmount = Mathf.Clamp(m_GammaAmount, 0f, 10f);
			if (m_GammaAmount != 1f)
			{
				tmp[i++] = "GAMMA";
				currentMaterial.SetFloat("_GammaValue", 1f / m_GammaAmount);
			}
			m_BrightnessAmount = Mathf.Clamp(m_BrightnessAmount, 0f, 2f);
			if (m_BrightnessAmount != 1f)
			{
				tmp[i++] = "BRIGHTNESS";
				currentMaterial.SetFloat("_BrightnessValue", m_BrightnessAmount - 1f);
			}
			if (i == -1)
			{
				currentMaterial.shaderKeywords = null;
			}
			else
			{
				while (i < 2)
				{
					tmp[i] = null;
					i++;
				}
				currentMaterial.shaderKeywords = tmp;
			}
			enabled = (m_BrightnessAmount != 1f || m_GammaAmount != 1f);
		}
	}
}
