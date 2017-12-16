using System;
using Legacy;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;

public class VeilOfDoom1 : MonoBehaviour
{
	private Single m_FogFactor;

	private Single m_FogFactorTarget = 0.01f;

	private GlobalFogC m_FogEffect;

	private Single speed;

	private Single globalDensity = 1f;

	private void Awake()
	{
		enabled = false;
	}

	private void OnBeginEffect(UnityEventArgs<FXArgs> p_args)
	{
		m_FogEffect = FXMainCamera.Instance.CurrentCamera.gameObject.AddComponent<GlobalFogC>();
		m_FogEffect.fogShader = Shader.Find("Hidden/GlobalFog");
		m_FogEffect.globalDensity = 0.01f;
		m_FogEffect.startDistance = 0.1f;
		m_FogEffect.heightScale = 100f;
		m_FogEffect.height = 100f;
		m_FogEffect.globalFogColor = Color.black;
		enabled = true;
		m_FogFactorTarget = 0.3f;
	}

	private void OnEndEffect(UnityEventArgs<FXArgs> e)
	{
		enabled = true;
		m_FogFactorTarget = 0f;
		Destroy(m_FogEffect, 4f);
		Destroy(gameObject, 4f);
	}

	private void Update()
	{
		if (m_FogFactorTarget == 0.3f)
		{
			speed = 4f;
		}
		if (m_FogFactorTarget == 0f)
		{
			speed = 1f;
		}
		m_FogFactor = Mathf.Lerp(m_FogFactor, m_FogFactorTarget, Time.deltaTime * speed);
		m_FogEffect.globalDensity = globalDensity * m_FogFactor;
		if (m_FogFactor == m_FogFactorTarget)
		{
			enabled = false;
		}
	}
}
