using System;
using Legacy;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;

public class VeilOfFatigue1 : MonoBehaviour
{
	private Single m_BlurFactor = 1f;

	private Single m_BlurFactorTarget = 0.9f;

	private BlurEffectC m_BlurEffect;

	private Single speed = 1f;

	public Int32 DownSample = 1;

	private void Awake()
	{
		enabled = false;
	}

	private void OnBeginEffect(UnityEventArgs<FXArgs> p_args)
	{
		m_BlurEffect = FXMainCamera.Instance.CurrentCamera.gameObject.AddComponent<BlurEffectC>();
		m_BlurEffect.blurShader = Shader.Find("Hidden/BlurEffectConeTap");
		m_BlurEffect.DownSample = 1f;
		enabled = true;
		m_BlurFactorTarget = 0f;
	}

	private void OnEndEffect(UnityEventArgs<FXArgs> e)
	{
		enabled = true;
		m_BlurFactorTarget = 1f;
	}

	private void Update()
	{
		if (m_BlurFactorTarget == 0f)
		{
			speed = 2f;
		}
		if (m_BlurFactorTarget == 1f)
		{
			speed = 0.5f;
		}
		m_BlurFactor = Mathf.Lerp(m_BlurFactor, m_BlurFactorTarget, Time.deltaTime * speed);
		m_BlurEffect.DownSample = DownSample * m_BlurFactor;
		if (m_BlurFactor == m_BlurFactorTarget)
		{
			enabled = false;
		}
		if (m_BlurFactorTarget == 1f && DownSample * m_BlurFactor > 0.8f)
		{
			Destroy(m_BlurEffect);
			Destroy(gameObject);
		}
	}
}
