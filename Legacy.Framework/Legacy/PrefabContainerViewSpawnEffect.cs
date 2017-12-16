using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Views;
using UnityEngine;
using Object = System.Object;

namespace Legacy
{
	[AddComponentMenu("MM Legacy/Views/PrefabContainerViewSpawnEffect")]
	public class PrefabContainerViewSpawnEffect : BaseView
	{
		[SerializeField]
		private GameObject m_spawnObject;

		protected override void Awake()
		{
			base.Awake();
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
				GameObject gameObject = (GameObject)Instantiate(m_spawnObject, transform.position, transform.rotation);
				gameObject.transform.parent = transform;
			}
		}
	}
}
