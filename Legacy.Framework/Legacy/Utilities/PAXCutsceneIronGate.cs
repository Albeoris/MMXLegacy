using System;
using UnityEngine;

namespace Legacy.Utilities
{
	public class PAXCutsceneIronGate : MonoBehaviour
	{
		[SerializeField]
		private Explosion m_explosion;

		[SerializeField]
		private ParticleSystem m_sparks;

		[SerializeField]
		private GameObject[] m_fxSparksObjects;

		[SerializeField]
		private ParticleSystem[] m_particleSystems;

		private void Awake()
		{
			foreach (GateExplosionEffects gateExplosionEffects in GetComponentsInChildren<GateExplosionEffects>(true))
			{
				gateExplosionEffects.enabled = false;
			}
		}

		private void OnEnable()
		{
			CutsceneAction();
		}

		public void CutsceneAction()
		{
			foreach (GateExplosionEffects gateExplosionEffects in GetComponentsInChildren<GateExplosionEffects>(true))
			{
				gateExplosionEffects.enabled = true;
			}
			m_explosion.Explode();
			DoFx();
		}

		private void DoFx()
		{
			foreach (GameObject gameObject in m_fxSparksObjects)
			{
				if (gameObject != null)
				{
					m_sparks.transform.position = gameObject.transform.position;
					m_sparks.Play(true);
				}
			}
			foreach (ParticleSystem particleSystem in m_particleSystems)
			{
				if (particleSystem != null)
				{
					particleSystem.Play(true);
				}
			}
		}
	}
}
