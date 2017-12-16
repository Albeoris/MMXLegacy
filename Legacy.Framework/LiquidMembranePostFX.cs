using System;
using System.Collections;
using Legacy;
using Legacy.EffectEngine.Effects;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LiquidMembranePostFX : HitAbsorbingFXBase
{
	private Single MIN_FX_TIME__DURING_PARTY_TURN = 0.7f;

	private Single IDLE_TINT_AMOUNT = 0.1f;

	private Single IDLE_AMOUNT_MIN = 0.005f;

	private Single IDLE_AMOUNT_MAX = 0.01f;

	private Single ACTIVE_TINT_AMOUNT = 1f;

	private Single ACTIVE_AMOUNT_MIN = 0.05f;

	private Single ACTIVE_AMOUNT_MAX = 0.0785f;

	private Single AMOUNT_ANIM_SPEED = 1f;

	private Vector2 UV_ANIM_SPEED = Vector2.one * 0.1f;

	private Single STATE_CHANGE_SPEED = 2f;

	private Single m_currBumpAmt;

	private Single m_currTintAmt = 0.0001f;

	private Vector2 m_currUV = Vector2.zero;

	private Shader m_shader;

	private Material m_Material;

	private Boolean m_isDestroyed;

	private Material material
	{
		get
		{
			if (m_Material == null)
			{
				m_Material = new Material(m_shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		}
	}

	private void Awake()
	{
		m_shader = Shader.Find("Hidden/LiquidMembranePostFXShader");
		material.SetTexture("_TintTex", Helper.ResourcesLoad<Texture2D>("FXMaterials/LiquidMembraneTintTex"));
		material.SetTexture("_BumpMap", Helper.ResourcesLoad<Texture2D>("FXMaterials/LiquidMembraneDistortTex"));
	}

	protected override void Update()
	{
		base.Update();
		if (m_isDestroyed)
		{
			m_currTintAmt = Mathf.Lerp(m_currTintAmt, 0.0001f, Time.deltaTime * STATE_CHANGE_SPEED);
			m_currBumpAmt = Mathf.Lerp(m_currBumpAmt, 0f, Time.deltaTime * STATE_CHANGE_SPEED);
			if (m_currTintAmt < 0.01f && m_currBumpAmt < 0.1f)
			{
				UnityEngine.Object.Destroy(this);
			}
		}
		else if (m_isAbsorbingHit)
		{
			m_currTintAmt = Mathf.Lerp(m_currTintAmt, ACTIVE_TINT_AMOUNT, Time.deltaTime * STATE_CHANGE_SPEED);
			m_currBumpAmt = Mathf.Lerp(m_currBumpAmt, ACTIVE_AMOUNT_MIN + Mathf.PerlinNoise(Time.time * AMOUNT_ANIM_SPEED, Time.time * AMOUNT_ANIM_SPEED) * (ACTIVE_AMOUNT_MAX - ACTIVE_AMOUNT_MIN), Time.deltaTime * STATE_CHANGE_SPEED);
		}
		else
		{
			m_currTintAmt = Mathf.Lerp(m_currTintAmt, IDLE_TINT_AMOUNT, Time.deltaTime * STATE_CHANGE_SPEED);
			m_currBumpAmt = Mathf.Lerp(m_currBumpAmt, IDLE_AMOUNT_MIN + Mathf.PerlinNoise(Time.time * AMOUNT_ANIM_SPEED, Time.time * AMOUNT_ANIM_SPEED) * (IDLE_AMOUNT_MAX - IDLE_AMOUNT_MIN), Time.deltaTime * STATE_CHANGE_SPEED);
		}
		material.SetFloat("_TintAmt", m_currTintAmt);
		material.SetFloat("_BumpAmt", m_currBumpAmt);
		m_currUV.x = m_currUV.x + UV_ANIM_SPEED.x * Time.deltaTime * (Mathf.PerlinNoise(m_currUV.x, m_currUV.y) - 0.5f);
		m_currUV.y = m_currUV.y + UV_ANIM_SPEED.y * Time.deltaTime;
		material.SetTextureOffset("_TintTex", m_currUV);
		material.SetTextureOffset("_BumpMap", m_currUV);
	}

	protected override void ShowAbsorbHitFX()
	{
		AudioController.Play("LiquidMembraneHit");
	}

	protected override IEnumerator ShowStandbyFX()
	{
		yield return new WaitForEndOfFrame();
		yield break;
	}

	protected override Single MinFXTimeDuringPartyTurn()
	{
		return MIN_FX_TIME__DURING_PARTY_TURN;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, material);
	}

	private void OnDisable()
	{
		if (m_Material != null)
		{
			DestroyImmediate(m_Material);
			m_Material = null;
		}
	}

	private void Destroy()
	{
		m_isDestroyed = true;
	}
}
