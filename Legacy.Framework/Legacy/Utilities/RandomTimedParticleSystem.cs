using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[RequireComponent(typeof(ParticleSystem))]
	[AddComponentMenu("MM Legacy/Utility/Random Timed Particle System")]
	public class RandomTimedParticleSystem : MonoBehaviour
	{
		private ParticleSystem particleObject;

		public Single minTimer = 5f;

		public Single maxTimer = 10f;

		public AmbientSoundSource soundSource;

		public String oneShotAudioIDFromMainAudioController;

		private void OnEnable()
		{
			particleObject = particleSystem;
			if (particleObject == null)
			{
				return;
			}
			Invoke("RestartParticleSystem", Random.Range(minTimer, maxTimer));
		}

		private void OnDisable()
		{
			CancelInvoke("RestartParticleSystem");
		}

		private void RestartParticleSystem()
		{
			if (!String.IsNullOrEmpty(oneShotAudioIDFromMainAudioController))
			{
				AudioController.Play(oneShotAudioIDFromMainAudioController, transform);
			}
			particleObject.Stop();
			particleObject.Play();
			if (soundSource != null)
			{
				soundSource.enabled = true;
				Invoke("StopSound", particleObject.duration);
			}
			Invoke("RestartParticleSystem", Random.Range(minTimer, maxTimer));
		}

		private void StopSound()
		{
			if (soundSource != null)
			{
				soundSource.enabled = false;
			}
		}
	}
}
