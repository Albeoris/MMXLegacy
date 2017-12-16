using System;
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
	[AddComponentMenu("MM Legacy/Views/InteractiveObject Generic SoundView")]
	public class InteractiveObjectGenericSoundView : BaseView
	{
		[SerializeField]
		private String m_Sound = "Gold_loot";

		[SerializeField]
		private Boolean m_playOnInit;

		[SerializeField]
		private EInteractiveObjectState m_playOnState;

		[SerializeField]
		private EEventType m_registerEvent = EEventType.OBJECT_STATE_CHANGED;

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
				LegacyLogic.Instance.EventManager.UnregisterEvent(m_registerEvent, new EventHandler(OnInteractiveObjectStateChanged));
			}
			if (MyController != null)
			{
				ObjectState(MyController.State, true);
				LegacyLogic.Instance.EventManager.RegisterEvent(m_registerEvent, new EventHandler(OnInteractiveObjectStateChanged));
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
				ObjectState(interactiveObject.State, false);
			}
		}

		public void ObjectState(EInteractiveObjectState p_state, Boolean p_isSetup)
		{
			if (p_state == m_playOnState)
			{
				if (!p_isSetup || m_playOnInit)
				{
					AudioManager.Instance.RequestPlayAudioID(m_Sound, 0, -1f, transform, m_soundVolume, 0f, 0f, null);
				}
			}
			else
			{
				AudioController.Stop(m_Sound);
			}
		}
	}
}
