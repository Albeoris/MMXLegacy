using System;
using UnityEngine;

namespace Legacy.Audio
{
	[AddComponentMenu("MM Legacy/Audio/Sound Source")]
	public class SoundSource : SoundSourceBase
	{
		[SerializeField]
		private Single m_AudioSourceMinDist = 15f;

		[SerializeField]
		private Single m_AudioSourceMaxDist = 100f;

		[SerializeField]
		protected Single m_AudioDelay;

		[SerializeField]
		protected Single m_AudioStartTime;

		[SerializeField]
		protected Boolean m_PlayOnEnable = true;

		public Single AudioSourceMinDist
		{
			get => m_AudioSourceMinDist;
		    set => m_AudioSourceMinDist = value;
		}

		public Single AudioSourceMaxDist
		{
			get => m_AudioSourceMaxDist;
		    set => m_AudioSourceMaxDist = value;
		}

		public Single AudioDelay
		{
			get => m_AudioDelay;
		    set => m_AudioDelay = value;
		}

		public Single AudioStartTime
		{
			get => m_AudioStartTime;
		    set => m_AudioStartTime = value;
		}

		public void PlaySound()
		{
			if (!AudioManager.Instance.IsValidAudioID(m_AudioID))
			{
				Debug.LogError("AudioID: '" + m_AudioID + "' not found!\n", this);
				enabled = false;
				return;
			}
			AudioManager.Instance.RequestPlayAudioID(m_AudioID, 0, -1f, transform, m_AudioVolume, m_AudioDelay, m_AudioStartTime, delegate(AudioObject audioObj)
			{
				if (audioObj != null)
				{
					audioObj.audio.minDistance = m_AudioSourceMinDist;
					audioObj.audio.maxDistance = m_AudioSourceMaxDist;
				}
			});
		}

		protected virtual void OnEnable()
		{
			if (m_PlayOnEnable && IsSoundPlayable())
			{
				PlaySound();
			}
		}
	}
}
