using System;
using Legacy.EffectEngine;
using UnityEngine;

public class OverlayLightEffect : MonoBehaviour
{
	private ScreenOverlay Overlay;

	public Texture2D tex;

	public Single lerpSpeed = 2f;

	private Single targetIntensity = 1.7f;

	private Single curIntensity;

	private Boolean reachedMax;

	private void Start()
	{
		Overlay = FXMainCamera.Instance.CurrentCamera.AddComponent<ScreenOverlay>();
		Overlay.overlayShader = Shader.Find("Hidden/BlendModesOverlay");
		Overlay.blendMode = ScreenOverlay.OverlayBlendMode.Additive;
		Overlay.intensity = 0f;
		Overlay.texture = tex;
	}

	private void Update()
	{
		if (!reachedMax)
		{
			curIntensity = Mathf.Lerp(curIntensity, targetIntensity, Time.deltaTime * lerpSpeed);
			Overlay.intensity = curIntensity;
			if (curIntensity > 1.5f)
			{
				reachedMax = true;
			}
		}
		if (reachedMax && curIntensity > 0.2f)
		{
			curIntensity = Mathf.Lerp(curIntensity, 0f, Time.deltaTime * lerpSpeed * 2f);
			Overlay.intensity = curIntensity;
			if (curIntensity <= 0.2f)
			{
				Destroy(Overlay);
			}
		}
	}
}
