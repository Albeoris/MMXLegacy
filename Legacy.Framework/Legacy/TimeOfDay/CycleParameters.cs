using System;

namespace Legacy.TimeOfDay
{
	[Serializable]
	public class CycleParameters
	{
		public Single TimeOfDay = 12f;

		public Single JulianDate = 60f;

		public Single Latitude;

		public Single Longitude;

		public Single UTC;
	}
}
