using System;
using Legacy.Utilities;
using UnityEngine;

namespace Legacy.EffectEngine.Effects
{
	public class DetachAndFollowParent : MonoBehaviour
	{
		[SerializeField]
		private Single FOLLOW_SMOOTH_TIME = 10f;

		[SerializeField]
		private Single DESTROY_DELAY;

		[SerializeField]
		private Boolean STOP_PARTICLES;

		[SerializeField]
		private Boolean STOP_AUDIO;

		[SerializeField]
		private Boolean STOP_PROJECTOR;

		[SerializeField]
		private Boolean REWIND_TWEENERS;

		private Transform m_parent;

		private Vector3 m_offset;

		private Vector3 m_v = Vector3.zero;

		private Boolean m_isDestroyed;

		public Boolean IsDestroyed => m_isDestroyed;

	    private void Start()
		{
			if (transform.parent == null)
			{
				Debug.LogError("DetachAndFollowParent: parent is null!");
				Destroy(this);
				return;
			}
			m_parent = transform.parent;
			transform.parent = null;
			m_offset = transform.position - m_parent.position;
		}

		private void Update()
		{
			if (m_parent == null)
			{
				if (!m_isDestroyed)
				{
					m_isDestroyed = true;
					Destroy(gameObject, DESTROY_DELAY);
					if (STOP_PARTICLES)
					{
						ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
						foreach (ParticleSystem particleSystem in componentsInChildren)
						{
							particleSystem.Stop();
						}
					}
					if (STOP_AUDIO)
					{
						AudioObject[] componentsInChildren2 = GetComponentsInChildren<AudioObject>();
						foreach (AudioObject audioObject in componentsInChildren2)
						{
							audioObject.Stop();
						}
					}
					if (STOP_PROJECTOR)
					{
						ProjectorTweenAlphaTween[] componentsInChildren3 = GetComponentsInChildren<ProjectorTweenAlphaTween>();
						foreach (ProjectorTweenAlphaTween projectorTweenAlphaTween in componentsInChildren3)
						{
							projectorTweenAlphaTween.FadeOut();
						}
					}
					if (REWIND_TWEENERS)
					{
						UITweener[] componentsInChildren4 = GetComponentsInChildren<UITweener>(true);
						foreach (UITweener uitweener in componentsInChildren4)
						{
							uitweener.Play(false);
						}
					}
				}
			}
			else
			{
				transform.position = Vector3.SmoothDamp(transform.position, m_parent.position + m_offset, ref m_v, FOLLOW_SMOOTH_TIME);
			}
		}
	}
}
