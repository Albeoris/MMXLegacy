using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Displacement/Twirl")]
[ExecuteInEditMode]
public class TwirlEffect : ImageEffectBase
{
	public Vector2 radius = new Vector2(0.3f, 0.3f);

	public Single angle = 50f;

	public Vector2 center = new Vector2(0.5f, 0.5f);

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		ImageEffects.RenderDistortion(material, source, destination, angle, center, radius);
	}
}
