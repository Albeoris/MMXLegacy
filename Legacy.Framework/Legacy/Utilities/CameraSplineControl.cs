using System;
using UnityEngine;

namespace Legacy.Utilities
{
	public class CameraSplineControl : MonoBehaviour
	{
		[SerializeField]
		private BezierSpline m_PositionCurve;

		[SerializeField]
		private BezierSpline m_LookAtCurve;

		public Single TotalTime;

		public Boolean Loop;

		public Boolean SyncTimes;

		private Single m_TimePos;

		private Single m_TimeLook;

		private void Update()
		{
			BezierSpline.SplineSampleData dataOnCurveTime = m_PositionCurve.GetDataOnCurveTime(m_TimePos);
			BezierSpline.SplineSampleData dataOnCurveTime2 = m_LookAtCurve.GetDataOnCurveTime(m_TimeLook);
			if (m_TimePos > 1f)
			{
				m_TimePos = (!Loop) ? 1 : 0;
			}
			if (m_TimeLook > 1f)
			{
				m_TimeLook = (!Loop) ? 1 : 0;
			}
			if (SyncTimes)
			{
				m_TimePos += dataOnCurveTime.Speed * Time.deltaTime * (1f / TotalTime);
				m_TimeLook = m_TimePos;
			}
			else
			{
				m_TimePos += dataOnCurveTime.Speed * Time.deltaTime * (1f / TotalTime);
				m_TimeLook += dataOnCurveTime2.Speed * Time.deltaTime * (1f / TotalTime);
			}
			transform.position = dataOnCurveTime.Position;
			transform.LookAt(dataOnCurveTime2.Position);
		}

		public void SetCameraTime(Single time)
		{
		}
	}
}
