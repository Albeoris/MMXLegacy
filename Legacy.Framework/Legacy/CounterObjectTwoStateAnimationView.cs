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
	[AddComponentMenu("MM Legacy/Views/CounterObject/State Change Animation View")]
	[RequireComponent(typeof(Animation))]
	public class CounterObjectTwoStateAnimationView : BaseView
	{
		[SerializeField]
		private AnimationClip m_activate;

		[SerializeField]
		private AnimationClip m_deactivate;

		[SerializeField]
		private EInteractiveObjectState m_activeState;

		protected override void Awake()
		{
			base.Awake();
		}

		protected new CounterObject MyController => (CounterObject)base.MyController;

	    protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_COUNTER_CHANGED, new EventHandler(OnCounterChanged));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_COUNTER_CHANGED, new EventHandler(OnCounterChanged));
				OnCounterChanged(MyController.State);
			}
		}

		private void OnCounterChanged(Object p_sender, EventArgs p_args)
		{
			if ((BaseObject)p_sender == MyController)
			{
				OnCounterChanged(MyController.State);
			}
		}

		private void OnCounterChanged(EInteractiveObjectState p_state)
		{
			if (p_state == m_activeState)
			{
				animation.Play(m_activate.name);
			}
			else
			{
				animation.Play(m_deactivate.name);
			}
		}
	}
}
