using System;
using Legacy;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using UnityEngine;
using Object = System.Object;

internal class Pixelation : MonoBehaviour
{
	private Int32 m_LastScale;

	private Int32 m_LastScreenW;

	private Int32 m_LastScreenH;

	private Boolean m_LastHdr;

	private Camera m_LastCamera;

	private RenderTexture m_PixelRenderTex;

	private Material m_Mat;

	[SerializeField]
	private Int32 m_Scale = 4;

	public Int32 Scale
	{
		get => m_Scale;
	    set
		{
			if (m_Scale != value)
			{
				m_Scale = value;
				enabled = (value > 0);
			}
		}
	}

	private void Awake()
	{
		m_Mat = new Material(Helper.FindShader("Custom/Blit"));
		m_Mat.hideFlags = HideFlags.DontSave;
		OnOptionsChanged(null, null);
		LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnOptionsChanged));
	}

	private void OnDestroy()
	{
		Helper.DestroyImmediate<RenderTexture>(ref m_PixelRenderTex);
		Helper.DestroyImmediate<Material>(ref m_Mat);
		LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OPTIONS_CHANGED, new EventHandler(OnOptionsChanged));
	}

	private void OnDisable()
	{
		if (m_LastCamera != null)
		{
			m_LastCamera.targetTexture = null;
		}
		m_LastCamera = null;
	}

	private void OnOptionsChanged(Object sender, EventArgs e)
	{
		Scale = ConfigManager.Instance.Options.RetroScreenDivisor;
		enabled = ConfigManager.Instance.Options.RetroMode;
	}

	private void Update()
	{
		if (FXMainCamera.Instance == null)
		{
			return;
		}
		if (m_Scale <= 0)
		{
			enabled = false;
			return;
		}
		GameObject currentCamera = FXMainCamera.Instance.CurrentCamera;
		if (currentCamera == null)
		{
			return;
		}
		Camera camera = currentCamera.camera;
		if (m_LastCamera != camera)
		{
			if (m_LastCamera != null)
			{
				m_LastCamera.targetTexture = null;
			}
			m_LastCamera = camera;
			if (camera != null)
			{
				if (m_PixelRenderTex == null || m_LastScreenW != Screen.width || m_LastScreenH != Screen.height || m_LastHdr != camera.hdr || m_LastScale != m_Scale)
				{
					UpdateRenderTexture();
				}
				camera.targetTexture = m_PixelRenderTex;
			}
		}
		else if (m_PixelRenderTex == null || m_LastScreenW != Screen.width || m_LastScreenH != Screen.height || m_LastHdr != camera.hdr || m_LastScale != m_Scale)
		{
			UpdateRenderTexture();
		}
	}

	private void UpdateRenderTexture()
	{
		if (m_LastCamera != null)
		{
			if (m_PixelRenderTex != null)
			{
				m_PixelRenderTex.Release();
				DestroyImmediate(m_PixelRenderTex);
			}
			m_LastScale = m_Scale;
			m_LastScreenW = Screen.width;
			m_LastScreenH = Screen.height;
			m_LastHdr = m_LastCamera.hdr;
			m_PixelRenderTex = new RenderTexture(m_LastScreenW / m_LastScale, m_LastScreenH / m_LastScale, 24, (!m_LastHdr) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf);
			m_PixelRenderTex.name = "PixelationRT";
			m_PixelRenderTex.filterMode = FilterMode.Point;
			m_PixelRenderTex.hideFlags = HideFlags.DontSave;
			m_LastCamera.targetTexture = m_PixelRenderTex;
		}
	}

	private void OnPreRender()
	{
		GL.PushMatrix();
		GL.Begin(7);
		GL.LoadOrtho();
		m_Mat.mainTexture = m_PixelRenderTex;
		m_Mat.SetPass(0);
		GL.TexCoord2(0f, 0f);
		GL.Vertex3(0f, 0f, 0f);
		GL.TexCoord2(1f, 0f);
		GL.Vertex3(1f, 0f, 0f);
		GL.TexCoord2(1f, 1f);
		GL.Vertex3(1f, 1f, 0f);
		GL.TexCoord2(0f, 1f);
		GL.Vertex3(0f, 1f, 0f);
		GL.End();
		GL.PopMatrix();
	}
}
