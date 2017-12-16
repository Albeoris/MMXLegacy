using System;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class TimeStopFX : MonoBehaviour
	{
		[SerializeField]
		private AnimationCurve m_fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[SerializeField]
		private AnimationCurve m_fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		[SerializeField]
		private AnimationCurve m_animCurve = AnimationCurve.EaseInOut(0f, 10f, 1f, 1f);

		[SerializeField]
		private String m_startSoundID = String.Empty;

		[SerializeField]
		private String m_loopSoundID = String.Empty;

		[SerializeField]
		private String m_endSoundID = String.Empty;

		private DetachAndFollowParent m_parent;

		private Single m_fadeInStartTime = -1f;

		private Single m_fadeOutStartTime = -1f;

		private Material m_material;

		private Vector3 m_maxScale = Vector3.one;

		private Single m_maxBumpAmp = -1f;

		private Single m_uvSpeedX = -1f;

		private AdvancedUVAnimatorAndScroller m_anim;

		private AudioObject m_loopSound;

		protected virtual void Awake()
		{
			m_fadeInStartTime = Time.time;
			m_parent = transform.parent.GetComponent<DetachAndFollowParent>();
			m_anim = GetComponent<AdvancedUVAnimatorAndScroller>();
			m_maxScale = transform.localScale;
			transform.localScale = Vector3.zero;
		}

		private void Update()
		{
			if (m_material == null)
			{
				m_material = renderer.sharedMaterial;
				m_maxBumpAmp = m_material.GetFloat("_BumpAmt");
				m_uvSpeedX = m_anim.m_uvSpeed.x;
				AudioController.Play(m_startSoundID);
				m_loopSound = AudioController.Play(m_loopSoundID);
			}
			if (m_parent.IsDestroyed)
			{
				if (m_fadeOutStartTime == -1f)
				{
					m_fadeOutStartTime = Time.time;
					AudioController.Play(m_endSoundID);
					if (m_loopSound != null)
					{
						m_loopSound.Stop();
						m_loopSound = null;
					}
				}
				SetFade(m_fadeOutCurve.Evaluate(Time.time - m_fadeOutStartTime));
			}
			else if (m_fadeInStartTime != -2f)
			{
				Single num = Time.time - m_fadeInStartTime;
				Single time = m_fadeInCurve.keys[m_fadeInCurve.length - 1].time;
				if (num >= time)
				{
					num = time;
					m_fadeInStartTime = -2f;
				}
				SetFade(m_fadeInCurve.Evaluate(num));
			}
		}

		private void SetFade(Single p_fade)
		{
			transform.localScale = m_maxScale * p_fade;
			m_material.SetFloat("_BumpAmt", m_maxBumpAmp * p_fade);
			m_material.SetFloat("_TintAmt", p_fade);
			m_anim.m_uvSpeed.x = m_animCurve.Evaluate(p_fade) * m_uvSpeedX;
		}
	}
}
