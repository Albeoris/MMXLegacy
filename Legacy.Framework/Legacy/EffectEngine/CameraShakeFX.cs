using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class CameraShakeFX : MonoBehaviour
	{
		private void Awake()
		{
			Vector3 impactDirection = new Vector3(0f, 0.2f, 0f);
			FXMainCamera.Instance.PlayShakeFX(0.25f, impactDirection);
		}
	}
}
