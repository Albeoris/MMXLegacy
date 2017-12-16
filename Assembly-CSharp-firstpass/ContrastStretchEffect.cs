using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Contrast Stretch")]
public class ContrastStretchEffect : MonoBehaviour
{
	public Single adaptationSpeed = 0.02f;

	public Single limitMinimum = 0.2f;

	public Single limitMaximum = 0.6f;

	private RenderTexture[] adaptRenderTex = new RenderTexture[2];

	private Int32 curAdaptIndex;

	public Shader shaderLum;

	private Material m_materialLum;

	public Shader shaderReduce;

	private Material m_materialReduce;

	public Shader shaderAdapt;

	private Material m_materialAdapt;

	public Shader shaderApply;

	private Material m_materialApply;

	protected Material materialLum
	{
		get
		{
			if (m_materialLum == null)
			{
				m_materialLum = new Material(shaderLum);
				m_materialLum.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_materialLum;
		}
	}

	protected Material materialReduce
	{
		get
		{
			if (m_materialReduce == null)
			{
				m_materialReduce = new Material(shaderReduce);
				m_materialReduce.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_materialReduce;
		}
	}

	protected Material materialAdapt
	{
		get
		{
			if (m_materialAdapt == null)
			{
				m_materialAdapt = new Material(shaderAdapt);
				m_materialAdapt.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_materialAdapt;
		}
	}

	protected Material materialApply
	{
		get
		{
			if (m_materialApply == null)
			{
				m_materialApply = new Material(shaderApply);
				m_materialApply.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_materialApply;
		}
	}

	private void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
		if (!shaderAdapt.isSupported || !shaderApply.isSupported || !shaderLum.isSupported || !shaderReduce.isSupported)
		{
			enabled = false;
			return;
		}
	}

	private void OnEnable()
	{
		for (Int32 i = 0; i < 2; i++)
		{
			if (!adaptRenderTex[i])
			{
				adaptRenderTex[i] = new RenderTexture(1, 1, 0);
				adaptRenderTex[i].hideFlags = HideFlags.HideAndDontSave;
			}
		}
	}

	private void OnDisable()
	{
		for (Int32 i = 0; i < 2; i++)
		{
			DestroyImmediate(adaptRenderTex[i]);
			adaptRenderTex[i] = null;
		}
		if (m_materialLum)
		{
			DestroyImmediate(m_materialLum);
		}
		if (m_materialReduce)
		{
			DestroyImmediate(m_materialReduce);
		}
		if (m_materialAdapt)
		{
			DestroyImmediate(m_materialAdapt);
		}
		if (m_materialApply)
		{
			DestroyImmediate(m_materialApply);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture renderTexture = RenderTexture.GetTemporary(source.width / 1, source.height / 1);
		Graphics.Blit(source, renderTexture, materialLum);
		while (renderTexture.width > 1 || renderTexture.height > 1)
		{
			Int32 num = renderTexture.width / 2;
			if (num < 1)
			{
				num = 1;
			}
			Int32 num2 = renderTexture.height / 2;
			if (num2 < 1)
			{
				num2 = 1;
			}
			RenderTexture temporary = RenderTexture.GetTemporary(num, num2);
			Graphics.Blit(renderTexture, temporary, materialReduce);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		CalculateAdaptation(renderTexture);
		materialApply.SetTexture("_AdaptTex", adaptRenderTex[curAdaptIndex]);
		Graphics.Blit(source, destination, materialApply);
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	private void CalculateAdaptation(Texture curTexture)
	{
		Int32 num = curAdaptIndex;
		curAdaptIndex = (curAdaptIndex + 1) % 2;
		Single num2 = 1f - Mathf.Pow(1f - adaptationSpeed, 30f * Time.deltaTime);
		num2 = Mathf.Clamp(num2, 0.01f, 1f);
		materialAdapt.SetTexture("_CurTex", curTexture);
		materialAdapt.SetVector("_AdaptParams", new Vector4(num2, limitMinimum, limitMaximum, 0f));
		Graphics.SetRenderTarget(adaptRenderTex[curAdaptIndex]);
		GL.Clear(false, true, Color.black);
		Graphics.Blit(adaptRenderTex[num], adaptRenderTex[curAdaptIndex], materialAdapt);
	}
}
