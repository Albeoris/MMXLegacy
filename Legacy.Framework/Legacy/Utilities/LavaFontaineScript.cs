using System;
using System.Collections;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Lava Fontaine")]
	public class LavaFontaineScript : MonoBehaviour
	{
		public Single minDelay;

		public Single maxDelay;

		public Single durationLightOff;

		public Single intensityInterval;

		public Single intensityLossPerInterval;

		public GameObject lightSource;

		private Boolean waitToTurnOff;

		private Boolean coroutineInProgess;

		private Single lightIntensity;

		private IEnumerator Eruption()
		{
			Single p_Delay = Random.Range(minDelay, maxDelay);
			yield return new WaitForSeconds(p_Delay);
			StartCoroutine(LastTurnsTheLightOff());
			gameObject.particleSystem.Play();
			lightSource.light.intensity = lightIntensity;
			lightSource.SetActive(true);
			StartCoroutine(Eruption());
			yield break;
		}

		private IEnumerator LastTurnsTheLightOff()
		{
			waitToTurnOff = true;
			yield return new WaitForSeconds(durationLightOff);
			lightSource.SetActive(false);
			waitToTurnOff = false;
			yield break;
		}

		private IEnumerator FadeOut()
		{
			coroutineInProgess = true;
			yield return new WaitForSeconds(intensityInterval);
			lightSource.light.intensity = lightSource.light.intensity - intensityLossPerInterval;
			coroutineInProgess = false;
			yield break;
		}

		private void Start()
		{
			lightIntensity = lightSource.light.intensity;
			lightSource.SetActive(false);
			StartCoroutine(Eruption());
		}

		private void Update()
		{
			if (waitToTurnOff && !coroutineInProgess)
			{
				StartCoroutine(FadeOut());
			}
		}
	}
}
