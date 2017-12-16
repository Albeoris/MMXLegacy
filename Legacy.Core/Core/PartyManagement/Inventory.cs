using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core.PartyManagement
{
	public class Inventory : ISaveGameObject, IInventory
	{
		private BaseItem[] m_items;

		private Int32 m_maxItems;

		private Int32 m_count;

		private Boolean m_isOffer;

		public Inventory(Int32 p_maxItems)
		{
			m_maxItems = p_maxItems;
			m_items = new BaseItem[p_maxItems];
		}

		public void Clear()
		{
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if (m_items[i] != null)
				{
					RemoveItemAt(i);
				}
			}
		}

		public Boolean isOffer
		{
			get => m_isOffer;
		    set => m_isOffer = value;
		}

		public Boolean AddItem(BaseItem p_item)
		{
			Int32 autoSlot = GetAutoSlot(p_item);
			if (autoSlot >= 0)
			{
				AddItem(p_item, autoSlot);
			}
			return autoSlot >= 0;
		}

		public Int32 GetAutoSlot(BaseItem p_item)
		{
			if (p_item is Consumable)
			{
				for (Int32 i = 0; i < m_items.Length; i++)
				{
					BaseItem p_item2 = m_items[i];
					if (Consumable.AreSameConsumables(p_item2, p_item))
					{
						return i;
					}
				}
			}
			for (Int32 j = 0; j < m_items.Length; j++)
			{
				if (m_items[j] == null)
				{
					return j;
				}
			}
			return -1;
		}

		public void AddItem(BaseItem p_item, Int32 p_slot)
		{
			BaseItem itemAt = GetItemAt(p_slot);
			if (Consumable.AreSameConsumables(itemAt, p_item))
			{
				Consumable consumable = (Consumable)itemAt;
				Consumable consumable2 = (Consumable)p_item;
				consumable.Counter += consumable2.Counter;
				if (!isOffer)
				{
					InventoryItemEventArgs p_eventArgs = new InventoryItemEventArgs(new InventorySlotRef(this, p_slot));
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_ADDED, p_eventArgs);
				}
			}
			else
			{
				RemoveItemAt(p_slot);
				if (p_item != null)
				{
					m_items[p_slot] = p_item;
					m_count++;
					if (!isOffer)
					{
						InventoryItemEventArgs p_eventArgs2 = new InventoryItemEventArgs(new InventorySlotRef(this, p_slot));
						LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_ADDED, p_eventArgs2);
					}
				}
			}
		}

		public Boolean CanAddItem(BaseItem p_item)
		{
			return p_item is GoldStack || GetAutoSlot(p_item) >= 0;
		}

		public Boolean IsFull()
		{
			return m_count >= m_maxItems;
		}

		public void RemoveItemAt(Int32 p_slot)
		{
			if (m_items[p_slot] != null)
			{
				InventoryItemEventArgs p_eventArgs = new InventoryItemEventArgs(new InventorySlotRef(this, p_slot));
				m_items[p_slot] = null;
				m_count--;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REMOVED, p_eventArgs);
			}
		}

		public BaseItem GetItemAt(Int32 p_slot)
		{
			if (p_slot < m_maxItems && p_slot >= 0)
			{
				return m_items[p_slot];
			}
			return null;
		}

		public Int32 GetCurrentItemCount()
		{
			return m_count;
		}

		public Int32 GetMaximumItemCount()
		{
			return m_maxItems;
		}

		public void ChangeItemPosition(Int32 p_slot1, Int32 p_slot2)
		{
			BaseItem itemAt = GetItemAt(p_slot1);
			BaseItem itemAt2 = GetItemAt(p_slot2);
			AddItem(itemAt, p_slot2);
			AddItem(itemAt2, p_slot1);
		}

		public Boolean IsItemPlaceableAt(BaseItem p_item, Int32 p_slot)
		{
			return true;
		}

		public Boolean IsSlotOccupied(Int32 p_slot)
		{
			return m_items[p_slot] != null;
		}

		public T GetRandomItem<T>() where T : Equipment
		{
			List<T> list = new List<T>();
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if (m_items[i] is T)
				{
					T t = (T)m_items[i];
					if (!t.Broken)
					{
						list.Add((T)m_items[i]);
					}
				}
			}
			if (list.Count > 0)
			{
				Int32 index = Random.Range(0, list.Count - 1);
				return list[index];
			}
			return null;
		}

		public void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count)
		{
			Consumable consumable = p_item as Consumable;
			LegacyLogic.Instance.WorldManager.Party.ChangeGold(p_item.Price * p_count);
			InventoryItemEventArgs p_eventArgs = new InventoryItemEventArgs(new InventorySlotRef(this, p_itemSlot));
			if (consumable != null && consumable.Counter > p_count)
			{
				consumable.Counter -= p_count;
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_CHANGED, p_eventArgs);
			}
			else
			{
				RemoveItemAt(p_itemSlot);
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_SOLD, p_eventArgs);
			}
		}

		public IInventory GetEventSource()
		{
			return this;
		}

		public Boolean HasBrokenItems()
		{
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if (m_items[i] is Equipment && ((Equipment)m_items[i]).Broken)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean HasBrokenWeapons()
		{
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if ((m_items[i] is MeleeWeapon || m_items[i] is RangedWeapon) && ((Equipment)m_items[i]).Broken)
				{
					return true;
				}
			}
			return false;
		}

		public Boolean HasBrokenArmors()
		{
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if ((m_items[i] is Armor || m_items[i] is Shield) && ((Equipment)m_items[i]).Broken)
				{
					return true;
				}
			}
			return false;
		}

		public Int32 GetBrokenItemsCount()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if (m_items[i] is Equipment && ((Equipment)m_items[i]).Broken)
				{
					num++;
				}
			}
			return num;
		}

		public Int32 GetBrokenArmorsCount()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if ((m_items[i] is Armor || m_items[i] is Shield) && ((Equipment)m_items[i]).Broken)
				{
					num++;
				}
			}
			return num;
		}

		public Int32 GetBrokenWeaponsCount()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if ((m_items[i] is MeleeWeapon || m_items[i] is RangedWeapon) && ((Equipment)m_items[i]).Broken)
				{
					num++;
				}
			}
			return num;
		}

		public List<Equipment> GetBrokenItems()
		{
			List<Equipment> list = new List<Equipment>();
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if (m_items[i] is Equipment && ((Equipment)m_items[i]).Broken && !list.Contains(m_items[i] as Equipment))
				{
					list.Add((Equipment)m_items[i]);
				}
			}
			return list;
		}

		public List<Equipment> GetUnidentifiedItems()
		{
			List<Equipment> list = new List<Equipment>();
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if (m_items[i] is Equipment && !((Equipment)m_items[i]).Identified)
				{
					list.Add((Equipment)m_items[i]);
				}
			}
			return list;
		}

		public Boolean RepairAllItems()
		{
			Boolean result = false;
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if (m_items[i] is Equipment && ((Equipment)m_items[i]).Broken)
				{
					((Equipment)m_items[i]).Broken = false;
					result = true;
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new InventoryItemEventArgs(InventoryItemEventArgs.ERepairType.ALL));
			return result;
		}

		public Boolean RepairWeapons()
		{
			Boolean result = false;
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if ((m_items[i] is MeleeWeapon || m_items[i] is RangedWeapon) && ((Equipment)m_items[i]).Broken)
				{
					((Equipment)m_items[i]).Broken = false;
					result = true;
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new InventoryItemEventArgs(InventoryItemEventArgs.ERepairType.WEAPONS));
			return result;
		}

		public Boolean RepairArmorAndShields()
		{
			Boolean result = false;
			for (Int32 i = 0; i < m_maxItems; i++)
			{
				if ((m_items[i] is Armor || m_items[i] is Shield) && ((Equipment)m_items[i]).Broken)
				{
					((Equipment)m_items[i]).Broken = false;
					result = true;
				}
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, new InventoryItemEventArgs(InventoryItemEventArgs.ERepairType.ARMOR));
			return result;
		}

		public void Load(SaveGameData p_data)
		{
			Int32 num = p_data.Get<Int32>("ItemCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				Int32 p_slot = p_data.Get<Int32>("Slot" + i, 0);
				Int32 p_type = (Int32)p_data.Get<EDataType>("DataType" + i, EDataType.NONE);
				SaveGameData saveGameData = p_data.Get<SaveGameData>("Item" + i, null);
				if (saveGameData != null)
				{
					try
					{
						BaseItem baseItem = ItemFactory.CreateItem((EDataType)p_type);
						baseItem.Load(saveGameData);
						AddItem(baseItem, p_slot);
					}
					catch (Exception ex)
					{
						LegacyLogger.Log(ex.ToString());
					}
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("ItemCount", m_count);
			Int32 num = 0;
			for (Int32 i = 0; i < m_items.Length; i++)
			{
				if (m_items[i] != null)
				{
					p_data.Set<Int32>("Slot" + num, i);
					EDataType itemType = m_items[i].GetItemType();
					p_data.Set<Int32>("DataType" + num, (Int32)itemType);
					if (itemType != EDataType.NONE)
					{
						SaveGameData saveGameData = new SaveGameData("Item" + num);
						m_items[i].Save(saveGameData);
						p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
						num++;
					}
				}
			}
		}
	}
}
