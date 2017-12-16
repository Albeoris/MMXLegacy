using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.EffectEngine;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/InteractiveObject Camera LookAt Target")]
	public class InteractiveObjectLookAtTarget : BaseView
	{
		[SerializeField]
		private Transform mTargetLookAt;

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
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_INTERACTIVE_OBJECT_IN_FRONT, new EventHandler(OnObjectInFront));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_INTERACTIVE_OBJECT_IN_FRONT, new EventHandler(OnObjectInFront));
			}
		}

		private void OnObjectInFront(Object p_sender, EventArgs p_args)
		{
			InteractiveObject interactiveObject = (InteractiveObject)p_sender;
			if (interactiveObject == MyController)
			{
				SetLookAtPosAtCamera();
			}
		}

		private void SetLookAtPosAtCamera()
		{
			if (mTargetLookAt != null)
			{
				InteractiveObjectCamera.Instance.ActivateInteractiveObjectLook(mTargetLookAt);
			}
		}
	}
}
