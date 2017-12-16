using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Map;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Views
{
	[AddComponentMenu("MM Legacy/Views/PrefabContainer BaseView")]
	internal class PrefabContainerBaseView : BaseView
	{
		protected override void OnChangeMyController(BaseObject oldController)
		{
			base.OnChangeMyController(oldController);
			if (oldController != MyController)
			{
				LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
			}
			if (MyController != null)
			{
				LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.MOVE_ENTITY, new EventHandler(OnMoveEntity));
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
				Vector3 localPosition = Helper.SlotLocalPosition(moveEntityEventArgs.Position, p_height);
				transform.localPosition = localPosition;
			}
		}
	}
}
