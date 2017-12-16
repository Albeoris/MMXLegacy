using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class FallingCamera : BaseCameraFX
	{
		[SerializeField]
		private Single m_acceleration = 20f;

		[SerializeField]
		private Vector3 m_targetAngle = new Vector3(-80f, 0f, 0f);

		[SerializeField]
		private Single m_rotateSpeed = 0.8f;

		private Single m_currentVelocity;

		public Boolean IsFalling { get; set; }

		public override void CancelEffect()
		{
		}

		private void Update()
		{
			if (IsFalling)
			{
				m_currentVelocity += m_acceleration * Time.deltaTime;
				Vector3 localPosition = transform.localPosition;
				localPosition.y -= m_currentVelocity * Time.deltaTime;
				transform.localPosition = localPosition;
				Single num = Quaternion.Angle(Quaternion.Euler(m_targetAngle), transform.localRotation);
				if (num > 1f)
				{
					transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(m_targetAngle), Time.deltaTime * m_rotateSpeed);
				}
			}
		}
	}
}
