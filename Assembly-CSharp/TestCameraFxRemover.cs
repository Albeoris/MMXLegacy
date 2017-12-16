using System;
using Legacy.EffectEngine;
using Legacy.Utilities;
using UnityEngine;

public class TestCameraFxRemover : MonoBehaviour
{
	private void Start()
	{
		Destroy(GetComponent<WalkCameraFX>());
		Destroy(GetComponent<ShakeCameraFX>());
		Destroy(GetComponent<ReboundCameraFX>());
		Destroy(GetComponent<InteractiveObjectCamera>());
		Destroy(GetComponent<FreeRotationCamera>());
		Destroy(GetComponent<CameraObliqueFrustum>());
	}
}
