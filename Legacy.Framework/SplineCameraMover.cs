using System;
using UnityEngine;

public class SplineCameraMover : SplineMover
{
	public SplineController LookSpline;

	public CameraDriveMode DriveMode;

	public AnimationCurve LookAtSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public Single LookAtSpeed = 1f;

	public RunMode LookAtMoveMode;

	public Boolean ChangeFOV;

	public AnimationCurve LookAtFOVCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public Single mMinFOV = 60f;

	public Single mMaxFOV = 60f;

	private SplineTimer mLookTimer;

	public Single Timer
	{
		get => PositionTimer.Time;
	    set
		{
			PositionTimer.Time = value;
			if (PositionSpline != null && LookSpline != null)
			{
				transform.position = PositionSpline.ResolvePositionOnSplineTime(PositionTimer.Time);
				transform.LookAt(LookSpline.ResolvePositionOnSplineTime(PositionTimer.Time));
				if (camera && ChangeFOV)
				{
					camera.fieldOfView = Mathf.Lerp(mMinFOV, mMaxFOV, LookAtFOVCurve.Evaluate(PositionTimer.Time));
				}
			}
		}
	}

	public override void Start()
	{
		base.Start();
		mLookTimer = new SplineTimer(LookAtMoveMode);
	}

	public override void Update()
	{
		if (DriveMode == CameraDriveMode.Position || DriveMode == CameraDriveMode.Independed)
		{
			base.Update();
		}
		if (LookSpline == null)
		{
			return;
		}
		Single p_time = LookAtSpeedCurve.Evaluate(mLookTimer.Time);
		if (DriveMode == CameraDriveMode.LookAt || DriveMode == CameraDriveMode.Independed)
		{
			mLookTimer.Update();
			Single num = LookSpline.ResolveSpeedOnSplineTime(p_time);
			mLookTimer.ChangeTime(Mathf.Min(Time.deltaTime * num, LookSpline.GetMaxTimeStep()) * LookAtSpeed);
		}
		else
		{
			mLookTimer.Time = PositionTimer.Time;
		}
		if (DriveMode == CameraDriveMode.LookAt)
		{
			PositionTimer.Time = mLookTimer.Time;
			PositionUpdate();
		}
		transform.LookAt(LookSpline.ResolvePositionOnSplineTime(p_time));
		if (ChangeFOV && camera)
		{
			camera.fieldOfView = Mathf.Lerp(mMinFOV, mMaxFOV, LookAtFOVCurve.Evaluate(mLookTimer.Time));
		}
	}

	public enum CameraDriveMode
	{
		Position,
		LookAt,
		Independed
	}
}
