using System;
using UnityEngine;

public class ZoneEffectTargetCamera : MonoBehaviour
{
	private void Start()
	{
		if (ZoneEffectController.Instance != null)
		{
			ZoneEffectController.Instance.BloomController = GetComponent<Bloom>();
			ZoneEffectController.Instance.SunShaftsController = GetComponent<SunShafts>();
			ZoneEffectController.Instance.VignettingController = GetComponent<Vignetting>();
			GlobalFog[] components = GetComponents<GlobalFog>();
			ZoneEffectController.Instance.GlobalFogNearController = ((components.Length <= 0) ? null : components[0]);
			ZoneEffectController.Instance.GlobalFogFarController = ((components.Length <= 1) ? null : components[1]);
			ZoneEffectController.Instance.GlobalFogGroundController = ((components.Length <= 2) ? null : components[2]);
			ZoneEffectController.Instance.ColorCorrectionController = GetComponent<ColorCorrection>();
			ZoneEffectController.Instance.EdgeDetectionEffectController = GetComponent<EdgeDetectionEffect>();
			ZoneEffectController.Instance.ColorCorrectionEffectBlendController = GetComponent<ColorCorrectionEffectBlend>();
		}
	}
}
