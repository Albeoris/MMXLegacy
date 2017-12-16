using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Audio;
using UnityEngine;

namespace Legacy.Animations
{
	public class AnimSoundEffects : MonoBehaviour
	{
		[SerializeField]
		private String m_monsterAudioPrefix;

		[SerializeField]
		private MonsterSize m_monsterSize = MonsterSize.MEDIUM;

		private AudioObject m_loopingSound;

		private HashSet<EAnimType> m_missingAnimTypes;

		private HashSet<EAnimType> m_missingBarks;

		private void Awake()
		{
			String text = "AudioToolkit/AudioCategories/AudioController_" + m_monsterAudioPrefix;
			if (!AudioManager.Instance.IsValidCategory(text))
			{
				Debug.LogError("CategoryID: '" + text + "' not found!\nCompoent will be destroyed!", this);
				Destroy(this);
				return;
			}
			AudioManager.Instance.Request(text);
		}

		private void OnPlayAnimation(PlayAnimationUnityEventArgs e)
		{
			if (m_loopingSound != null)
			{
				if (m_loopingSound.audio.loop)
				{
					m_loopingSound.audio.loop = false;
				}
				else
				{
					AudioObject loopingSound = m_loopingSound;
					StartCoroutine(StopLoopAfter(m_loopingSound.clipLength - m_loopingSound.audio.time + 0.3f, loopingSound));
				}
			}
			if (m_missingAnimTypes == null)
			{
				FindMissingSounds();
			}
			if (!m_missingAnimTypes.Contains(e.Type))
			{
				Play(GetAudioID(e.Type));
			}
			else if (e.Type == EAnimType.MOVE || e.Type == EAnimType.TURN_LEFT || e.Type == EAnimType.TURN_RIGHT)
			{
				switch (m_monsterSize)
				{
				case MonsterSize.LIGHT:
					Play("Light_Move_Ground");
					break;
				case MonsterSize.MEDIUM:
					Play("Medium_Move_Ground");
					break;
				case MonsterSize.HEAVY:
					Play("Heavy_Move_Ground");
					break;
				}
			}
			if (!m_missingBarks.Contains(e.Type))
			{
				AudioHelper.PlayWithProbabilityOfFirstItem(getBarkID(GetAudioID(e.Type)), transform, 1f, 0f, 0f);
			}
		}

		private void FindMissingSounds()
		{
			m_missingAnimTypes = new HashSet<EAnimType>();
			m_missingBarks = new HashSet<EAnimType>();
			Boolean flag = false;
			foreach (EAnimType eanimType in EnumUtil<EAnimType>.Values)
			{
				String audioID = GetAudioID(eanimType);
				if (AudioController.GetAudioItem(audioID) == null)
				{
					m_missingAnimTypes.Add(eanimType);
				}
				else
				{
					flag = true;
				}
				if (AudioController.GetAudioItem(getBarkID(audioID)) == null)
				{
					m_missingBarks.Add(eanimType);
				}
			}
			if (!flag)
			{
				Debug.LogError(String.Concat(new String[]
				{
					"AnimSoundEffects: '",
					name,
					"' could not find any sounds for '",
					m_monsterAudioPrefix,
					"'"
				}));
			}
		}

		private IEnumerator StopLoopAfter(Single p_Time, AudioObject p_Audio)
		{
			yield return new WaitForSeconds(p_Time);
			if (p_Audio != null)
			{
				p_Audio.DestroyAudioObject();
				p_Audio = null;
			}
			yield break;
		}

		private void Play(String p_AudioID)
		{
			AudioObject audioObject = AudioController.Play(p_AudioID, transform);
			if (audioObject != null && audioObject.audioItem.Loop != AudioItem.LoopMode.DoNotLoop)
			{
				m_loopingSound = audioObject;
			}
		}

		private String GetAudioID(EAnimType p_Type)
		{
			return m_monsterAudioPrefix + p_Type.ToString().ToLowerInvariant();
		}

		private static String getBarkID(String audioID)
		{
			return audioID + "_bark";
		}

		public enum MonsterSize
		{
			LIGHT,
			MEDIUM,
			HEAVY
		}
	}
}
