using System;
using UnityEngine;

namespace Legacy
{
	internal class PlayParticleSystemOnAnimationEvent : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem m_ParticleSystem1;

		[SerializeField]
		private ParticleSystem m_ParticleSystem2;

		[SerializeField]
		private ParticleSystem[] m_ParticleSystem;

		private void PlayParticleSystem1()
		{
			m_ParticleSystem1.Play(true);
		}

		private void PlayParticleSystem2()
		{
			m_ParticleSystem2.Play(true);
		}

		private void PlayParticleSystem(Int32 index)
		{
			if (m_ParticleSystem != null && index >= 0 && index < m_ParticleSystem.Length)
			{
				if (m_ParticleSystem[index] == null)
				{
					Debug.LogError("ParticleSystem at index " + index + " not found!");
					return;
				}
				m_ParticleSystem[index].Play(true);
			}
		}
	}
}
