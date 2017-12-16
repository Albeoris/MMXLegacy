using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Game.IngameManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyInventoryItemContainer")]
	public class PartyInventoryItemContainer : RectangularItemContainer
	{
		private ItemSlot m_itemToSell;

		public override void Init(IInventory p_inventory)
		{
			base.Init(p_inventory);
			DragDropManager.Instance.ShortcutRightclickEvent += OnShortcutRightclick;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_CHANGED, new EventHandler(OnItemChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemRepairStatusChanged));
		}

		public override void CleanUp()
		{
			base.CleanUp();
			DragDropManager.Instance.ShortcutRightclickEvent -= OnShortcutRightclick;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_CHANGED, new EventHandler(OnItemChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemRepairStatusChanged));
		}

		private void OnEnable()
		{
			LegacyLogic.Instance.WorldManager.Party.ActiveInventory = (PartyInventoryController)Inventory;
		}

		private void OnItemChanged(Object p_sender, EventArgs p_args)
		{
			InventoryItemEventArgs inventoryItemEventArgs = p_args as InventoryItemEventArgs;
			if (inventoryItemEventArgs != null && inventoryItemEventArgs.Slot.Inventory == m_inventory.GetEventSource())
			{
				Int32 slot = inventoryItemEventArgs.Slot.Slot;
				if (slot < m_itemSlots.Length)
				{
					BaseItem itemAt = m_inventory.GetItemAt(slot);
					m_itemSlots[slot].SetItem(itemAt);
				}
			}
		}

		private void OnItemRepairStatusChanged(Object p_sender, EventArgs p_args)
		{
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				m_itemSlots[i].UpdateItemBrokenState();
			}
		}

		public override void ItemRightClick(ItemSlot p_slot)
		{
			if (p_slot.Item == null)
			{
				return;
			}
			if (IngameController.Instance.BilateralScreen.LootScreen.IsOpen)
			{
				return;
			}
			PartyInventoryController partyInventoryController = (PartyInventoryController)m_inventory;
			ConversationManager conversationManager = LegacyLogic.Instance.ConversationManager;
			if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.TradingInventory.IsTrading && conversationManager.CurrentNpc.StaticData.AllowItemSell)
			{
				BaseItem item = p_slot.Item;
				Consumable consumable = item as Consumable;
				Equipment equipment = item as Equipment;
				if (consumable != null && consumable.Counter > 1)
				{
					PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.SPLIT_ITEMS, String.Empty, LocaManager.GetText("POPUP_REQUEST_ITEMS_SELL_LABEL"), new PopupRequest.RequestCallback(ItemSellSplitterCallback));
					PopupRequest.Instance.ItemSplitter.Open(PopupItemSplitter.Mode.SELL, consumable.Counter, consumable, p_slot.Index, null, -1);
				}
				else if (equipment != null && ((equipment.Prefixes.Count > 0 && equipment.Suffixes.Count > 0) || equipment.IsRelic() || !equipment.Identified))
				{
					m_itemToSell = p_slot;
					PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.CONFIRM_CANCEL, String.Empty, LocaManager.GetText("POPUP_REQUEST_CONFIRM_SELL_ITEM", p_slot.Item.Name), new PopupRequest.RequestCallback(SellItemCallback));
				}
				else
				{
					partyInventoryController.SellItem(p_slot.Item, p_slot.Index, 1);
				}
			}
			else if (p_slot.Item is Consumable)
			{
				partyInventoryController.ConsumeItem(p_slot.Index, LegacyLogic.Instance.WorldManager.Party.CurrentCharacter);
			}
			else if (p_slot.Item is Equipment)
			{
				DragDropManager.Instance.ShortcutRightClick(p_slot);
			}
		}

		private void SellItemCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				m_inventory.SellItem(m_itemToSell.Item, m_itemToSell.Index, 1);
			}
			m_itemToSell = null;
		}

		private void ItemSellSplitterCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				Int32 count = PopupRequest.Instance.ItemSplitter.Count;
				m_inventory.SellItem(PopupRequest.Instance.ItemSplitter.Item, PopupRequest.Instance.ItemSplitter.ItemSlotIndex, count);
			}
			PopupRequest.Instance.ItemSplitter.Finish();
		}

		private void OnShortcutRightclick(Object p_sender, EventArgs p_args)
		{
			ItemSlot itemSlot = p_sender as ItemSlot;
			if (itemSlot != null && itemSlot.Parent.Inventory is CharacterInventoryController)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				if (m_inventory == party.MuleInventory && !party.HirelingHandler.HasEffect(ETargetCondition.HIRE_MULE))
				{
					return;
				}
				Int32 autoSlot = m_inventory.GetAutoSlot(itemSlot.Item);
				if (autoSlot >= 0)
				{
					SwitchItems(m_itemSlots[autoSlot], itemSlot, itemSlot.Item);
				}
			}
		}
	}
}
