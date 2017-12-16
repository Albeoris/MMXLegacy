using System;
using Legacy.Core.Api;
using UnityEngine;

namespace Legacy.Audio
{
	public abstract class SoundSourceBase : MonoBehaviour
	{
		[SerializeField]
		protected internal String m_AudioID = String.Empty;

		[SerializeField]
		protected internal Single m_AudioVolume = 1f;

		[SerializeField]
		protected Boolean m_IsPlayedDuringDawn = true;

		[SerializeField]
		protected Boolean m_IsPlayedDuringDay = true;

		[SerializeField]
		protected Boolean m_IsPlayedDuringDusk = true;

		[SerializeField]
		protected Boolean m_IsPlayedDuringNight = true;

		private Transform m_CachedTransform;

		private GameObject m_CachedGameObject;

		public String AudioID => m_AudioID;

	    public Single AudioVolume => m_AudioVolume;

	    public new Transform transform
		{
			get
			{
				if (m_CachedTransform == null)
				{
					m_CachedTransform = base.transform;
				}
				return m_CachedTransform;
			}
			private set => m_CachedTransform = value;
	    }

		public new GameObject gameObject
		{
			get
			{
				if (m_CachedGameObject == null)
				{
					m_CachedGameObject = base.gameObject;
				}
				return m_CachedGameObject;
			}
			private set => m_CachedGameObject = value;
		}

		protected virtual void Awake()
		{
			transform = base.transform;
			gameObject = base.gameObject;
		}

		protected virtual void Start()
		{
			if (!AudioManager.Instance.IsValidAudioID(m_AudioID))
			{
				Debug.LogError("AudioID: '" + name + "' not found!", this);
				enabled = false;
			}
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, "Sound", false);
		}

		protected virtual Boolean IsSoundPlayable()
		{
			switch (LegacyLogic.Instance.GameTime.DayState)
			{
			case EDayState.DAWN:
				return m_IsPlayedDuringDawn;
			case EDayState.DAY:
				return m_IsPlayedDuringDay;
			case EDayState.DUSK:
				return m_IsPlayedDuringDusk;
			case EDayState.NIGHT:
				return m_IsPlayedDuringNight;
			default:
				return true;
			}
		}
	}
}
