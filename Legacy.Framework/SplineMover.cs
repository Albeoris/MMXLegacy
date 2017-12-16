using System;
using UnityEngine;

public class SplineMover : MonoBehaviour
{
	public RunMode MoveMode;

	public SplineController PositionSpline;

	public Single Speed = 1f;

	public AnimationCurve SpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private Transform m_CachedTransform;

	private SplineTimer mPositionTimer;

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

	public SplineTimer PositionTimer
	{
		get
		{
			if (mPositionTimer == null)
			{
				mPositionTimer = new SplineTimer(MoveMode);
			}
			return mPositionTimer;
		}
	}

	public virtual void Awake()
	{
		transform = base.transform;
	}

	public virtual void Start()
	{
		mPositionTimer = new SplineTimer(MoveMode);
	}

	public virtual void Update()
	{
		if (PositionSpline == null)
		{
			return;
		}
		mPositionTimer.Update();
		PositionUpdate();
	}

	protected void PositionUpdate()
	{
		if (PositionSpline == null)
		{
			return;
		}
		Single p_time = SpeedCurve.Evaluate(mPositionTimer.Time);
		SplineController.SplineSampleData splineSampleData = PositionSpline.ResolveDataOnSplineTime(p_time);
		mPositionTimer.ChangeTime(Mathf.Min(Time.deltaTime * splineSampleData.Speed, PositionSpline.GetMaxTimeStep()) * Speed);
		transform.position = splineSampleData.Position;
		transform.rotation = splineSampleData.Rotation;
	}

	public enum RunMode
	{
		Forward_Clamp,
		Backward_Clamp,
		Forward_Loop,
		Backward_Loop,
		Forward_PingPong,
		Backward_PingPong
	}

	public class SplineTimer
	{
		public Boolean mForward;

		public Single mTime;

		public RunMode mMoveMode;

		public SplineTimer(RunMode Mode)
		{
			mMoveMode = Mode;
			switch (mMoveMode)
			{
			case RunMode.Forward_Clamp:
			case RunMode.Forward_Loop:
			case RunMode.Forward_PingPong:
				mForward = true;
				mTime = 0f;
				break;
			case RunMode.Backward_Clamp:
			case RunMode.Backward_Loop:
			case RunMode.Backward_PingPong:
				mForward = false;
				mTime = 1f;
				break;
			}
		}

		public Single Time
		{
			get => mTime;
		    set => mTime = value;
		}

		public void ChangeTime(Single p_timeChange)
		{
			mTime += p_timeChange * ((!mForward) ? -1 : 1);
		}

		public void Update()
		{
			if (mTime >= 1f)
			{
				switch (mMoveMode)
				{
				case RunMode.Forward_Clamp:
					mTime = 1f;
					break;
				case RunMode.Forward_Loop:
					mTime = 0f;
					break;
				case RunMode.Forward_PingPong:
				case RunMode.Backward_PingPong:
					mForward = false;
					break;
				}
			}
			else if (mTime <= 0f)
			{
				switch (mMoveMode)
				{
				case RunMode.Backward_Clamp:
					mTime = 0f;
					break;
				case RunMode.Backward_Loop:
					mTime = 1f;
					break;
				case RunMode.Forward_PingPong:
				case RunMode.Backward_PingPong:
					mForward = true;
					break;
				}
			}
		}
	}
}
