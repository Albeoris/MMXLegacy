using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class ReboundCameraFX : BaseCameraFX
	{
		[SerializeField]
		private Single REBOUND_SPEED = 5.5f;

		[SerializeField]
		private Single REBOUND_MAX_DIST = 0.5f;

		private Vector3 m_originalPos;

		private Vector3 m_reboundDirection;

		private Single m_currentDist;

		private Single m_currentDir;

		protected override void Awake()
		{
			base.Awake();
			enabled = false;
		}

		public void Play(Vector3 reboundDirection)
		{
			m_reboundDirection = transform.rotation * reboundDirection;
			if (transform.forward != Vector3.forward && transform.forward != Vector3.back)
			{
				m_reboundDirection *= -1f;
			}
			if (!enabled)
			{
				m_originalPos = transform.localPosition;
			}
			m_currentDist = 0f;
			m_currentDir = 1f;
			enabled = true;
		}

		public override void CancelEffect()
		{
			m_currentDist = 0f;
		}

		private void Update()
		{
			m_currentDist += Time.deltaTime * REBOUND_SPEED * m_currentDir;
			transform.localPosition = Vector3.Lerp(transform.localPosition, m_originalPos + m_currentDist * m_reboundDirection, Time.deltaTime * 100f);
			if (m_currentDist >= REBOUND_MAX_DIST)
			{
				m_currentDir = -1f;
			}
			else if (m_currentDist <= 0f)
			{
				transform.localPosition = m_originalPos;
				enabled = false;
			}
		}
	}
}
