using System;
using UnityEngine;

public class AutoRotator : MonoBehaviour
{
	public Vector3 RotationValues;

	public Single Speed = 1f;

	private Single mTimer;

	private Vector3 mStartPos;

	private void Start()
	{
		mStartPos = transform.localEulerAngles;
	}

	private void Update()
	{
		if (mTimer < 1f)
		{
			mTimer += Time.deltaTime * Speed;
			transform.localEulerAngles = Vector3.Lerp(mStartPos, mStartPos + RotationValues, mTimer);
			if (mTimer >= 1f)
			{
				mTimer = 0f;
			}
		}
	}
}
