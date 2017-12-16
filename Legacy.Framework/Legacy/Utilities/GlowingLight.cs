using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/GlowingLight")]
	[RequireComponent(typeof(Light))]
	public class GlowingLight : MonoBehaviour
	{
		public Single minIntensity = 0.25f;

		public Single maxIntensity = 0.5f;

		public Single speed = 1f;

		public Single timeOffset;

		private void Update()
		{
			Single t = 0.5f * (Mathf.Sin(Time.time * speed + timeOffset) + 1f);
			light.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
		}
	}
}
