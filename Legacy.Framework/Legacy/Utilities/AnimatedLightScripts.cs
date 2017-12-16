using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Animated Lights")]
	public class AnimatedLightScripts : MonoBehaviour
	{
		[SerializeField]
		private String description = "Enter description here";

		[SerializeField]
		private Light[] m_Lights = Arrays<Light>.Empty;

		[SerializeField]
		private AnimationCurve m_IntensityCurve = Helper.AnimationCurveEaseInOut(0f, 0f, 1f, 1f, WrapMode.Loop);

		[SerializeField]
		private AnimationCurve m_MovementCurve = Helper.AnimationCurveEaseInOut(0f, -1f, 1f, 1f, WrapMode.Loop);

		[SerializeField]
		private Single intensityStartValue = 1f;

		[SerializeField]
		private Single flickerSpeed = 1f;

		[SerializeField]
		private Single intensityDelta = 0.5f;

		[SerializeField]
		private Single intensityScaleFactor = 1f;

		[SerializeField]
		private Vector3 positionOffset = Vector3.up;

		[SerializeField]
		private Boolean m_isFlicker;

		private Single m_random;

		private Vector3[] m_lightPosition;

		private Single[] m_lightIntensity;

		private void Start()
		{
			m_random = Random.Range(0f, 65535f);
			m_lightPosition = new Vector3[m_Lights.Length];
			m_lightIntensity = new Single[m_Lights.Length];
			for (Int32 i = 0; i < m_Lights.Length; i++)
			{
				if (m_Lights[i] != null)
				{
					m_lightPosition[i] = m_Lights[i].transform.localPosition;
					m_lightIntensity[i] = m_Lights[i].intensity;
				}
			}
		}

		private void Update()
		{
			for (Int32 i = 0; i < m_Lights.Length; i++)
			{
				if (m_Lights[i] != null)
				{
					Transform transform = m_Lights[i].transform;
					if (m_isFlicker)
					{
						Single num = Mathf.PerlinNoise(m_random, flickerSpeed * Time.time);
						m_Lights[i].intensity = Mathf.Lerp(m_lightIntensity[i], m_lightIntensity[i] + intensityDelta, num * intensityScaleFactor);
						Vector3 a = Vector3.Lerp(Vector3.zero, positionOffset, num);
						transform.localPosition = m_lightPosition[i] + Vector3.Scale(a, transform.lossyScale);
					}
					else
					{
						Single num2 = m_IntensityCurve.Evaluate((Int32)(flickerSpeed * Time.time * 10000f) % 10000f / 10000f);
						m_Lights[i].intensity = num2 * intensityScaleFactor;
						Vector3 a2 = Vector3.Lerp(Vector3.zero, positionOffset, m_MovementCurve.Evaluate((Int32)(flickerSpeed * Time.time * 10000f) % 10000f / 10000f));
						transform.localPosition = m_lightPosition[i] + Vector3.Scale(a2, transform.lossyScale);
					}
				}
			}
		}
	}
}
