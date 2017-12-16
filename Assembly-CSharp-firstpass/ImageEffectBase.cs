using System;
using UnityEngine;

[AddComponentMenu("")]
[RequireComponent(typeof(Camera))]
public class ImageEffectBase : MonoBehaviour
{
	public Shader shader;

	private Material m_Material;

	protected virtual void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
		if (!shader || !shader.isSupported)
		{
			enabled = false;
		}
	}

	protected Material material
	{
		get
		{
			if (m_Material == null)
			{
				m_Material = new Material(shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		}
	}

	protected virtual void OnDisable()
	{
		if (m_Material)
		{
			DestroyImmediate(m_Material);
		}
	}
}
