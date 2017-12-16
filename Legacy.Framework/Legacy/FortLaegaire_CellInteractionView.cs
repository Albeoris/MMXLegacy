using System;
using System.Collections;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/FortLaegaire CellInteractionView")]
	public class FortLaegaire_CellInteractionView : BaseView
	{
		[SerializeField]
		private String m_knockSound = "Gold_loot";

		[SerializeField]
		private String m_InmateSound = "Gold_loot";

		[SerializeField]
		private Single m_soundDelay_KnockToInmate = 1f;

		[SerializeField]
		private Transform m_InmateSoundPosition;

		[SerializeField]
		private Single m_soundVolume = 1f;

		protected override void Awake()
		{
			base.Awake();
		}

		protected new InteractiveObject MyController => (InteractiveObject)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
			}
		}

		private void OnInteractiveObjectStateChanged(Object p_sender, EventArgs p_args)
		{
			InteractiveObject interactiveObject;
			if (p_sender is InteractiveObject)
			{
				interactiveObject = (InteractiveObject)p_sender;
			}
			else
			{
				BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
				interactiveObject = (InteractiveObject)baseObjectEventArgs.Object;
			}
			if (interactiveObject == MyController)
			{
				ObjectState(interactiveObject.State);
			}
		}

		public void ObjectState(EInteractiveObjectState p_state)
		{
			StopAllCoroutines();
			if (p_state == EInteractiveObjectState.BUTTON_DOWN)
			{
				StartCoroutine(PlayAudioSequence());
			}
			else
			{
				AudioController.Stop(m_knockSound);
				AudioController.Stop(m_InmateSound);
			}
		}

		private IEnumerator PlayAudioSequence()
		{
			AudioManager.Instance.RequestPlayAudioID(m_knockSound, 0, -1f, transform, m_soundVolume, 0f, 0f, null);
			yield return new WaitForSeconds(m_soundDelay_KnockToInmate);
			AudioManager.Instance.RequestPlayAudioID(m_InmateSound, 0, -1f, m_InmateSoundPosition, m_soundVolume, 0f, 0f, null);
			yield break;
		}
	}
}
