using System;
using UnityEngine;

namespace Legacy.Utilities
{
	public class BezierSpline : MonoBehaviour
	{
		public Boolean Closed;

		public Int32 NormalizeSamples = 1000;

		private BezierSplineContolPoint[] m_Contolpoints;

		private Single[] m_SamplePointTime;

		private Single m_timeInc;

		private SplineSampleData[] SampleData;

		private static Int32[][] m_BinominalArray = new Int32[][]
		{
			new Int32[]
			{
				1
			},
			new Int32[]
			{
				1,
				1
			},
			new Int32[]
			{
				1,
				2,
				1
			},
			new Int32[]
			{
				1,
				3,
				3,
				1
			}
		};

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			m_Contolpoints = gameObject.GetComponentsInChildren<BezierSplineContolPoint>();
			m_timeInc = 1f / NormalizeSamples;
			Vector3[] array = new Vector3[NormalizeSamples + 1];
			m_SamplePointTime = new Single[NormalizeSamples + 1];
			for (Int32 i = 0; i <= NormalizeSamples; i++)
			{
				Single p_curveTime = i * m_timeInc;
				array[i] = GetPointOnCurveTime(p_curveTime, false);
			}
			Single num = 0f;
			for (Int32 j = 1; j <= NormalizeSamples; j++)
			{
				num += Vector3.Distance(array[j - 1], array[j]);
			}
			m_SamplePointTime[0] = 0f;
			Single num2 = 0f;
			for (Int32 k = 1; k <= NormalizeSamples; k++)
			{
				num2 += Vector3.Distance(array[k - 1], array[k]) / num;
				m_SamplePointTime[k] = num2;
			}
			m_SamplePointTime[NormalizeSamples] = 1f;
			SampleData = new SplineSampleData[NormalizeSamples];
			Vector3 b = GetPointOnCurveTime(-m_timeInc);
			for (Int32 l = 0; l < SampleData.Length; l++)
			{
				Vector3 pointOnCurveTime = GetPointOnCurveTime(l * m_timeInc);
				Quaternion rot = Quaternion.LookRotation(pointOnCurveTime - b);
				b = pointOnCurveTime;
				Single speedOnCurveTime = GetSpeedOnCurveTime(l * m_timeInc);
				SampleData[l] = new SplineSampleData(pointOnCurveTime, rot, speedOnCurveTime);
			}
		}

		public Single GetSpeedOnCurveTime(Single p_curveTime)
		{
			return GetSpeedOnCurveTime(p_curveTime, true);
		}

		public SplineSampleData GetDataOnCurveTime(Single p_curveTime)
		{
			Int32 num = Mathf.Clamp(Mathf.FloorToInt(p_curveTime / m_timeInc), 0, NormalizeSamples - 1);
			Int32 num2 = Mathf.Clamp(Mathf.CeilToInt(p_curveTime / m_timeInc), 0, NormalizeSamples - 1);
			Single t = (p_curveTime - num * m_timeInc) / m_timeInc;
			return new SplineSampleData(Vector3.Lerp(SampleData[num].Position, SampleData[num2].Position, t), Quaternion.Lerp(SampleData[num].Rotation, SampleData[num2].Rotation, t), Mathf.Lerp(SampleData[num].Speed, SampleData[num2].Speed, t));
		}

		public Vector3 GetPointOnCurveTime(Single p_curveTime)
		{
			return GetPointOnCurveTime(p_curveTime, true);
		}

		private BezierSplineContolPoint GetControlPoint(Int32 p_index)
		{
			if (Closed)
			{
				Int32 i;
				for (i = p_index; i < 0; i += m_Contolpoints.Length)
				{
				}
				i %= m_Contolpoints.Length;
				return m_Contolpoints[i];
			}
			Int32 num = Mathf.Clamp(p_index, 0, m_Contolpoints.Length - 1);
			return m_Contolpoints[num];
		}

		private Int32 GetControlPointLength()
		{
			return (!Closed) ? m_Contolpoints.Length : (m_Contolpoints.Length + 1);
		}

