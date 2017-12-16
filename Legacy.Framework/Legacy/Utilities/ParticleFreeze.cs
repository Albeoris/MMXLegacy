using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/ParticleFreeze")]
	public class ParticleFreeze : MonoBehaviour
	{
		private ParticleSystem[] m_particleSystems;

		private void Start()
		{
			m_particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in m_particleSystems)
			{
				particleSystem.Simulate(particleSystem.duration, true);
				particleSystem.Pause(true);
			}
		}

		private void OnEnable()
		{
			if (m_particleSystems != null)
			{
				foreach (ParticleSystem particleSystem in m_particleSystems)
				{
					particleSystem.Simulate(particleSystem.duration, true);
					particleSystem.Pause(true);
				}
			}
		}
	}
}
