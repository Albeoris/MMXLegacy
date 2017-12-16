using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class OutlineParticleSystemFX : OutlineFXBase
	{
		private const Single COLOR_FACTOR = 0.75f;

		[SerializeField]
		private ParticleSystem m_oneShotHighlightParticles;

		[SerializeField]
		private ParticleSystem m_loopingParticles;

		private Boolean m_isOutlineShown;

		private Boolean m_isDestroyed;

		private Color m_currentColor = Color.black;

		public override Boolean IsDestroyed => m_isDestroyed;

	    public override Boolean IsOutlineShown => m_isOutlineShown;

	    public override Color OutlineColor => m_currentColor;

	    public override void SetGlobalIntensity(Single intensity)
		{
		}

		public override void ShowOutline(Boolean doHighlight, Color color)
		{
			if (m_isDestroyed)
			{
				Debug.LogError("This OutlineParticleSystemFX has been destroyed in this frame you cannot call any functions!");
				return;
			}
			m_currentColor = color;
			Color color2 = color * 0.75f - particleSystem.startColor;
			if (doHighlight)
			{
				color2.a = 1f - particleSystem.startColor.a;
			}
			m_loopingParticles.startColor = color * 0.75f;
			if (m_loopingParticles.isPlaying && color2.a > 0f)
			{
				m_loopingParticles.Stop();
			}
			m_loopingParticles.Play(doHighlight);
			if (color2.a < 0f)
			{
				ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleSystem.particleCount];
				m_loopingParticles.GetParticles(array);
				for (Int32 i = 0; i < array.Length; i++)
				{
					Color32 color3 = array[i].color;
					color3.r = (Byte)Mathf.Clamp(color3.r + color2.r * 255f, 0f, 255f);
					color3.g = (Byte)Mathf.Clamp(color3.g + color2.g * 255f, 0f, 255f);
					color3.b = (Byte)Mathf.Clamp(color3.b + color2.b * 255f, 0f, 255f);
					color3.a = (Byte)Mathf.Clamp(color3.a + color2.a * 255f, 0f, 255f);
					array[i].color = color3;
				}
				m_loopingParticles.SetParticles(array, array.Length);
			}
			m_isOutlineShown = true;
		}

		public override void HideOutline()
		{
			if (m_isDestroyed)
			{
				Debug.LogError("This OutlineParticleSystemFX has been destroyed in this frame you cannot call any functions!");
				return;
			}
			if (m_isOutlineShown)
			{
				m_loopingParticles.Stop();
				m_loopingParticles.Clear();
				m_isOutlineShown = false;
			}
		}

		public override void Destroy()
		{
			if (!m_isDestroyed)
			{
				HideOutline();
				m_isDestroyed = true;
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		private void Start()
		{
			if (m_oneShotHighlightParticles == null || m_loopingParticles == null)
			{
				Debug.LogError("OutlineParticleSystemFX: particle systems are not assigned!");
			}
		}
	}
}
