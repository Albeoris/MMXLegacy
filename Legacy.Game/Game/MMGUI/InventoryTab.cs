using System;
using Legacy.Core.Entities.Items;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/InventoryTab")]
	public class InventoryTab : MonoBehaviour
	{
		[SerializeField]
		private Single m_hoverSwitchDuration = 1f;

		private PartyScreen m_parent;

		private PartyInventoryItemContainer m_itemContainer;

		private Tab m_tab;

		private Int32 m_tabIndex;

		private Boolean m_isHovered;

		private Single m_hoveredTime;

		public void Init(PartyInventoryItemContainer p_itemContainer, PartyScreen p_parent, Int32 p_tabIndex)
		{
			m_itemContainer = p_itemContainer;
			m_parent = p_parent;
			m_tabIndex = p_tabIndex;
		}

		private void Start()
		{
			m_tab = gameObject.GetComponent<Tab>();
		}

		private void OnDisable()
		{
			m_isHovered = false;
		}

		private void OnDrop(GameObject m_drag)
		{
			if (UICamera.currentTouchID == -1)
			{
				if (DragDropManager.Instance.DraggedItem is ItemDragObject)
				{
					ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
					if (itemDragObject.ItemSlot != null)
					{
						BaseItem item = itemDragObject.Item;
						Int32 autoSlot = m_itemContainer.Inventory.GetAutoSlot(item);
						if (autoSlot >= 0)
						{
							ItemSlot p_targetSlot = m_itemContainer.ItemSlots[autoSlot];
							m_itemContainer.DropItem(p_targetSlot);
						}
					}
				}
				else if (DragDropManager.Instance.DraggedItem is ShopDragObject)
				{
					ShopDragObject shopDragObject = (ShopDragObject)DragDropManager.Instance.DraggedItem;
					shopDragObject.ItemSlot.Parent.DropCallback(m_itemContainer.Inventory, -1);
					DragDropManager.Instance.EndDragAction();
				}
				else if (DragDropManager.Instance.DraggedItem is LootDragObject)
				{
					LootDragObject lootDragObject = (LootDragObject)DragDropManager.Instance.DraggedItem;
					lootDragObject.ItemSlot.Parent.DropCallback(m_itemContainer.Inventory, -1);
					DragDropManager.Instance.EndDragAction();
				}
				else
				{
					DragDropManager.Instance.CancelDragAction();
				}
			}
		}

		private void OnDragHover(DragHoverEventArgs p_eventArgs)
		{
			BaseDragObject draggedItem = DragDropManager.Instance.DraggedItem;
			Boolean flag = draggedItem is ItemDragObject || draggedItem is ShopDragObject || draggedItem is LootDragObject;
			m_tab.OnHover(p_eventArgs.IsHovered);
			if (p_eventArgs.IsHovered && flag)
			{
				m_isHovered = true;
				m_hoveredTime = Time.time;
			}
			else
			{
				m_isHovered = false;
			}
		}

		private void Update()
		{
			if (m_isHovered && Time.time > m_hoveredTime + m_hoverSwitchDuration)
			{
				m_isHovered = false;
				m_parent.Open(m_tabIndex);
			}
		}
	}
}
