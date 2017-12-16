using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/LootBagView")]
	public class LootBagView : BaseView
	{
		public new Container MyController => base.MyController as Container;

	    public void ObjectState(Boolean p_stateValue)
		{
			AudioController.Play("Gold_loot", transform);
			enabled = true;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnEntityDestroyed));
			}
			if (MyController != null)
			{
				ObjectState(MyController.State == EInteractiveObjectState.CONTAINER_OPENED);
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DESTROY_BASEOBJECT, new EventHandler(OnEntityDestroyed));
			}
		}

		private void OnInteractiveObjectStateChanged(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = p_args as BaseObjectEventArgs;
			if (baseObjectEventArgs != null && baseObjectEventArgs.Object == MyController)
			{
				ObjectState(MyController.State == EInteractiveObjectState.CONTAINER_OPENED);
			}
		}

		private void OnEntityDestroyed(Object p_sender, EventArgs p_args)
		{
			BaseObjectEventArgs baseObjectEventArgs = (BaseObjectEventArgs)p_args;
			if (baseObjectEventArgs.Object == MyController)
			{
				Destroy(gameObject);
			}
		}
	}
}
