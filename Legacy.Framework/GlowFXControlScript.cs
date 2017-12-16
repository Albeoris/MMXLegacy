using System;
using System.Collections.Generic;
using UnityEngine;

public class GlowFXControlScript
{
	private static Dictionary<GameObject, Int32> s_objectsWithDeeplySavedColor = new Dictionary<GameObject, Int32>();

	private static Dictionary<Material, Color> s_originalMaterialColors = new Dictionary<Material, Color>();

	public static void SetObjectGlowDeep(GameObject pObject, Color pColor, Single pBlendFactor)
	{
		SetObjectGlowDeep(pObject, pColor, pBlendFactor, pBlendFactor, false, null);
	}

	public static void SetObjectGlowDeep(GameObject pObject, Color pColor, Single pBlendFactorColor, Single pBlendFactorAlpha)
	{
		SetObjectGlowDeep(pObject, pColor, pBlendFactorColor, pBlendFactorAlpha, false, null);
	}

	public static void SetObjectGlowDeep(GameObject pObject, Color pColor, Single pBlendFactor, Boolean pIsBackgroundGlow, Camera pGlowCamera)
	{
		SetObjectGlowDeep(pObject, pColor, pBlendFactor, pBlendFactor, pIsBackgroundGlow, pGlowCamera);
	}

	public static void SetObjectGlowDeep(GameObject pObject, Color pColor, Single pBlendFactorColor, Single pBlendFactorAlpha, Boolean pIsBackgroundGlow, Camera pGlowCamera)
	{
		Boolean? flag = isBackgroundGlow(pIsBackgroundGlow, pGlowCamera);
		if (pObject != null && flag != null)
		{
			if (flag == true)
			{
				setObjectBackgroundGlow(pObject, pColor, pBlendFactorColor, pBlendFactorAlpha, pIsBackgroundGlow, pGlowCamera);
			}
			else
			{
				saveOriginalColorDeep(pObject);
				Renderer[] componentsInChildren = pObject.GetComponentsInChildren<Renderer>(true);
				foreach (Renderer pRenderedObject in componentsInChildren)
				{
					setColorForAllMaterials(pRenderedObject, pColor, pBlendFactorColor, pBlendFactorAlpha);
				}
			}
		}
	}

	public static void RemoveGlowDeep(GameObject pObject, Camera pGlowCamera)
	{
		SetObjectGlowDeep(pObject, Color.black, 0f, pGlowCamera != null, pGlowCamera);
	}

	public static void StartNewScene()
	{
		s_objectsWithDeeplySavedColor.Clear();
		s_originalMaterialColors.Clear();
	}

	private static void setObjectBackgroundGlow(GameObject pObject, Color pColor, Single pBlendFactorColor, Single pBlendFactorAlpha, Boolean pIsBackgroundGlow, Camera pGlowCamera)
	{
		if (pColor.a * pBlendFactorAlpha <= 0f || !pIsBackgroundGlow)
		{
			RedrawCameraGlow redrawCameraGlowScript = RedrawCameraGlow.GetRedrawCameraGlowScript(pGlowCamera, false);
			if (redrawCameraGlowScript != null)
			{
				redrawCameraGlowScript.RemoveFromRedrawList(pObject);
			}
		}
		else if (pIsBackgroundGlow)
		{
			RedrawCameraGlow redrawCameraGlowScript2 = RedrawCameraGlow.GetRedrawCameraGlowScript(pGlowCamera, true);
			Color pTintColor = pColor * pBlendFactorColor;
			pTintColor.a = pColor.a * pBlendFactorAlpha;
			redrawCameraGlowScript2.AddToRedrawList(pObject, pTintColor);
		}
	}

	private static void saveOriginalColorDeep(GameObject pObject)
	{
		Int32 num = pObject.GetComponentsInChildren<Transform>(true).Length;
		if (!s_objectsWithDeeplySavedColor.ContainsKey(pObject))
		{
			s_objectsWithDeeplySavedColor.Add(pObject, num);
			foreach (Renderer pRenderedObject in pObject.GetComponentsInChildren<Renderer>(true))
			{
				saveOriginalColor(pRenderedObject);
			}
		}
		else if (s_objectsWithDeeplySavedColor[pObject] != num)
		{
			s_objectsWithDeeplySavedColor[pObject] = num;
			foreach (Renderer pRenderedObject2 in pObject.GetComponentsInChildren<Renderer>(true))
			{
				saveOriginalColor(pRenderedObject2);
			}
		}
	}

	private static void saveOriginalColor(Renderer pRenderedObject)
	{
		Material[] materials = pRenderedObject.materials;
		foreach (Material material in materials)
		{
			if (material.HasProperty("_Color") && !s_originalMaterialColors.ContainsKey(material))
			{
				s_originalMaterialColors.Add(material, material.color);
			}
		}
	}

	private static Boolean setColorForAllMaterials(Renderer pRenderedObject, Color pColor, Single pBlendFactorColor, Single pBlendFactorAlpha)
	{
		Boolean result = false;
		Material[] materials = pRenderedObject.materials;
		foreach (Material material in materials)
		{
			if (material.HasProperty("_Color"))
			{
				Color color = Color.white;
				if (s_originalMaterialColors.ContainsKey(material))
				{
					color = pColor * pBlendFactorColor + s_originalMaterialColors[material] * (1f - pBlendFactorColor);
					color.a = pColor.a * pBlendFactorAlpha + s_originalMaterialColors[material].a * (1f - pBlendFactorAlpha);
				}
				material.color = color;
				result = true;
			}
		}
		return result;
	}

	private static Boolean? isBackgroundGlow(Boolean pIsBackgroundGlow, Camera pGlowCamera)
	{
		if (!pIsBackgroundGlow)
		{
			return new Boolean?(false);
		}
		if (pGlowCamera != null)
		{
			return new Boolean?(true);
		}
		Debug.LogError("GlowFXControlScript: SetObjectGlow pIsBackgroundGlow is true, but pGlowCamera is null!");
		return null;
	}
}
