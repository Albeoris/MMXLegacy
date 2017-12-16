using System;
using UnityEngine;

internal class CameraLookAt : MonoBehaviour
{
	private void LateUpdate()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			transform.LookAt(main.transform);
		}
	}
}
