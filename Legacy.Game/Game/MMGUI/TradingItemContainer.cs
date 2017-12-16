using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/TradingItemContainer")]
	public class TradingItemContainer : MonoBehaviour
	{
		public const Int32 ITEM_VISIBLE_AMOUNT = 8;

		[SerializeField]
		protected GameObject m_itemSlotPrefab;

		[SerializeField]
		protected UIButton m_buyButton;

		[SerializeField]
		protected UILabel m_buyButtonText;

		protected List<ItemSlotTrading> m_itemSlots;

		protected TradingInventoryController m_inventory;

		protected ItemSlotTrading m_selectedItem;

		protected UIGrid m_grid;

		protected TradingScreen m_tradingScreen;

		private ItemDragObject m_itemToSell;

		public TradingInventoryController Inventory => m_inventory;

	    public UIGrid Grid => m_grid;

	    public ItemSlotTrading SelectedItem => m_selectedItem;

	    private void OnEnable()
		{
			m_grid.Reposition();
		}

		public void Init(TradingScreen p_tradingScreen)
		{
			m_tradingScreen = p_tradingScreen;
			m_itemSlots = new List<ItemSlotTrading>();
			m_grid = GetComponent<UIGrid>();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnResourcesChanged));
		}

		public void SetInventory(TradingInventoryController p_inventory)
		{
			m_inventory = p_inventory;
			UpdateItems();
			SelectItemSlot(null);
		}

		public virtual void UpdateItems()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_inventory.GetItemAt(i);
				if (itemAt != null)
				{
					AddItem(itemAt, num, i);
					num++;
				}
			}
			for (Int32 j = num; j < m_itemSlots.Count; j++)
			{
				m_itemSlots[j].SetItem(null, 0);
				NGUITools.SetActiveSelf(m_itemSlots[j].gameObject, false);
			}
			m_grid.Reposition();
		}

		public virtual void CleanUp()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnResourcesChanged));
		}

		protected virtual void AddItem(BaseItem p_item, Int32 p_slotIndex, Int32 p_inventorySlot)
		{
			while (p_slotIndex >= m_itemSlots.Count)
			{
				AddItemSlot();
			}
			m_itemSlots[p_slotIndex].SetItem(p_item, p_inventorySlot);
			NGUITools.SetActiveSelf(m_itemSlots[p_slotIndex].gameObject, true);
		}

		public virtual void AddItemSlot()
		{
			GameObject gameObject = NGUITools.AddChild(this.gameObject, m_itemSlotPrefab);
			ItemSlotTrading component = gameObject.GetComponent<ItemSlotTrading>();
			m_itemSlots.Add(component);
			component.Index = m_itemSlots.Count - 1;
			component.Parent = this;
		}

		public virtual void SelectItemSlot(ItemSlotTrading p_slot)
		{
			if (m_selectedItem != null)
			{
				m_selectedItem.SetSelected(false);
			}
			if (p_slot != null)
			{
				p_slot.SetSelected(true);
				m_selectedItem = p_slot;
				Boolean flag = CanBuyConditionsMet(p_slot.Item.Price, LegacyLogic.Instance.WorldManager.Party.Gold);
				if (m_buyButton != null)
				{
					m_buyButton.isEnabled = flag;
					m_buyButtonText.color = ((!flag) ? Color.gray : Color.white);
				}
			}
			else
			{
				m_selectedItem = null;
				if (m_buyButton != null)
				{
					m_buyButton.isEnabled = false;
					m_buyButtonText.color = Color.gray;
				}
			}
		}

		public virtual Boolean NeedsScrollBar()
		{
			return Inventory.GetCurrentItemCount() > 8;
		}

		public void OnResourcesChanged(Object p_sender, EventArgs p_args)
		{
			foreach (ItemSlotTrading itemSlotTrading in m_itemSlots)
			{
				if (itemSlotTrading.Item != null)
				{
					itemSlotTrading.UpdateItemCostColor();
				}
			}
			if (m_selectedItem != null)
			{
				Boolean flag = CanBuyConditionsMet(m_selectedItem.Item.Price, LegacyLogic.Instance.WorldManager.Party.Gold);
				if (m_buyButton != null)
				{
					m_buyButton.isEnabled = flag;
					m_buyButtonText.color = ((!flag) ? Color.gray : Color.white);
				}
			}
		}

		public void OnDrop(GameObject m_drag)
		{
			if (DragDropManager.Instance.DraggedItem is ItemDragObject && m_inventory.Npc.StaticData.AllowItemSell)
			{
				ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
				if (itemDragObject.Item != null)
				{
					Consumable consumable = itemDragObject.Item as Consumable;
					if (consumable != null && consumable.Counter > 1)
					{
						PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.SPLIT_ITEMS, String.Empty, LocaManager.GetText("POPUP_REQUEST_ITEMS_SELL_LABEL"), new PopupRequest.RequestCallback(ItemSellSplitterCallback));
						PopupRequest.Instance.ItemSplitter.Open(PopupItemSplitter.Mode.SELL, consumable.Counter, consumable, itemDragObject.ItemSlot.Index, m_inventory, -1);
					}
					else if (itemDragObject.Item is Equipment && ((((Equipment)itemDragObject.Item).Prefixes.Count > 0 && ((Equipment)itemDragObject.Item).Suffixes.Count > 0) || ((Equipment)itemDragObject.Item).IsRelic() || !((Equipment)itemDragObject.Item).Identified))
					{
						m_itemToSell = itemDragObject;
						PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("POPUP_REQUEST_CONFIRM_SELL_ITEM", itemDragObject.Item.Name), new PopupRequest.RequestCallback(SellItemCallback));
					}
					else
					{
						itemDragObject.ItemSlot.Parent.Inventory.SellItem(itemDragObject.Item, itemDragObject.ItemSlot.Index, 1);
						DragDropManager.Instance.EndDragAction();
					}
				}
			}
			else
			{
				DragDropManager.Instance.CancelDragAction();
			}
		}

		private void SellItemCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				m_itemToSell.ItemSlot.Parent.Inventory.SellItem(m_itemToSell.Item, m_itemToSell.ItemSlot.Index, 1);
			}
			DragDropManager.Instance.EndDragAction();
			m_itemToSell = null;
		}

		private void ItemSellSplitterCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				PartyInventoryController partyInventoryController = LegacyLogic.Instance.WorldManager.Party.Inventory;
				if (partyInventoryController.GetItemAt(PopupRequest.Instance.ItemSplitter.ItemSlotIndex) != PopupRequest.Instance.ItemSplitter.Item)
				{
					partyInventoryController = LegacyLogic.Instance.WorldManager.Party.MuleInventory;
				}
				Int32 count = PopupRequest.Instance.ItemSplitter.Count;
				partyInventoryController.SellItem(PopupRequest.Instance.ItemSplitter.Item, PopupRequest.Instance.ItemSplitter.ItemSlotIndex, count);
			}
			PopupRequest.Instance.ItemSplitter.Finish();
		}

		public virtual void ItemRightClick(ItemSlotTrading p_slot)
		{
			SelectItemSlot(p_slot);
			if (m_tradingScreen != null)
			{
				m_tradingScreen.OnBuyButtonClicked();
			}
		}

		public void DropCallback(IInventory p_inventory, Int32 p_targetSlot)
		{
			if (m_tradingScreen != null)
			{
				m_tradingScreen.BuyItem(p_inventory, p_targetSlot);
			}
		}

		public virtual Boolean CanBuyConditionsMet(Int32 p_need, Int32 p_own)
		{
			return p_own >= p_need;
		}
	}
}
