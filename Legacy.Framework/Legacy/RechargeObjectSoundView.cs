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
	[AddComponentMenu("MM Legacy/Views/InteractiveObject Recharge Object SoundView")]
	public class RechargeObjectSoundView : BaseView
	{
		[SerializeField]
		private String m_isReadySound = "Gold_loot";

		[SerializeField]
		private String m_useSound = "Gold_loot";

		[SerializeField]
		private Single m_soundVolume = 1f;

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
				ObjectState(MyController.State, true);
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
				interactiveObject = (InteractiveObject)((BaseObjectEventArgs)p_args).Object;
			}
			if (interactiveObject == MyController)
			{
				ObjectState(interactiveObject.State, false);
			}
		}

		public void ObjectState(EInteractiveObjectState p_state, Boolean p_skipAnimation)
		{
			AudioController.Stop(m_useSound);
			AudioController.Stop(m_isReadySound);
			if (!p_skipAnimation)
			{
				AudioManager.Instance.RequestPlayAudioID((p_state != EInteractiveObjectState.OFF) ? m_isReadySound : m_useSound, 1, -1f, transform, m_soundVolume, 0f, 0f, null);
			}
		}
	}
}
