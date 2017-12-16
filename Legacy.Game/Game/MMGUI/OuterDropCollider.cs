using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/OuterDropCollider")]
	public class OuterDropCollider : MonoBehaviour
	{
		private void OnDrop(GameObject m_drag)
		{
			if (UICamera.currentTouchID == -1 && DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
				String text = LocaManager.GetText("POPUP_REQUEST_ITEM_THROW_AWAY", itemDragObject.Item.Name);
				PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, text, new PopupRequest.RequestCallback(OnRequestCallback));
				DragDropManager.Instance.DelayDragResolve();
			}
		}

		private void OnRequestCallback(PopupRequest.EResultType p_result, String p_text)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
				LegacyLogic.Instance.EventManager.InvokeEvent(itemDragObject.Item, EEventType.INVENTORY_ITEM_DUMPED, EventArgs.Empty);
				DragDropManager.Instance.EndDragAction();
				AudioController.Play("SOU_ANNO4_Item_Destroy");
			}
			else
			{
				DragDropManager.Instance.CancelDragAction();
			}
		}
	}
}
