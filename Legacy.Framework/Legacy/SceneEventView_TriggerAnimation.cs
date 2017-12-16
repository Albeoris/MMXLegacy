using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/Scene/SceneEventView_TriggerAnimation")]
	public class SceneEventView_TriggerAnimation : BaseView
	{
		[SerializeField]
		private String m_viewListenCommandName;

		[SerializeField]
		private String m_DeactiveClip = "Passive";

		[SerializeField]
		private String m_ActivateClip = "Activate";

		protected override void Awake()
		{
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
			if (animation != null && animation.GetClip(m_DeactiveClip) != null)
			{
				animation.Play(m_DeactiveClip, PlayMode.StopAll);
			}
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null && p_sender == null)
			{
				String[] array = stringEventArgs.text.Split(new Char[]
				{
					'_'
				});
				if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Activate")
				{
					if (animation != null && animation.GetClip(m_ActivateClip) != null)
					{
						animation.Play(m_ActivateClip, PlayMode.StopAll);
					}
				}
				else if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Deactivate" && animation != null && animation.GetClip(m_DeactiveClip) != null)
				{
					animation.Play(m_DeactiveClip, PlayMode.StopAll);
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}
	}
}
