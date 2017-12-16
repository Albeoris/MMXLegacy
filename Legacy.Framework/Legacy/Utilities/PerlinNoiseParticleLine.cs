using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/PerlinNoise Particle Line")]
	public class PerlinNoiseParticleLine : MonoBehaviour
	{
		public Transform target;

		public Int32 zigs = 100;

		public Single speed = 1f;

		public Single scale = 1f;

		private Single oneOverZigs;

		private ParticleSystem.Particle[] m_particles;

		private ParticleSystem m_particleSystem;

		private Boolean m_getparticles;

		private void Start()
		{
			m_particles = new ParticleSystem.Particle[zigs];
			oneOverZigs = 1f / zigs;
			m_particleSystem = particleSystem;
			m_particleSystem.loop = false;
			m_particleSystem.enableEmission = false;
			m_particleSystem.Emit(zigs);
		}

		private void LateUpdate()
		{
			if (!m_getparticles)
			{
				m_getparticles = true;
				m_particleSystem.GetParticles(m_particles);
			}
			Single num = Time.time * speed * 0.1365143f;
			Single num2 = Time.time * speed * 1.21688f;
			Single num3 = Time.time * speed * 2.5564f;
			Vector3 position = transform.position;
			Vector3 position2 = target.position;
			for (Int32 i = 0; i < m_particles.Length; i++)
			{
				Vector3 vector = Vector3.Lerp(position, position2, oneOverZigs * i);
				Vector3 a;
				a.x = Perlin.Noise(num + vector.x, num + vector.y, num + vector.z);
				a.y = Perlin.Noise(num2 + vector.x, num2 + vector.y, num2 + vector.z);
				a.z = Perlin.Noise(num3 + vector.x, num3 + vector.y, num3 + vector.z);
				vector += a * scale * (i * oneOverZigs);
				m_particles[i].position = vector;
				m_particles[i].lifetime = 1f;
			}
			m_particleSystem.SetParticles(m_particles, m_particles.Length);
		}
	}
}
