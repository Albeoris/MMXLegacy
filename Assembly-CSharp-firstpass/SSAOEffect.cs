using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Rendering/Screen Space Ambient Occlusion")]
public class SSAOEffect : MonoBehaviour
{
	public Single m_Radius = 0.4f;

	public SSAOSamples m_SampleCount = SSAOSamples.Medium;

	public Single m_OcclusionIntensity = 1.5f;

	public Int32 m_Blur = 2;

	public Int32 m_Downsampling = 2;

	public Single m_OcclusionAttenuation = 1f;

	public Single m_MinZ = 0.01f;

	public Shader m_SSAOShader;

	private Material m_SSAOMaterial;

	public Texture2D m_RandomTexture;

	private Boolean m_Supported;

	private static Material CreateMaterial(Shader shader)
	{
		if (!shader)
		{
			return null;
		}
		return new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	private static void DestroyMaterial(Material mat)
	{
		if (mat)
		{
			DestroyImmediate(mat);
			mat = null;
		}
	}

	private void OnDisable()
	{
		DestroyMaterial(m_SSAOMaterial);
	}

	private void Start()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			m_Supported = false;
			enabled = false;
			return;
		}
		CreateMaterials();
		if (!m_SSAOMaterial || m_SSAOMaterial.passCount != 5)
		{
			m_Supported = false;
			enabled = false;
			return;
		}
		m_Supported = true;
	}

	private void OnEnable()
	{
		camera.depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	private void CreateMaterials()
	{
		if (!m_SSAOMaterial && m_SSAOShader.isSupported)
		{
			m_SSAOMaterial = CreateMaterial(m_SSAOShader);
			m_SSAOMaterial.SetTexture("_RandomTexture", m_RandomTexture);
		}
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!m_Supported || !m_SSAOShader.isSupported)
		{
			enabled = false;
			return;
		}
		CreateMaterials();
		m_Downsampling = Mathf.Clamp(m_Downsampling, 1, 6);
		m_Radius = Mathf.Clamp(m_Radius, 0.05f, 1f);
		m_MinZ = Mathf.Clamp(m_MinZ, 1E-05f, 0.5f);
		m_OcclusionIntensity = Mathf.Clamp(m_OcclusionIntensity, 0.5f, 4f);
		m_OcclusionAttenuation = Mathf.Clamp(m_OcclusionAttenuation, 0.2f, 2f);
		m_Blur = Mathf.Clamp(m_Blur, 0, 4);
		RenderTexture renderTexture = RenderTexture.GetTemporary(source.width / m_Downsampling, source.height / m_Downsampling, 0);
		Single fieldOfView = camera.fieldOfView;
		Single farClipPlane = camera.farClipPlane;
		Single num = Mathf.Tan(fieldOfView * 0.0174532924f * 0.5f) * farClipPlane;
		Single x = num * camera.aspect;
		m_SSAOMaterial.SetVector("_FarCorner", new Vector3(x, num, farClipPlane));
		Int32 num2;
		Int32 num3;
		if (m_RandomTexture)
		{
			num2 = m_RandomTexture.width;
			num3 = m_RandomTexture.height;
		}
		else
		{
			num2 = 1;
			num3 = 1;
		}
		m_SSAOMaterial.SetVector("_NoiseScale", new Vector3(renderTexture.width / (Single)num2, renderTexture.height / (Single)num3, 0f));
		m_SSAOMaterial.SetVector("_Params", new Vector4(m_Radius, m_MinZ, 1f / m_OcclusionAttenuation, m_OcclusionIntensity));
		Boolean flag = m_Blur > 0;
		Graphics.Blit((!flag) ? source : null, renderTexture, m_SSAOMaterial, (Int32)m_SampleCount);
		if (flag)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
			m_SSAOMaterial.SetVector("_TexelOffsetScale", new Vector4(m_Blur / (Single)source.width, 0f, 0f, 0f));
			m_SSAOMaterial.SetTexture("_SSAO", renderTexture);
			Graphics.Blit(null, temporary, m_SSAOMaterial, 3);
			RenderTexture.ReleaseTemporary(renderTexture);
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0);
			m_SSAOMaterial.SetVector("_TexelOffsetScale", new Vector4(0f, m_Blur / (Single)source.height, 0f, 0f));
			m_SSAOMaterial.SetTexture("_SSAO", temporary);
			Graphics.Blit(source, temporary2, m_SSAOMaterial, 3);
			RenderTexture.ReleaseTemporary(temporary);
			renderTexture = temporary2;
		}
		m_SSAOMaterial.SetTexture("_SSAO", renderTexture);
		Graphics.Blit(source, destination, m_SSAOMaterial, 4);
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	public enum SSAOSamples
	{
		Low,
		Medium,
		High
	}
}
