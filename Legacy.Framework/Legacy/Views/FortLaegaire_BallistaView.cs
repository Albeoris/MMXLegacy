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
	[AddComponentMenu("MM Legacy/Views/FortLaegaire BallistaView")]
	public class FortLaegaire_BallistaView : BaseView
	{
		[SerializeField]
		private GameObject m_BrokenObject;

		[SerializeField]
		private GameObject m_PassiveObject;

		[SerializeField]
		private String m_breakSound = "Gold_loot";

		[SerializeField]
		private Single m_soundVolume = 1f;

		public new Container MyController => base.MyController as Container;

	    public void ObjectState(Boolean p_stateValue, Boolean p_skipAnimation)
		{
			AudioController.Stop(m_breakSound);
			if (!p_skipAnimation && p_stateValue)
			{
				AudioManager.Instance.RequestPlayAudioID(m_breakSound, 1, -1f, gameObject.transform, m_soundVolume, 0f, 0f, null);
			}
			if (m_BrokenObject != null)
			{
				m_BrokenObject.SetActive(p_stateValue);
			}
			if (m_PassiveObject != null)
			{
				m_PassiveObject.SetActive(!p_stateValue);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (m_BrokenObject != null)
			{
				m_PassiveObject.SetActive(true);
			}
			if (m_PassiveObject != null)
			{
				m_BrokenObject.SetActive(false);
			}
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.CONTAINER_STATE_CHANGED, new EventHandler(OnInteractiveObjectStateChanged));
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
	}
}
