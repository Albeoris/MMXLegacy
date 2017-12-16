using System;
using UnityEngine;

namespace Legacy
{
	public class SkyBoxRotator : MonoBehaviour
	{
		public Vector3 SkyBoxRotation = Vector3.zero;

		private void Start()
		{
			Quaternion q = Quaternion.Euler(SkyBoxRotation);
			Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
			RenderSettings.skybox.SetMatrix("_Rotation", matrix);
			enabled = false;
		}
	}
}
