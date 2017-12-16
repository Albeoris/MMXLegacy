using System;
using Legacy.Core.Api;
using Legacy.Core.MapLoading;
using UnityEngine;

namespace Legacy.Audio
{
	[AddComponentMenu("MM Legacy/Audio/Audio Listener Controller")]
	public class AudioListenerController : MonoBehaviour
	{
		private AudioListener m_FallbackListener;

		private Boolean IsPlayerSpawning => LegacyLogic.Instance.MapLoader.State > EMapLoaderState.LOADING_DYNAMIC_OBJECTS;

	    private void Update()
		{
			if (m_FallbackListener != null)
			{
				if (IsPlayerSpawning)
				{
					Destroy(m_FallbackListener);
				}
				else
				{
					CheckAudioListeners();
				}
			}
		}

		private void OnLevelWasLoaded(Int32 p_Level)
		{
			CheckAudioListeners();
		}

		private void CheckAudioListeners()
		{
			UnityEngine.Object[] array = FindObjectsOfType(typeof(AudioListener));
			if (array.Length == 0)
			{
				if (!IsPlayerSpawning)
				{
					m_FallbackListener = gameObject.AddComponent<AudioListener>();
				}
			}
			else if (array.Length > 1 && m_FallbackListener != null)
			{
				Destroy(m_FallbackListener);
			}
		}
	}
}
