using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Noise/Noise and Scratches")]
[ExecuteInEditMode]
public class NoiseEffect : MonoBehaviour
{
	public Boolean monochrome = true;

	private Boolean rgbFallback;

	public Single grainIntensityMin = 0.1f;

	public Single grainIntensityMax = 0.2f;

	public Single grainSize = 2f;

	public Single scratchIntensityMin = 0.05f;

	public Single scratchIntensityMax = 0.25f;

	public Single scratchFPS = 10f;

	public Single scratchJitter = 0.01f;

	public Texture grainTexture;

	public Texture scratchTexture;

	public Shader shaderRGB;

	public Shader shaderYUV;

	private Material m_MaterialRGB;

	private Material m_MaterialYUV;

	private Single scratchTimeLeft;

	private Single scratchX;

	private Single scratchY;

	protected void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
		if (shaderRGB == null || shaderYUV == null)
		{
			Debug.Log("Noise shaders are not set up! Disabling noise effect.");
			enabled = false;
		}
		else if (!shaderRGB.isSupported)
		{
			enabled = false;
		}
		else if (!shaderYUV.isSupported)
		{
			rgbFallback = true;
		}
	}

	protected Material material
	{
		get
		{
			if (m_MaterialRGB == null)
			{
				m_MaterialRGB = new Material(shaderRGB);
				m_MaterialRGB.hideFlags = HideFlags.HideAndDontSave;
			}
			if (m_MaterialYUV == null && !rgbFallback)
			{
				m_MaterialYUV = new Material(shaderYUV);
				m_MaterialYUV.hideFlags = HideFlags.HideAndDontSave;
			}
			return (rgbFallback || monochrome) ? m_MaterialRGB : m_MaterialYUV;
		}
	}

	protected void OnDisable()
	{
		if (m_MaterialRGB)
		{
			DestroyImmediate(m_MaterialRGB);
		}
		if (m_MaterialYUV)
		{
			DestroyImmediate(m_MaterialYUV);
		}
	}

	private void SanitizeParameters()
	{
		grainIntensityMin = Mathf.Clamp(grainIntensityMin, 0f, 5f);
		grainIntensityMax = Mathf.Clamp(grainIntensityMax, 0f, 5f);
		scratchIntensityMin = Mathf.Clamp(scratchIntensityMin, 0f, 5f);
		scratchIntensityMax = Mathf.Clamp(scratchIntensityMax, 0f, 5f);
		scratchFPS = Mathf.Clamp(scratchFPS, 1f, 30f);
		scratchJitter = Mathf.Clamp(scratchJitter, 0f, 1f);
		grainSize = Mathf.Clamp(grainSize, 0.1f, 50f);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		SanitizeParameters();
		if (scratchTimeLeft <= 0f)
		{
			scratchTimeLeft = UnityEngine.Random.value * 2f / scratchFPS;
			scratchX = UnityEngine.Random.value;
			scratchY = UnityEngine.Random.value;
		}
		scratchTimeLeft -= Time.deltaTime;
		Material material = this.material;
		material.SetTexture("_GrainTex", grainTexture);
		material.SetTexture("_ScratchTex", scratchTexture);
		Single num = 1f / grainSize;
		material.SetVector("_GrainOffsetScale", new Vector4(UnityEngine.Random.value, UnityEngine.Random.value, Screen.width / (Single)grainTexture.width * num, Screen.height / (Single)grainTexture.height * num));
		material.SetVector("_ScratchOffsetScale", new Vector4(scratchX + UnityEngine.Random.value * scratchJitter, scratchY + UnityEngine.Random.value * scratchJitter, Screen.width / (Single)scratchTexture.width, Screen.height / (Single)scratchTexture.height));
		material.SetVector("_Intensity", new Vector4(UnityEngine.Random.Range(grainIntensityMin, grainIntensityMax), UnityEngine.Random.Range(scratchIntensityMin, scratchIntensityMax), 0f, 0f));
		Graphics.Blit(source, destination, material);
	}
}
