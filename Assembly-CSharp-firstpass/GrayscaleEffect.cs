using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Grayscale")]
public class GrayscaleEffect : ImageEffectBase
{
	public Texture textureRamp;

	public Single rampOffset;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetTexture("_RampTex", textureRamp);
		material.SetFloat("_RampOffset", rampOffset);
		Graphics.Blit(source, destination, material);
	}
}
