using System;
using UnityEngine;

public class SplineController : MonoBehaviour
{
	private Single[] m_SamplePointTime;

	private SplineSampleData[] m_SampleData;

	[SerializeField]
	private SplineSegment[] m_SplineSegments;

	public InterpolationMode m_SplineInterpolation;

	public Int32 SampleCount = 1000;

	public Single NormalizedSplineTime = 1f;

	public SplineSegment[] SplineSegments
	{
		get
		{
			if (m_SplineSegments == null)
			{
				m_SplineSegments = GetComponentsInChildren<SplineSegment>();
			}
			return m_SplineSegments;
		}
		set => m_SplineSegments = value;
	}

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		InterpolationMode splineInterpolation = m_SplineInterpolation;
		if (splineInterpolation != InterpolationMode.Native_Sampled)
		{
			if (splineInterpolation == InterpolationMode.Normalized_Sampled)
			{
				InitializeNormalizeSamples();
				InitializeSampleData();
			}
		}
		else
		{
			InitializeSampleData();
		}
	}

	public SplineSampleData ResolveDataOnSplineTime(Single p_time)
	{
		if (SplineSegments.Length == 0)
		{
			return default(SplineSampleData);
		}
		if (m_SplineInterpolation == InterpolationMode.Native)
		{
			Vector3 vector = ResolvePositionOnSplineTime(p_time);
			Quaternion rot = Quaternion.identity;
			Vector3 vector2;
			if (p_time > 0f)
			{
				vector2 = ResolvePositionOnSplineTime(p_time - 0.01f);
				vector2 = vector - vector2;
			}
			else
			{
				vector2 = ResolvePositionOnSplineTime(p_time + 0.01f);
				vector2 -= vector;
			}
			if (vector2 != Vector3.zero)
			{
				rot = Quaternion.LookRotation(vector2);
			}
			return new SplineSampleData(vector, rot, ResolveSpeedOnSplineTime(p_time));
		}
		if (m_SampleData == null || m_SampleData.Length == 0 || m_SampleData.Length != SampleCount)
		{
			Initialize();
		}
		Single num = 1f / (SampleCount - 1);
		Int32 num2 = Mathf.Clamp(Mathf.FloorToInt(p_time / num), 0, SampleCount - 1);
		Int32 num3 = Mathf.Clamp(Mathf.CeilToInt(p_time / num), 0, SampleCount - 1);
		Single t = Mathf.Clamp01((p_time - num2 * num) / num);
		return new SplineSampleData(Vector3.Lerp(m_SampleData[num2].Position, m_SampleData[num3].Position, t), Quaternion.Lerp(m_SampleData[num2].Rotation, m_SampleData[num3].Rotation, t), Mathf.Lerp(m_SampleData[num2].Speed, m_SampleData[num3].Speed, t));
	}

	public Vector3 ResolvePositionOnSplineTime(Single p_time)
	{
		if (SplineSegments.Length == 0)
		{
			return default(Vector3);
		}
		switch (m_SplineInterpolation)
		{
		case InterpolationMode.Normalized_Sampled:
			return InternalResolvePositionOnNormalizedSplineTime(p_time);
		}
		return InternalResolvePositionOnSplineTime(p_time);
	}

	public Single ResolveSpeedOnSplineTime(Single p_time)
	{
		switch (m_SplineInterpolation)
		{
		case InterpolationMode.Normalized_Sampled:
			return 1f / NormalizedSplineTime;
		}
		if (SplineSegments.Length == 0)
		{
			return 0f;
		}
		return SplineSegments[ResolveSegmentFromTime(p_time)].GetSpeedOnSpline(NormalizeTimeOnSegment(p_time)) / SplineSegments.Length;
	}

	public Single GetMaxTimeStep()
	{
		return 1f / SplineSegments.Length;
	}

	private Vector3 InternalResolvePositionOnSplineTime(Single p_time)
	{
		return SplineSegments[ResolveSegmentFromTime(p_time)].GetPointOnSpline(NormalizeTimeOnSegment(p_time));
	}

	private Vector3 InternalResolvePositionOnNormalizedSplineTime(Single p_time)
	{
		Single p_time2 = ResolveNormalizedTime(p_time);
		return SplineSegments[ResolveSegmentFromTime(p_time2)].GetPointOnSpline(NormalizeTimeOnSegment(p_time2));
	}

	private Single ResolveNormalizedTime(Single p_time)
	{
		Single num = 1f / SampleCount;
		if (p_time == 0f || p_time == 1f)
		{
			return p_time;
		}
		if (p_time > 1f)
		{
			return 1f;
		}
		Int32 i = 0;
		while (i < m_SamplePointTime.Length)
		{
			if (m_SamplePointTime[i] >= p_time)
			{
				if (m_SamplePointTime[i] == p_time)
				{
					return i * num;
				}
				Single num2 = (m_SamplePointTime[i] - p_time) / (m_SamplePointTime[i] - m_SamplePointTime[i - 1]);
				return (i - num2) * num;
			}
			else
			{
				i++;
			}
		}
		return 0f;
	}

	private Single NormalizeTimeOnSegment(Single p_time)
	{
		Single num = 1f / SplineSegments.Length;
		return p_time % num * SplineSegments.Length;
	}

	private Int32 ResolveSegmentFromTime(Single p_time)
	{
		Int32 num = Mathf.FloorToInt(p_time * SplineSegments.Length) % SplineSegments.Length;
		num = Mathf.Clamp(num, 0, SplineSegments.Length - 1);
		if (p_time == 1f)
		{
			num = SplineSegments.Length - 1;
		}
		return num;
	}

	private void InitializeNormalizeSamples()
	{
		Single num = 1f / SampleCount;
		Vector3[] array = new Vector3[SampleCount + 1];
		m_SamplePointTime = new Single[SampleCount + 1];
		for (Int32 i = 0; i <= SampleCount; i++)
		{
			Single p_time = i * num;
			array[i] = InternalResolvePositionOnSplineTime(p_time);
		}
		Single num2 = 0f;
		for (Int32 j = 1; j <= SampleCount; j++)
		{
			num2 += Mathf.Abs(Vector3.Distance(array[j - 1], array[j]));
		}
		m_SamplePointTime[0] = 0f;
		Single num3 = 0f;
		for (Int32 k = 1; k <= SampleCount; k++)
		{
			num3 += Mathf.Abs(Vector3.Distance(array[k - 1], array[k])) / num2;
			m_SamplePointTime[k] = num3;
		}
		m_SamplePointTime[SampleCount] = 1f;
	}

	private void InitializeSampleData()
	{
		Single num = 1f / (SampleCount - 1);
		m_SampleData = new SplineSampleData[SampleCount];
		Vector3 vector = ResolvePositionOnSplineTime(0f);
		Vector3 a = ResolvePositionOnSplineTime(num);
		m_SampleData[0] = new SplineSampleData(vector, Quaternion.LookRotation(a - vector), ResolveSpeedOnSplineTime(0f));
		for (Int32 i = 1; i < SampleCount; i++)
		{
			Single p_time = i * num;
			Vector3 vector2 = ResolvePositionOnSplineTime(p_time);
			Quaternion rot = Quaternion.LookRotation(vector2 - vector);
			vector = vector2;
			Single speed = ResolveSpeedOnSplineTime(p_time);
			m_SampleData[i] = new SplineSampleData(vector2, rot, speed);
		}
	}

	public enum SplineType
	{
		Linear,
		Bezier,
		Catmull_Rom,
		Cardinal
	}

	public enum InterpolationMode
	{
		Native,
		Native_Sampled,
		Normalized_Sampled
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
