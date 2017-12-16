using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using Legacy.Core.UpdateLogic.Interactions;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/PlatformView")]
	public class PlatformView : BaseView
	{
		private Vector3 m_oldPos;

		private Vector3 m_newPos;

		private Single m_timer = 1f;

		[SerializeField]
		private Single m_speed = 1f;

		public new InteractiveObject MyController => (InteractiveObject)base.MyController;

	    protected override void Awake()
		{
			base.Awake();
			enabled = false;
		}

		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PLATFORM_MOVED, new EventHandler(OnMove));
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PLATFORM_RESET, new EventHandler(OnReset));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PLATFORM_MOVED, new EventHandler(OnMove));
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PLATFORM_RESET, new EventHandler(OnReset));
				OnSetPosition(MyController.Position, true);
			}
		}

		private void OnMove(Object p_sender, EventArgs p_args)
		{
			InteractiveObject myController = MyController;
			MoveEntityEventArgs moveEntityEventArgs = p_args as MoveEntityEventArgs;
			if (p_args != null && p_sender == myController)
			{
				OnSetPosition(moveEntityEventArgs.Position, false);
			}
		}

		private void OnReset(Object p_sender, EventArgs p_args)
		{
			InteractiveObject myController = MyController;
			m_oldPos = transform.position;
			Vector3 newPos = Helper.SlotLocalPosition(myController.OriginalPosition, myController.OriginalHeight) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			m_newPos = newPos;
			m_timer = 0f;
			enabled = true;
			((ResetPlatformInteraction)p_sender).Notify(myController, myController.Position, myController.OriginalPosition);
		}

		private void OnSetPosition(Position p_pos, Boolean p_immediate)
		{
			m_oldPos = transform.position;
			Vector3 vector = Helper.SlotLocalPosition(p_pos, MyController.OriginalHeight) + LegacyLogic.Instance.MapLoader.Grid.GetOffset();
			if (!p_immediate)
			{
				m_newPos = vector;
				m_timer = 0f;
				enabled = true;
			}
			else
			{
				transform.position = vector;
			}
		}

		private void Update()
		{
			if (m_timer <= 1f)
			{
				m_timer += Time.deltaTime * m_speed;
				transform.position = Vector3.Lerp(m_oldPos, m_newPos, m_timer);
			}
			else
			{
				enabled = false;
			}
		}
	}
}
