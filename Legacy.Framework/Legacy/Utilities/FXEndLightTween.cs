using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/FX EndLightTween")]
	[RequireComponent(typeof(Light))]
	public class FXEndLightTween : LightTween
	{
		protected override void Awake()
		{
			base.Awake();
			enabled = false;
		}

		private void OnEndEffect()
		{
			enabled = true;
			StartTween();
		}
	}
}
