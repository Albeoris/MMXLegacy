using System;

namespace Legacy.TimeOfDay
{
	[Serializable]
	public class CloudParameters
	{
		public Single Tone = 1.5f;

		public Single Shading = 0.5f;

		public Single Density = 1f;

		public Single Sharpness = 1f;

		public Single Scale1 = 3f;

		public Single Scale2 = 7f;

		public Boolean ShadowProjector = true;

		public Single ShadowStrength = 0.75f;
	}
}
