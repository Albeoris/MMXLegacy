using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Audio
{
	public static class AudioHelper
	{
		private static Single s_defaultGUIVolume = -1f;

		private static Dictionary<String, Single> s_defaultCategoryVolumes = new Dictionary<String, Single>();

		public static AudioObject PlayWithProbabilityOfFirstItem(String p_AudioID, Transform p_Transform, Single p_volume, Single p_delay, Single p_startTime)
		{
			AudioItem audioItem = AudioController.GetAudioItem(p_AudioID);
			if (audioItem != null && audioItem.subItems.Length > 0)
			{
				AudioSubItem audioSubItem = audioItem.subItems[0];
				if (audioSubItem.Probability >= Random.Value)
				{
					return AudioController.Play(p_AudioID, p_Transform, p_volume, p_delay, p_startTime);
				}
			}
			else
			{
				Debug.LogError("Could not find Audio for AudioID=" + p_AudioID);
			}
			return null;
		}

		public static void MuteGUI()
		{
			if (AudioController.GetCategory("GUI") == null)
			{
				Debug.LogError("AudioHelper: MuteGUI: 'GUI' category not found!");
				return;
			}
			if (s_defaultGUIVolume == -1f)
			{
				s_defaultGUIVolume = AudioController.GetCategory("GUI").Volume;
			}
			AudioController.GetCategory("GUI").Volume = 0f;
		}

		public static void UnmuteGUI()
		{
			if (s_defaultGUIVolume == -1f)
			{
				Debug.LogError("AudioHelper: MuteGUI: 'GUI' category was never muted! Cannot not unmute!");
				return;
			}
			if (AudioController.GetCategory("GUI") == null)
			{
				Debug.LogError("AudioHelper: MuteGUI: 'GUI' category not found!");
				return;
			}
			AudioController.GetCategory("GUI").Volume = s_defaultGUIVolume;
			s_defaultGUIVolume = -1f;
		}

		public static void SetVolume(Single p_sfxVolume, Single p_partyBarkVolume, Single p_musicVolume)
		{
			SetVolumeAll(p_sfxVolume, p_partyBarkVolume);
			MusicController.Instance.SetVolume(GetScaledVolumeMusic(p_musicVolume));
		}

		public static void SetVolumeForCtrl(Single p_sfxVolume, Single p_partyBarkVolume, AudioController p_ctrl)
		{
			foreach (AudioCategory audioCategory in p_ctrl.AudioCategories)
			{
				if (audioCategory.parentCategory == null && !audioCategory.Name.Contains("Music") && !audioCategory.Name.Contains("music"))
				{
					if (!s_defaultCategoryVolumes.ContainsKey(audioCategory.Name))
					{
						s_defaultCategoryVolumes[audioCategory.Name] = audioCategory.Volume;
					}
					Single num = GetScaledVolumeSFX(p_sfxVolume);
					if (audioCategory.Name.Contains("Bark") || audioCategory.Name.Contains("bark"))
					{
						num = GetScaledVolumePartyBarks(p_partyBarkVolume);
					}
					else if (audioCategory.Name.Contains("VoiceOver2d") || audioCategory.Name.Contains("voiceover2d"))
					{
						num = GetScaledVolumeVoiceOver2d(p_partyBarkVolume);
					}
					audioCategory.Volume = num * s_defaultCategoryVolumes[audioCategory.Name];
				}
			}
		}

		private static void SetVolumeAll(Single p_sfxVolume, Single p_partyBarkVolume)
		{
			AudioController[] array = (AudioController[])UnityEngine.Object.FindObjectsOfType(typeof(AudioController));
			foreach (AudioController p_ctrl in array)
			{
				SetVolumeForCtrl(p_sfxVolume, p_partyBarkVolume, p_ctrl);
			}
		}

		private static Single GetScaledVolumeMusic(Single p_volume)
		{
			return 0.55f * p_volume;
		}

		private static Single GetScaledVolumeSFX(Single p_volume)
		{
			return 0.55f * p_volume;
		}

		private static Single GetScaledVolumePartyBarks(Single p_volume)
		{
			return 1f * p_volume;
		}

		private static Single GetScaledVolumeVoiceOver2d(Single p_volume)
		{
			return 1f * p_volume;
		}
	}
}
