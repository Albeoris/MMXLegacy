using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	[Serializable]
	public class NightParameters
	{
		public Color AdditiveColor = Color.black;

		public Color HazeColor = Color.black;

		public Single Haziness = 0.5f;
	}
}
