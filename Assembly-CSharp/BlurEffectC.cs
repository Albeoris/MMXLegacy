using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Blur")]
public class BlurEffectC : MonoBehaviour
{
	public Int32 iterations = 3;

	public static Single i;

	public Boolean fadeIn;

	public Single fadeInSpeed = 1f;

	public Single maxBlur = 3f;

	public Single blurSpread;

	public Shader blurShader;

	public Single DownSample = 1f;

	private static Material m_Material;

	private Vector2[] m_Cache = new Vector2[4];

	protected Material material
	{
		get
		{
			if (m_Material == null)
			{
				m_Material = new Material(blurShader);
				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		}
	}

	protected void OnDisable()
	{
		if (m_Material)
		{
			DestroyImmediate(m_Material);
		}
	}

	private void FixedUpdate()
	{
		if (fadeIn)
		{
			blurSpread = i;
			i += fadeInSpeed * Time.smoothDeltaTime;
		}
		if (i >= maxBlur)
		{
			i = maxBlur;
		}
	}

	protected void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
		if (!blurShader || !material.shader.isSupported)
		{
			enabled = false;
			return;
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, Int32 iteration)
	{
		Single num = Mathf.Clamp01(DownSample);
		Single off = (0.5f + iteration * blurSpread) * (1f - num);
		Graphics.BlitMultiTap(source, dest, material, PixelOffset(off));
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		Single num = Mathf.Clamp01(DownSample);
		Single off = 1f * (1f - num);
		Graphics.BlitMultiTap(source, dest, material, PixelOffset(off));
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Single num = Mathf.Clamp(DownSample, 0.125f, 1f);
		Int32 width = (Int32)(source.width * num);
		Int32 height = (Int32)(source.height * num);
		RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0);
		try
		{
			DownSample4x(source, temporary);
			Boolean flag = true;
			for (Int32 i = 0; i < iterations; i++)
			{
				if (flag)
				{
					FourTapCone(temporary, temporary2, i);
				}
				else
				{
					FourTapCone(temporary2, temporary, i);
				}
				flag = !flag;
			}
			if (flag)
			{
				Graphics.Blit(temporary, destination);
			}
			else
			{
				Graphics.Blit(temporary2, destination);
			}
		}
		finally
		{
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(temporary2);
		}
	}

	private Vector2[] PixelOffset(Single off)
	{
		m_Cache[0] = new Vector2(-off, -off);
		m_Cache[1] = new Vector2(-off, off);
		m_Cache[2] = new Vector2(off, off);
		m_Cache[3] = new Vector2(off, -off);
		return m_Cache;
	}
}
