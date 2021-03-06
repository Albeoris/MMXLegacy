﻿using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/Scene/SceneEventView_TwoObjects")]
	public class SceneEventView_TwoObjects : BaseView
	{
		[SerializeField]
		private String m_viewListenCommandName;

		[SerializeField]
		private GameObject m_passiveObject;

		[SerializeField]
		private GameObject m_activeObject;

		protected override void Awake()
		{
		}

		private void Start()
		{
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
			SetActiveRecursively(m_passiveObject, true);
			SetActiveRecursively(m_activeObject, false);
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
					SetActiveRecursively(m_passiveObject, false);
					SetActiveRecursively(m_activeObject, true);
				}
				else if (array.Length > 1 && array[0] == m_viewListenCommandName && array[1] == "Deactivate")
				{
					SetActiveRecursively(m_passiveObject, true);
					SetActiveRecursively(m_activeObject, false);
				}
			}
		}

		private void SetActiveRecursively(GameObject obj, Boolean state)
		{
			if (obj != null)
			{
				obj.SetActive(state);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PREFAB_CONTAINER_TRIGGER_ANIM, new EventHandler(OnAnimTriggered));
		}
	}
}
