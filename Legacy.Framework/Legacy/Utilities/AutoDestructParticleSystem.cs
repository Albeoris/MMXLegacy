using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/AutoDestructParticleSystem")]
	public class AutoDestructParticleSystem : MonoBehaviour
	{
		protected ParticleSystem[] m_ParticleSystems;

		public void Play()
		{
			if (m_ParticleSystems != null)
			{
				for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
				{
					if (m_ParticleSystems[i] != null)
					{
						m_ParticleSystems[i].Play(false);
					}
				}
			}
		}

		public void Stop()
		{
			if (m_ParticleSystems != null)
			{
				for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
				{
					if (m_ParticleSystems[i] != null)
					{
						m_ParticleSystems[i].Stop(false);
					}
				}
			}
		}

		public void Clear()
		{
			if (m_ParticleSystems != null)
			{
				for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
				{
					if (m_ParticleSystems[i] != null)
					{
						m_ParticleSystems[i].Clear(false);
					}
				}
			}
		}

		public void SetStartDelay(Single delay)
		{
			delay = Mathf.Max(delay, 0f);
			if (m_ParticleSystems != null)
			{
				for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
				{
					if (m_ParticleSystems[i] != null)
					{
						m_ParticleSystems[i].startDelay += delay;
					}
				}
			}
		}

		protected virtual void Awake()
		{
			m_ParticleSystems = GetComponentsInChildren<ParticleSystem>(true);
			LateUpdate();
		}

		protected virtual void LateUpdate()
		{
			for (Int32 i = 0; i < m_ParticleSystems.Length; i++)
			{
				if (m_ParticleSystems[i] != null && m_ParticleSystems[i].isPlaying && m_ParticleSystems[i].IsAlive(false))
				{
					return;
				}
			}
			Destroy(gameObject);
		}
	}
}
