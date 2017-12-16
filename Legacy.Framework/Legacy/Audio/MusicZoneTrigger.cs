using System;
using UnityEngine;

namespace Legacy.Audio
{
	[RequireComponent(typeof(Collider))]
	public class MusicZoneTrigger : MonoBehaviour
	{
		private const String TRIGGER_TAG = "Player";

		[SerializeField]
		private Int32 m_Priority;

		[SerializeField]
		private Single m_FadeIn = 1f;

		[SerializeField]
		private String m_MusikAudioID;

		private void Awake()
		{
			collider.isTrigger = true;
		}

		private void OnDestroy()
		{
			if (MusicController.Instance != null)
			{
				MusicController.Instance.DeregisterMusicZone(this);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Player") || (other.attachedRigidbody != null && !other.attachedRigidbody.CompareTag("Player")))
			{
				return;
			}
			if (MusicController.Instance != null)
			{
				MusicController.Instance.RegisterMusicZone(this, m_MusikAudioID, m_Priority, m_FadeIn);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Player") || (other.attachedRigidbody != null && !other.attachedRigidbody.CompareTag("Player")))
			{
				return;
			}
			if (MusicController.Instance != null)
			{
				MusicController.Instance.DeregisterMusicZone(this);
			}
		}
	}
}
