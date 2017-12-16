using System;
using Legacy.EffectEngine;
using UnityEngine;

public class ChangeTonemappingForEffect : MonoBehaviour
{
	private Tonemapping Tonecontrol;

	private Single oldMidGrey;

	private Single oldWhite;

	private void Start()
	{
		Tonecontrol = FXMainCamera.Instance.CurrentCamera.GetComponent<Tonemapping>();
		oldMidGrey = Tonecontrol.middleGrey;
		oldWhite = Tonecontrol.white;
		Tonecontrol.middleGrey = 0.4f;
		Tonecontrol.white = 1.1f;
	}

	private void OnDestroy()
	{
		Tonecontrol.middleGrey = oldMidGrey;
		Tonecontrol.white = oldWhite;
	}
}
