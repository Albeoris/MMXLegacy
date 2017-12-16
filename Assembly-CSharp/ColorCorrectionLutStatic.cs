using System;
using Legacy;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Adjustments/Color Cor. LUT Legacy static")]
[ExecuteInEditMode]
public class ColorCorrectionLutStatic : MonoBehaviour
{
	private Material m_Material;

	[SerializeField]
	private Texture3D m_LutTarget;

	[SerializeField]
	private Shader m_Shader;

	private void OnEnable()
	{
		if (!SystemInfo.supports3DTextures || m_LutTarget == null)
		{
			enabled = false;
			return;
		}
		CheckResources();
		UpdateMaterial();
	}

	private void OnDestroy()
	{
		Helper.DestroyImmediate<Material>(ref m_Material);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!SystemInfo.supports3DTextures || m_LutTarget == null)
		{
			Graphics.Blit(source, destination);
			enabled = false;
			return;
		}
		CheckResources();
		if (m_Material == null)
		{
			Graphics.Blit(source, destination);
			enabled = false;
			return;
		}
		Int32 pass = (QualitySettings.activeColorSpace != ColorSpace.Linear) ? 0 : 1;
		Graphics.Blit(source, destination, m_Material, pass);
	}

	private void CheckResources()
	{
		if (m_Material == null && m_Shader != null)
		{
			m_Material = new Material(m_Shader);
			m_Material.name = "CCC_LUT_MATERIAL";
			m_Material.hideFlags = HideFlags.DontSave;
		}
	}

	private void UpdateMaterial()
	{
		if (m_Material != null && m_LutTarget != null)
		{
			Int32 width = m_LutTarget.width;
			m_LutTarget.wrapMode = TextureWrapMode.Clamp;
			m_Material.SetFloat("_Scale", (width - 1) / (1f * width));
			m_Material.SetFloat("_Offset", 1f / (2f * width));
			m_Material.SetTexture("_ClutTex", m_LutTarget);
		}
	}
}
