using System;
using System.Collections;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class HitAbsorbingParticleFXBase : HitAbsorbingFXBase
	{
		protected Single EMISSION_RATE_ABSORB_HIT_FACTOR = 1f;

		protected Single START_COLOR_ABSORB_HIT_FACTOR = 1f;

		protected Single START_SIZE_ABSORB_HIT_FACTOR = 4f;

		protected Single START_LIFETIME_ABSORB_HIT_FACTOR = 0.4f;

		protected Single START_SPEED_ABSORB_HIT = 1.5f;

		protected Single LERP_ANIM_SPEED = 6f;

		protected Single VOLUME_ABSORB_HIT_FACTOR = 3f;

		protected String AUDIO_ID = String.Empty;

		protected Single MIN_HIT_ANIM_TIME = 1f;

		protected Boolean IS_START_SPEED_CHANGED = true;

		protected ParticleSystem[] m_particleSysArray;

		protected Color[] m_originalStartColors;

		protected Single[] m_originalStartSizes;

		protected Single[] m_originalEmissionRates;

		protected Single[] m_originalStartLifeTimes;

		protected Single m_originalVolume;

		protected Single m_startLifetimeFactor;

		protected Single m_emissionRateFactor;

		protected Single m_startColorFactor;

		protected Single m_startSizeFactor;

		protected Single m_startSpeed;

		protected Single m_volume;

		protected Boolean m_isAnimationDone = true;

		protected Single m_lastHitTime;

		protected AudioObject m_audio;

		protected override void Start()
		{
			m_lastHitTime = -MIN_HIT_ANIM_TIME;
			m_particleSysArray = gameObject.GetComponentsInChildren<ParticleSystem>();
			m_originalStartColors = new Color[m_particleSysArray.Length];
			m_originalEmissionRates = new Single[m_particleSysArray.Length];
			m_originalStartLifeTimes = new Single[m_particleSysArray.Length];
			m_originalStartSizes = new Single[m_particleSysArray.Length];
			for (Int32 i = 0; i < m_particleSysArray.Length; i++)
			{
				m_originalStartColors[i] = m_particleSysArray[i].startColor;
				m_originalEmissionRates[i] = m_particleSysArray[i].emissionRate;
				m_originalStartLifeTimes[i] = m_particleSysArray[i].startLifetime;
				m_originalStartSizes[i] = m_particleSysArray[i].startSize;
			}
			if (!String.IsNullOrEmpty(AUDIO_ID))
			{
				m_audio = AudioController.Play(AUDIO_ID, transform);
				if (m_audio != null)
				{
					m_originalVolume = m_audio.volume;
				}
			}
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
			if (!m_isAnimationDone)
			{
				LerpParticleSystem();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			for (Int32 i = 0; i < m_particleSysArray.Length; i++)
			{
				ParticleSystem particleSystem = m_particleSysArray[i];
				if (Camera.main != null)
				{
					particleSystem.transform.parent = Camera.main.transform;
				}
				particleSystem.Stop();
				Destroy(particleSystem.gameObject, particleSystem.startLifetime + Mathf.Max(0f, MIN_HIT_ANIM_TIME - (Time.time - m_lastHitTime)));
			}
			if (m_audio != null)
			{
				m_audio.Stop();
				m_audio = null;
			}
		}

		protected override void ShowAbsorbHitFX()
		{
			m_startLifetimeFactor = START_LIFETIME_ABSORB_HIT_FACTOR;
			m_emissionRateFactor = EMISSION_RATE_ABSORB_HIT_FACTOR;
			m_startColorFactor = START_COLOR_ABSORB_HIT_FACTOR;
			m_startSizeFactor = START_SIZE_ABSORB_HIT_FACTOR;
			m_startSpeed = START_SPEED_ABSORB_HIT;
			m_volume = m_originalVolume * VOLUME_ABSORB_HIT_FACTOR;
			m_isAnimationDone = false;
			m_lastHitTime = Time.time;
		}

		protected override IEnumerator ShowStandbyFX()
		{
			if (m_lastHitTime + MIN_HIT_ANIM_TIME > Time.time)
			{
				yield return new WaitForSeconds(m_lastHitTime + MIN_HIT_ANIM_TIME - Time.time);
			}
			m_startLifetimeFactor = 1f;
			m_emissionRateFactor = 1f;
			m_startColorFactor = 1f;
			m_startSizeFactor = 1f;
			m_startSpeed = 0f;
			m_volume = m_originalVolume;
			m_isAnimationDone = false;
			yield break;
		}

		protected override Single MinFXTimeDuringPartyTurn()
		{
			return MIN_HIT_ANIM_TIME;
		}

		protected virtual void LerpParticleSystem()
		{
			m_isAnimationDone = true;
			if (m_audio != null)
			{
				m_audio.volume = Mathf.Lerp(m_audio.volume, m_volume, Time.deltaTime * LERP_ANIM_SPEED);
				if (Mathf.Abs(m_audio.volume - m_volume) > 0.01f)
				{
					m_isAnimationDone = false;
				}
				else
				{
					m_audio.volume = m_volume;
				}
			}
			for (Int32 i = 0; i < m_particleSysArray.Length; i++)
			{
				ParticleSystem particleSystem = m_particleSysArray[i];
				Color color = m_originalStartColors[i] * m_startColorFactor;
				Single startSpeed = m_startSpeed;
				Single num = m_originalEmissionRates[i] * m_emissionRateFactor;
				Single num2 = m_originalStartLifeTimes[i] * m_startLifetimeFactor;
				Single num3 = m_originalStartSizes[i] * m_startSizeFactor;
				if ((Mathf.Abs(particleSystem.startSpeed - startSpeed) > 0.01f && IS_START_SPEED_CHANGED) || Mathf.Abs(particleSystem.emissionRate - num) > 0.01f || Mathf.Abs(particleSystem.startLifetime - num2) > 0.01f || Mathf.Abs(particleSystem.startSize - num3) > 0.01f || Mathf.Abs(particleSystem.startColor.grayscale - color.grayscale) > 0.01f || Mathf.Abs(particleSystem.startColor.a - color.a) > 0.01f)
				{
					particleSystem.startColor = Color.Lerp(particleSystem.startColor, color, Time.deltaTime * LERP_ANIM_SPEED);
					if (IS_START_SPEED_CHANGED)
					{
						particleSystem.startSpeed = Mathf.Lerp(particleSystem.startSpeed, startSpeed, Time.deltaTime * LERP_ANIM_SPEED);
					}
					particleSystem.emissionRate = Mathf.Lerp(particleSystem.emissionRate, num, Time.deltaTime * LERP_ANIM_SPEED);
					particleSystem.startLifetime = Mathf.Lerp(particleSystem.startLifetime, num2, Time.deltaTime * LERP_ANIM_SPEED);
					particleSystem.startSize = Mathf.Lerp(particleSystem.startSize, num3, Time.deltaTime * LERP_ANIM_SPEED);
					m_isAnimationDone = false;
				}
				else
				{
					particleSystem.startColor = color;
					if (IS_START_SPEED_CHANGED)
					{
						particleSystem.startSpeed = startSpeed;
					}
					particleSystem.emissionRate = num;
					particleSystem.startLifetime = num2;
					particleSystem.startSize = num3;
				}
			}
		}
	}
}
