using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Animated Lights v2")]
	public class AnimatedLightScripts_v2 : MonoBehaviour
	{
		public String description = "Enter description here";

		private Light m_Lights;

		public AnimationCurve m_IntensityCurve = Helper.AnimationCurveEaseInOut(0f, 0f, 1f, 1f, WrapMode.Loop);

		public AnimationCurve m_MovementCurve = Helper.AnimationCurveEaseInOut(0f, -1f, 1f, 1f, WrapMode.Loop);

		public Single intensityStartValue = 1f;

		public Single flickerSpeed = 1f;

		public Single intensityDelta = 0.5f;

		public Single intensityScaleFactor = 1f;

		public Vector3 positionOffset = Vector3.up;

		public Boolean m_isFlicker;

		private Single m_random;

		private Vector3 m_lightPosition;

		private Single m_lightIntensity;

		private void Start()
		{
			m_Lights = light;
			if (m_Lights == null)
			{
				enabled = false;
				return;
			}
			m_random = Random.Range(0f, 65535f);
			m_lightPosition = default(Vector3);
			m_lightIntensity = 0f;
			m_lightPosition = m_Lights.transform.localPosition;
			m_lightIntensity = m_Lights.intensity;
		}

		private void Update()
		{
			if (m_Lights == null)
			{
				enabled = false;
				return;
			}
			if (m_isFlicker)
			{
				Single num = Mathf.PerlinNoise(m_random, flickerSpeed * Time.time);
				m_Lights.intensity = Mathf.Lerp(m_lightIntensity, m_lightIntensity + intensityDelta, num * intensityScaleFactor);
				Vector3 a = Vector3.Lerp(Vector3.zero, positionOffset, num);
				m_Lights.transform.localPosition = m_lightPosition + Vector3.Scale(a, m_Lights.transform.lossyScale);
			}
			else
			{
				Single num2 = m_IntensityCurve.Evaluate((Int32)(flickerSpeed * Time.time * 10000f) % 10000f / 10000f);
				m_Lights.intensity = num2 * intensityScaleFactor;
				Vector3 a2 = Vector3.Lerp(Vector3.zero, positionOffset, m_MovementCurve.Evaluate((Int32)(flickerSpeed * Time.time * 10000f) % 10000f / 10000f));
				m_Lights.transform.localPosition = m_lightPosition + Vector3.Scale(a2, m_Lights.transform.lossyScale);
			}
		}
	}
}
