using System;
using System.Collections.Generic;
using Legacy;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RedrawCameraGlow : RedrawCamera
{
	private const String REDRAW_CAMERA_OBJECT_NAME = "RedrawCameraGlow";

	private BackgroundGlowFX m_screenComposer;

	private Dictionary<Renderer, GlowData> m_bgGlowData = new Dictionary<Renderer, GlowData>();

	private Dictionary<GameObject, GlowData> m_bgGlowDataList = new Dictionary<GameObject, GlowData>();

	private List<KeyValuePair<Material, Color>> m_glowMaterialColors = new List<KeyValuePair<Material, Color>>();

	private Boolean m_hasActivatedNormalAndDepth;

	public BackgroundGlowFX.Registration RegistrationData => m_screenComposer.RegistrationData;

    public static RedrawCameraGlow GetRedrawCameraGlowScript(Camera pParentCamera, Boolean pCreate)
	{
		if (pParentCamera != null)
		{
			Transform transform = pParentCamera.transform.FindChild("RedrawCameraGlow");
			if (transform != null)
			{
				return transform.GetComponent<RedrawCameraGlow>();
			}
			if (pCreate)
			{
				GameObject gameObject = new GameObject("RedrawCameraGlow", new Type[]
				{
					typeof(Camera)
				});
				RedrawCameraGlow redrawCameraGlow = gameObject.AddComponent<RedrawCameraGlow>();
				redrawCameraGlow.Initialize(pParentCamera, -1);
				redrawCameraGlow.m_screenComposer = BackgroundGlowFX.GetBackgroundGlowFX(pParentCamera, true);
				redrawCameraGlow.m_screenComposer.RegistrationData.Camera = redrawCameraGlow;
				if ((pParentCamera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
				{
					pParentCamera.depthTextureMode |= DepthTextureMode.Depth;
					redrawCameraGlow.m_hasActivatedNormalAndDepth = true;
				}
				return redrawCameraGlow;
			}
		}
		return null;
	}

	public override void AddToRedrawList(GameObject pObject)
	{
		throw new Exception("Sorry, I know... very bad software design. However NEVER CALL THIS! Use the overload instead. Cheers Denis :-S.");
	}

	public void AddToRedrawList(GameObject pObject, Color pTintColor)
	{
		if (!m_redrawObjects.ContainsKey(pObject))
		{
			m_redrawObjects.Add(pObject, pObject.layer);
			GlowData value = new GlowData(pTintColor, pObject.layer);
			m_bgGlowDataList.Add(pObject, value);
			foreach (Renderer key in pObject.GetComponentsInChildren<Renderer>())
			{
				m_bgGlowData[key] = value;
			}
		}
		else
		{
			m_bgGlowDataList[pObject].TintColor = pTintColor;
		}
		if (m_redrawCamera != null && m_screenComposer != null)
		{
			Behaviour redrawCamera = m_redrawCamera;
			Boolean enabled = true;
			m_screenComposer.enabled = enabled;
			redrawCamera.enabled = enabled;
		}
	}

	public override void RemoveFromRedrawList(GameObject pObject)
	{
		m_redrawObjects.Remove(pObject);
		m_bgGlowDataList.Remove(pObject);
		foreach (Renderer key in pObject.GetComponentsInChildren<Renderer>())
		{
			m_bgGlowData.Remove(key);
		}
		if (m_redrawCamera != null && m_screenComposer != null)
		{
			Behaviour redrawCamera = m_redrawCamera;
			Boolean enabled = m_redrawObjects.Count != 0;
			m_screenComposer.enabled = enabled;
			redrawCamera.enabled = enabled;
		}
	}

	public override void RemoveAllFromRedrawList()
	{
		base.RemoveAllFromRedrawList();
		m_bgGlowDataList.Clear();
		m_bgGlowData.Clear();
	}

	private void setColorForAllMaterials(Renderer pRenderedObject, Color pGlowColor)
	{
		Material[] materials = pRenderedObject.materials;
		foreach (Material material in materials)
		{
			if (material.HasProperty("_Color"))
			{
				m_glowMaterialColors.Add(new KeyValuePair<Material, Color>(material, material.color));
				material.color = pGlowColor;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_replacementShader = Helper.FindShader("Hidden/BackGroundGlowMask");
		m_replacementTag = "RenderType";
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (m_hasActivatedNormalAndDepth && m_parentCamera != null)
		{
			m_parentCamera.depthTextureMode &= ~DepthTextureMode.Depth;
		}
		m_glowMaterialColors.Clear();
	}

	protected override void PreModifyObjects()
	{
		m_glowMaterialColors.Clear();
		Boolean flag = false;
		foreach (KeyValuePair<Renderer, GlowData> keyValuePair in m_bgGlowData)
		{
			if (keyValuePair.Key != null)
			{
				GameObject gameObject = keyValuePair.Key.gameObject;
				if (gameObject.activeSelf && keyValuePair.Key.enabled)
				{
					gameObject.layer = 31;
					setColorForAllMaterials(keyValuePair.Key, keyValuePair.Value.TintColor);
				}
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			for (;;)
			{
				IL_9B:
				foreach (KeyValuePair<Renderer, GlowData> keyValuePair2 in m_bgGlowData)
				{
					if (keyValuePair2.Key == null)
					{
						m_bgGlowData.Remove(keyValuePair2.Key);
						goto IL_9B;
					}
				}
				break;
			}
		}
	}

	protected override void PostModifyObjects()
	{
		foreach (KeyValuePair<Renderer, GlowData> keyValuePair in m_bgGlowData)
		{
			if (keyValuePair.Key != null)
			{
				keyValuePair.Key.gameObject.layer = keyValuePair.Value.OriginalLayer;
			}
			else
			{
				Debug.LogError("Null key!");
			}
		}
		foreach (KeyValuePair<Material, Color> keyValuePair2 in m_glowMaterialColors)
		{
			keyValuePair2.Key.color = keyValuePair2.Value;
		}
	}

	private class GlowData
	{
		public Color TintColor;

		public Int32 OriginalLayer;

		public GlowData(Color pColor, Int32 pLayer)
		{
			TintColor = pColor;
			OriginalLayer = pLayer;
		}
	}
}
