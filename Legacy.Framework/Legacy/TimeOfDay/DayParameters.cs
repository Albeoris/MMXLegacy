using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	[Serializable]
	public class DayParameters
	{
		public Color AdditiveColor = Color.black;

		public Single RayleighMultiplier = 1f;

		public Single MieMultiplier = 0.1f;

		public Single Brightness = 10f;

		public Single Directionality = 1.5f;

		public Single Haziness = 0.5f;
	}
}
