using System;
using UnityEngine;

public class GlobalFogC : PostEffectsBaseC
{
	public FogMode fogMode;

	private Single CAMERA_NEAR = 0.5f;

	private Single CAMERA_FAR = 50f;

	private Single CAMERA_FOV = 60f;

	private Single CAMERA_ASPECT_RATIO = 1.333333f;

	public Single startDistance = 200f;

	public Single globalDensity = 1f;

	public Single heightScale = 100f;

	public Single height;

	public Color globalFogColor = Color.grey;

	public Shader fogShader;

	private Material fogMaterial;

	private void Awake()
	{
		fogShader = Shader.Find("Hidden/GlobalFog");
	}

	private void OnDisable()
	{
		if (fogMaterial)
		{
			DestroyImmediate(fogMaterial);
		}
	}

	protected override Boolean CheckResources()
	{
		CheckSupport();
		fogMaterial = CheckShaderAndCreateMaterial(fogShader, fogMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		CAMERA_NEAR = camera.nearClipPlane;
		CAMERA_FAR = camera.farClipPlane;
		CAMERA_FOV = camera.fieldOfView;
		CAMERA_ASPECT_RATIO = camera.aspect;
		Matrix4x4 identity = Matrix4x4.identity;
		Single num = CAMERA_FOV * 0.5f;
		Vector3 b = camera.transform.right * CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f) * CAMERA_ASPECT_RATIO;
		Vector3 b2 = camera.transform.up * CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f);
		Vector3 vector = camera.transform.forward * CAMERA_NEAR - b + b2;
		Single num2 = vector.magnitude * CAMERA_FAR / CAMERA_NEAR;
		vector.Normalize();
		vector *= num2;
		Vector3 vector2 = camera.transform.forward * CAMERA_NEAR + b + b2;
		vector2.Normalize();
		vector2 *= num2;
		Vector3 vector3 = camera.transform.forward * CAMERA_NEAR + b - b2;
		vector3.Normalize();
		vector3 *= num2;
		Vector3 vector4 = camera.transform.forward * CAMERA_NEAR - b - b2;
		vector4.Normalize();
		vector4 *= num2;
		identity.SetRow(0, vector);
		identity.SetRow(1, vector2);
		identity.SetRow(2, vector3);
		identity.SetRow(3, vector4);
		fogMaterial.SetMatrix("_FrustumCornersWS", identity);
		fogMaterial.SetVector("_CameraWS", camera.transform.position);
		fogMaterial.SetVector("_StartDistance", new Vector4(1f / startDistance, num2 - startDistance));
		fogMaterial.SetVector("_Y", new Vector4(height, 1f / heightScale));
		fogMaterial.SetFloat("_GlobalDensity", globalDensity * 0.01f);
		fogMaterial.SetColor("_FogColor", globalFogColor);
		CustomGraphicsBlit(source, destination, fogMaterial, (Int32)fogMode);
	}

	private static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, Int32 passNr)
	{
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(passNr);
		GL.Begin(7);
		GL.MultiTexCoord2(0, 0f, 0f);
		GL.Vertex3(0f, 0f, 3f);
		GL.MultiTexCoord2(0, 1f, 0f);
		GL.Vertex3(1f, 0f, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, 0f, 1f);
		GL.Vertex3(0f, 1f, 0f);
		GL.End();
		GL.PopMatrix();
	}

	public enum FogMode
	{
		AbsoluteYAndDistance,
		AbsoluteY,
		Distance,
		RelativeYAndDistance
	}
}
