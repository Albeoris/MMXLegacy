using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class OutlineGlowFX : OutlineFXBase
	{
		private const Single BLUR_SPREAD_INITIAL = 0.2f;

		private const Single BLUR_SPREAD_INC_INITIAL = 0.5f;

		private const Single TIME_RETURN_WIDTH_TO_NORMAL = 0.25f;

		public Single GLOW_INTENSITY = 0.5f;

		public Single GLOW_INTENSITY_INC = 3f;

		public Single BLUR_SPREAD = 0.2f;

		public Single BLUR_SPREAD_INC = 0.5f;

		private Boolean m_isOutlineShown;

		private Color m_outlineColor = Color.clear;

		private Single m_transitionTime;

		private BackgroundGlowFX.Registration m_Registration;

		private Camera m_cameraUsedAtShowTime;

		private Boolean m_isDestroyed;

		public override Boolean IsDestroyed => m_isDestroyed;

	    public override Boolean IsOutlineShown => m_isOutlineShown;

	    public override Color OutlineColor => m_outlineColor;

	    private void Update()
		{
			if (!m_isDestroyed && m_Registration != null)
			{
				Single num = 0f;
				Single num2 = 0f;
				Single num3 = 1f - (m_transitionTime - Time.time) / 0.25f;
				if (num3 >= 0f)
				{
					if (num3 <= 0.5f)
					{
						num = num3 * BLUR_SPREAD_INC;
						num2 = num3 * GLOW_INTENSITY_INC;
					}
					else
					{
						num = (1f - num3) * BLUR_SPREAD_INC;
						num2 = (1f - num3) * GLOW_INTENSITY_INC;
					}
				}
				if (num < 0f)
				{
					num = 0f;
				}
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				m_Registration.BlurSpread = BLUR_SPREAD + num;
				m_Registration.GlowIntensity = GLOW_INTENSITY + num2;
			}
		}

		public override void SetGlobalIntensity(Single intensity)
		{
			BLUR_SPREAD = 0.2f * intensity;
			BLUR_SPREAD_INC = 0.5f * intensity;
		}

		public override void ShowOutline(Boolean doHighlight, Color color)
		{
			if (m_isDestroyed)
			{
				Debug.LogError("This OutlineGlowFX has been destroyed in this frame you cannot call any functions!");
				return;
			}
			if (Camera.main != null)
			{
				m_outlineColor = color;
				m_cameraUsedAtShowTime = Camera.main;
				GlowFXControlScript.SetObjectGlowDeep(gameObject, color, 1f, true, m_cameraUsedAtShowTime);
				m_Registration = RedrawCameraGlow.GetRedrawCameraGlowScript(m_cameraUsedAtShowTime, false).RegistrationData;
				m_Registration.DownScale = 2;
				m_Registration.BlurSpread = BLUR_SPREAD;
				m_Registration.GlowIntensity = GLOW_INTENSITY;
				m_isOutlineShown = true;
				m_transitionTime = ((!doHighlight) ? Time.time : (Time.time + 0.25f));
			}
		}

		public override void HideOutline()
		{
			if (m_isDestroyed)
			{
				Debug.LogError("This OutlineGlowFX has been destroyed in this frame you cannot call any functions!");
				return;
			}
			if (m_isOutlineShown)
			{
				if (m_cameraUsedAtShowTime != null)
				{
					GlowFXControlScript.RemoveGlowDeep(gameObject, m_cameraUsedAtShowTime);
				}
				m_Registration = null;
				m_isOutlineShown = false;
				m_outlineColor = Color.clear;
			}
		}

		public override void Destroy()
		{
			if (!m_isDestroyed)
			{
				HideOutline();
				m_isDestroyed = true;
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}