		private Single GetSpeedOnCurveTime(Single p_curveTime, Boolean p_normalized)
		{
			if (!p_normalized)
			{
				if (p_curveTime > 1f)
				{
					p_curveTime = 1f;
				}
				Int32 num = Mathf.FloorToInt((m_Contolpoints.Length - 1) * p_curveTime);
				Int32 num2 = Mathf.Clamp(num + 1, 0, m_Contolpoints.Length - 1);
				Single num3 = 1f / (m_Contolpoints.Length - 1);
				Single num4 = num * num3;
				Single num5 = num4 + num3;
				Single t = (p_curveTime - num4) / (num5 - num4);
				return Mathf.Lerp(m_Contolpoints[num].speed, m_Contolpoints[num2].speed, t);
			}
			if (p_curveTime == 0f || p_curveTime == 1f)
			{
				return GetSpeedOnCurveTime(p_curveTime, false);
			}
			if (p_curveTime > 1f)
			{
				return GetSpeedOnCurveTime(p_curveTime + m_timeInc, false);
			}
			Int32 i = 0;
			while (i < m_SamplePointTime.Length)
			{
				if (m_SamplePointTime[i] >= p_curveTime)
				{
					if (m_SamplePointTime[i] == p_curveTime)
					{
						return GetSpeedOnCurveTime(i * m_timeInc, false);
					}
					Single num6 = 0f;
					if (i + 1 < m_SamplePointTime.Length)
					{
						num6 = (m_SamplePointTime[i] - p_curveTime) / (m_SamplePointTime[i + 1] - m_SamplePointTime[i]);
					}
					return GetSpeedOnCurveTime((i + num6) * m_timeInc, false);
				}
				else
				{
					i++;
				}
			}
			return 1f;
		}

		private Vector3 GetPointOnCurveTime(Single p_curveTime, Boolean p_normalized)
		{
			if (!p_normalized)
			{
				if (p_curveTime > 1f)
				{
					p_curveTime = 1f;
				}
				Vector3 a = Vector3.zero;
				Int32 controlPointLength = GetControlPointLength();
				Int32 num = Mathf.FloorToInt((controlPointLength - 1) * p_curveTime);
				Int32 p_index = Mathf.Clamp(num + 1, 0, controlPointLength - 1);
				Single num2 = 1f / (controlPointLength - 1);
				Single num3 = num * num2;
				Single num4 = num3 + num2;
				BezierSplineContolPoint controlPoint = GetControlPoint(num);
				BezierSplineContolPoint controlPoint2 = GetControlPoint(p_index);
				Vector3[] array = new Vector3[]
				{
					controlPoint.GlobalPosition,
					controlPoint.GlobalPosition + controlPoint.RightSubControlGlobalPosition * controlPoint.weight,
					controlPoint2.GlobalPosition + controlPoint2.LeftSubControlGlobalPosition * controlPoint2.weight,
					controlPoint2.GlobalPosition
				};
				Single num5 = (p_curveTime - num3) / (num4 - num3);
				Single num6 = 0f;
				for (Int32 i = 0; i < array.Length; i++)
				{
					Single num7 = m_BinominalArray[array.Length - 1][i] * Mathf.Pow(num5, i) * Mathf.Pow(1f - num5, array.Length - 1 - i);
					a += array[i] * num7;
					num6 += num7;
				}
				return a / num6;
			}
			if (m_SamplePointTime == null || m_SamplePointTime.Length == 0)
			{
				Initialize();
			}
			if (p_curveTime == 0f || p_curveTime == 1f)
			{
				return GetPointOnCurveTime(p_curveTime, false);
			}
			if (p_curveTime > 1f)
			{
				return GetPointOnCurveTime(p_curveTime + m_timeInc, false);
			}
			Int32 j = 0;
			while (j < m_SamplePointTime.Length)
			{
				if (m_SamplePointTime[j] >= p_curveTime)
				{
					if (m_SamplePointTime[j] == p_curveTime)
					{
						return GetPointOnCurveTime(j * m_timeInc, false);
					}
					Single num8 = 0f;
					if (j + 1 < m_SamplePointTime.Length)
					{
						num8 = (p_curveTime - m_SamplePointTime[j]) / (m_SamplePointTime[j + 1] - m_SamplePointTime[j]);
					}
					return GetPointOnCurveTime((j + num8) * m_timeInc, false);
				}
				else
				{
					j++;
				}
			}
			return Vector3.zero;
		}

		private Single GetClosedPointOnCurve(Vector3 p_refPoint, Single p_samplingValue)
		{
			Single num = 0f;
			Single num2 = Single.MaxValue;
			Single result = 0f;
			while (num <= 1f)
			{
				Single num3 = Vector3.Distance(p_refPoint, GetPointOnCurveTime(num));
				if (num3 < num2)
				{
					num2 = num3;
					result = num;
				}
				num += p_samplingValue;
			}
			return result;
		}

		public struct SplineSampleData
		{
			public Vector3 Position;

			public Quaternion Rotation;

			public Single Speed;

			public SplineSampleData(Vector3 pos, Quaternion rot, Single speed)
			{
				Position = pos;
				Rotation = rot;
				Speed = speed;
			}
		}
	}
}
