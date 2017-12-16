using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class ShakeCameraFX : BaseCameraFX
	{
		[SerializeField]
		private Single SHAKE_DECAY = 0.5f;

		[SerializeField]
		private Single RANDOM_ROTATION_LIMIT = 0.25f;

		[SerializeField]
		private Single REBOUND_SPEED = 13f;

		[SerializeField]
		private Single REBOUND_MAX_DIST = 2f;

		private Single m_shakeIntensity;

		private Vector3 m_reboundDirection;

		private Single m_currentReboundDist;

		private Single m_currentReboundDir;

		private Vector3 m_originalPos;

		private Quaternion m_originalRot;

		protected override void Awake()
		{
			base.Awake();
			enabled = false;
		}

		public void Play(Single shakeIntensity, Vector3 impactDirection)
		{
			m_shakeIntensity = shakeIntensity;
			if (m_reboundDirection == Vector3.zero)
			{
				m_reboundDirection = transform.rotation * impactDirection;
				if (transform.forward != Vector3.forward && transform.forward != Vector3.back)
				{
					m_reboundDirection *= -1f;
				}
				m_currentReboundDist = 0f;
				m_currentReboundDir = 1f;
			}
			if (!enabled)
			{
				m_originalPos = transform.localPosition;
				m_originalRot = transform.localRotation;
			}
			enabled = true;
		}

		public override void CancelEffect()
		{
			m_shakeIntensity = 0f;
			m_currentReboundDist = 0f;
		}

		private void Update()
		{
			if (m_shakeIntensity > 0f)
			{
				if (m_reboundDirection == Vector3.zero)
				{
					transform.localPosition = m_originalPos + UnityEngine.Random.insideUnitSphere * m_shakeIntensity;
				}
				transform.localRotation = new Quaternion(m_originalRot.x + Random.Range(-m_shakeIntensity, m_shakeIntensity) * RANDOM_ROTATION_LIMIT, m_originalRot.y + Random.Range(-m_shakeIntensity, m_shakeIntensity) * RANDOM_ROTATION_LIMIT, m_originalRot.z + Random.Range(-m_shakeIntensity, m_shakeIntensity) * RANDOM_ROTATION_LIMIT, m_originalRot.w + Random.Range(-m_shakeIntensity, m_shakeIntensity) * RANDOM_ROTATION_LIMIT);
				m_shakeIntensity -= SHAKE_DECAY * Time.deltaTime;
			}
			if (m_currentReboundDist >= 0f)
			{
				m_currentReboundDist += Time.deltaTime * REBOUND_SPEED * m_currentReboundDir;
				transform.localPosition = m_originalPos + m_currentReboundDist * m_reboundDirection;
				if (m_currentReboundDist >= REBOUND_MAX_DIST)
				{
					m_currentReboundDir = -1f;
				}
			}
			if (m_shakeIntensity <= 0f && m_currentReboundDist <= 0f)
			{
				transform.localPosition = m_originalPos;
				transform.localRotation = m_originalRot;
				m_reboundDirection = Vector3.zero;
				enabled = false;
			}
		}
	}
}
