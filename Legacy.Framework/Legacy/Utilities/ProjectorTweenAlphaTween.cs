using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/ProjectorTweenAlphaTween")]
	[RequireComponent(typeof(Projector))]
	public class ProjectorTweenAlphaTween : MonoBehaviour
	{
		private Projector m_projector;

		private Single m_beginTime;

		private Single m_animTime;

		public Single m_maxAlpha;

		public Single m_maxLerpSpeed = 1f;

		public Single m_startAlpha;

		public Single m_delay;

		public Boolean m_loop = true;

		[SerializeField]
		private AnimationCurve m_AlphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		public void FadeOut()
		{
			m_AlphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
			m_loop = false;
			m_maxAlpha = m_projector.material.color.a;
			m_beginTime = Time.time;
		}

		protected virtual void Awake()
		{
			m_animTime = m_AlphaCurve.keys[m_AlphaCurve.length - 1].time;
			m_projector = gameObject.GetComponent<Projector>();
			if (m_projector == null)
			{
				Destroy(gameObject);
			}
			StartTween();
		}

		protected virtual void Update()
		{
			if (m_beginTime > Time.time)
			{
				return;
			}
			Single num = Time.time - m_beginTime;
			if (m_loop && num > m_animTime)
			{
				m_beginTime += m_animTime;
				num = Time.time - m_beginTime;
			}
			Single num2 = m_AlphaCurve.Evaluate(num);
			Color color = m_projector.material.color;
			color.a = Mathf.Lerp(color.a, Math.Max(m_maxAlpha * num2, 0f), Time.deltaTime * m_maxLerpSpeed);
			m_projector.material.color = color;
		}

		protected void StartTween()
		{
			m_beginTime = Time.time;
			m_projector.material = Helper.Instantiate<Material>(m_projector.material);
			if (m_maxAlpha == 0f)
			{
				m_maxAlpha = m_projector.material.color.a;
			}
			if (m_delay > 0f)
			{
				m_beginTime += m_delay;
				Color color = m_projector.material.color;
				color.a = m_startAlpha;
				m_projector.material.color = color;
			}
		}
	}
}
