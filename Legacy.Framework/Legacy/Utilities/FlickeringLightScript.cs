using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Flickering Light")]
	[RequireComponent(typeof(Light))]
	public class FlickeringLightScript : MonoBehaviour
	{
		public Single minIntensity = 0.25f;

		public Single maxIntensity = 0.5f;

		public Single IntensityDeltaForDayTime = 0.5f;

		public Single maxPositionOffset = 1f;

		public Single flickerSpeed = 1f;

		public Single intensityScaleFactor = 1f;

		private Single m_random;

		private Vector3 m_lightPosition = default(Vector3);

		private Boolean m_dayTimeMode;

		private Single m_dayTimeIntensity = 1f;

		private void Start()
		{
			m_random = Random.Range(0f, 65535f);
			m_lightPosition = light.transform.localPosition;
		}

		private void Update()
		{
			Single num = Mathf.PerlinNoise(m_random, flickerSpeed * Time.time);
			if (m_dayTimeMode)
			{
				light.intensity = Mathf.Lerp(m_dayTimeIntensity - IntensityDeltaForDayTime, m_dayTimeIntensity + IntensityDeltaForDayTime, num * intensityScaleFactor);
			}
			else
			{
				light.intensity = Mathf.Lerp(minIntensity, maxIntensity, num * intensityScaleFactor);
			}
			Single num2 = Mathf.Lerp(0f, maxPositionOffset, num);
			light.transform.localPosition = new Vector3(m_lightPosition.x, m_lightPosition.y + num2 * light.transform.lossyScale.y, m_lightPosition.z);
		}

		public void SetDayTimeIntensity(Single p_Intensity)
		{
			m_dayTimeIntensity = p_Intensity;
			m_dayTimeMode = true;
		}

		public Single GetDayTimeIntensity()
		{
			m_dayTimeMode = true;
			return m_dayTimeIntensity;
		}
	}
}
