using System;
using System.Collections;
using Legacy;
using UnityEngine;
using Object = System.Object;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Color Cor. LUT Legacy")]
public class ColorCorrection : MonoBehaviour
{
	private const Int32 TEX_DIM = 16;

	private const Single ANIM_FPS = 0.04f;

	private static WaitForSeconds m_AnimDelay = new WaitForSeconds(0.04f);

	private static WaitForEndOfFrame m_OneFrameDelay = new WaitForEndOfFrame();

	private Single m_UpdateTime;

	private Material m_Material;

	private Color[] m_SourceBuffer;

	private Color[] m_TargetBuffer;

	private Boolean m_TargetIsIdentity;

	private Color[] m_PixelBuffer;

	private Texture3D m_Texture;

	[SerializeField]
	private Texture3D m_BlendTarget;

	public Shader shader;

	public Single BlendLenth = 1f;

	public Texture3D BlendTarget
	{
		get => m_BlendTarget;
	    set
		{
			if (value != null && (value.width != 16 || value.height != 16 || value.depth != 16))
			{
				Debug.LogError(String.Concat(new Object[]
				{
					"Blend lut target wrong dimension. (",
					16,
					"x",
					16,
					"x",
					16,
					")"
				}));
				return;
			}
			if (m_BlendTarget != value)
			{
				m_BlendTarget = value;
				if (m_BlendTarget != null)
				{
					BlendTo(m_BlendTarget);
				}
				else
				{
					BlendToIdentity();
				}
			}
		}
	}

	public void BlendToIdentity()
	{
		StopAllCoroutines();
		m_PixelBuffer.CopyTo(m_SourceBuffer);
		SetIdentityLut(m_TargetBuffer);
		m_TargetIsIdentity = true;
		enabled = true;
		StartCoroutine(BlendTexture());
	}

	private void BlendTo(Texture3D texture)
	{
		StopAllCoroutines();
		m_PixelBuffer.CopyTo(m_SourceBuffer);
		m_TargetBuffer = texture.GetPixels();
		m_TargetIsIdentity = false;
		enabled = true;
		StartCoroutine(BlendTexture());
	}

	private void Awake()
	{
		m_BlendTarget = null;
	}

	private void OnEnable()
	{
		if (!SystemInfo.supports3DTextures)
		{
			enabled = false;
			return;
		}
		CheckResources();
	}

	private void OnDestroy()
	{
		Helper.DestroyImmediate<Material>(ref m_Material);
		Helper.DestroyImmediate<Texture3D>(ref m_Texture);
		m_SourceBuffer = null;
		m_TargetBuffer = null;
		m_PixelBuffer = null;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!SystemInfo.supports3DTextures)
		{
			Graphics.Blit(source, destination);
			enabled = false;
			return;
		}
		CheckResources();
		Int32 pass = (QualitySettings.activeColorSpace != ColorSpace.Linear) ? 0 : 1;
		Graphics.Blit(source, destination, m_Material, pass);
	}

	private IEnumerator BlendTexture()
	{
		Single blendTime = 0f;
		while (blendTime < BlendLenth)
		{
			blendTime += 0.04f;
			Single blend = (BlendLenth != 0f) ? Mathf.Clamp01(blendTime / BlendLenth) : 1f;
			Color color;
			color.a = 1f;
			for (Int32 i = 0; i < m_PixelBuffer.Length; i++)
			{
				Color source = m_SourceBuffer[i];
				Color target = m_TargetBuffer[i];
				color.r = source.r + (target.r - source.r) * blend;
				color.g = source.g + (target.g - source.g) * blend;
				color.b = source.b + (target.b - source.b) * blend;
				m_PixelBuffer[i] = color;
			}
			m_Material.SetTexture("_ClutTex", m_Texture);
			m_Texture.SetPixels(m_PixelBuffer);
			m_Texture.Apply();
			yield return m_AnimDelay;
		}
		m_Material.SetTexture("_ClutTex", m_BlendTarget);
		yield return m_OneFrameDelay;
		enabled = !m_TargetIsIdentity;
		yield break;
	}

	private void CheckResources()
	{
		if (m_SourceBuffer == null)
		{
			m_SourceBuffer = new Color[4096];
			SetIdentityLut(m_SourceBuffer);
		}
		if (m_PixelBuffer == null)
		{
			m_PixelBuffer = new Color[4096];
			SetIdentityLut(m_PixelBuffer);
		}
		if (m_Material == null && shader != null)
		{
			m_Material = new Material(shader);
			m_Material.name = "CCC_LUT_MATERIAL";
			m_Material.hideFlags = HideFlags.DontSave;
			m_Material.SetFloat("_Scale", 0.999755859f);
			m_Material.SetFloat("_Offset", 0.000122070313f);
			if (m_Texture != null)
			{
				m_Material.SetTexture("_ClutTex", m_Texture);
			}
		}
		if (m_Texture == null && m_Material != null)
		{
			m_Texture = new Texture3D(16, 16, 16, TextureFormat.RGBA32, false);
			m_Texture.name = "CCC_LUT_BUFFER";
			m_Texture.hideFlags = HideFlags.DontSave;
			m_Texture.wrapMode = TextureWrapMode.Clamp;
			m_Texture.SetPixels(m_PixelBuffer);
			m_Texture.Apply();
			m_Material.SetTexture("_ClutTex", m_Texture);
		}
	}

	private void SetIdentityLut(Color[] buffer)
	{
		for (Int32 i = 0; i < 16; i++)
		{
			for (Int32 j = 0; j < 16; j++)
			{
				for (Int32 k = 0; k < 16; k++)
				{
					buffer[i + j * 16 + k * 16 * 16] = new Color(i * 0.06666667f, j * 0.06666667f, k * 0.06666667f, 1f);
				}
			}
		}
	}
}
