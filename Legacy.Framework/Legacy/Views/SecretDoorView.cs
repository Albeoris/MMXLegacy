using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.UpdateLogic.Interactions;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/SecretDoorView")]
	public class SecretDoorView : BaseView
	{
		private BaseInteraction m_interaction;

		[SerializeField]
		private GameObject m_OpenObject;

		[SerializeField]
		private GameObject m_ClosedObject;

		[SerializeField]
		private GameObject m_OpenEffect;

		[SerializeField]
		private Single m_triggerStateChangeTime = 1f;

		[SerializeField]
		private String m_doorOpenSoundID = "door_open";

		[SerializeField]
		private Single m_soundVolume = 1f;

		public new Door MyController => (Door)base.MyController;

	    protected override void Awake()
		{
			base.Awake();
			ChangeActiveState(m_OpenObject, false);
			ChangeActiveState(m_ClosedObject, true);
			ChangeActiveState(m_OpenEffect, false);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DOOR_STATE_CHANGED, new EventHandler(OnDoorStateChanged));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DOOR_STATE_CHANGED, new EventHandler(OnDoorStateChanged));
				ObjectState(MyController.State, false);
			}
		}

		private void OnDoorStateChanged(Object p_sender, EventArgs p_args)
		{
			DoorEntityEventArgs doorEntityEventArgs = (DoorEntityEventArgs)p_args;
			if (doorEntityEventArgs.Animate && doorEntityEventArgs.Object == MyController)
			{
				m_interaction = (BaseInteraction)p_sender;
				if (doorEntityEventArgs.TargetState == EInteractiveObjectState.DOOR_OPEN || doorEntityEventArgs.TargetState == EInteractiveObjectState.DOOR_CLOSED)
				{
					ObjectState(doorEntityEventArgs.TargetState, true);
				}
				else
				{
					Debug.LogError("Secret Door State Change - Wrong TargetState: " + doorEntityEventArgs.TargetState);
				}
			}
		}

		public void ObjectState(EInteractiveObjectState p_stateValue, Boolean p_animate)
		{
			if (AudioController.DoesInstanceExist())
			{
				AudioController.Stop(m_doorOpenSoundID);
			}
			if (p_stateValue != EInteractiveObjectState.DOOR_OPEN)
			{
				if (p_stateValue == EInteractiveObjectState.DOOR_CLOSED)
				{
					ChangeActiveState(m_OpenObject, false);
					ChangeActiveState(m_ClosedObject, true);
					ChangeActiveState(m_OpenEffect, false);
					Invoke("InvokeTriggerStateChange", 0f);
				}
			}
			else
			{
				ChangeActiveState(m_OpenObject, true);
				ChangeActiveState(m_ClosedObject, false);
				if (p_animate)
				{
					ChangeActiveState(m_OpenEffect, true);
					if (AudioController.DoesInstanceExist())
					{
						AudioController.Play(m_doorOpenSoundID, transform, m_soundVolume, 0f, 0f);
					}
				}
				Invoke("InvokeTriggerStateChange", m_triggerStateChangeTime);
			}
		}

		private void InvokeTriggerStateChange()
		{
			if (m_interaction != null)
			{
				m_interaction.TriggerStateChange();
				m_interaction.ReleaseInteractLock();
			}
		}

		private static void ChangeActiveState(GameObject p_obj, Boolean p_newState)
		{
			if (p_obj != null)
			{
				p_obj.SetActive(p_newState);
			}
		}
	}
}
