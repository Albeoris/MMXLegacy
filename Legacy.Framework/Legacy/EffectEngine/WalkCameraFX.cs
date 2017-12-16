using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class WalkCameraFX : BaseCameraFX
	{
		private const Single WAIT_TIME_CONTINUE_WALKING = 0.05f;

		[SerializeField]
		private Single BOBBING_SPEED = 2.8f;

		[SerializeField]
		private Single BOBBING_AMOUNT = 0.1f;

		[SerializeField]
		private Single MID_POINT = 5f;

		private Single m_enabledTimer;

		private Single m_bobbingSpeed;

		private Single m_stopTimer;

		protected override void Awake()
		{
			base.Awake();
			enabled = false;
		}

		public void Play(Single moveTime)
		{
			m_bobbingSpeed = BOBBING_SPEED / moveTime;
			m_stopTimer = 0f;
			enabled = true;
		}

		public void Stop()
		{
			if (!enabled)
			{
				return;
			}
			m_stopTimer = Time.time + 0.05f;
		}

		public override void CancelEffect()
		{
			Stop();
		}

		private void Update()
		{
			if (m_stopTimer != 0f && m_stopTimer < Time.time)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.y = MID_POINT;
				transform.localPosition = localPosition;
				m_enabledTimer = 0f;
				enabled = false;
			}
			else if (m_stopTimer != 0f)
			{
				Vector3 localPosition2 = transform.localPosition;
				localPosition2.y -= Time.deltaTime;
				if (localPosition2.y < MID_POINT)
				{
					localPosition2.y = MID_POINT;
				}
				transform.localPosition = localPosition2;
			}
			else
			{
				Single num = Mathf.Abs(Mathf.Sin(m_enabledTimer));
				m_enabledTimer += m_bobbingSpeed * Time.deltaTime;
				if (m_enabledTimer > 6.28318548f)
				{
					m_enabledTimer -= 6.28318548f;
				}
				Single num2 = num * BOBBING_AMOUNT;
				Vector3 localPosition3 = transform.localPosition;
				localPosition3.y = MID_POINT + num2;
				transform.localPosition = localPosition3;
			}
		}
	}
}
