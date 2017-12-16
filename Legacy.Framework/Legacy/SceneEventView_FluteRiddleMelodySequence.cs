using System;
using System.Collections;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/Scene/SceneEventView_FluteRiddleMelodySequence")]
	public class SceneEventView_FluteRiddleMelodySequence : BaseView
	{
		[SerializeField]
		private String m_viewListenCommandName;

		[SerializeField]
		private ToneEvent[] m_ToneSequence;

		[SerializeField]
		private String m_FluteTheme;

		[SerializeField]
		private Single m_FluteThemeDuration;

		[SerializeField]
		private String m_CrystalTheme;

		[SerializeField]
		private Single m_soundVolume = 1f;

		protected override void Awake()
		{
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
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
					StartCoroutine(PlaySequence());
				}
				else if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Deactivate")
				{
					StopAllCoroutines();
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}

		[ContextMenu("Play Sequence")]
		private void Activate()
		{
			StartCoroutine(PlaySequence());
		}

		private IEnumerator PlaySequence()
		{
			AudioController.Stop(m_FluteTheme);
			AudioController.Play(m_FluteTheme, transform, m_soundVolume, 0f, 0f);
			yield return new WaitForSeconds(m_FluteThemeDuration);
			AudioController.Stop(m_CrystalTheme);
			AudioController.Play(m_CrystalTheme, transform, m_soundVolume, 0f, 0f);
			foreach (ToneEvent Tone in m_ToneSequence)
			{
				yield return new WaitForSeconds(Tone.WaitTime);
				Tone.ToneCrystal.ActivateWithoutSound();
			}
			yield break;
		}

		[Serializable]
		public class ToneEvent
		{
			public SceneEventView_FluteRiddleCrystal ToneCrystal;

			public Single WaitTime;
		}
	}
}
