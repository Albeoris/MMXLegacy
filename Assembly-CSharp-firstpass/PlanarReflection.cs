using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(WaterBase))]
public class PlanarReflection : MonoBehaviour
{
	public LayerMask reflectionMask;

	public Boolean reflectSkybox;

	public Color clearColor = Color.grey;

	public String reflectionSampler = "_ReflectionTex";

	public Single clipPlaneOffset = 0.07f;

	private Vector3 oldpos = Vector3.zero;

	private Camera reflectionCamera;

	private Material sharedMaterial;

	private Dictionary<Camera, Boolean> helperCameras;

	private List<RenderTexture> renderTexture = new List<RenderTexture>();

	public void Start()
	{
		sharedMaterial = ((WaterBase)GetComponent(typeof(WaterBase))).sharedMaterial;
		if (reflectionCamera != null)
		{
			SetStandardCameraParameter(reflectionCamera, reflectionMask);
		}
	}

	private Camera CreateReflectionCameraFor(Camera cam)
	{
		String name = this.name + "_Reflection_" + cam.name;
		GameObject gameObject = GameObject.Find(name);
		if (!gameObject)
		{
			gameObject = new GameObject(name);
		}
		Camera camera = gameObject.camera;
		if (camera == null)
		{
			camera = (Camera)gameObject.AddComponent(typeof(Camera));
		}
		camera.eventMask = 0;
		camera.useOcclusionCulling = false;
		camera.backgroundColor = clearColor;
		camera.clearFlags = ((!reflectSkybox) ? CameraClearFlags.Color : CameraClearFlags.Skybox);
		SetStandardCameraParameter(camera, reflectionMask);
		return camera;
	}

	private void SetStandardCameraParameter(Camera cam, LayerMask mask)
	{
		Int32 num = 1 << LayerMask.NameToLayer("GUI") | 1 << LayerMask.NameToLayer("Indoor") | 1 << LayerMask.NameToLayer("Redraw_DONT_USE") | 1 << LayerMask.NameToLayer("Water");
		cam.cullingMask = (mask & ~num);
		cam.backgroundColor = Color.black;
		cam.enabled = false;
	}

	private RenderTexture CreateTextureFor(Camera cam)
	{
		RenderTexture renderTexture = new RenderTexture(Mathf.FloorToInt(cam.pixelWidth * 0.5f), Mathf.FloorToInt(cam.pixelHeight * 0.5f), 16);
		renderTexture.name = "PlanarReflectionTexture";
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.Create();
		this.renderTexture.Add(renderTexture);
		return renderTexture;
	}

	public void RenderHelpCameras(Camera currentCam)
	{
		if (helperCameras == null)
		{
			helperCameras = new Dictionary<Camera, Boolean>();
		}
		Boolean flag;
		if (!helperCameras.TryGetValue(currentCam, out flag))
		{
			helperCameras.Add(currentCam, false);
		}
		if (flag)
		{
			return;
		}
		helperCameras[currentCam] = true;
		if (reflectionCamera == null)
		{
			reflectionCamera = CreateReflectionCameraFor(currentCam);
		}
		RenderTexture targetTexture = reflectionCamera.targetTexture;
		if (targetTexture == null)
		{
			reflectionCamera.targetTexture = CreateTextureFor(currentCam);
		}
		else if (targetTexture.width != Mathf.FloorToInt(currentCam.pixelWidth * 0.5f) || targetTexture.height != Mathf.FloorToInt(currentCam.pixelHeight * 0.5f))
		{
			renderTexture.Remove(targetTexture);
			reflectionCamera.targetTexture = CreateTextureFor(currentCam);
			targetTexture.Release();
			DestroyImmediate(targetTexture);
		}
		RenderReflectionFor(currentCam, reflectionCamera);
	}

	public void LateUpdate()
	{
		if (helperCameras != null && helperCameras.Count > 0)
		{
			helperCameras.Clear();
		}
	}

	public void WaterTileBeingRendered(Camera currentCam)
	{
		RenderHelpCameras(currentCam);
		if (reflectionCamera && sharedMaterial)
		{
			sharedMaterial.SetTexture(reflectionSampler, reflectionCamera.targetTexture);
		}
	}

	public void OnEnable()
	{
		Shader.EnableKeyword("WATER_REFLECTIVE");
		Shader.DisableKeyword("WATER_SIMPLE");
	}

	public void OnDisable()
	{
		Shader.EnableKeyword("WATER_SIMPLE");
		Shader.DisableKeyword("WATER_REFLECTIVE");
		foreach (RenderTexture renderTexture in this.renderTexture)
		{
			if (renderTexture != null)
			{
				renderTexture.Release();
				DestroyImmediate(renderTexture);
			}
		}
		this.renderTexture.Clear();
	}

	private void RenderReflectionFor(Camera cam, Camera reflectCamera)
	{
		if (!reflectCamera)
		{
			return;
		}
		if (sharedMaterial && !sharedMaterial.HasProperty(reflectionSampler))
		{
			return;
		}
		Int32 num = 1 << LayerMask.NameToLayer("GUI") | 1 << LayerMask.NameToLayer("Indoor") | 1 << LayerMask.NameToLayer("Redraw_DONT_USE") | 1 << LayerMask.NameToLayer("Water");
		reflectCamera.cullingMask = (reflectionMask & ~num);
		SaneCameraSettings(reflectCamera);
		reflectCamera.backgroundColor = clearColor;
		reflectCamera.clearFlags = ((!reflectSkybox) ? CameraClearFlags.Color : CameraClearFlags.Skybox);
		if (reflectSkybox && cam.GetComponent(typeof(Skybox)))
		{
			Skybox skybox = (Skybox)reflectCamera.GetComponent(typeof(Skybox));
			if (!skybox)
			{
				skybox = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
			}
			skybox.material = ((Skybox)cam.GetComponent(typeof(Skybox))).material;
		}
		GL.SetRevertBackfacing(true);
		Transform transform = this.transform;
		Vector3 eulerAngles = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
		reflectCamera.transform.position = cam.transform.position;
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		Single w = -Vector3.Dot(up, position) - clipPlaneOffset;
		Vector4 vector = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 rhs;
		CalculateReflectionMatrix(out rhs, ref vector);
		oldpos = cam.transform.position;
		Vector3 position2 = rhs.MultiplyPoint(oldpos);
		reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * rhs;
		Vector4 clipPlane = CameraSpacePlane(reflectCamera, position, up, 1f);
		Matrix4x4 matrix4x = cam.projectionMatrix;
		matrix4x = CalculateObliqueMatrix(matrix4x, clipPlane);
		reflectCamera.projectionMatrix = matrix4x;
		reflectCamera.transform.position = position2;
		Vector3 eulerAngles2 = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
		reflectCamera.Render();
		GL.SetRevertBackfacing(false);
	}

	private void SaneCameraSettings(Camera helperCam)
	{
		helperCam.depthTextureMode = DepthTextureMode.None;
		helperCam.backgroundColor = Color.black;
		helperCam.clearFlags = CameraClearFlags.Color;
		helperCam.renderingPath = RenderingPath.Forward;
	}

	private static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
		return projection;
	}

	private static void CalculateReflectionMatrix(out Matrix4x4 reflectionMat, ref Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}

	private static Single sgn(Single a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, Single sideSign)
	{
		Vector3 v = pos + normal * clipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(v);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
	}
}
