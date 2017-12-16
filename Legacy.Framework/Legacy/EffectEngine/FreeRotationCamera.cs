using System;
using UnityEngine;

namespace Legacy.EffectEngine
{
	public class FreeRotationCamera : BaseCameraFX
	{
		private Vector3 m_LookAngle;

		[SerializeField]
		private Single m_ReturnSpeed = 6f;

		[SerializeField]
		private Single m_MouseSensitivityX = 5f;

		[SerializeField]
		private Single m_MouseSensitivityY = 5f;

		[SerializeField]
		private Single m_MinimumY = -60f;

		[SerializeField]
		private Single m_MaximumY = 60f;

		[SerializeField]
		private Single m_MinimumX = -90f;

		[SerializeField]
		private Single m_MaximumX = 90f;

		public FreeRotationCamera()
		{
			EnableMouseEvent = true;
		}

		public Boolean IsIdle { get; private set; }

		public Boolean EnableMouseEvent { get; set; }

		public override void CancelEffect()
		{
		}

		private void Update()
		{
			if (EnableMouseEvent && Input.GetMouseButton(1))
			{
				m_LookAngle.x = m_LookAngle.x - Input.GetAxis("Mouse Y") * m_MouseSensitivityY;
				m_LookAngle.x = Mathf.Clamp(m_LookAngle.x, m_MinimumY, m_MaximumY);
				m_LookAngle.y = m_LookAngle.y + Input.GetAxis("Mouse X") * m_MouseSensitivityX;
				m_LookAngle.y = Mathf.Clamp(m_LookAngle.y, m_MinimumX, m_MaximumX);
				IsIdle = false;
			}
			else
			{
				m_LookAngle.x = 0f;
				m_LookAngle.y = 0f;
				IsIdle = true;
			}
			Single num = Quaternion.Angle(Quaternion.Euler(m_LookAngle), transform.localRotation);
			if (num > 0.1f)
			{
				transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(m_LookAngle), Time.deltaTime * m_ReturnSpeed * num);
			}
			else if (IsIdle)
			{
				transform.localRotation = Quaternion.identity;
			}
		}
	}
}
