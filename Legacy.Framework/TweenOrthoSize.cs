using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Tween/Orthographic Size")]
public class TweenOrthoSize : UITweener
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

	public Single orthoSize
	{
		get => cachedCamera.orthographicSize;
	    set => cachedCamera.orthographicSize = value;
	}

	protected override void OnUpdate(Single factor, Boolean isFinished)
	{
		cachedCamera.orthographicSize = from * (1f - factor) + to * factor;
	}

	public static TweenOrthoSize Begin(GameObject go, Single duration, Single to)
	{
		TweenOrthoSize tweenOrthoSize = UITweener.Begin<TweenOrthoSize>(go, duration);
		tweenOrthoSize.from = tweenOrthoSize.orthoSize;
		tweenOrthoSize.to = to;
		if (duration <= 0f)
		{
			tweenOrthoSize.Sample(1f, true);
			tweenOrthoSize.enabled = false;
		}
		return tweenOrthoSize;
	}
}
