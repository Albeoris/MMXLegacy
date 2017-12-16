using System;
using System.Collections;
using UnityEngine;

namespace Legacy.Audio
{
	[AddComponentMenu("MM Legacy/Audio/Animation Sound Source")]
	public class AnimationSoundSource : MonoBehaviour
	{
		public String[] m_Presets = Arrays<String>.Empty;

		public Single[] m_Durations = Arrays<Single>.Empty;

		public void PlayPreset(Int32 p_Index)
		{
			if (m_Presets.Length == m_Durations.Length && m_Presets.Length > p_Index && p_Index >= 0)
			{
				if (m_Durations[p_Index] != -1f)
				{
					PlayOneShot(m_Presets[p_Index] + "|" + m_Durations[p_Index]);
				}
				else
				{
					PlayOneShot(m_Presets[p_Index]);
				}
			}
			else
			{
				Debug.LogError("AnimationSoundSource: PlayPreset: preset '" + p_Index + "' not found!");
			}
		}

		public void PlayOneShot(String p_Params)
		{
			if (p_Params.Contains("|"))
			{
				String[] array = p_Params.Split(new Char[]
				{
					'|'
				});
				AudioObject p_Obj = AudioController.Play(array[0], transform);
				StartCoroutine(StopDelayed(p_Obj, Single.Parse(array[1])));
			}
			else
			{
				AudioController.Play(p_Params, transform);
			}
		}

		private IEnumerator StopDelayed(AudioObject p_Obj, Single p_Duration)
		{
			yield return new WaitForSeconds(p_Duration);
			if (p_Obj != null && p_Obj.IsPlaying())
			{
				p_Obj.Stop();
			}
			yield break;
		}
	}
}
