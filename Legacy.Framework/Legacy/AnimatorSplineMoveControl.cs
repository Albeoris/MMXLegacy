using System;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy
{
	public class AnimatorSplineMoveControl : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_gameObject;

		[SerializeField]
		private BezierSpline m_bezier;

		public Single TotalTime;

		private Single m_Time;

		private Single m_CurrSpeed;

		private Vector3 m_CurrPos;

		private Quaternion m_CurrRota;

		private void Update()
		{
			if (m_gameObject == null || m_bezier == null)
			{
				enabled = false;
				return;
			}
			if (m_Time > 1f)
			{
				m_Time = 0f;
			}
			m_Time += m_CurrSpeed * Time.deltaTime * (1f / TotalTime);
			BezierSpline.SplineSampleData dataOnCurveTime = m_bezier.GetDataOnCurveTime(m_Time);
			m_CurrPos = dataOnCurveTime.Position;
			m_CurrRota = dataOnCurveTime.Rotation;
			m_CurrSpeed = dataOnCurveTime.Speed;
			m_gameObject.transform.position = m_CurrPos;
			m_gameObject.transform.rotation = m_CurrRota;
		}
	}
}
