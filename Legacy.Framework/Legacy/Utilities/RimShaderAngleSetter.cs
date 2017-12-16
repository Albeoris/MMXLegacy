using System;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/Rim Shader Angle Setter")]
	[ExecuteInEditMode]
	public class RimShaderAngleSetter : MonoBehaviour
	{
		public Single KeyAngle = 30f;

		public Single LightProbeAngle = 45f;

		private void Awake()
		{
			SetShaderProperties();
		}

		private void SetShaderProperties()
		{
			Single f = LightProbeAngle / 180f * 3.14f;
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetRow(0, new Vector4(Mathf.Cos(f), 0f, Mathf.Sin(f), 0f));
			matrix.SetRow(1, new Vector4(0f, 1f, 0f, 0f));
			matrix.SetRow(2, new Vector4(-Mathf.Sin(f), 0f, Mathf.Cos(f), 0f));
			matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			Single f2 = KeyAngle / 180f * 3.14f;
			Matrix4x4 matrix2 = default(Matrix4x4);
			matrix2.SetRow(0, new Vector4(Mathf.Cos(f2), 0f, Mathf.Sin(f2), 0f));
			matrix2.SetRow(1, new Vector4(0f, 1f, 0f, 0f));
			matrix2.SetRow(2, new Vector4(-Mathf.Sin(f2), 0f, Mathf.Cos(f2), 0f));
			matrix2.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
			{
				foreach (Material material in renderer.materials)
				{
					if (material.HasProperty("_LightProbeRotMatrix"))
					{
						material.SetMatrix("_LightProbeRotMatrix", matrix);
					}
					if (material.HasProperty("_KeyLightRotMatrix"))
					{
						material.SetMatrix("_KeyLightRotMatrix", matrix2);
					}
				}
			}
		}
	}
}
