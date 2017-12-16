using System;
using Legacy;
using UnityEngine;

public class BackgroundGlowFX : GlowFXBase
{
	public Shader addGlowShader;

	public Shader cutDownShader;

	public Shader redrawAlphaMaskedShader;

	public Shader downsampleShader2x;

	private Registration m_registration = new Registration();

	private Material m_AddGlowMaterial;

	private Material m_CutDownMaterial;

	private Material m_redrawAlphaMaskedMaterial;

	private Material m_downsampleMaterial2x;

	private Material AddGlowMaterial
	{
		get
		{
			if (m_AddGlowMaterial == null)
			{
				m_AddGlowMaterial = new Material(addGlowShader);
				m_AddGlowMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_AddGlowMaterial;
		}
	}

	private Material CutDownMaterial
	{
		get
		{
			if (m_CutDownMaterial == null)
			{
				m_CutDownMaterial = new Material(cutDownShader);
				m_CutDownMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_CutDownMaterial;
		}
	}

	private Material RedrawAlphaMaskedMaterial
	{
		get
		{
			if (m_redrawAlphaMaskedMaterial == null)
			{
				m_redrawAlphaMaskedMaterial = new Material(redrawAlphaMaskedShader);
				m_redrawAlphaMaskedMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_redrawAlphaMaskedMaterial;
		}
	}

	private Material DownsampleMaterial2x
	{
		get
		{
			if (m_downsampleMaterial2x == null)
			{
				m_downsampleMaterial2x = new Material(downsampleShader2x);
				m_downsampleMaterial2x.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_downsampleMaterial2x;
		}
	}

	public Registration RegistrationData => m_registration;

    public static BackgroundGlowFX GetBackgroundGlowFX(Camera pCamera, Boolean p_AddOnMiss)
	{
		BackgroundGlowFX backgroundGlowFX = pCamera.GetComponent<BackgroundGlowFX>();
		if (p_AddOnMiss && backgroundGlowFX == null)
		{
			backgroundGlowFX = pCamera.gameObject.AddComponent<BackgroundGlowFX>();
			backgroundGlowFX.Awake();
		}
		return backgroundGlowFX;
	}

	public void Register(Registration pData)
	{
		if (m_registration != null)
		{
			Debug.LogError("BackgroundGlowFX: Register: dublicate registration data! Registration call is skipped...");
		}
		else
		{
			m_registration = pData;
			enabled = true;
		}
	}

	public void Remove(Registration pData)
	{
		if (m_registration == null || m_registration == pData)
		{
			m_registration = null;
			enabled = false;
		}
		else
		{
			Debug.LogError("BackgroundGlowFX: Remove: wrong registration data! Remove call is skipped...");
		}
	}

	private void Awake()
	{
		if (addGlowShader == null)
		{
			addGlowShader = Helper.FindShader("Hidden/PostFXAdd");
		}
		if (cutDownShader == null)
		{
			cutDownShader = Helper.FindShader("Hidden/BackGroundGlowMaskCutDown");
		}
		if (downsampleShader == null)
		{
			downsampleShader = Helper.FindShader("Hidden/Glow Downsample");
		}
		if (downsampleShader2x == null)
		{
			downsampleShader2x = Helper.FindShader("Hidden/Glow Downsample 2x");
		}
		if (blurShader == null)
		{
			blurShader = Helper.FindShader("Hidden/GlowConeTap");
		}
		if (redrawAlphaMaskedShader == null)
		{
			redrawAlphaMaskedShader = Helper.FindShader("Hidden/BackGroundGlowRedraw");
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Helper.DestroyImmediate<Material>(ref m_AddGlowMaterial);
		Helper.DestroyImmediate<Material>(ref m_downsampleMaterial2x);
		Helper.DestroyImmediate<Material>(ref m_CutDownMaterial);
		Helper.DestroyImmediate<Material>(ref m_redrawAlphaMaskedMaterial);
		m_registration = null;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
		if (m_registration == null)
		{
			enabled = false;
			return;
		}
		RenderTexture renderTexture = null;
		RenderTexture renderTexture2 = null;
		try
		{
			renderTexture = RenderTexture.GetTemporary(source.width / m_registration.DownScale, source.height / m_registration.DownScale, 0);
			renderTexture.filterMode = FilterMode.Trilinear;
			renderTexture.anisoLevel = 1;
			renderTexture2 = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0);
			renderTexture2.filterMode = FilterMode.Trilinear;
			renderTexture2.anisoLevel = 1;
			CutDownMaterial.SetTexture("_DepthTex", m_registration.Camera.DepthRenderTex);
			Graphics.Blit(m_registration.Camera.DiffuseRenderTex, m_registration.Camera.DiffuseRenderTex, CutDownMaterial);
			if (m_registration.DownScale == 1)
			{
				Graphics.Blit(m_registration.Camera.DiffuseRenderTex, renderTexture);
			}
			else if (m_registration.DownScale == 2)
			{
				DownSample2x(m_registration.Camera.DiffuseRenderTex, renderTexture, m_registration.TintColor);
			}
			else
			{
				DownSample4x(m_registration.Camera.DiffuseRenderTex, renderTexture, m_registration.TintColor);
			}
			Single num = Mathf.Clamp01((m_registration.GlowIntensity - 1f) / 4f);
			blurMaterial.color = new Color(1f, 1f, 1f, 0.25f + num);
			Boolean flag = true;
			for (Int32 i = 0; i < m_registration.BlurIterations; i++)
			{
				if (flag)
				{
					FourTapCone(renderTexture, renderTexture2, i, m_registration.BlurSpread, blurMaterial);
				}
				else
				{
					FourTapCone(renderTexture2, renderTexture, i, m_registration.BlurSpread, blurMaterial);
				}
				flag = !flag;
			}
			if (!flag)
			{
				RenderTexture renderTexture3 = renderTexture;
				renderTexture = renderTexture2;
				renderTexture2 = renderTexture3;
			}
			RedrawAlphaMaskedMaterial.SetTexture("_MaskTex", m_registration.Camera.DiffuseRenderTex);
			Graphics.Blit(renderTexture, destination, RedrawAlphaMaskedMaterial);
		}
		finally
		{
			if (renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			if (renderTexture2 != null)
			{
				RenderTexture.ReleaseTemporary(renderTexture2);
			}
		}
	}

	private void DownSample2x(RenderTexture source, RenderTexture dest, Color tint)
	{
		tint.a *= 0.25f;
		DownsampleMaterial2x.color = tint;
		Graphics.Blit(source, dest, DownsampleMaterial2x);
	}

	public class Registration
	{
		public RedrawCameraGlow Camera;

		public Single GlowIntensity = 1.5f;

		public Int32 BlurIterations = 3;

		public Single BlurSpread = 0.7f;

		public Int32 DownScale = 4;

		public Color TintColor = new Color(1f, 1f, 1f, 0f);
	}
}
