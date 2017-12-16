using System;
using Legacy.Audio;
using UnityEngine;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Audio/Play Sound On Destroy")]
	public class PlaySoundOnDestroy : MonoBehaviour
	{
		public String m_AudioID;

		public void OnDestroy()
		{
			if (!AudioManager.Instance.IsValidAudioID(m_AudioID))
			{
				Debug.LogError("PlaySoundOnDestroy: AudioID not found! " + m_AudioID);
				return;
			}
			AudioManager.Instance.RequestPlayAudioID(m_AudioID, 0, -1f, transform.position, null, 1f, 0f, 0f, null);
		}
	}
}
