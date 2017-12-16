using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Utilities;
using Legacy.World;
using UnityEngine;

namespace Legacy
{
	[RequireComponent(typeof(Light))]
	[AddComponentMenu("MM Legacy/World/Daytime Dependent Light")]
	public class DaytimeDependentLight : DaytimeDependentBase
	{
		private const Single FADE_TIME = 3.1f;

		private const Single RANDOM_RANGE = 1f;

		[SerializeField]
		private LightSetting m_dawn = new LightSetting();

		[SerializeField]
		private LightSetting m_day = new LightSetting();

		[SerializeField]
		private LightSetting m_dusk = new LightSetting();

		[SerializeField]
		private LightSetting m_night = new LightSetting();

		private FlickeringLightScript m_flickerScript;

		protected override void Awake()
		{
			base.Awake();
			m_flickerScript = GetComponent<FlickeringLightScript>();
		}

		public override void ChangeState(EDayState newState)
		{
			StopAllCoroutines();
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(Fade(newState));
			}
		}

		private IEnumerator Fade(EDayState state)
		{
			Single rndStartOffset = UnityEngine.Random.value * 1f;
			yield return new WaitForSeconds(rndStartOffset);
			Single startIntensity = GetIntensity();
			Single startRange = light.range;
			Color startColor = light.color;
			LightSetting targetSetting = GetTargetSetting(state);
			UpdateParticles(targetSetting);
			Single currTime = Time.time - Time.deltaTime;
			Single endTime = Time.time + 3.1f + UnityEngine.Random.value * 1f - rndStartOffset;
			WaitForEndOfFrame wait = new WaitForEndOfFrame();
			while (currTime < endTime)
			{
				currTime += Time.deltaTime;
				if (currTime > endTime)
				{
					currTime = endTime;
				}
				Single lerp = (endTime - currTime) / 3.1f;
				Single nextIntensity = Mathf.Lerp(targetSetting.Intensity, startIntensity, lerp);
				Single nextRange = Mathf.Lerp(targetSetting.Range, startRange, lerp);
				Color nextColor = Color.Lerp(targetSetting.Color, startColor, lerp);
				light.enabled = (nextIntensity > 0f && nextRange > 0f && (nextColor.r > 0f || nextColor.g > 0f || nextColor.b > 0f));
				SetIntensity(nextIntensity);
				light.range = nextRange;
				light.color = nextColor;
				yield return wait;
			}
			yield break;
		}

		private LightSetting GetTargetSetting(EDayState state)
		{
			switch (state)
			{
			case EDayState.DAWN:
				return m_dawn;
			case EDayState.DAY:
				return m_day;
			case EDayState.DUSK:
				return m_dusk;
			default:
				return m_night;
			}
		}

		private Single GetIntensity()
		{
			if (m_flickerScript != null)
			{
				return m_flickerScript.GetDayTimeIntensity();
			}
			return light.intensity;
		}

		private void SetIntensity(Single p_intensity)
		{
			if (m_flickerScript != null)
			{
				m_flickerScript.SetDayTimeIntensity(p_intensity);
			}
			else
			{
				light.intensity = p_intensity;
			}
		}

		private void UpdateParticles(LightSetting p_nextSetting)
		{
			UpdateParticles(m_dawn, p_nextSetting);
			UpdateParticles(m_day, p_nextSetting);
			UpdateParticles(m_dusk, p_nextSetting);
			UpdateParticles(m_night, p_nextSetting);
		}

		private static void UpdateParticles(LightSetting p_setting, LightSetting p_nextSetting)
		{
			if (p_setting.ParticleSys != null)
			{
				if (p_nextSetting.ParticleSys == p_setting.ParticleSys)
				{
					if (!p_setting.ParticleSys.isPlaying)
					{
						p_setting.ParticleSys.Play();
					}
				}
				else if (p_setting.ParticleSys.isPlaying)
				{
					p_setting.ParticleSys.Stop();
				}
			}
		}

		[Serializable]
		private class LightSetting
		{
			[SerializeField]
			private Single m_intensity = 1f;

			[SerializeField]
			private Single m_range = 10f;

			[SerializeField]
			private Color m_color = Color.white;

			[SerializeField]
			private ParticleSystem m_particleSys;

			public Single Intensity => m_intensity;

		    public Single Range => m_range;

		    public Color Color => m_color;

		    public ParticleSystem ParticleSys => m_particleSys;
		}
	}
}
