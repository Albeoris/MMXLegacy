using System;
using Legacy.EffectEngine;
using UnityEngine;

public class DeactivateLutForEffect : MonoBehaviour
{
	private ColorCorrectionLut LutControl;

	private void Start()
	{
		LutControl = FXMainCamera.Instance.CurrentCamera.GetComponent<ColorCorrectionLut>();
		LutControl.enabled = false;
	}

	private void OnDestroy()
	{
		LutControl.enabled = true;
	}
}
