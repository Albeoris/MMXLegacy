using System;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[RequireComponent(typeof(Animation))]
	[AddComponentMenu("MM Legacy/Views/ChestView (ContainerView)")]
	public class ChestView : BaseView
	{
		private Animation m_Animation;

		private Single m_animEndTime = -1f;

		[SerializeField]
		private String m_ActiveClip = "Open";

		[SerializeField]
		private String m_DeactiveClip = "Close";

		[SerializeField]
		private String m_openSound = "Gold_loot";

		[SerializeField]
		private String m_closedIdleSound;

		[SerializeField]
		private Single m_soundVolume = 1f;

		[SerializeField]
		private GameObject m_lootObject;

		public new Container MyController => base.MyController as Container;

	    public void ObjectState(Boolean p_stateValue, Boolean p_skipAnimation)
		{
			enabled = true;
			AudioController.Stop(m_closedIdleSound);
			AudioController.Stop(m_openSound);
			String text = (!p_stateValue) ? m_DeactiveClip : m_ActiveClip;
			if (p_skipAnimation)
			{
				if (m_Animation[text] != null)
				{
					m_Animation.Play(text, PlayMode.StopAll);
					m_Animation[text].normalizedTime = 1f;
					m_Animation.Sample();
				}
				m_animEndTime = Time.time;
				SendMessage("ChestView_Skipped_Animation", p_stateValue, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				AudioManager.Instance.RequestPlayAudioID((!p_stateValue) ? m_closedIdleSound : m_openSound, 1, -1f, gameObject.transform, m_soundVolume, 0f, 0f, null);
				if (m_Animation[text] != null)
				{
					m_Animation.Play(text, PlayMode.StopAll);
					m_animEndTime = Time.time + m_Animation[text].clip.length;
				}
			}
			if (m_lootObject != null)
			{
				m_lootObject.SetActive(!MyController.IsEmpty());
			}
		}

		protected override void Awake()
		{
			base.Awake();
			m_Animation = this.GetComponent< Animation>(true);
			m_Animation.Play(m_DeactiveClip, PlayMode.StopAll);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CONTAINER_DONE_LOOTING, new EventHandler(OnContainerLooted));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CONTAINER_SCREEN_CLOSED, new EventHandler(OnContainerClosed));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CONTAINER_DONE_LOOTING, new EventHandler(OnContainerLooted));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CONTAINER_SCREEN_CLOSED, new EventHandler(OnContainerClosed));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
				ObjectState(MyController.State == EInteractiveObjectState.CONTAINER_OPENED, true);
			}
		}

		private void OnInteractiveObjectStateChanged(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				ObjectState(MyController.State == EInteractiveObjectState.CONTAINER_OPENED, false);
			}
		}

		private void OnContainerLooted(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController && MyController.IsEmpty() && m_lootObject != null)
			{
				m_lootObject.SetActive(false);
			}
		}

		private void OnContainerClosed(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController && MyController.IsEmpty() && m_lootObject != null)
			{
				m_lootObject.SetActive(false);
			}
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				Single p_height = 0f;
				GridSlot slot = LegacyLogic.Instance.MapLoader.Grid.GetSlot(moveEntityEventArgs.Position);
				if (slot != null)
				{
					p_height = slot.Height;
				}
				Vector3 a = Helper.SlotLocalPosition(moveEntityEventArgs.Position, p_height);
				InteractiveObject interactiveObject = p_sender as InteractiveObject;
				transform.localPosition = a + new Vector3(interactiveObject.OffsetPosition.X, interactiveObject.OffsetPosition.Y, interactiveObject.OffsetPosition.Z);
			}
		}

		private void Update()
		{
			if (m_animEndTime != -1f && m_animEndTime <= Time.time)
			{
				m_animEndTime = -1f;
				enabled = false;
			}
		}
	}
}
