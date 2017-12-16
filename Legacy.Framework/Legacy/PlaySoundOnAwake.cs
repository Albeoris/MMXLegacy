using System;
using Legacy.Audio;
using UnityEngine;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Audio/Play Sound On Awake")]
	public class PlaySoundOnAwake : MonoBehaviour
	{
		[SerializeField]
		private String m_AudioID;

		[SerializeField]
		private Boolean m_IsAttachedToTransform = true;

		[SerializeField]
		private Single m_PlayDelayed;

		private Single m_TargetTime;

		private Boolean m_SoundPlayed;

		private void Awake()
		{
			m_TargetTime = Time.time + m_PlayDelayed;
		}

		private void Update()
		{
			if (!m_SoundPlayed && m_TargetTime < Time.time)
			{
				m_SoundPlayed = true;
				enabled = false;
				PlaySound();
			}
		}

		private void PlaySound()
		{
			if (!AudioManager.Instance.IsValidAudioID(m_AudioID))
			{
				Debug.LogError("PlaySoundOnAwake: AudioID not found! " + m_AudioID);
				return;
			}
			if (m_IsAttachedToTransform)
			{
				AudioManager.Instance.RequestPlayAudioID(m_AudioID, 0, -1f, transform, 1f, 0f, 0f, null);
			}
			else
			{
				AudioManager.Instance.RequestPlayAudioID(m_AudioID, 0, -1f, transform.position, null, 1f, 0f, 0f, null);
			}
		}
	}
}
