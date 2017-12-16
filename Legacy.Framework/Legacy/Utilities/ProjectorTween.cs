using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/ProjectorTween")]
	[RequireComponent(typeof(Projector))]
	public class ProjectorTween : MonoBehaviour
	{
		private Projector m_projector;

		private Single m_beginTime;

		public Single m_beginFoV;

		[SerializeField]
		private AnimationCurve m_FoVScaleCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		protected virtual void Awake()
		{
			m_projector = gameObject.GetComponent<Projector>();
			if (m_projector == null)
			{
				Destroy(gameObject);
			}
			StartTween();
		}

		protected virtual void Update()
		{
			Single time = Time.time - m_beginTime;
			Single num = m_FoVScaleCurve.Evaluate(time);
			m_projector.fieldOfView = Math.Max(m_beginFoV * num, 0f);
		}

		protected void StartTween()
		{
			m_beginTime = Time.time;
			if (m_beginFoV == 0f)
			{
				m_beginFoV = m_projector.fieldOfView;
			}
		}
	}
}
