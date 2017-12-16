using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Blur/Blur")]
[ExecuteInEditMode]
public class BlurEffect : MonoBehaviour
{
	public Int32 iterations = 3;

	public Single blurSpread = 0.6f;

	public Shader blurShader;

	private static Material m_Material;

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
		Single num = 0.5f + iteration * blurSpread;
		Graphics.BlitMultiTap(source, dest, material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		Single num = 1f;
		Graphics.BlitMultiTap(source, dest, material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Int32 width = source.width / 4;
		Int32 height = source.height / 4;
		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0);
		DownSample4x(source, renderTexture);
		for (Int32 i = 0; i < iterations; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			FourTapCone(renderTexture, temporary, i);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		Graphics.Blit(renderTexture, destination);
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
