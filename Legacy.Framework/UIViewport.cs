using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Viewport Camera")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class UIViewport : MonoBehaviour
{
	public Camera sourceCamera;

	public Transform topLeft;

	public Transform bottomRight;

	public Single fullSize = 1f;

	private Camera mCam;

	private void Start()
	{
		mCam = camera;
		if (sourceCamera == null)
		{
			sourceCamera = Camera.main;
		}
	}

	private void LateUpdate()
	{
		if (topLeft != null && bottomRight != null)
		{
			Vector3 vector = sourceCamera.WorldToScreenPoint(topLeft.position);
			Vector3 vector2 = sourceCamera.WorldToScreenPoint(bottomRight.position);
			Rect rect = new Rect(vector.x / Screen.width, vector2.y / Screen.height, (vector2.x - vector.x) / Screen.width, (vector.y - vector2.y) / Screen.height);
			Single num = fullSize * rect.height;
			if (rect != mCam.rect)
			{
				mCam.rect = rect;
			}
			if (mCam.orthographicSize != num)
			{
				mCam.orthographicSize = num;
			}
		}
	}
}
