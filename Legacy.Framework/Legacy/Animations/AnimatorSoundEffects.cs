using System;
using System.Collections;
using Legacy.Audio;
using UnityEngine;

namespace Legacy.Animations
{
	public class AnimatorSoundEffects : MonoBehaviour
	{
		private AudioObject m_loopingSound;

		private Transform m_transform;

		private AudioRequest m_request;

		[SerializeField]
		private String m_audioPrefix;

		public Boolean IsMuted { get; set; }

		public void PlaySound(String audioID)
		{
			if (IsMuted)
			{
				return;
			}
			if (m_loopingSound != null)
			{
				if (m_loopingSound.audio.loop)
				{
					m_loopingSound.audio.loop = false;
				}
				else
				{
					StartCoroutine(StopLoopAfter(m_loopingSound.clipLength - m_loopingSound.audio.time + 0.3f, m_loopingSound));
					m_loopingSound = null;
				}
			}
			if (!AudioManager.Instance.IsValidAudioID(audioID))
			{
				Debug.LogError("AnimatorSoundEffects; PlaySound: AudioID '" + audioID + "' not found!", this);
				return;
			}
			if (AudioManager.Instance.InAudioRange(m_transform.position, audioID))
			{
				if (AudioManager.Instance.IsAudioIDLoaded(audioID))
				{
					AudioObject audioObject = AudioHelper.PlayWithProbabilityOfFirstItem(audioID, transform, 1f, 0f, 0f);
					if (audioObject != null && audioObject.audioItem.Loop != AudioItem.LoopMode.DoNotLoop)
					{
						m_loopingSound = audioObject;
					}
				}
				else
				{
					if (m_request != null && m_request.IsLoading)
					{
						if (m_request.CategoryName == AudioManager.Instance.FindCategoryNameByAudioID(audioID))
						{
							return;
						}
						m_request.AbortLoad();
					}
					m_request = AudioManager.Instance.RequestByAudioID(audioID, 5, delegate(AudioRequest a)
					{
						if (a.Controller != null)
						{
							AudioObject audioObject2 = AudioHelper.PlayWithProbabilityOfFirstItem(audioID, transform, 1f, 0f, 0f);
							if (audioObject2 != null && audioObject2.audioItem.Loop != AudioItem.LoopMode.DoNotLoop)
							{
								m_loopingSound = audioObject2;
							}
						}
					});
				}
			}
		}

		private void Awake()
		{
			String categoryName = "AudioToolkit/AudioCategories/AudioController_" + m_audioPrefix;
			AudioManager.Instance.AddRefController(categoryName);
			m_transform = transform;
		}

		private void OnDestroy()
		{
			if (AudioManager.ExistInstance)
			{
				String categoryName = "AudioToolkit/AudioCategories/AudioController_" + m_audioPrefix;
				AudioManager.Instance.ReleaseRefController(categoryName);
			}
		}

		private IEnumerator StopLoopAfter(Single p_Time, AudioObject p_Audio)
		{
			yield return new WaitForSeconds(p_Time);
			if (p_Audio != null)
			{
				p_Audio.DestroyAudioObject();
			}
			yield break;
		}
	}
}
