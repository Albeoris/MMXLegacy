using System;
using System.Collections;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class FireShieldCameraFX : HitAbsorbingParticleFXBase
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
			EMISSION_RATE_ABSORB_HIT_FACTOR = 1f;
			START_COLOR_ABSORB_HIT_FACTOR = 1f;
			START_SIZE_ABSORB_HIT_FACTOR = 4f;
			LIGHT_ABSORB_HIT_FACTOR = 2.5f;
			START_LIFETIME_ABSORB_HIT_FACTOR = 0.4f;
			START_SPEED_ABSORB_HIT = 1.5f;
			LERP_ANIM_SPEED = 6f;
			VOLUME_ABSORB_HIT_FACTOR = 3f;
			AUDIO_ID = "FireShieldIdle";
			MIN_HIT_ANIM_TIME = 1f;
			IS_START_SPEED_CHANGED = true;
			m_lastHitTime = -MIN_HIT_ANIM_TIME;
		}

		protected override void Start()
		{
			Camera.main.transform.AddChildAlignOrigin(transform);
			GameObject gameObject = m_partcileRoot.gameObject;
			GameObject gameObject2 = Helper.Instantiate<GameObject>(gameObject);
			transform.AddChildAlignOrigin(gameObject2.transform);
			gameObject2.transform.localEulerAngles = Vector3.up * 180f;
			gameObject.AddComponent<AlignToFOV>().Alignment = AlignToFOV.EAlignment.LEFT;
			gameObject2.AddComponent<AlignToFOV>().Alignment = AlignToFOV.EAlignment.RIGHT;
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
