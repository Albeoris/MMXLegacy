using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Ramp) Blend")]
public class ColorCorrectionEffectBlend : ImageEffectBase
{
	public Texture textureRamp;

	public Single intensity;

	public Single BlendTime = 1f;

	private Boolean m_activate = true;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetTexture("_RampTex", textureRamp);
		material.SetFloat("_Intensity", intensity);
		Graphics.Blit(source, destination, material);
	}

	private void Update()
	{
		intensity += ((!m_activate) ? -1 : 1) * Time.deltaTime / BlendTime;
		intensity = Mathf.Clamp01(intensity);
		if (intensity == 0f && !m_activate)
		{
			enabled = false;
		}
	}

	public void Activate()
	{
		m_activate = true;
		enabled = true;
		intensity = 0f;
	}

	public void Deactivate()
	{
		m_activate = false;
	}
}
