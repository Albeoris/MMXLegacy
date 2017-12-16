using System;
using UnityEngine;

namespace Legacy.TimeOfDay
{
	public class SkyAnimation : MonoBehaviour
	{
		public Single WindDegrees;

		public Single WindSpeed = 3f;

		internal Vector4 CloudUV;

		protected void Update()
		{
			Vector2 vector = new Vector2(Mathf.Cos(0.0174532924f * (WindDegrees + 15f)), Mathf.Sin(0.0174532924f * (WindDegrees + 15f)));
			Vector2 vector2 = new Vector2(Mathf.Cos(0.0174532924f * (WindDegrees - 15f)), Mathf.Sin(0.0174532924f * (WindDegrees - 15f)));
			Vector4 a = WindSpeed / 100f * new Vector4(vector.x, vector.y, vector2.x, vector2.y);
			CloudUV += Time.deltaTime * a;
		}
	}
}
