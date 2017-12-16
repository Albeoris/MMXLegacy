using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/TimedDestructParticleSystem")]
	public class TimedDestructParticleSystem : AutoDestructParticleSystem
	{
		private Single m_DestroyTime;

		private Single m_DelayTime;

		[SerializeField]
		private Single m_PlayDelayMin;

		[SerializeField]
		private Single m_PlayDelayMax;

		[SerializeField]
		private Single m_DestroyTimeMin = 5f;

		[SerializeField]
		private Single m_DestroyTimeMax = 5f;

		protected override void Awake()
		{
			base.Awake();
			m_PlayDelayMax = Mathf.Clamp(m_PlayDelayMax, 0f, Single.MaxValue);
			m_PlayDelayMin = Mathf.Clamp(m_PlayDelayMin, 0f, m_PlayDelayMax);
			m_DestroyTimeMax = Mathf.Clamp(m_DestroyTimeMax, 0f, Single.MaxValue);
			m_DestroyTimeMin = Mathf.Clamp(m_DestroyTimeMin, 0f, m_DestroyTimeMax);
		}

		private void Start()
		{
			Single num = Random.Range(m_PlayDelayMin, m_PlayDelayMax);
			if (num > 0f)
			{
				m_DelayTime = Time.time + num;
				Stop();
				Clear();
			}
			Single num2 = Random.Range(m_DestroyTimeMin, m_DestroyTimeMax);
			m_DestroyTime = Time.time + num + num2;
		}

		protected override void LateUpdate()
		{
			Single time = Time.time;
			if (m_DelayTime != 0f && m_DelayTime < time)
			{
				m_DelayTime = Single.MaxValue;
				Play();
			}
			else if (m_DelayTime == 0f || m_DelayTime == 3.40282347E+38f)
			{
				base.LateUpdate();
			}
			if (m_DestroyTime < time)
			{
				m_DestroyTime = Single.MaxValue;
				Stop();
			}
		}
	}
}
