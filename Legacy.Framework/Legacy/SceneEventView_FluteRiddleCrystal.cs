using System;
using System.Collections;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/Scene/SceneEventView_FluteRiddleCrystal")]
	public class SceneEventView_FluteRiddleCrystal : BaseView
	{
		[SerializeField]
		private String m_viewListenCommandName;

		[SerializeField]
		private Light m_CrystalLight;

		[SerializeField]
		private Single m_LightIntensity;

		[SerializeField]
		private String m_PlayedTone;

		[SerializeField]
		private Single m_onTime;

		[SerializeField]
		private Single m_fadeOutTime;

		[SerializeField]
		private Single m_soundVolume = 1f;

		protected override void Awake()
		{
			AudioManager.Instance.RequestByAudioID(m_PlayedTone);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
			m_CrystalLight.intensity = 0f;
			m_CrystalLight.enabled = false;
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null && p_sender == null)
			{
				String[] array = stringEventArgs.text.Split(new Char[]
				{
					'_'
				});
				if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Activate")
				{
					StartCoroutine(PlayTone(true));
				}
				else if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Deactivate")
				{
					StopAllCoroutines();
				}
			}
		}

		[ContextMenu("Play Tone")]
		public void ActivateWithSound()
		{
			StartCoroutine(PlayTone(true));
		}

		[ContextMenu("Play Crystal Response")]
		public void ActivateWithoutSound()
		{
			StartCoroutine(PlayTone(false));
		}

		private IEnumerator PlayTone(Boolean p_playSound)
		{
			if (p_playSound)
			{
				AudioController.Stop(m_PlayedTone);
				AudioController.Play(m_PlayedTone, transform, m_soundVolume, 0f, 0f);
			}
			if (m_CrystalLight == null)
			{
				yield break;
			}
			Single time = 0f;
			m_CrystalLight.enabled = true;
			m_CrystalLight.intensity = m_LightIntensity;
			while (time <= m_onTime)
			{
				yield return new WaitForEndOfFrame();
				time += Time.deltaTime;
			}
			for (time = 0f; time <= m_fadeOutTime; time += Time.deltaTime)
			{
				Single relativeTime = (m_fadeOutTime - time) / m_fadeOutTime;
				m_CrystalLight.intensity = m_LightIntensity * relativeTime;
				yield return new WaitForEndOfFrame();
			}
			m_CrystalLight.enabled = false;
			yield break;
		}
	}
}
