using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public class SkyLine : MonoBehaviour
	{
		public Vector3 WorldCenter = default(Vector3);

		public Vector3 DirectionOffsetScale = default(Vector3);

		public AnimationCurve TimeOfDayLerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public Transform CameraPosition;

		public Sky SkyScript;

		private Material mSkylineMaterial;

		protected void Start()
		{
			mSkylineMaterial = GetComponent<Renderer>().material;
			if (SkyScript == null && mSkylineMaterial == null && CameraPosition == null)
			{
				enabled = false;
			}
		}

		protected void Update()
		{
			if (CameraPosition != null)
			{
				Vector3 vector = CameraPosition.position - WorldCenter;
				vector.Scale(DirectionOffsetScale);
				Vector4 vector2 = new Vector4(vector.x, vector.y, vector.z, vector.z);
				mSkylineMaterial.SetVector("_PositionOffset", vector2);
			}
			if (SkyScript != null && mSkylineMaterial != null)
			{
				Single value = TimeOfDayLerpCurve.Evaluate(SkyScript.Cycle.TimeOfDay / 24f);
				mSkylineMaterial.SetFloat("_CubeMapFade", value);
			}
		}
	}
}
