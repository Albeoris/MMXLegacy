using System;
using UnityEngine;

namespace Legacy
{
	internal class PlayDarkNova_ErebosCutScene : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] m_ParticleSystem;

		private void EnableParticleSystems(Int32 index)
		{
			if (m_ParticleSystem != null && index >= 0 && index < m_ParticleSystem.Length)
			{
				m_ParticleSystem[index].gameObject.SetActive(true);
			}
		}
	}
}
