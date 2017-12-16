using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/DoubleView")]
public class DoubleViewEffect : MonoBehaviour
{
	private Vector2[] m_Cache = new Vector2[2];

	public Shader invertShader;

	public Single pixelmovement = 20f;

	public Single colorInvert;

	private static Material m_Material;

	protected Material material
	{
		get
		{
			if (m_Material == null)
			{
				m_Material = new Material(invertShader);
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
		if (!invertShader || !material.shader.isSupported)
		{
			enabled = false;
			return;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_ColorInvertFac", Mathf.Clamp01(colorInvert));
		Graphics.BlitMultiTap(source, destination, material, PixelOffset(pixelmovement));
	}

	private Vector2[] PixelOffset(Single off)
	{
		m_Cache[0] = new Vector2(off, 0f);
		m_Cache[1] = new Vector2(-off, 0f);
		return m_Cache;
	}
}
