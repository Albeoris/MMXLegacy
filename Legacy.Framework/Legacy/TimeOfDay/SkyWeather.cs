using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public class SkyWeather : MonoBehaviour
	{
		public Single FadeTime = 10f;

		public CloudType Clouds;

		public WeatherType Weather;

		private Single dayRayleighDefault;

		private Single dayMieDefault;

		private Single dayBrightnessDefault;

		private Single dayHazinessDefault;

		private Single cloudToneDefault;

		private Single cloudDensityDefault;

		private Single dayRayleigh;

		private Single dayMie;

		private Single dayBrightness;

		private Single dayHaziness;

		private Single cloudTone;

		private Single cloudDensity;

		private Single cloudSharpness;

		private Sky sky;

		protected void Start()
		{
			sky = GetComponent<Sky>();
			dayRayleigh = (dayRayleighDefault = sky.Day.RayleighMultiplier);
			dayMie = (dayMieDefault = sky.Day.MieMultiplier);
			dayBrightness = (dayBrightnessDefault = sky.Day.Brightness);
			dayHaziness = (dayHazinessDefault = sky.Day.Haziness);
			cloudTone = (cloudToneDefault = sky.Clouds.Tone);
			cloudDensity = (cloudDensityDefault = sky.Clouds.Density);
			cloudSharpness = sky.Clouds.Sharpness;
		}

		protected void Update()
		{
			switch (Clouds)
			{
			case CloudType.Custom:
				cloudDensity = sky.Clouds.Density;
				cloudSharpness = sky.Clouds.Sharpness;
				break;
			case CloudType.None:
				cloudDensity = 0f;
				cloudSharpness = 1f;
				break;
			case CloudType.Few:
				cloudDensity = cloudDensityDefault;
				cloudSharpness = 6f;
				break;
			case CloudType.Scattered:
				cloudDensity = cloudDensityDefault;
				cloudSharpness = 3f;
				break;
			case CloudType.Broken:
				cloudDensity = cloudDensityDefault;
				cloudSharpness = 1f;
				break;
			case CloudType.Overcast:
				cloudDensity = cloudDensityDefault;
				cloudSharpness = 0.2f;
				break;
			}
			switch (Weather)
			{
			case WeatherType.Custom:
				dayRayleigh = sky.Day.RayleighMultiplier;
				dayMie = sky.Day.MieMultiplier;
				dayBrightness = sky.Day.Brightness;
				dayHaziness = sky.Day.Haziness;
				cloudTone = sky.Clouds.Tone;
				break;
			case WeatherType.Clear:
				dayRayleigh = dayRayleighDefault;
				dayMie = dayMieDefault;
				dayBrightness = dayBrightnessDefault;
				dayHaziness = dayHazinessDefault;
				cloudTone = cloudToneDefault;
				break;
			case WeatherType.Storm:
				dayRayleigh = 0.1f;
				dayMie = 0.3f;
				dayBrightness = dayBrightnessDefault;
				dayHaziness = 0f;
				cloudTone = 0.5f;
				break;
			case WeatherType.Dust:
				dayRayleigh = dayRayleighDefault;
				dayMie = 0.05f;
				dayBrightness = dayBrightnessDefault;
				dayHaziness = 1f;
				cloudTone = cloudToneDefault;
				break;
			case WeatherType.Fog:
				dayRayleigh = 10f;
				dayMie = 0f;
				dayBrightness = dayBrightnessDefault;
				dayHaziness = 1f;
				cloudTone = cloudToneDefault;
				break;
			}
			Single t = Time.deltaTime / FadeTime;
			sky.Day.RayleighMultiplier = Mathf.Lerp(sky.Day.RayleighMultiplier, dayRayleigh, t);
			sky.Day.MieMultiplier = Mathf.Lerp(sky.Day.MieMultiplier, dayMie, t);
			sky.Day.Brightness = Mathf.Lerp(sky.Day.Brightness, dayBrightness, t);
			sky.Day.Haziness = Mathf.Lerp(sky.Day.Haziness, dayHaziness, t);
			sky.Clouds.Tone = Mathf.Lerp(sky.Clouds.Tone, cloudTone, t);
			sky.Clouds.Density = Mathf.Lerp(sky.Clouds.Density, cloudDensity, t);
			sky.Clouds.Sharpness = Mathf.Lerp(sky.Clouds.Sharpness, cloudSharpness, t);
		}

		public enum CloudType
		{
			Custom,
			None,
			Few,
			Scattered,
			Broken,
			Overcast
		}

		public enum WeatherType
		{
			Custom,
			Clear,
			Storm,
			Dust,
			Fog
		}
	}
}
