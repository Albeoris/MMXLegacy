using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.UpdateLogic;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/EquipmentItemContainer")]
	public class EquipmentItemContainer : ItemContainer
	{
		[SerializeField]
		private EquipmentSlot m_bodySlot;

		[SerializeField]
		private EquipmentSlot m_feetSlot;

		[SerializeField]
		private EquipmentSlot m_finger1Slot;

		[SerializeField]
		private EquipmentSlot m_finger2Slot;

		[SerializeField]
		private EquipmentSlot m_handsSlot;

		[SerializeField]
		private EquipmentSlot m_headSlot;

		[SerializeField]
		private EquipmentSlot m_mainHandSlot;

		[SerializeField]
		private EquipmentSlot m_neckSlot;

		[SerializeField]
		private EquipmentSlot m_offHandSlot;

		[SerializeField]
		private EquipmentSlot m_rangeWeaponSlot;

		public override void Init(IInventory p_inventory)
		{
			base.Init(p_inventory);
			AddItemSlot(m_bodySlot, 4);
			AddItemSlot(m_feetSlot, 6);
			AddItemSlot(m_finger1Slot, 8);
			AddItemSlot(m_finger2Slot, 9);
			AddItemSlot(m_handsSlot, 5);
			AddItemSlot(m_headSlot, 3);
			AddItemSlot(m_mainHandSlot, 0);
			AddItemSlot(m_neckSlot, 7);
			AddItemSlot(m_offHandSlot, 1);
			AddItemSlot(m_rangeWeaponSlot, 2);
			InitStartItems();
			DragDropManager.Instance.ShortcutRightclickEvent += OnShortcutRightclick;
			DragDropManager.Instance.DragEvent += OnDragEvent;
			DragDropManager.Instance.DropEvent += OnDropEvent;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemRepairStatusChanged));
		}

		public override void CleanUp()
		{
			base.CleanUp();
			DragDropManager.Instance.ShortcutRightclickEvent -= OnShortcutRightclick;
			DragDropManager.Instance.DragEvent -= OnDragEvent;
			DragDropManager.Instance.DropEvent -= OnDropEvent;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new EventHandler(OnItemRepairStatusChanged));
		}

		public void UpdateTwoHandedHighlight(EquipmentSlot p_hoveredSlot, DragHoverEventArgs p_eventArgs)
		{
			EEquipSlots eequipSlots = EEquipSlots.MAIN_HAND;
			if (p_hoveredSlot.Index == 0)
			{
				eequipSlots = EEquipSlots.OFF_HAND;
			}
			EquipmentSlot equipmentSlot = (EquipmentSlot)m_itemSlots[(Int32)eequipSlots];
			equipmentSlot.ForceDragHover(p_eventArgs);
		}

		public override void ItemRightClick(ItemSlot p_slot)
		{
			DragDropManager.Instance.ShortcutRightClick(p_slot);
		}

		private void OnItemRepairStatusChanged(Object p_sender, EventArgs p_args)
		{
			if (p_sender == Inventory || p_sender == Inventory.GetEventSource())
			{
				for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
				{
					((EquipmentSlot)m_itemSlots[i]).UpdateItemBrokenState();
				}
			}
		}

		private void OnShortcutRightclick(Object p_sender, EventArgs p_args)
		{
			ItemSlot itemSlot = p_sender as ItemSlot;
			if (itemSlot != null && itemSlot.Parent.Inventory is PartyInventoryController)
			{
				Int32 autoSlot = m_inventory.GetAutoSlot(itemSlot.Item);
				if (autoSlot >= 0 && m_inventory.IsItemPlaceableAt(itemSlot.Item, autoSlot))
				{
					SwitchItems(m_itemSlots[autoSlot], itemSlot, itemSlot.Item);
				}
			}
		}

		protected void OnDragEvent(Object p_sender, EventArgs p_args)
		{
			if (DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				if (DragDropManager.Instance.DraggedItem != null && DragDropManager.Instance.DraggedItem is ItemDragObject)
				{
					Equipment equipment = ((ItemDragObject)DragDropManager.Instance.DraggedItem).Item as Equipment;
					if (equipment != null)
					{
						for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
						{
							((EquipmentSlot)m_itemSlots[i]).EquipmentDragStarted(equipment);
						}
					}
				}
			}
			else if (DragDropManager.Instance.DraggedItem is ShopDragObject && DragDropManager.Instance.DraggedItem != null)
			{
				Equipment equipment2 = ((ShopDragObject)DragDropManager.Instance.DraggedItem).Item as Equipment;
				if (equipment2 != null)
				{
					for (Int32 j = 0; j < m_inventory.GetMaximumItemCount(); j++)
					{
						((EquipmentSlot)m_itemSlots[j]).ShopEquipmentDragStarted(equipment2);
					}
				}
			}
		}

		protected void OnDropEvent(Object p_sender, EventArgs p_args)
		{
			if (DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				if (DragDropManager.Instance.DraggedItem != null && DragDropManager.Instance.DraggedItem is ItemDragObject)
				{
					for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
					{
						((EquipmentSlot)m_itemSlots[i]).EquipmentDragStopped();
					}
				}
			}
			else if (DragDropManager.Instance.DraggedItem is ShopDragObject)
			{
				Equipment equipment = ((ShopDragObject)DragDropManager.Instance.DraggedItem).Item as Equipment;
				if (equipment != null)
				{
					for (Int32 j = 0; j < m_inventory.GetMaximumItemCount(); j++)
					{
						((EquipmentSlot)m_itemSlots[j]).ShopEquipmentDragStopped();
					}
				}
			}
		}

		protected override Boolean SwitchItems(ItemSlot p_targetSlot, ItemSlot p_originSlot, BaseItem p_originItem)
		{
			ItemContainer parent = p_originSlot.Parent;
			BaseItem item = p_targetSlot.Item;
			if (parent == this && p_targetSlot == p_originSlot)
			{
				return false;
			}
			EquipCommand p_command = new EquipCommand(m_inventory, parent.Inventory, item, p_originItem, p_targetSlot.Index, p_originSlot.Index);
			if (LegacyLogic.Instance.UpdateManager.PartyTurnActor.CanDoCommand(p_command, LegacyLogic.Instance.WorldManager.Party.CurrentCharacter))
			{
				LegacyLogic.Instance.CommandManager.AddCommand(p_command);
				return true;
			}
			return false;
		}

		private Boolean IsTwoHanded(BaseItem p_item)
		{
			Equipment equipment = p_item as Equipment;
			return equipment != null && equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND;
		}
	}
}
