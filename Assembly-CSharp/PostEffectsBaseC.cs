using System;
using UnityEngine;

public class PostEffectsBaseC : MonoBehaviour
{
	protected Boolean supportHDRTextures = true;

	protected Boolean isSupported = true;

	public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			enabled = false;
			return null;
		}
		if (s.isSupported && m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			NotSupported();
			Debug.LogError(String.Concat(new String[]
			{
				"The shader ",
				s.ToString(),
				" on effect ",
				ToString(),
				" is not supported on this platform!"
			}));
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}

	private Material CreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			return null;
		}
		if (m2Create && m2Create.shader == s && s.isSupported)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if (m2Create)
		{
			return m2Create;
		}
		return null;
	}

	private void OnEnable()
	{
		isSupported = true;
	}

	public Boolean CheckSupport()
	{
		return CheckSupport(false);
	}

	protected virtual Boolean CheckResources()
	{
		Debug.LogWarning("CheckResources () for " + ToString() + " should be overwritten.");
		return isSupported;
	}

	protected virtual void Start()
	{
		CheckResources();
	}

	public Boolean CheckSupport(Boolean needDepth)
	{
		isSupported = true;
		supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			NotSupported();
			return false;
		}
		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			NotSupported();
			return false;
		}
		if (needDepth)
		{
			camera.depthTextureMode |= DepthTextureMode.Depth;
		}
		return true;
	}

	public Boolean CheckSupport(Boolean needDepth, Boolean needHdr)
	{
		if (!CheckSupport(needDepth))
		{
			return false;
		}
		if (needHdr && !supportHDRTextures)
		{
			NotSupported();
			return false;
		}
		return true;
	}

	public void ReportAutoDisable()
	{
		Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
	}

	private Boolean CheckShader(Shader s)
	{
		Debug.Log(String.Concat(new String[]
		{
			"The shader ",
			s.ToString(),
			" on effect ",
			ToString(),
			" is not part of the Unity 3.2f+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package."
		}));
		if (!s.isSupported)
		{
			NotSupported();
			return false;
		}
		return false;
	}

	private void NotSupported()
	{
		enabled = false;
		isSupported = false;
	}

	private void DrawBorder(RenderTexture dest, Material material)
	{
		RenderTexture.active = dest;
		Boolean flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (Int32 i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			Single y;
			Single y2;
			if (flag)
			{
				y = 1f;
				y2 = 0f;
			}
			else
			{
				y = 0f;
				y2 = 1f;
			}
			Single x = 0f;
			Single x2 = 0f + 1f / (dest.width * 1f);
			Single y3 = 0f;
			Single y4 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			x = 1f - 1f / (dest.width * 1f);
			x2 = 1f;
			y3 = 0f;
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			x = 0f;
			x2 = 1f;
			y3 = 0f;
			y4 = 0f + 1f / (dest.height * 1f);
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			x = 0f;
			x2 = 1f;
			y3 = 1f - 1f / (dest.height * 1f);
			y4 = 1f;
			GL.TexCoord2(0f, y);
			GL.Vertex3(x, y3, 0.1f);
			GL.TexCoord2(1f, y);
			GL.Vertex3(x2, y3, 0.1f);
			GL.TexCoord2(1f, y2);
			GL.Vertex3(x2, y4, 0.1f);
			GL.TexCoord2(0f, y2);
			GL.Vertex3(x, y4, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}
}
