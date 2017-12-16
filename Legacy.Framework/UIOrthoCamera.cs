using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Orthographic Camera")]
[RequireComponent(typeof(Camera))]
public class UIOrthoCamera : MonoBehaviour
{
	private Camera mCam;

	private Transform mTrans;

	private void Start()
	{
		mCam = camera;
		mTrans = transform;
		mCam.orthographic = true;
	}

	private void Update()
	{
		Single num = mCam.rect.yMin * Screen.height;
		Single num2 = mCam.rect.yMax * Screen.height;
		Single num3 = (num2 - num) * 0.5f * mTrans.lossyScale.y;
		if (!Mathf.Approximately(mCam.orthographicSize, num3))
		{
			mCam.orthographicSize = num3;
		}
	}
}
