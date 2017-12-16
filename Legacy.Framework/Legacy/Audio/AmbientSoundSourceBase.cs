using System;
using UnityEngine;

namespace Legacy.Audio
{
	public abstract class AmbientSoundSourceBase : SoundSourceBase
	{
		protected const Single REACTIVATE_AFTER = 5f;

		[SerializeField]
		protected Single m_RetryIntervalSeconds = 5f;

		protected Boolean m_IsPlaying;

		protected AudioObject m_AudioObj;

		protected Single m_ReactivateAt = -1f;

		protected virtual void OnNewAudioObject()
		{
		}

		protected override void Start()
		{
			if (!AudioManager.Instance.IsValidAudioID(m_AudioID))
			{
				Debug.LogError("AudioID: '" + m_AudioID + "' not found!\n", this);
				enabled = false;
				return;
			}
			base.Start();
			AudioManager.Instance.RequestByAudioID(m_AudioID);
		}

		protected virtual void Update()
		{
			if (!IsSoundPlayable())
			{
				if (m_IsPlaying)
				{
					if (m_AudioObj != null)
					{
						m_AudioObj.DestroyAudioObject();
						m_AudioObj = null;
					}
					m_IsPlaying = false;
				}
			}
			else if ((!m_IsPlaying || m_ReactivateAt <= Time.time) && (m_AudioObj == null || !m_AudioObj.IsPlaying()) && AudioManager.Instance.IsAudioIDLoaded(m_AudioID))
			{
				m_AudioObj = AudioHelper.PlayWithProbabilityOfFirstItem(m_AudioID, transform, m_AudioVolume, 0f, 0f);
				if (m_AudioObj != null)
				{
					OnNewAudioObject();
				}
				m_ReactivateAt = Time.time + m_RetryIntervalSeconds;
				m_IsPlaying = true;
			}
		}

		protected override void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, "AmbientSound", false);
		}
	}
}
