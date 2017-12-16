using System;
using System.Collections;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class CelestialArmorCameraFX : HitAbsorbingParticleFXBase
	{
		protected Single LIGHT_ABSORB_HIT_FACTOR = 2.5f;

		[SerializeField]
		private Transform m_partcileRoot;

		[SerializeField]
		private Transform m_lightRoot;

		protected Single[] m_originalLights;

		protected Single m_lightFactor;

		private FlickeringLightScript[] m_lightArray;

		private void Awake()
		{
			EMISSION_RATE_ABSORB_HIT_FACTOR = 10f;
			START_COLOR_ABSORB_HIT_FACTOR = 1f;
			START_SIZE_ABSORB_HIT_FACTOR = 2f;
			LIGHT_ABSORB_HIT_FACTOR = 1.5f;
			START_LIFETIME_ABSORB_HIT_FACTOR = 0.2f;
			START_SPEED_ABSORB_HIT = 0f;
			LERP_ANIM_SPEED = 6f;
			VOLUME_ABSORB_HIT_FACTOR = 3f;
			MIN_HIT_ANIM_TIME = 1f;
			IS_START_SPEED_CHANGED = true;
			m_lastHitTime = -MIN_HIT_ANIM_TIME;
		}

		protected override void Start()
		{
			Camera.main.transform.AddChildAlignOrigin(transform);
			m_lightArray = m_lightRoot.GetComponentsInChildren<FlickeringLightScript>();
			m_originalLights = new Single[m_lightArray.Length];
			for (Int32 i = 0; i < m_lightArray.Length; i++)
			{
				m_originalLights[i] = m_lightArray[i].maxIntensity;
			}
			base.Start();
		}

		protected override void ShowAbsorbHitFX()
		{
			m_lightFactor = LIGHT_ABSORB_HIT_FACTOR;
			base.ShowAbsorbHitFX();
		}

		protected override IEnumerator ShowStandbyFX()
		{
			m_lightFactor = 1f;
			return base.ShowStandbyFX();
		}

		protected override void LerpParticleSystem()
		{
			base.LerpParticleSystem();
			for (Int32 i = 0; i < m_lightArray.Length; i++)
			{
				Single num = m_originalLights[i] * m_lightFactor;
				m_lightArray[i].maxIntensity = Mathf.Lerp(m_lightArray[i].maxIntensity, num, Time.deltaTime * LERP_ANIM_SPEED);
				if (Mathf.Abs(m_lightArray[i].maxIntensity - num) > 0.01f)
				{
					m_isAnimationDone = false;
				}
				else
				{
					m_lightArray[i].maxIntensity = num;
				}
			}
		}
	}
}
