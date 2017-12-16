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
	[AddComponentMenu("MM Legacy/Views/InteractiveObjectView")]
	[RequireComponent(typeof(Animation))]
	public class InteractiveObjectView : BaseView
	{
		private Animation m_Animation;

		private Single m_EndTime;

		private BaseInteraction m_interaction;

		[SerializeField]
		private Boolean m_state;

		[SerializeField]
		private String m_ActiveClip = "Activate";

		[SerializeField]
		private String m_DeactiveClip = "Deactivate";

		protected override void Awake()
		{
			base.Awake();
			m_Animation = animation;
			if (m_Animation == null)
			{
				throw new ComponentNotFoundException("Animation not found!");
			}
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.OBJECT_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			}
		}

		private void OnInteractiveObjectStateChanged(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				m_interaction = (BaseInteraction)p_sender;
				m_state = !m_state;
				ObjectState(m_state);
			}
		}

		private void OnMoveEntity(Object p_sender, EventArgs p_args)
		{
			MoveEntityEventArgs moveEntityEventArgs = (MoveEntityEventArgs)p_args;
			if (moveEntityEventArgs != null && p_sender == MyController)
			{
				InteractiveObject interactiveObject = (InteractiveObject)p_sender;
				Vector3 localPosition = Helper.SlotLocalPosition(interactiveObject.Position, 0f);
				transform.localPosition = localPosition;
			}
		}

		private void ObjectState(Boolean p_stateValue)
		{
			enabled = true;
			String text = (!p_stateValue) ? m_DeactiveClip : m_ActiveClip;
			m_EndTime = 0f;
			if (m_Animation[text] != null)
			{
				m_Animation.Play(text, PlayMode.StopAll);
				m_EndTime = Time.time + m_Animation[text].clip.length;
			}
		}

		private void Update()
		{
			if (m_EndTime != -1f && m_EndTime <= Time.time)
			{
				m_EndTime = -1f;
				enabled = false;
				if (m_interaction != null)
				{
					m_interaction.FinishExecution();
				}
			}
		}
	}
}
