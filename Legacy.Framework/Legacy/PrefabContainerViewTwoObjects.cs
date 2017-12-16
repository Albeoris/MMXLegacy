using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/PrefabContainerViewTwoObjects")]
	public class PrefabContainerViewTwoObjects : BaseView
	{
		[SerializeField]
		private GameObject m_passiveObject;

		[SerializeField]
		private GameObject m_activeObject;

		protected override void Awake()
		{
			base.Awake();
		}

		private void Start()
		{
			SetActiveRecursively(m_passiveObject, true);
			SetActiveRecursively(m_activeObject, false);
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
			}
		}

		private void OnAnimTriggered(Object p_sender, EventArgs p_args)
		{
			StringEventArgs stringEventArgs = p_args as StringEventArgs;
			if (stringEventArgs != null && p_sender != null && p_sender as BaseObject == MyController)
			{
				if (stringEventArgs.text == "Activate")
				{
					SetActiveRecursively(m_passiveObject, false);
					SetActiveRecursively(m_activeObject, true);
				}
				else if (stringEventArgs.text == "Deactivate")
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
	}
}
