using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	[Serializable]
	public class SunParameters
	{
		public Color LightColor = Color.white;

		public Single LightIntensity = 0.75f;

		public Single ShadowStrength = 0.75f;

		public Single Falloff = 1.25f;

		public Single Coloring = 0.25f;
	}
}
