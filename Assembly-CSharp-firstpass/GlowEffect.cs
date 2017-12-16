using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Bloom and Glow/Glow (Deprecated)")]
[ExecuteInEditMode]
public class GlowEffect : MonoBehaviour
{
	public Single glowIntensity = 1.5f;

	public Int32 blurIterations = 3;

	public Single blurSpread = 0.7f;

	public Color glowTint = new Color(1f, 1f, 1f, 0f);

	public Shader compositeShader;

	private Material m_CompositeMaterial;

	public Shader blurShader;

	private Material m_BlurMaterial;

	public Shader downsampleShader;

	private Material m_DownsampleMaterial;

	protected Material compositeMaterial
	{
		get
		{
			if (m_CompositeMaterial == null)
			{
				m_CompositeMaterial = new Material(compositeShader);
				m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_CompositeMaterial;
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

	protected void OnDisable()
	{
		if (m_CompositeMaterial)
		{
			DestroyImmediate(m_CompositeMaterial);
		}
		if (m_BlurMaterial)
		{
			DestroyImmediate(m_BlurMaterial);
		}
		if (m_DownsampleMaterial)
		{
			DestroyImmediate(m_DownsampleMaterial);
		}
	}

	protected void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
		if (downsampleShader == null)
		{
			Debug.Log("No downsample shader assigned! Disabling glow.");
			enabled = false;
		}
		else
		{
			if (!blurMaterial.shader.isSupported)
			{
				enabled = false;
			}
			if (!compositeMaterial.shader.isSupported)
			{
				enabled = false;
			}
			if (!downsampleMaterial.shader.isSupported)
			{
				enabled = false;
			}
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, Int32 iteration)
	{
		Single num = 0.5f + iteration * blurSpread;
		Graphics.BlitMultiTap(source, dest, blurMaterial, new Vector2[]
		{
			new Vector2(num, num),
			new Vector2(-num, num),
			new Vector2(num, -num),
			new Vector2(-num, -num)
		});
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		downsampleMaterial.color = new Color(glowTint.r, glowTint.g, glowTint.b, glowTint.a / 4f);
		Graphics.Blit(source, dest, downsampleMaterial);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		glowIntensity = Mathf.Clamp(glowIntensity, 0f, 10f);
		blurIterations = Mathf.Clamp(blurIterations, 0, 30);
		blurSpread = Mathf.Clamp(blurSpread, 0.5f, 1f);
		Int32 width = source.width / 4;
		Int32 height = source.height / 4;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);
		DownSample4x(source, renderTexture);
		Single num = Mathf.Clamp01((glowIntensity - 1f) / 4f);
		blurMaterial.color = new Color(1f, 1f, 1f, 0.25f + num);
		for (Int32 i = 0; i < blurIterations; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			FourTapCone(renderTexture, temporary, i);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Graphics.Blit(source, destination);
		BlitGlow(renderTexture, destination);
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	public void BlitGlow(RenderTexture source, RenderTexture dest)
	{
		compositeMaterial.color = new Color(1f, 1f, 1f, Mathf.Clamp01(glowIntensity));
		Graphics.Blit(source, dest, compositeMaterial);
	}
}
