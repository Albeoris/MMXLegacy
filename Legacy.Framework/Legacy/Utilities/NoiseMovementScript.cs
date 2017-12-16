using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Noise Movement")]
	public class NoiseMovementScript : MonoBehaviour
	{
		public Vector3 noiseScale = Vector3.one;

		public Single speed = 1f;

		private Single m_random;

		private Vector3 m_startPosition = default(Vector3);

		private void Start()
		{
			m_random = Random.Range(0f, 65535f);
			m_startPosition = transform.position;
		}

		private void Update()
		{
			Single num = m_random + Time.time * speed;
			Single num2 = Perlin.Noise(num + m_startPosition.x, m_startPosition.y, m_startPosition.z);
			Single num3 = Perlin.Noise(m_startPosition.x, num + m_startPosition.y, m_startPosition.z);
			Single num4 = Perlin.Noise(m_startPosition.x, m_startPosition.y, num + m_startPosition.z);
			Vector3 b = new Vector3(num2 * noiseScale.x, num3 * noiseScale.y, num4 * noiseScale.z);
			transform.position = m_startPosition + b;
		}
	}
}
