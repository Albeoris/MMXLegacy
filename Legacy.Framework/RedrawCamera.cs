using System;
using System.Collections.Generic;
using Legacy;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RedrawCamera : MonoBehaviour
{
	private const String REDRAW_CAMERA_OBJECT_NAME = "RedrawCamera";

	public const Int32 GLOW_LAYER = 31;

	public const Int32 CULLING_MASK = -2147483648;

	private const Single GLOWSCALE = 0.5f;

	protected Dictionary<GameObject, Int32> m_redrawObjects = new Dictionary<GameObject, Int32>();

	protected Dictionary<GameObject, Int32> m_objToLayers = new Dictionary<GameObject, Int32>();

	protected Camera m_redrawCamera;

	protected Camera m_parentCamera;

	protected Boolean m_destroyFlag;

	protected String m_replacementTag;

	protected Shader m_replacementShader;

	private Int32 m_lastWidth;

	private Int32 m_lastHeight;

	private Int32 m_depthOffset = 1;

	private RenderTexture m_diffuseRenderTex;

	private RenderTexture m_depthRenderTex;

	private Material m_getNormalAndDepthMaterial;

	private Material getNormalAndDepthMaterial
	{
		get
		{
			if (m_getNormalAndDepthMaterial == null)
			{
				m_getNormalAndDepthMaterial = new Material(Helper.FindShader("Hidden/GetNormalAndDepth"));
				m_getNormalAndDepthMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_getNormalAndDepthMaterial;
		}
	}

	public RenderTexture DiffuseRenderTex => m_diffuseRenderTex;

    public RenderTexture DepthRenderTex => m_depthRenderTex;

    public void Initialize(Camera parentCamera, Int32 pCameraDepthOffset)
	{
		transform.parent = parentCamera.transform;
		m_depthOffset = pCameraDepthOffset;
		m_parentCamera = parentCamera;
		CreateDiffuseRenderTexture();
		CreateDepthRenderTexture();
	}

	public virtual void AddToRedrawList(GameObject pObject)
	{
		m_redrawObjects[pObject] = pObject.layer;
		foreach (Renderer renderer in pObject.GetComponentsInChildren<Renderer>())
		{
			m_objToLayers[renderer.gameObject] = renderer.gameObject.layer;
		}
		m_redrawCamera.enabled = true;
	}

	public virtual void RemoveFromRedrawList(GameObject pObject)
	{
		m_redrawObjects.Remove(pObject);
		foreach (Renderer renderer in pObject.GetComponentsInChildren<Renderer>())
		{
			m_objToLayers.Remove(renderer.gameObject);
		}
		m_redrawCamera.enabled = (m_redrawObjects.Count != 0);
	}

	public virtual void RemoveAllFromRedrawList()
	{
		m_redrawObjects.Clear();
		m_objToLayers.Clear();
		m_redrawCamera.enabled = false;
	}

	protected virtual void Awake()
	{
		m_redrawCamera = GetComponent<Camera>();
	}

	protected virtual void CreateDepthRenderTexture()
	{
		if (m_depthRenderTex != null)
		{
			m_depthRenderTex.Release();
			Helper.DestroyImmediate<RenderTexture>(ref m_depthRenderTex);
		}
		m_depthRenderTex = DM_OffScreenRenderer.createRenderTexture(DM_OffScreenRenderer.getUniqueTextureName(), (Int32)(m_parentCamera.pixelWidth * 0.5f), (Int32)(m_parentCamera.pixelHeight * 0.5f), 0, RenderTextureFormat.ARGB32);
		m_depthRenderTex.filterMode = FilterMode.Point;
		m_depthRenderTex.anisoLevel = 0;
		m_depthRenderTex.Create();
	}

	protected virtual void CreateDiffuseRenderTexture()
	{
		if (m_diffuseRenderTex != null)
		{
			m_diffuseRenderTex.Release();
			Helper.DestroyImmediate<RenderTexture>(ref m_diffuseRenderTex);
		}
		m_diffuseRenderTex = DM_OffScreenRenderer.createRenderTexture(DM_OffScreenRenderer.getUniqueTextureName(), (Int32)(m_parentCamera.pixelWidth * 0.5f), (Int32)(m_parentCamera.pixelHeight * 0.5f), 0, RenderTextureFormat.ARGB32);
		m_diffuseRenderTex.filterMode = FilterMode.Trilinear;
		m_diffuseRenderTex.anisoLevel = 1;
		m_diffuseRenderTex.Create();
		m_redrawCamera.targetTexture = m_diffuseRenderTex;
	}

	protected virtual void LateUpdate()
	{
		if (m_parentCamera != null && (m_lastWidth != (Int32)(m_parentCamera.pixelWidth * 0.5f) || m_lastHeight != (Int32)(m_parentCamera.pixelHeight * 0.5f)))
		{
			m_lastWidth = (Int32)(m_parentCamera.pixelWidth * 0.5f);
			m_lastHeight = (Int32)(m_parentCamera.pixelHeight * 0.5f);
			CreateDepthRenderTexture();
			CreateDiffuseRenderTexture();
		}
	}

	protected virtual void OnDestroy()
	{
		if (m_diffuseRenderTex != null)
		{
			m_diffuseRenderTex.Release();
			Helper.DestroyImmediate<RenderTexture>(ref m_diffuseRenderTex);
		}
		if (m_depthRenderTex != null)
		{
			m_depthRenderTex.Release();
			Helper.DestroyImmediate<RenderTexture>(ref m_depthRenderTex);
		}
		Helper.DestroyImmediate<Material>(ref m_getNormalAndDepthMaterial);
		RemoveAllFromRedrawList();
	}

	protected virtual void OnPreCull()
	{
		m_redrawCamera.CopyFrom(m_parentCamera);
		m_redrawCamera.projectionMatrix = m_parentCamera.projectionMatrix;
		m_redrawCamera.renderingPath = RenderingPath.Forward;
		m_redrawCamera.cullingMask = Int32.MinValue;
		m_redrawCamera.depth = m_parentCamera.depth + m_depthOffset;
		m_redrawCamera.eventMask = 0;
		if (m_diffuseRenderTex != null)
		{
			m_redrawCamera.targetTexture = m_diffuseRenderTex;
			m_redrawCamera.clearFlags = CameraClearFlags.Color;
			m_redrawCamera.backgroundColor = Color.clear;
		}
		else
		{
			m_redrawCamera.clearFlags = CameraClearFlags.Depth;
		}
		if (m_replacementShader != null)
		{
			m_redrawCamera.SetReplacementShader(m_replacementShader, m_replacementTag);
		}
		if (m_depthRenderTex != null)
		{
			m_redrawCamera.depthTextureMode |= DepthTextureMode.Depth;
		}
		PreModifyObjects();
	}

	protected virtual void OnPostRender()
	{
		PostModifyObjects();
		if (m_depthRenderTex != null)
		{
			Graphics.Blit(null, m_depthRenderTex, getNormalAndDepthMaterial);
		}
	}

	protected virtual void PreModifyObjects()
	{
		foreach (GameObject gameObject in m_objToLayers.Keys)
		{
			if (gameObject != null)
			{
				gameObject.layer = 31;
			}
		}
	}

	protected virtual void PostModifyObjects()
	{
		foreach (KeyValuePair<GameObject, Int32> keyValuePair in m_objToLayers)
		{
			if (keyValuePair.Key != null)
			{
				keyValuePair.Key.layer = keyValuePair.Value;
			}
		}
	}
}
