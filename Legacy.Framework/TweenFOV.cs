using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Tween/Field of View")]
public class TweenFOV : UITweener
{
	public Single from;

	public Single to;

	private Camera mCam;

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = camera;
			}
			return mCam;
		}
	}

	public Single fov
	{
		get => cachedCamera.fieldOfView;
	    set => cachedCamera.fieldOfView = value;
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		cachedCamera.fieldOfView = from * (1f - factor) + to * factor;
	}

	public static TweenFOV Begin(GameObject go, Single duration, Single to)
	{
		TweenFOV tweenFOV = UITweener.Begin<TweenFOV>(go, duration);
		tweenFOV.from = tweenFOV.fov;
		tweenFOV.to = to;
		if (duration <= 0f)
		{
			tweenFOV.Sample(1f, true);
			tweenFOV.enabled = false;
		}
		return tweenFOV;
	}
}
