using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/InteractiveObject State Change Particle Play View")]
	public class InteractiveObjectParticlePlayView : BaseView
	{
		[SerializeField]
		private ParticleSystem mParticles;

		[SerializeField]
		private EInteractiveObjectState m_reactOnState;

		[SerializeField]
		private EEventType m_registerEvent = EEventType.OBJECT_STATE_CHANGED;

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
				LegacyLogic.Instance.EventManager.RegisterEvent(m_registerEvent, new EventHandler(OnInteractiveObjectStateChanged));
				ObjectState(MyController.State);
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
			if (p_state == m_reactOnState)
			{
				if (mParticles != null)
				{
					mParticles.Play(true);
				}
			}
			else if (mParticles != null)
			{
				mParticles.Stop(true);
			}
		}
	}
}
