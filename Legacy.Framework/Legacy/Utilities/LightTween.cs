using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/LightTween")]
	[RequireComponent(typeof(Light))]
	public class LightTween : MonoBehaviour
	{
		private Light m_light;

		private Single m_beginTime;

		public Single m_beginInstensity;

		private Single m_beginRange;

		[SerializeField]
		private AnimationCurve m_intensityScaleCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		[SerializeField]
		private AnimationCurve m_rangeScaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		protected virtual void Awake()
		{
			m_light = light;
			if (m_light == null)
			{
				Destroy(gameObject);
			}
			StartTween();
		}

		protected virtual void Update()
		{
			Single time = Time.time - m_beginTime;
			Single num = m_intensityScaleCurve.Evaluate(time);
			m_light.intensity = Math.Max(m_beginInstensity * num, 0f);
			num = m_rangeScaleCurve.Evaluate(time);
			m_light.range = Math.Max(m_beginRange * num, 0f);
		}

		protected void StartTween()
		{
			m_beginTime = Time.time;
			if (m_beginInstensity == 0f)
			{
				m_beginInstensity = m_light.intensity;
			}
			m_beginRange = m_light.range;
		}
	}
}
