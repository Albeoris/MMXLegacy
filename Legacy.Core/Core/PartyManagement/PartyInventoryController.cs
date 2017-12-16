using System;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.Hints;
using Legacy.Core.SaveGameManagement;

namespace Legacy.Core.PartyManagement
{
	public class PartyInventoryController : ISaveGameObject, IInventory
	{
		private Inventory m_inventory;

		private Party m_party;

		public PartyInventoryController(Party p_party)
		{
			m_party = p_party;
			m_inventory = new Inventory(ConfigManager.Instance.Game.InventorySize);
		}

		public Inventory Inventory => m_inventory;

	    public void ConsumeItem(Int32 p_slot, Int32 p_targetCharacter)
		{
			BaseItem itemAt = GetItemAt(p_slot);
			if (itemAt is Consumable)
			{
				Consumable consumable = (Consumable)itemAt;
				consumable.Consume(new InventorySlotRef(this, p_slot), p_targetCharacter);
			}
		}

		public void ConsumeSuccess(Consumable p_consumable)
		{
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				if (GetItemAt(i) == p_consumable)
				{
					ConsumeSuccess(i);
					break;
				}
			}
		}

		public void ConsumeSuccess(Int32 p_slot)
		{
			BaseItem itemAt = GetItemAt(p_slot);
			if (itemAt is Consumable)
			{
				Consumable consumable = (Consumable)itemAt;
				consumable.Counter--;
				InventoryItemEventArgs p_eventArgs = new InventoryItemEventArgs(new InventorySlotRef(m_inventory, p_slot));
				LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_CHANGED, p_eventArgs);
				if (consumable.Counter <= 0)
				{
					RemoveItemAt(p_slot);
				}
				else
				{
					InventoryItemEventArgs p_eventArgs2 = new InventoryItemEventArgs(new InventorySlotRef(m_inventory, p_slot));
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_REMOVED, p_eventArgs2);
				}
			}
		}

		public void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count)
		{
			m_inventory.SellItem(p_item, p_itemSlot, p_count);
		}

		public Boolean HasUnidentifiedItems()
		{
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = GetItemAt(i);
				if (itemAt is Equipment && !((Equipment)itemAt).Identified)
				{
					return true;
				}
			}
			return false;
		}

		public void IdentifyAllItems()
		{
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				Equipment equipment = GetItemAt(i) as Equipment;
				if (equipment != null)
				{
					equipment.Identified = true;
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.INVENTORY_ITEM_CHANGED, new InventoryItemEventArgs(new InventorySlotRef(m_inventory, i)));
				}
			}
		}

		public Int32 GetTotalAmountOfConsumable(Consumable p_consumable)
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = GetItemAt(i);
				if (Consumable.AreSameConsumables(itemAt, p_consumable))
				{
					num += ((Consumable)itemAt).Counter;
				}
			}
			return num;
		}

		public Boolean AddItem(BaseItem p_item)
		{
			if (p_item != null)
			{
				if (p_item is GoldStack)
				{
					GoldStack goldStack = (GoldStack)p_item;
					m_party.ChangeGold(goldStack.Amount);
					return true;
				}
				p_item.PriceMultiplicator = LegacyLogic.Instance.WorldManager.ItemResellMultiplicator;
			}
			Boolean flag = m_inventory.AddItem(p_item);
			if (flag && p_item is Equipment && ((Equipment)p_item).IsRelic())
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.RELICS);
			}
			return flag;
		}

		public Int32 GetAutoSlot(BaseItem p_item)
		{
			return m_inventory.GetAutoSlot(p_item);
		}

		public void AddItem(BaseItem p_item, Int32 p_slot)
		{
			if (p_item != null)
			{
				if (p_item is GoldStack)
				{
					GoldStack goldStack = (GoldStack)p_item;
					m_party.ChangeGold(goldStack.Amount);
					return;
				}
				p_item.PriceMultiplicator = LegacyLogic.Instance.WorldManager.ItemResellMultiplicator;
			}
			m_inventory.AddItem(p_item, p_slot);
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

		public Int32 GetSlotIndexForItem(BaseItem p_item)
		{
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_inventory.GetItemAt(i);
				if (itemAt != null && p_item == itemAt)
				{
					return i;
				}
			}
			return -1;
		}

		public Potion GetBestPotion(EPotionType p_type)
		{
			Potion potion = null;
			if (p_type == EPotionType.HEALTH_POTION || p_type == EPotionType.MANA_POTION)
			{
				Int32 num = 0;
				for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
				{
					BaseItem itemAt = m_inventory.GetItemAt(i);
					if (itemAt != null)
					{
						num++;
					}
					if (itemAt is Potion)
					{
						Potion potion2 = (Potion)itemAt;
						if (potion2.PotionType == p_type && potion2.Counter > 0 && (potion == null || potion2.Value > potion.Value))
						{
							potion = potion2;
						}
					}
					if (num >= m_inventory.GetCurrentItemCount())
					{
						break;
					}
				}
			}
			return potion;
		}

		public Int32 GetConsumableSlot(Consumable p_consumable)
		{
			for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_inventory.GetItemAt(i);
				if (Consumable.AreSameConsumables(itemAt, p_consumable))
				{
					return i;
				}
			}
			return -1;
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
	}
}
