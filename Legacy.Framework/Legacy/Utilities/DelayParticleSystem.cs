using System;
using System.Collections;
using UnityEngine;

namespace Legacy.Utilities
{
	[AddComponentMenu("MM Legacy/Utility/DelayParticleSystem")]
	public class DelayParticleSystem : AutoDestructParticleSystem
	{
		[SerializeField]
		private Single m_StartDelayMin = 3f;

		[SerializeField]
		private Single m_StartDelayMax = 4f;

		[SerializeField]
		private Single m_StopAudioAfterSec = -1f;

		[SerializeField]
		private String m_AudioID = String.Empty;

		[SerializeField]
		private Boolean m_IsAudioAttachedToTransform = true;

		private AudioObject m_AudioObj;

		protected override void Awake()
		{
			base.Awake();
			m_StartDelayMin = Mathf.Clamp(m_StartDelayMin, 0f, m_StartDelayMin);
			m_StartDelayMax = Mathf.Clamp(m_StartDelayMax, m_StartDelayMin, Single.MaxValue);
		}

		private void Start()
		{
			Single num = Random.Range(m_StartDelayMin, m_StartDelayMax);
			if (num > 0f)
			{
				Stop();
				Clear();
				SetStartDelay(num);
				Play();
			}
			if (!String.IsNullOrEmpty(m_AudioID))
			{
				StartCoroutine(PlaySound(num));
			}
		}

		private void OnDestroy()
		{
			if (m_AudioObj != null && m_AudioObj.IsPlaying())
			{
				m_AudioObj.transform.parent = null;
				m_AudioObj.Stop();
			}
		}

		private IEnumerator PlaySound(Single p_Delay)
		{
			if (p_Delay > 0f)
			{
				yield return new WaitForSeconds(p_Delay);
			}
			if (m_IsAudioAttachedToTransform)
			{
				m_AudioObj = AudioController.Play(m_AudioID, transform);
			}
			else
			{
				m_AudioObj = AudioController.Play(m_AudioID, transform.position, null);
			}
			if (m_StopAudioAfterSec > 0f)
			{
				yield return new WaitForSeconds(m_StopAudioAfterSec);
				if (m_AudioObj != null && m_AudioObj.IsPlaying())
				{
					m_AudioObj.transform.parent = null;
					m_AudioObj.Stop();
				}
			}
			yield break;
		}
	}
}
