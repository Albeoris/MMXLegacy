using System;
using Legacy;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GlowFXBase : MonoBehaviour
{
	private static Vector2[] m_Cache = new Vector2[4];

	public Shader downsampleShader;

	private Material m_DownsampleMaterial;

	public Shader blurShader;

	private Material m_BlurMaterial;

	protected Material downsampleMaterial
	{
		get
		{
			if (m_DownsampleMaterial == null)
			{
				m_DownsampleMaterial = new Material(downsampleShader);
				m_DownsampleMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_DownsampleMaterial;
		}
	}

	protected Material blurMaterial
	{
		get
		{
			if (m_BlurMaterial == null)
			{
				m_BlurMaterial = new Material(blurShader);
				m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_BlurMaterial;
		}
	}

	protected virtual void OnDestroy()
	{
		Helper.DestroyImmediate<Material>(ref m_DownsampleMaterial);
		Helper.DestroyImmediate<Material>(ref m_BlurMaterial);
	}

	protected void DownSample4x(RenderTexture source, RenderTexture dest, Color tint)
	{
		downsampleMaterial.color = new Color(tint.r, tint.g, tint.b, tint.a * 0.25f);
		Graphics.Blit(source, dest, downsampleMaterial);
	}

	protected void FourTapCone(RenderTexture source, RenderTexture dest, Int32 iteration, Single blurSpread)
	{
		FourTapCone(source, dest, iteration, blurSpread, blurMaterial);
	}

	protected static void FourTapCone(RenderTexture source, RenderTexture dest, Int32 iteration, Single blurSpread, Material material)
	{
		Single num = 0.5f + iteration * blurSpread;
		m_Cache[0] = new Vector2(num, num);
		m_Cache[1] = new Vector2(-num, num);
		m_Cache[2] = new Vector2(num, -num);
		m_Cache[3] = new Vector2(-num, -num);
		Graphics.BlitMultiTap(source, dest, material, m_Cache);
	}
}
