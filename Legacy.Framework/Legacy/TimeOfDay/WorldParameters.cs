using System;

namespace Legacy.TimeOfDay
{
	[Serializable]
	public class WorldParameters
	{
		public Boolean SetFogColor;

		public Boolean SetAmbientLight;

		public Single AmbientIntensity = 0.8f;
	}
}
