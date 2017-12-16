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
	[AddComponentMenu("MM Legacy/Views/CounterObject/State Change View")]
	public class CounterObjectTwoStateView : BaseView
	{
		[SerializeField]
		private GameObject m_activeObject;

		[SerializeField]
		private GameObject m_passiveObject;

		[SerializeField]
		private EInteractiveObjectState m_activeState;

		[SerializeField]
		private Single m_activateDelay;

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
				Invoke("Activate", m_activateDelay);
			}
			else
			{
				Invoke("Deactivate", m_activateDelay);
			}
		}

		private void Activate()
		{
			SetActiveRecursively(m_passiveObject, false);
			SetActiveRecursively(m_activeObject, true);
		}

		private void Deactivate()
		{
			SetActiveRecursively(m_passiveObject, true);
			SetActiveRecursively(m_activeObject, false);
		}

		private void SetActiveRecursively(GameObject obj, Boolean state)
		{
			if (obj != null)
			{
				obj.SetActive(state);
			}
		}
	}
}
