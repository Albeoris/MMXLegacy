using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class WindShieldCameraFX : HitAbsorbingParticleFXBase
	{
		private void Awake()
		{
			EMISSION_RATE_ABSORB_HIT_FACTOR = 5f;
			START_COLOR_ABSORB_HIT_FACTOR = 4f;
			START_SIZE_ABSORB_HIT_FACTOR = 1.1f;
			START_LIFETIME_ABSORB_HIT_FACTOR = 1f;
			START_SPEED_ABSORB_HIT = 1f;
			LERP_ANIM_SPEED = 6f;
			VOLUME_ABSORB_HIT_FACTOR = 4.5f;
			AUDIO_ID = "WindShield";
			MIN_HIT_ANIM_TIME = 1.8f;
			IS_START_SPEED_CHANGED = false;
			m_lastHitTime = -MIN_HIT_ANIM_TIME;
		}

		protected override void Start()
		{
			Camera.main.transform.AddChildAlignOrigin(transform);
			base.Start();
		}
	}
}
