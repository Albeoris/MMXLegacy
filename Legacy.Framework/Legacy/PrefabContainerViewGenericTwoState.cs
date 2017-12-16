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
	[RequireComponent(typeof(Animation))]
	[AddComponentMenu("MM Legacy/Views/PrefabContainerViewGenericTwoState")]
	public class PrefabContainerViewGenericTwoState : BaseView
	{
		[SerializeField]
		private String m_DeactiveClip = "Deactivate";

		[SerializeField]
		private String m_ActiveClip = "Activate";

		private Animation m_Animation;

		private Single m_animEndTime = -1f;

		protected override void Awake()
		{
			base.Awake();
			m_Animation = this.GetComponent<Animation>(true);
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
				if (MyController is PrefabContainer)
				{
					ObjectState(((PrefabContainer)MyController).CurrentAnim == "Activate");
				}
			}
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null && p_sender != null && p_sender as BaseObject == MyController)
			{
				ObjectState(stringEventArgs.text == "Activate");
			}
		}

		public void ObjectState(Boolean p_stateValue)
		{
			enabled = true;
			if (p_stateValue)
			{
				m_Animation.Play(m_ActiveClip, PlayMode.StopAll);
				if (m_Animation[m_ActiveClip] != null)
				{
					m_animEndTime = Time.time + m_Animation[m_ActiveClip].clip.length;
				}
				else
				{
					m_animEndTime = Time.time;
				}
			}
			else
			{
				m_Animation.Play(m_DeactiveClip, PlayMode.StopAll);
				if (m_Animation[m_DeactiveClip] != null)
				{
					m_animEndTime = Time.time + m_Animation[m_DeactiveClip].clip.length;
				}
				else
				{
					m_animEndTime = Time.time;
				}
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
