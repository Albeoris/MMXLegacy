using System;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.UpdateLogic.Interactions;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[RequireComponent(typeof(Animation))]
	[AddComponentMenu("MM Legacy/Views/DoorView")]
	public class DoorView : BaseView
	{
		private Animation m_Animation;

		private Single m_animEndTime = -1f;

		private Single m_interactionTime = -1f;

		private BaseInteraction m_interaction;

		[SerializeField]
		private String m_initClip = "Close";

		[SerializeField]
		private String m_ActiveClip = "Open";

		[SerializeField]
		private String m_DeactiveClip = "Close";

		[SerializeField]
		private Single m_triggerAfterPlayTimePercentage = 0.7f;

		[SerializeField]
		private String m_doorOpenSoundID = "door_open";

		[SerializeField]
		private String m_doorCloseSoundID = "door_close";

		[SerializeField]
		private Single m_soundVolume = 1f;

		public new Door MyController => base.MyController as Door;

	    public void ObjectState(Boolean p_stateValue, Boolean p_skipAnimation)
		{
			enabled = true;
			AudioController.Stop(m_doorCloseSoundID);
			AudioController.Stop(m_doorOpenSoundID);
			String text = (!p_stateValue) ? m_DeactiveClip : m_ActiveClip;
			if (collider != null)
			{
				collider.enabled = !p_stateValue;
			}
			if (p_skipAnimation)
			{
				if (m_Animation[text] != null)
				{
					m_Animation.Play(text, PlayMode.StopAll);
					m_Animation[text].normalizedTime = 1f;
					m_Animation.Sample();
				}
				m_animEndTime = Time.time;
				m_interactionTime = Time.time;
				if (m_interaction != null)
				{
					enabled = false;
					m_interaction.ReleaseInteractLock();
					m_interaction.TriggerStateChange();
				}
			}
			else
			{
				AudioManager.Instance.RequestPlayAudioID((!p_stateValue) ? m_doorCloseSoundID : m_doorOpenSoundID, 1, -1f, gameObject.transform, m_soundVolume, 0f, 0f, null);
				m_animEndTime = Time.time;
				m_interactionTime = Time.time;
				if (m_Animation[text] != null)
				{
					m_Animation.Play(text, PlayMode.StopAll);
					m_animEndTime += m_Animation[text].clip.length;
					if (p_stateValue)
					{
						m_interactionTime += m_Animation[text].clip.length * m_triggerAfterPlayTimePercentage;
					}
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			m_Animation = this.GetComponent< Animation>(true);
			m_Animation.Play(m_initClip, PlayMode.StopAll);
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
				ObjectState(MyController.State == EInteractiveObjectState.DOOR_OPEN, true);
			}
		}

		private void OnDoorStateChanged(Object p_sender, EventArgs p_args)
		{
			DoorEntityEventArgs doorEntityEventArgs = (DoorEntityEventArgs)p_args;
			if (doorEntityEventArgs.Animate && doorEntityEventArgs.Object == MyController)
			{
				m_interaction = (BaseInteraction)p_sender;
				ObjectState(doorEntityEventArgs.TargetState == EInteractiveObjectState.DOOR_OPEN, false);
			}
		}

		private void Update()
		{
			if (m_interactionTime != -1f && m_interactionTime <= Time.time)
			{
				m_interactionTime = -1f;
				if (m_interaction != null)
				{
					m_interaction.TriggerStateChange();
				}
			}
			if (m_animEndTime != -1f && m_animEndTime <= Time.time)
			{
				m_animEndTime = -1f;
				enabled = false;
				if (m_interaction != null)
				{
					m_interaction.ReleaseInteractLock();
				}
			}
		}
	}
}
