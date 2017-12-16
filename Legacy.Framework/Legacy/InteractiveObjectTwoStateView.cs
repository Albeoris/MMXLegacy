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
	[AddComponentMenu("MM Legacy/Views/InteractiveObject Two State View")]
	public class InteractiveObjectTwoStateView : BaseView
	{
		[SerializeField]
		private GameObject m_passiveObject;

		[SerializeField]
		private GameObject m_activeObject;

		[SerializeField]
		private Single m_delay;

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
				ObjectState(MyController.State, true);
			}
		}

		private void OnInteractiveObjectStateChanged(Object p_sender, EventArgs p_args)
		{
			InteractiveObject interactiveObject = (InteractiveObject)p_sender;
			if (interactiveObject == MyController)
			{
				ObjectState(interactiveObject.State, false);
			}
		}

		public void ObjectState(EInteractiveObjectState p_state, Boolean p_instantSet)
		{
			if (p_state != EInteractiveObjectState.ON)
			{
				if (p_state == EInteractiveObjectState.OFF)
				{
					Invoke("Deactivate", (!p_instantSet) ? m_delay : 0f);
				}
			}
			else
			{
				Invoke("Activate", (!p_instantSet) ? m_delay : 0f);
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
