using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	[Serializable]
	public class MoonParameters
	{
		public Color LightColor = Color.white;

		public Single LightIntensity = 0.25f;

		public Single ShadowStrength = 0.75f;

		public Single Halo = 0.5f;

		public Single Phase;
	}
}
