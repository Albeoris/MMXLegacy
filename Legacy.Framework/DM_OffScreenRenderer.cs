using System;
using UnityEngine;

public static class DM_OffScreenRenderer
{
	private static Int32 s_freeObjID;

	public static RenderTexture createRenderTexture(String texName, Int32 pWidth, Int32 pHeight, Int32 pDepth, RenderTextureFormat format)
	{
		return new RenderTexture(pWidth, pHeight, pDepth, format)
		{
			name = texName,
			hideFlags = HideFlags.DontSave
		};
	}

	public static String getUniqueTextureName()
	{
		return "OffscreenTexture_ID" + getNextObjID();
	}

	private static Int32 getNextObjID()
	{
		return s_freeObjID++;
	}
}
