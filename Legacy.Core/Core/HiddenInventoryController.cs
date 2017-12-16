using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core
{
	public class HiddenInventoryController : ISaveGameObject, IInventory
	{
		private Inventory m_inventory;

		private Party m_party;

		private Int32 m_goldAmount;

		private Boolean m_isTrading;

		public HiddenInventoryController(Party p_party)
		{
			m_party = p_party;
			m_inventory = new Inventory(500);
			m_goldAmount = 0;
		}

		public Inventory Inventory => m_inventory;

	    public Boolean IsTrading => m_isTrading;

	    public Boolean AddItem(BaseItem p_item)
		{
			if (p_item != null)
			{
				if (p_item is GoldStack)
				{
					GoldStack goldStack = (GoldStack)p_item;
					m_goldAmount += goldStack.Amount;
					return true;
				}
				p_item.PriceMultiplicator = LegacyLogic.Instance.WorldManager.ItemResellMultiplicator;
			}
			return m_inventory.AddItem(p_item);
		}

		public void AddItem(BaseItem p_item, Int32 p_slot)
		{
			if (p_item != null)
			{
				if (p_item is GoldStack)
				{
					GoldStack goldStack = (GoldStack)p_item;
					m_goldAmount += goldStack.Amount;
					return;
				}
				p_item.PriceMultiplicator = LegacyLogic.Instance.WorldManager.ItemResellMultiplicator;
			}
			m_inventory.AddItem(p_item, p_slot);
		}

		public void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count)
		{
			m_inventory.SellItem(p_item, p_itemSlot, p_count);
		}

		public Int32 GetAutoSlot(BaseItem p_item)
		{
			return m_inventory.GetAutoSlot(p_item);
		}

		public Boolean CanAddItem(BaseItem p_item)
		{
			return Inventory.CanAddItem(p_item);
		}

		public Boolean IsFull()
		{
			return m_inventory.IsFull();
		}

		public void RemoveItemAt(Int32 p_slot)
		{
			m_inventory.RemoveItemAt(p_slot);
		}

		public BaseItem GetItemAt(Int32 p_slot)
		{
			return m_inventory.GetItemAt(p_slot);
		}

		public Int32 GetCurrentItemCount()
		{
			return m_inventory.GetCurrentItemCount();
		}

		public Int32 GetMaximumItemCount()
		{
			return m_inventory.GetMaximumItemCount();
		}

		public void ChangeItemPosition(Int32 p_slot1, Int32 p_slot2)
		{
			m_inventory.ChangeItemPosition(p_slot1, p_slot2);
		}

		public Boolean IsItemPlaceableAt(BaseItem p_item, Int32 p_slot)
		{
			return m_inventory.IsItemPlaceableAt(p_item, p_slot);
		}

		public Boolean IsSlotOccupied(Int32 p_slot)
		{
			return m_inventory.IsSlotOccupied(p_slot);
		}

		public IInventory GetEventSource()
		{
			return m_inventory;
		}

		public void Load(SaveGameData p_data)
		{
			m_inventory.Load(p_data);
		}

		public void Save(SaveGameData p_data)
		{
			m_inventory.Save(p_data);
		}

		public void RemoveAmountOfItemAt(Int32 p_slot, Int32 p_count)
		{
			BaseItem itemAt = GetItemAt(p_slot);
			if (itemAt is Consumable)
			{
				Consumable consumable = (Consumable)itemAt;
				consumable.Counter -= p_count;
				if (consumable.Counter <= 0)
				{
					RemoveItemAt(p_slot);
				}
			}
			else
			{
				RemoveItemAt(p_slot);
			}
		}
	}
}
