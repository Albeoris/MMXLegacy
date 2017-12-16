using System;
using Legacy.Core.ActionLogging;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.PartyManagement
{
	public class CharacterInventoryController : ISaveGameObject, IInventory
	{
		private Inventory m_equipment;

		private Character m_character;

		public CharacterInventoryController(Character p_character)
		{
			m_character = p_character;
			m_equipment = new Inventory(10);
		}

		Boolean IInventory.CanAddItem(BaseItem p_item)
		{
			return GetAutoSlot(p_item as Equipment) >= EEquipSlots.MAIN_HAND;
		}

		Boolean IInventory.IsFull()
		{
			return false;
		}

		Boolean IInventory.AddItem(BaseItem p_item)
		{
			return AddItem(p_item as Equipment);
		}

		Int32 IInventory.GetAutoSlot(BaseItem p_item)
		{
			return (Int32)GetAutoSlot(p_item as Equipment);
		}

		void IInventory.AddItem(BaseItem p_item, Int32 p_slot)
		{
			AddItem(p_item, (EEquipSlots)p_slot);
		}

		void IInventory.RemoveItemAt(Int32 p_slot)
		{
			RemoveItemAt((EEquipSlots)p_slot);
		}

		BaseItem IInventory.GetItemAt(Int32 p_slot)
		{
			return GetItemAt((EEquipSlots)p_slot);
		}

		public Inventory Equipment => m_equipment;

	    public Character Character => m_character;

	    public void AddExp(Int32 p_exp)
		{
			for (Int32 i = 0; i < 10; i++)
			{
				Equipment equipment = (Equipment)m_equipment.GetItemAt(i);
				if (equipment != null)
				{
					equipment.AddExp(p_exp);
					if (equipment.LevelUpConditionsMet())
					{
						RemoveItemAt((EEquipSlots)i);
						Equipment equipment2 = (Equipment)ItemFactory.CreateItem(equipment.GetItemType(), equipment.NextLevelItemId);
						AddItem(equipment2);
						LegacyLogic.Instance.EventManager.InvokeEvent(equipment2, EEventType.INVENTORY_ITEM_RELIC_LEVEL_UP, EventArgs.Empty);
						RelicLevelUpEntryEventArgs p_args = new RelicLevelUpEntryEventArgs(equipment, equipment2);
						m_character.FeedActionLog(p_args);
					}
				}
			}
		}

		public Boolean IsMeleeAttackWeaponEquiped()
		{
			BaseItem itemAt = Character.Equipment.Equipment.GetItemAt(0);
			BaseItem itemAt2 = Character.Equipment.Equipment.GetItemAt(1);
			return itemAt is MeleeWeapon || itemAt2 is MeleeWeapon || itemAt is MagicFocus || itemAt2 is MagicFocus;
		}

		public Boolean IsRangedAttackWeaponEquiped()
		{
			return Character.Equipment.Equipment.GetItemAt(2) != null;
		}

		public Int32 GetSlotIndexForItem(BaseItem p_item)
		{
			for (Int32 i = 0; i < m_equipment.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_equipment.GetItemAt(i);
				if (itemAt != null && p_item == itemAt)
				{
					return i;
				}
			}
			return -1;
		}

		public Boolean AddItem(Equipment p_item)
		{
			EEquipSlots autoSlot = GetAutoSlot(p_item);
			if (autoSlot >= EEquipSlots.MAIN_HAND)
			{
				AddItem(p_item, autoSlot);
				p_item.PriceMultiplicator = LegacyLogic.Instance.WorldManager.ItemResellMultiplicator;
			}
			return autoSlot >= EEquipSlots.MAIN_HAND;
		}

		public EEquipSlots GetAutoSlot(Equipment p_equipment)
		{
			if (p_equipment != null)
			{
				switch (p_equipment.ItemSlot)
				{
				case EItemSlot.ITEM_SLOT_HEAD:
					return EEquipSlots.HEAD;
				case EItemSlot.ITEM_SLOT_TORSO:
					return EEquipSlots.BODY;
				case EItemSlot.ITEM_SLOT_HAND:
					return EEquipSlots.HANDS;
				case EItemSlot.ITEM_SLOT_FEET:
					return EEquipSlots.FEET;
				case EItemSlot.ITEM_SLOT_NECKLACE:
					return EEquipSlots.NECK;
				case EItemSlot.ITEM_SLOT_RING:
					if (!m_equipment.IsSlotOccupied(8) || m_equipment.IsSlotOccupied(9))
					{
						return EEquipSlots.FINGER1;
					}
					return EEquipSlots.FINGER2;
				case EItemSlot.ITEM_SLOT_1_HAND:
				{
					Boolean flag = m_equipment.IsSlotOccupied(0);
					Boolean flag2 = m_equipment.IsSlotOccupied(1);
					Boolean flag3 = IsItemPlaceableAt(p_equipment, 1);
					if (!flag || !flag3 || flag2)
					{
						return EEquipSlots.MAIN_HAND;
					}
					return EEquipSlots.OFF_HAND;
				}
				case EItemSlot.ITEM_SLOT_2_HAND:
					return EEquipSlots.MAIN_HAND;
				case EItemSlot.ITEM_SLOT_MAINHAND:
					return EEquipSlots.MAIN_HAND;
				case EItemSlot.ITEM_SLOT_OFFHAND:
					return EEquipSlots.OFF_HAND;
				case EItemSlot.ITEM_SLOT_RANGED:
					return EEquipSlots.RANGE_WEAPON;
				}
			}
			return (EEquipSlots)(-1);
		}

		public EEquipSlots GetAutoSlotEquipped(Equipment p_equipment)
		{
			if (p_equipment != null)
			{
				switch (p_equipment.ItemSlot)
				{
				case EItemSlot.ITEM_SLOT_HEAD:
					return EEquipSlots.HEAD;
				case EItemSlot.ITEM_SLOT_TORSO:
					return EEquipSlots.BODY;
				case EItemSlot.ITEM_SLOT_HAND:
					return EEquipSlots.HANDS;
				case EItemSlot.ITEM_SLOT_FEET:
					return EEquipSlots.FEET;
				case EItemSlot.ITEM_SLOT_NECKLACE:
					return EEquipSlots.NECK;
				case EItemSlot.ITEM_SLOT_RING:
					if (m_equipment.IsSlotOccupied(8) || !m_equipment.IsSlotOccupied(9))
					{
						return EEquipSlots.FINGER1;
					}
					return EEquipSlots.FINGER2;
				case EItemSlot.ITEM_SLOT_1_HAND:
				{
					Boolean flag = m_equipment.IsSlotOccupied(0);
					Boolean flag2 = m_equipment.IsSlotOccupied(1);
					if (flag || !flag2)
					{
						return EEquipSlots.MAIN_HAND;
					}
					return EEquipSlots.OFF_HAND;
				}
				case EItemSlot.ITEM_SLOT_2_HAND:
					return EEquipSlots.MAIN_HAND;
				case EItemSlot.ITEM_SLOT_MAINHAND:
					return EEquipSlots.MAIN_HAND;
				case EItemSlot.ITEM_SLOT_OFFHAND:
					return EEquipSlots.OFF_HAND;
				case EItemSlot.ITEM_SLOT_RANGED:
					return EEquipSlots.RANGE_WEAPON;
				}
			}
			return (EEquipSlots)(-1);
		}

		public void AddItem(BaseItem p_item, EEquipSlots p_slot)
		{
			Equipment equipment = GetItemAt(p_slot) as Equipment;
			if (equipment != null && equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND)
			{
				if (p_slot == EEquipSlots.MAIN_HAND)
				{
					m_equipment.RemoveItemAt(1);
				}
				else if (p_slot == EEquipSlots.OFF_HAND)
				{
					m_equipment.RemoveItemAt(0);
				}
			}
			Equipment equipment2 = p_item as Equipment;
			if (p_item != null)
			{
				p_item.PriceMultiplicator = LegacyLogic.Instance.WorldManager.ItemResellMultiplicator;
				if (equipment2.IsRelic() && !equipment2.IsTracked)
				{
					equipment2.IsTracked = true;
					LegacyLogic.Instance.TrackingManager.TrackRelicEquipped(equipment2, m_character);
				}
			}
			m_equipment.AddItem(p_item, (Int32)p_slot);
			if (p_item != null && equipment2.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND)
			{
				if (p_slot == EEquipSlots.MAIN_HAND)
				{
					m_equipment.AddItem(p_item, 1);
				}
				else if (p_slot == EEquipSlots.OFF_HAND)
				{
					m_equipment.AddItem(p_item, 0);
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, EventArgs.Empty);
			m_character.CalculateCurrentAttributes();
		}

		public void RemoveItemAt(EEquipSlots p_slot)
		{
			AddItem(null, p_slot);
		}

		public BaseItem GetItemAt(EEquipSlots p_slotType)
		{
			return m_equipment.GetItemAt((Int32)p_slotType);
		}

		public Int32 GetCurrentItemCount()
		{
			return m_equipment.GetCurrentItemCount();
		}

		public Int32 GetMaximumItemCount()
		{
			return m_equipment.GetMaximumItemCount();
		}

		public void ChangeItemPosition(Int32 p_slot1, Int32 p_slot2)
		{
			m_equipment.ChangeItemPosition(p_slot1, p_slot2);
		}

		public Boolean IsItemPlaceableAt(BaseItem p_item, Int32 p_slot)
		{
			if (p_item == null)
			{
				return true;
			}
			Equipment equipment = p_item as Equipment;
			if (equipment == null)
			{
				return false;
			}
			if (!equipment.Identified)
			{
				return false;
			}
			Boolean flag = IsCorrectSlot(equipment, p_slot);
			return flag && m_character.SkillHandler.IsSkillRequirementFulfilled(equipment, (EEquipSlots)p_slot);
		}

		public Boolean IsCorrectSlot(Equipment p_equipment, Int32 p_slot)
		{
			switch (p_equipment.ItemSlot)
			{
			case EItemSlot.ITEM_SLOT_HEAD:
				return p_slot == 3;
			case EItemSlot.ITEM_SLOT_TORSO:
				return p_slot == 4;
			case EItemSlot.ITEM_SLOT_HAND:
				return p_slot == 5;
			case EItemSlot.ITEM_SLOT_FEET:
				return p_slot == 6;
			case EItemSlot.ITEM_SLOT_NECKLACE:
				return p_slot == 7;
			case EItemSlot.ITEM_SLOT_RING:
				return p_slot == 8 || p_slot == 9;
			case EItemSlot.ITEM_SLOT_1_HAND:
				return p_slot == 0 || p_slot == 1;
			case EItemSlot.ITEM_SLOT_2_HAND:
				return p_slot == 0 || p_slot == 1;
			case EItemSlot.ITEM_SLOT_MAINHAND:
				return p_slot == 0;
			case EItemSlot.ITEM_SLOT_OFFHAND:
				return p_slot == 1;
			case EItemSlot.ITEM_SLOT_RANGED:
				return p_slot == 2;
			default:
				return false;
			}
		}

		public void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count)
		{
			m_equipment.SellItem(p_item, p_itemSlot, p_count);
		}

		public Boolean IsSlotOccupied(Int32 p_slot)
		{
			return m_equipment.IsSlotOccupied(p_slot);
		}

		public Equipment DoArmorBreakCheck()
		{
			LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.BREAKING_ITEMS);
			Armor randomItem = m_equipment.GetRandomItem<Armor>();
			if (randomItem != null && randomItem.BreakCheck())
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.BROKEN_ITEMS);
				ItemStatusEventArgs p_eventArgs = new ItemStatusEventArgs(randomItem, m_character);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, p_eventArgs);
				m_character.CalculateCurrentAttributes();
				return randomItem;
			}
			return null;
		}

		public Equipment DoMeleeWeaponBreakCheck()
		{
			LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.BREAKING_ITEMS);
			MeleeWeapon randomItem = m_equipment.GetRandomItem<MeleeWeapon>();
			if (randomItem != null && randomItem.BreakCheck())
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.BROKEN_ITEMS);
				ItemStatusEventArgs p_eventArgs = new ItemStatusEventArgs(randomItem, m_character);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, p_eventArgs);
				m_character.CalculateCurrentAttributes();
				return randomItem;
			}
			return null;
		}

		public Equipment DoShieldBreakCheck()
		{
			LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.BREAKING_ITEMS);
			Shield randomItem = m_equipment.GetRandomItem<Shield>();
			if (randomItem != null && randomItem.BreakCheck())
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.BROKEN_ITEMS);
				ItemStatusEventArgs p_eventArgs = new ItemStatusEventArgs(randomItem, m_character);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, p_eventArgs);
				m_character.CalculateCurrentAttributes();
				return randomItem;
			}
			return null;
		}

		public IInventory GetEventSource()
		{
			return m_equipment;
		}

		public void Load(SaveGameData p_data)
		{
			m_equipment.Load(p_data);
			BaseItem itemAt = m_equipment.GetItemAt(0);
			if (itemAt != null && ((Equipment)itemAt).ItemSlot == EItemSlot.ITEM_SLOT_2_HAND)
			{
				if (m_equipment.IsSlotOccupied(1))
				{
					BaseItem itemAt2 = m_equipment.GetItemAt(1);
					if (itemAt2 != itemAt)
					{
						m_equipment.RemoveItemAt(1);
					}
				}
				m_equipment.AddItem(itemAt, 1);
			}
		}

		public void Save(SaveGameData p_data)
		{
			BaseItem itemAt = m_equipment.GetItemAt(0);
			BaseItem itemAt2 = m_equipment.GetItemAt(1);
			BaseItem itemAt3 = m_equipment.GetItemAt(3);
			BaseItem itemAt4 = m_equipment.GetItemAt(8);
			BaseItem itemAt5 = m_equipment.GetItemAt(9);
			BaseItem itemAt6 = m_equipment.GetItemAt(7);
			BaseItem itemAt7 = m_equipment.GetItemAt(5);
			BaseItem itemAt8 = m_equipment.GetItemAt(4);
			BaseItem itemAt9 = m_equipment.GetItemAt(6);
			BaseItem itemAt10 = m_equipment.GetItemAt(2);
			Boolean flag = false;
			p_data.Set<Int32>("ItemCount", m_equipment.GetCurrentItemCount());
			Int32 p_counter = 0;
			if (itemAt != null)
			{
				flag = (((Equipment)itemAt).ItemSlot == EItemSlot.ITEM_SLOT_2_HAND);
				p_counter = SaveItem(EEquipSlots.MAIN_HAND, p_counter, itemAt, p_data);
			}
			if (itemAt2 != null && !flag)
			{
				p_counter = SaveItem(EEquipSlots.OFF_HAND, p_counter, itemAt2, p_data);
			}
			if (itemAt3 != null)
			{
				p_counter = SaveItem(EEquipSlots.HEAD, p_counter, itemAt3, p_data);
			}
			if (itemAt4 != null)
			{
				p_counter = SaveItem(EEquipSlots.FINGER1, p_counter, itemAt4, p_data);
			}
			if (itemAt5 != null)
			{
				p_counter = SaveItem(EEquipSlots.FINGER2, p_counter, itemAt5, p_data);
			}
			if (itemAt6 != null)
			{
				p_counter = SaveItem(EEquipSlots.NECK, p_counter, itemAt6, p_data);
			}
			if (itemAt7 != null)
			{
				p_counter = SaveItem(EEquipSlots.HANDS, p_counter, itemAt7, p_data);
			}
			if (itemAt8 != null)
			{
				p_counter = SaveItem(EEquipSlots.BODY, p_counter, itemAt8, p_data);
			}
			if (itemAt9 != null)
			{
				p_counter = SaveItem(EEquipSlots.FEET, p_counter, itemAt9, p_data);
			}
			if (itemAt10 != null)
			{
				p_counter = SaveItem(EEquipSlots.RANGE_WEAPON, p_counter, itemAt10, p_data);
			}
		}

		public Int32 SaveItem(EEquipSlots p_slot, Int32 p_counter, BaseItem p_item, SaveGameData p_data)
		{
			p_data.Set<Int32>("Slot" + p_counter, (Int32)p_slot);
			EDataType itemType = p_item.GetItemType();
			p_data.Set<Int32>("DataType" + p_counter, (Int32)itemType);
			if (itemType != EDataType.NONE)
			{
				SaveGameData saveGameData = new SaveGameData("Item" + p_counter);
				p_item.Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
			p_counter++;
			return p_counter;
		}
	}
}
