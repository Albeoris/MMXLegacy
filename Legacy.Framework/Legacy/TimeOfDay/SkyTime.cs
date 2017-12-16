using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public class SkyTime : MonoBehaviour
	{
		public Single DayLengthInMinutes = 30f;

		public Boolean ProgressTime = true;

		public Boolean ProgressDate = true;

		public Boolean ProgressMoonPhase = true;

		private Sky sky;

		protected void Start()
		{
			sky = GetComponent<Sky>();
		}

		protected void Update()
		{
			Single num = DayLengthInMinutes * 60f;
			Single num2 = num / 24f;
			Single num3 = Time.deltaTime / num2;
			Single num4 = Time.deltaTime / (30f * num) * 2f;
			if (ProgressTime)
			{
				sky.Cycle.TimeOfDay += num3;
				if (ProgressMoonPhase)
				{
					sky.Moon.Phase += num4;
					if (sky.Moon.Phase < -1f)
					{
						sky.Moon.Phase += 2f;
					}
					else if (sky.Moon.Phase > 1f)
					{
						sky.Moon.Phase -= 2f;
					}
				}
				if (sky.Cycle.TimeOfDay >= 24f)
				{
					sky.Cycle.TimeOfDay = 0f;
					if (ProgressDate)
					{
						sky.Cycle.JulianDate = ((sky.Cycle.JulianDate >= 365f) ? 1f : (sky.Cycle.JulianDate + 1f));
					}
				}
			}
		}
	}
}
