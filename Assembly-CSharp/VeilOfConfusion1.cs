using System;
using Legacy;
using Legacy.EffectEngine;
using Legacy.EffectEngine.Effects;
using UnityEngine;

public class VeilOfConfusion1 : MonoBehaviour
{
	private DoubleViewEffect m_DoubleViewEffect;

	private TwirlEffect m_TwirlEffect;

	private Single speed;

	private Single moveSpeed = 4f;

	private Single m_moveTarget;

	private Single m_colorTarget;

	private Single m_turnTarget;

	private Single m_moveFactor;

	private Single m_colorFactor;

	private Single m_turnFactor;

	private void Awake()
	{
		enabled = false;
	}

	private void OnBeginEffect(UnityEventArgs<FXArgs> p_args)
	{
		m_DoubleViewEffect = FXMainCamera.Instance.CurrentCamera.gameObject.AddComponent<DoubleViewEffect>();
		m_DoubleViewEffect.invertShader = Shader.Find("Hidden/InvertColor");
		m_DoubleViewEffect.pixelmovement = 0f;
		m_DoubleViewEffect.colorInvert = 0.1f;
		m_TwirlEffect = FXMainCamera.Instance.CurrentCamera.gameObject.AddComponent<TwirlEffect>();
		m_TwirlEffect.shader = Shader.Find("Hidden/Twirt Effect Shader");
		m_TwirlEffect.radius.x = 0.6f;
		m_TwirlEffect.radius.y = 0.6f;
		m_TwirlEffect.angle = 1f;
		enabled = true;
		m_moveTarget = 100f;
		m_colorTarget = 1f;
		m_turnTarget = 190f;
	}

	private void OnEndEffect(UnityEventArgs<FXArgs> e)
	{
	}

	private void Update()
	{
		if (m_colorTarget == 1f)
		{
			speed = 2f;
		}
		if (m_colorTarget == 0f)
		{
			speed = 1f;
		}
		m_colorFactor = Mathf.Lerp(m_colorFactor, m_colorTarget, Time.deltaTime * speed);
		m_DoubleViewEffect.colorInvert = m_colorFactor;
		m_turnFactor = Mathf.Lerp(m_turnFactor, m_turnTarget, Time.deltaTime * (speed * 1.5f));
		m_TwirlEffect.angle = m_turnFactor;
		m_moveFactor = Mathf.Lerp(m_moveFactor, m_moveTarget, Time.deltaTime * moveSpeed);
		m_DoubleViewEffect.pixelmovement = m_moveFactor;
		if (m_moveFactor > 90f)
		{
			m_moveTarget = 0f;
			moveSpeed -= 1f;
		}
		if (m_moveFactor < 1f)
		{
			m_moveTarget = 100f;
			moveSpeed -= 1f;
		}
		if (m_colorFactor > 0.98f && m_turnFactor > 148f)
		{
			m_colorTarget = 0f;
			m_turnTarget = 0f;
		}
		if (m_TwirlEffect.angle < 0.2 && m_DoubleViewEffect.colorInvert < 0.1f)
		{
			Destroy(m_DoubleViewEffect);
			Destroy(m_TwirlEffect);
			Destroy(gameObject);
		}
	}
}
