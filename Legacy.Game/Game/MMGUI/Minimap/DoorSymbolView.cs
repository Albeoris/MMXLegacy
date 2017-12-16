using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI.Minimap
{
	[AddComponentMenu("MM Legacy/MMGUI/Minimap/Door Symbol View")]
	public class DoorSymbolView : SimpleSymbolView
	{
		[SerializeField]
		private UIWidget m_DoorClosed;

		[SerializeField]
		private Single m_AnimationTime = 0.3f;

		public override void MakePixelPerfect()
		{
			transform.localScale = Vector3.one;
		}

		protected override void Awake()
		{
			base.Awake();
			MyUIWidget.alpha = 0f;
			m_DoorClosed.alpha = 0f;
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
				Door door = (Door)MyController;
				if (door.State == EInteractiveObjectState.DOOR_OPEN)
				{
					MyUIWidget.alpha = 1f;
					m_DoorClosed.alpha = 0f;
				}
				else if (door.State == EInteractiveObjectState.DOOR_CLOSED)
				{
					MyUIWidget.alpha = 0f;
					m_DoorClosed.alpha = 1f;
				}
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.DOOR_STATE_CHANGED, new EventHandler(OnDoorStateChanged));
			}
		}

		protected override void OnSetEntityPosition(Object p_sender, EventArgs p_args)
		{
			if (p_sender == MyController)
			{
				base.OnSetEntityPosition(p_sender, p_args);
				transform.localRotation = MyControllerRotation;
			}
		}

		private void OnDoorStateChanged(Object p_sender, EventArgs p_args)
		{
			DoorEntityEventArgs doorEntityEventArgs = (DoorEntityEventArgs)p_args;
			if (!doorEntityEventArgs.Animate && doorEntityEventArgs.Object == MyController)
			{
				Door door = (Door)MyController;
				if (door.State == EInteractiveObjectState.DOOR_OPEN)
				{
					TweenAlpha.Begin(MyUIWidget.gameObject, m_AnimationTime, 1f);
					TweenAlpha.Begin(m_DoorClosed.gameObject, m_AnimationTime, 0f);
				}
				else if (door.State == EInteractiveObjectState.DOOR_CLOSED)
				{
					TweenAlpha.Begin(MyUIWidget.gameObject, m_AnimationTime, 0f);
					TweenAlpha.Begin(m_DoorClosed.gameObject, m_AnimationTime, 1f);
				}
			}
		}
	}
}
