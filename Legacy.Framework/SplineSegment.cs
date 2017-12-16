using System;
using UnityEngine;

public class SplineSegment : MonoBehaviour
{
	private static Int32[] s_BinominalArray = new Int32[]
	{
		1,
		3,
		3,
		1
	};

	private static Vector3[] s_LocalPoints = new Vector3[4];

	private SplineController m_ParentController;

	[SerializeField]
	private Vector3 m_TangentIn = Vector3.right;

	[SerializeField]
	private Vector3 m_TangentOut = -Vector3.right;

	[SerializeField]
	private AnimationCurve m_SpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private SplineSegment mTargetSegment;

	[SerializeField]
	private SplineSegment mPrevSegment;

	public Single SegmentTime = 1f;

	public Boolean SharedTangents;

	public Single SpeedAdjustmentTime = 0.1f;

	public Single CurveTension;

	public SplineController.SplineType SplineType;

	private Transform m_CachedTransform;

	public SplineController ParentController
	{
		get
		{
			if (m_ParentController == null)
			{
				m_ParentController = transform.parent.GetComponent<SplineController>();
			}
			return m_ParentController;
		}
	}

	public SplineSegment TargetSegment
	{
		get => mTargetSegment;
	    set
		{
			mTargetSegment = value;
			if (value != null && mTargetSegment.PrevSegment != this)
			{
				mTargetSegment.PrevSegment = this;
			}
		}
	}

	public SplineSegment PrevSegment
	{
		get => mPrevSegment;
	    set
		{
			mPrevSegment = value;
			if (value != null && mPrevSegment.TargetSegment != this)
			{
				mPrevSegment.TargetSegment = this;
			}
		}
	}

	public Vector3 TangentIn
	{
		get => transform.localToWorldMatrix.MultiplyVector(m_TangentIn);
	    set
		{
			m_TangentIn = transform.worldToLocalMatrix.MultiplyVector(value);
			if (SharedTangents)
			{
				m_TangentOut = -m_TangentIn.normalized * m_TangentOut.magnitude;
			}
		}
	}

	public Vector3 TangentOut
	{
		get => transform.localToWorldMatrix.MultiplyVector(m_TangentOut);
	    set
		{
			m_TangentOut = transform.worldToLocalMatrix.MultiplyVector(value);
			if (SharedTangents)
			{
				m_TangentIn = -m_TangentOut.normalized * m_TangentIn.magnitude;
			}
		}
	}

	public AnimationCurve SpeedCurve
	{
		get
		{
			if (m_SpeedCurve == null)
			{
				m_SpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			}
			return m_SpeedCurve;
		}
		set => m_SpeedCurve = value;
	}

	public new Transform transform
	{
		get
		{
			if (m_CachedTransform == null)
			{
				m_CachedTransform = base.transform;
			}
			return m_CachedTransform;
		}
		private set => m_CachedTransform = value;
	}

	public Vector3 GetPointOnSpline(Single p_curveTime)
	{
		return GetPointOnSpline(SpeedCurve.Evaluate(p_curveTime), SplineType);
	}

	public Vector3 GetPointOnSpline(Single p_curveTime, SplineController.SplineType p_splineType)
	{
		if (!(TargetSegment != null))
		{
			return transform.position;
		}
		switch (p_splineType)
		{
		case SplineController.SplineType.Linear:
			return GetPointOnLinearSpline(p_curveTime);
		case SplineController.SplineType.Bezier:
			return GetPointOnBezierSpline(p_curveTime);
		case SplineController.SplineType.Catmull_Rom:
			return GetPointOnCatmullRomSpline(p_curveTime);
		case SplineController.SplineType.Cardinal:
			return GetPointOnCardinalSpline(p_curveTime);
		default:
			return transform.position;
		}
	}

	public Single GetSpeedOnSpline(Single p_curveTime)
	{
		if (SegmentTime <= 0f)
		{
			return Single.MaxValue;
		}
		if (p_curveTime >= 1f - SpeedAdjustmentTime && TargetSegment != null)
		{
			return 1f / Mathf.Lerp(SegmentTime, TargetSegment.SegmentTime, (p_curveTime - (1f - SpeedAdjustmentTime)) / SpeedAdjustmentTime);
		}
		return 1f / SegmentTime;
	}

	private void Awake()
	{
		transform = base.transform;
	}

	private Vector3 GetPointOnCatmullRomSpline(Single p_curveTime)
	{
		return GetPointOnCardinalOrCatmullRomSpline(p_curveTime, 0f);
	}

	private Vector3 GetPointOnCardinalSpline(Single p_curveTime)
	{
		return GetPointOnCardinalOrCatmullRomSpline(p_curveTime, CurveTension);
	}

	private Vector3 GetPointOnCardinalOrCatmullRomSpline(Single p_curveTime, Single p_tension)
	{
		p_curveTime = Mathf.Clamp01(p_curveTime);
		Vector3 a = Vector3.zero;
		s_LocalPoints[0] = transform.position;
		if (PrevSegment != null)
		{
			s_LocalPoints[1] = transform.position + (1f - p_tension) * 0.5f * (TargetSegment.transform.position - PrevSegment.transform.position);
		}
		else
		{
			s_LocalPoints[1] = transform.position;
		}
		if (TargetSegment.TargetSegment == null)
		{
			s_LocalPoints[2] = TargetSegment.transform.position;
		}
		else
		{
			s_LocalPoints[2] = TargetSegment.transform.position - (1f - p_tension) * 0.5f * (TargetSegment.TargetSegment.transform.position - transform.position);
		}
		s_LocalPoints[3] = TargetSegment.transform.position;
		Single num = 0f;
		for (Int32 i = 0; i < s_LocalPoints.Length; i++)
		{
			Single num2 = s_BinominalArray[i] * Mathf.Pow(p_curveTime, i) * Mathf.Pow(1f - p_curveTime, 3 - i);
			a += s_LocalPoints[i] * num2;
			num += num2;
		}
		return a / num;
	}

	private Vector3 GetPointOnBezierSpline(Single p_curveTime)
	{
		p_curveTime = Mathf.Clamp01(p_curveTime);
		Vector3 a = Vector3.zero;
		s_LocalPoints[0] = transform.position;
		s_LocalPoints[1] = transform.position + TangentOut;
		s_LocalPoints[2] = TargetSegment.transform.position + TargetSegment.TangentIn;
		s_LocalPoints[3] = TargetSegment.transform.position;
		Single num = 0f;
		for (Int32 i = 0; i < s_LocalPoints.Length; i++)
		{
			Single num2 = s_BinominalArray[i] * Mathf.Pow(p_curveTime, i) * Mathf.Pow(1f - p_curveTime, 3 - i);
			a += s_LocalPoints[i] * num2;
			num += num2;
		}
		return a / num;
	}

	private Vector3 GetPointOnLinearSpline(Single p_curveTime)
	{
		p_curveTime = Mathf.Clamp01(p_curveTime);
		return Vector3.Lerp(transform.position, TargetSegment.transform.position, p_curveTime);
	}
}
