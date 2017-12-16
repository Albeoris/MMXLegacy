using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ItemContainer")]
	public class ItemContainer : MonoBehaviour
	{
		protected ItemSlot[] m_itemSlots = Arrays<ItemSlot>.Empty;

		protected IInventory m_inventory;

		public IInventory Inventory => m_inventory;

	    public ItemSlot[] ItemSlots => m_itemSlots;

	    public virtual void Init(IInventory p_inventory)
		{
			m_itemSlots = new ItemSlot[p_inventory.GetMaximumItemCount()];
			m_inventory = p_inventory;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnItemAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_REMOVED, new EventHandler(OnItemRemoved));
		}

		public virtual void ChangeInventory(IInventory p_inventory)
		{
			if (DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
				if (itemDragObject.ItemSlot.Parent == this)
				{
					DragDropManager.Instance.CancelDragAction();
				}
			}
			m_inventory = p_inventory;
			if (m_itemSlots.Length != m_inventory.GetMaximumItemCount())
			{
				m_itemSlots = new ItemSlot[m_inventory.GetMaximumItemCount()];
			}
		}

		public virtual void InitStartItems()
		{
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_inventory.GetItemAt(i);
				m_itemSlots[i].SetItem(itemAt);
			}
		}

		public virtual void CleanUp()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnItemAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_REMOVED, new EventHandler(OnItemRemoved));
		}

		public void AddItemSlot(ItemSlot p_itemSlot, Int32 p_index)
		{
			m_itemSlots[p_index] = p_itemSlot;
			p_itemSlot.Index = p_index;
			p_itemSlot.Parent = this;
		}

		protected virtual void OnItemAdded(Object p_sender, EventArgs p_args)
		{
			InventoryItemEventArgs inventoryItemEventArgs = (InventoryItemEventArgs)p_args;
			if (inventoryItemEventArgs.Slot.Inventory == m_inventory.GetEventSource())
			{
				Int32 slot = inventoryItemEventArgs.Slot.Slot;
				if (slot < m_itemSlots.Length)
				{
					BaseItem itemAt = m_inventory.GetItemAt(slot);
					AddItem(slot, itemAt);
				}
			}
		}

		protected virtual void AddItem(Int32 p_slot, BaseItem p_item)
		{
			m_itemSlots[p_slot].SetItem(p_item);
		}

		protected virtual void OnItemRemoved(Object p_sender, EventArgs p_args)
		{
			InventoryItemEventArgs inventoryItemEventArgs = (InventoryItemEventArgs)p_args;
			if (inventoryItemEventArgs.Slot.Inventory == m_inventory.GetEventSource() && inventoryItemEventArgs.Slot.Slot < m_itemSlots.Length)
			{
				m_itemSlots[inventoryItemEventArgs.Slot.Slot].SetItem(m_inventory.GetItemAt(inventoryItemEventArgs.Slot.Slot));
			}
		}

		public virtual void ItemRightClick(ItemSlot p_slot)
		{
		}

		public virtual void DropItem(ItemSlot p_targetSlot)
		{
			if (DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
				if (itemDragObject.ItemSlot != null)
				{
					if (!SwitchItems(p_targetSlot, itemDragObject.ItemSlot, itemDragObject.Item))
					{
						DragDropManager.Instance.CancelDragAction();
					}
					DragDropManager.Instance.EndDragAction();
				}
			}
			else if (DragDropManager.Instance.DraggedItem is ShopDragObject)
			{
				ShopDragObject shopDragObject = (ShopDragObject)DragDropManager.Instance.DraggedItem;
				Int32 p_targetSlot2 = p_targetSlot.Index;
				if (p_targetSlot is EquipmentSlot)
				{
					Party party = LegacyLogic.Instance.WorldManager.Party;
					EquipmentSlot equipmentSlot = (EquipmentSlot)p_targetSlot;
					Equipment equipment = shopDragObject.Item as Equipment;
					Equipment equipment2 = equipmentSlot.Item as Equipment;
					Int32 num = 0;
					if (equipmentSlot.Item != null)
					{
						num++;
					}
					if (equipment != null && equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND && equipment2 != null && equipment2.ItemSlot != EItemSlot.ITEM_SLOT_2_HAND)
					{
						if (equipmentSlot.Index == 0 && equipmentSlot.Parent.ItemSlots[1].Item != null)
						{
							num++;
						}
						else if (equipmentSlot.Index == 1 && equipmentSlot.Parent.ItemSlots[0].Item != null)
						{
							num++;
						}
					}
					Int32 num2 = party.Inventory.GetMaximumItemCount() - party.Inventory.GetCurrentItemCount();
					Boolean flag = party.HirelingHandler.HasEffect(ETargetCondition.HIRE_MULE);
					if (flag)
					{
						num2 += party.MuleInventory.GetMaximumItemCount() - party.MuleInventory.GetCurrentItemCount();
					}
					if (!equipmentSlot.Parent.Inventory.IsItemPlaceableAt(shopDragObject.Item, equipmentSlot.Index) || num > num2)
					{
						DragDropManager.Instance.CancelDragAction();
						DragDropManager.Instance.EndDragAction();
						return;
					}
				}
				else if (p_targetSlot.Item != null && !Consumable.AreSameConsumables(shopDragObject.Item, p_targetSlot.Item))
				{
					p_targetSlot2 = -1;
				}
				shopDragObject.ItemSlot.Parent.DropCallback(m_inventory, p_targetSlot2);
				DragDropManager.Instance.EndDragAction();
			}
			else if (DragDropManager.Instance.DraggedItem is LootDragObject)
			{
				LootDragObject lootDragObject = (LootDragObject)DragDropManager.Instance.DraggedItem;
				Int32 p_targetSlot3 = p_targetSlot.Index;
				if (p_targetSlot.Item != null && !Consumable.AreSameConsumables(lootDragObject.Item, p_targetSlot.Item))
				{
					p_targetSlot3 = -1;
				}
				lootDragObject.ItemSlot.Parent.DropCallback(m_inventory, p_targetSlot3);
				DragDropManager.Instance.EndDragAction();
			}
			else
			{
				DragDropManager.Instance.CancelDragAction();
			}
		}

		protected virtual Boolean SwitchItems(ItemSlot p_targetSlot, ItemSlot p_originSlot, BaseItem p_originItem)
		{
			ItemContainer parent = p_originSlot.Parent;
			BaseItem item = p_targetSlot.Item;
			Boolean flag = parent.Inventory.IsItemPlaceableAt(item, p_originSlot.Index);
			Boolean flag2 = m_inventory.IsItemPlaceableAt(p_originItem, p_targetSlot.Index);
			if (!flag || !flag2)
			{
				return false;
			}
			if (!(parent.Inventory is CharacterInventoryController))
			{
				if (!Consumable.AreSameConsumables(p_originItem, item))
				{
					parent.Inventory.AddItem(item, p_originSlot.Index);
				}
				m_inventory.AddItem(p_originItem, p_targetSlot.Index);
				return true;
			}
			EquipCommand p_command = new EquipCommand(parent.Inventory, m_inventory, p_originItem, item, p_originSlot.Index, p_targetSlot.Index);
			if (LegacyLogic.Instance.UpdateManager.PartyTurnActor.CanDoCommand(p_command, LegacyLogic.Instance.WorldManager.Party.CurrentCharacter))
			{
				LegacyLogic.Instance.CommandManager.AddCommand(p_command);
				return true;
			}
			return false;
		}
	}
}
