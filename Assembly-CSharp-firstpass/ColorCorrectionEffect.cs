using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Ramp)")]
[ExecuteInEditMode]
public class ColorCorrectionEffect : ImageEffectBase
{
	public Texture textureRamp;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetTexture("_RampTex", textureRamp);
		Graphics.Blit(source, destination, material);
	}
}
