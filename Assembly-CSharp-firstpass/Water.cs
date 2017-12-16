using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

[ExecuteInEditMode]
public class Water : MonoBehaviour
{
	public WaterMode m_WaterMode = WaterMode.Refractive;

	public Boolean m_DisablePixelLights = true;

	public Int32 m_TextureSize = 256;

	public Single m_ClipPlaneOffset = 0.07f;

	public LayerMask m_ReflectLayers = -1;

	public LayerMask m_RefractLayers = -1;

	private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>();

	private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>();

	private RenderTexture m_ReflectionTexture;

	private RenderTexture m_RefractionTexture;

	private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;

	private Int32 m_OldReflectionTextureSize;

	private Int32 m_OldRefractionTextureSize;

	private static Boolean s_InsideWater;

	public void OnWillRenderObject()
	{
		if (!enabled || !renderer || !renderer.sharedMaterial || !renderer.enabled)
		{
			return;
		}
		Camera current = Camera.current;
		if (!current)
		{
			return;
		}
		if (s_InsideWater)
		{
			return;
		}
		s_InsideWater = true;
		m_HardwareWaterSupport = FindHardwareWaterSupport();
		WaterMode waterMode = GetWaterMode();
		Camera camera;
		Camera camera2;
		CreateWaterObjects(current, out camera, out camera2);
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		Int32 pixelLightCount = QualitySettings.pixelLightCount;
		if (m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		UpdateCameraModes(current, camera);
		UpdateCameraModes(current, camera2);
		if (waterMode >= WaterMode.Reflective)
		{
			Single w = -Vector3.Dot(up, position) - m_ClipPlaneOffset;
			Vector4 plane = new Vector4(up.x, up.y, up.z, w);
			Matrix4x4 zero = Matrix4x4.zero;
			CalculateReflectionMatrix(ref zero, plane);
			Vector3 position2 = current.transform.position;
			Vector3 position3 = zero.MultiplyPoint(position2);
			camera.worldToCameraMatrix = current.worldToCameraMatrix * zero;
			Vector4 clipPlane = CameraSpacePlane(camera, position, up, 1f);
			Matrix4x4 projectionMatrix = current.projectionMatrix;
			CalculateObliqueMatrix(ref projectionMatrix, clipPlane);
			camera.projectionMatrix = projectionMatrix;
			camera.cullingMask = (-17 & m_ReflectLayers.value);
			camera.targetTexture = m_ReflectionTexture;
			GL.SetRevertBackfacing(true);
			camera.transform.position = position3;
			Vector3 eulerAngles = current.transform.eulerAngles;
			camera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
			camera.Render();
			camera.transform.position = position2;
			GL.SetRevertBackfacing(false);
			renderer.sharedMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
		}
		if (waterMode >= WaterMode.Refractive)
		{
			camera2.worldToCameraMatrix = current.worldToCameraMatrix;
			Vector4 clipPlane2 = CameraSpacePlane(camera2, position, up, -1f);
			Matrix4x4 projectionMatrix2 = current.projectionMatrix;
			CalculateObliqueMatrix(ref projectionMatrix2, clipPlane2);
			camera2.projectionMatrix = projectionMatrix2;
			camera2.cullingMask = (-17 & m_RefractLayers.value);
			camera2.targetTexture = m_RefractionTexture;
			camera2.transform.position = current.transform.position;
			camera2.transform.rotation = current.transform.rotation;
			camera2.Render();
			renderer.sharedMaterial.SetTexture("_RefractionTex", m_RefractionTexture);
		}
		if (m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}
		switch (waterMode)
		{
		case WaterMode.Simple:
			Shader.EnableKeyword("WATER_SIMPLE");
			Shader.DisableKeyword("WATER_REFLECTIVE");
			Shader.DisableKeyword("WATER_REFRACTIVE");
			break;
		case WaterMode.Reflective:
			Shader.DisableKeyword("WATER_SIMPLE");
			Shader.EnableKeyword("WATER_REFLECTIVE");
			Shader.DisableKeyword("WATER_REFRACTIVE");
			break;
		case WaterMode.Refractive:
			Shader.DisableKeyword("WATER_SIMPLE");
			Shader.DisableKeyword("WATER_REFLECTIVE");
			Shader.EnableKeyword("WATER_REFRACTIVE");
			break;
		}
		s_InsideWater = false;
	}

	private void OnDisable()
	{
		if (m_ReflectionTexture)
		{
			DestroyImmediate(m_ReflectionTexture);
			m_ReflectionTexture = null;
		}
		if (m_RefractionTexture)
		{
			DestroyImmediate(m_RefractionTexture);
			m_RefractionTexture = null;
		}
		foreach (KeyValuePair<Camera, Camera> keyValuePair in m_ReflectionCameras)
		{
			DestroyImmediate(keyValuePair.Value.gameObject);
		}
		m_ReflectionCameras.Clear();
		foreach (KeyValuePair<Camera, Camera> keyValuePair2 in m_RefractionCameras)
		{
			DestroyImmediate(keyValuePair2.Value.gameObject);
		}
		m_RefractionCameras.Clear();
	}

	private void Update()
	{
		if (!renderer)
		{
			return;
		}
		Material sharedMaterial = renderer.sharedMaterial;
		if (!sharedMaterial)
		{
			return;
		}
		Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
		Single @float = sharedMaterial.GetFloat("_WaveScale");
		Vector4 vector2 = new Vector4(@float, @float, @float * 0.4f, @float * 0.45f);
		Double num = Time.timeSinceLevelLoad / 20.0;
		Vector4 vector3 = new Vector4((Single)Math.IEEERemainder(vector.x * vector2.x * num, 1.0), (Single)Math.IEEERemainder(vector.y * vector2.y * num, 1.0), (Single)Math.IEEERemainder(vector.z * vector2.z * num, 1.0), (Single)Math.IEEERemainder(vector.w * vector2.w * num, 1.0));
		sharedMaterial.SetVector("_WaveOffset", vector3);
		sharedMaterial.SetVector("_WaveScale4", vector2);
		Vector3 size = renderer.bounds.size;
		Vector3 s = new Vector3(size.x * vector2.x, size.z * vector2.y, 1f);
		Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(vector3.x, vector3.y, 0f), Quaternion.identity, s);
		sharedMaterial.SetMatrix("_WaveMatrix", matrix);
		s = new Vector3(size.x * vector2.z, size.z * vector2.w, 1f);
		matrix = Matrix4x4.TRS(new Vector3(vector3.z, vector3.w, 0f), Quaternion.identity, s);
		sharedMaterial.SetMatrix("_WaveMatrix2", matrix);
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
	{
		WaterMode waterMode = GetWaterMode();
		reflectionCamera = null;
		refractionCamera = null;
		if (waterMode >= WaterMode.Reflective)
		{
			if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
			{
				if (m_ReflectionTexture)
				{
					DestroyImmediate(m_ReflectionTexture);
				}
				m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
				m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
				m_ReflectionTexture.isPowerOfTwo = true;
				m_ReflectionTexture.hideFlags = HideFlags.DontSave;
				m_OldReflectionTextureSize = m_TextureSize;
			}
			m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
			if (!reflectionCamera)
			{
				GameObject gameObject = new GameObject(String.Concat(new Object[]
				{
					"Water Refl Camera id",
					GetInstanceID(),
					" for ",
					currentCamera.GetInstanceID()
				}), new Type[]
				{
					typeof(Camera),
					typeof(Skybox)
				});
				reflectionCamera = gameObject.camera;
				reflectionCamera.enabled = false;
				reflectionCamera.transform.position = transform.position;
				reflectionCamera.transform.rotation = transform.rotation;
				reflectionCamera.gameObject.AddComponent("FlareLayer");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				m_ReflectionCameras[currentCamera] = reflectionCamera;
			}
		}
		if (waterMode >= WaterMode.Refractive)
		{
			if (!m_RefractionTexture || m_OldRefractionTextureSize != m_TextureSize)
			{
				if (m_RefractionTexture)
				{
					DestroyImmediate(m_RefractionTexture);
				}
				m_RefractionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
				m_RefractionTexture.name = "__WaterRefraction" + GetInstanceID();
				m_RefractionTexture.isPowerOfTwo = true;
				m_RefractionTexture.hideFlags = HideFlags.DontSave;
				m_OldRefractionTextureSize = m_TextureSize;
			}
			m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
			if (!refractionCamera)
			{
				GameObject gameObject2 = new GameObject(String.Concat(new Object[]
				{
					"Water Refr Camera id",
					GetInstanceID(),
					" for ",
					currentCamera.GetInstanceID()
				}), new Type[]
				{
					typeof(Camera),
					typeof(Skybox)
				});
				refractionCamera = gameObject2.camera;
				refractionCamera.enabled = false;
				refractionCamera.transform.position = transform.position;
				refractionCamera.transform.rotation = transform.rotation;
				refractionCamera.gameObject.AddComponent("FlareLayer");
				gameObject2.hideFlags = HideFlags.HideAndDontSave;
				m_RefractionCameras[currentCamera] = refractionCamera;
			}
		}
	}

	private WaterMode GetWaterMode()
	{
		if (m_HardwareWaterSupport < m_WaterMode)
		{
			return m_HardwareWaterSupport;
		}
		return m_WaterMode;
	}

	private WaterMode FindHardwareWaterSupport()
	{
		if (!SystemInfo.supportsRenderTextures || !renderer)
		{
			return WaterMode.Simple;
		}
		Material sharedMaterial = renderer.sharedMaterial;
		if (!sharedMaterial)
		{
			return WaterMode.Simple;
		}
		String tag = sharedMaterial.GetTag("WATERMODE", false);
		if (tag == "Refractive")
		{
			return WaterMode.Refractive;
		}
		if (tag == "Reflective")
		{
			return WaterMode.Reflective;
		}
		return WaterMode.Simple;
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
		Vector3 v = pos + normal * m_ClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(v);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
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

	public enum WaterMode
	{
		Simple,
		Reflective,
		Refractive
	}
}
