using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class RepairInventoryController : IInventory
	{
		private ERepairType m_repairType;

		private Inventory m_inventory;

		private List<TradingItemOffer> m_repairOffers;

		private Int32 m_repairPrice;

		private Boolean m_isRepairing;

		public RepairInventoryController()
		{
			Init(ERepairType.ALL);
		}

		public event EventHandler UpdateDialog;

		public Int32 RepairPrice
		{
			get => m_repairPrice;
		    set => m_repairPrice = value;
		}

		public Boolean IsRepairing => m_isRepairing;

	    public ERepairType RepairType => m_repairType;

	    public void Init(ERepairType p_repairType)
		{
			m_repairOffers = new List<TradingItemOffer>();
			m_repairType = p_repairType;
			Int32 num = 0;
			switch (m_repairType)
			{
			case ERepairType.ALL:
				num += LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.GetBrokenItemsCount();
				num += LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.GetBrokenItemsCount();
				foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					if (character != null && character.Equipment.Equipment.HasBrokenItems())
					{
						num += character.Equipment.Equipment.GetBrokenItemsCount();
					}
				}
				break;
			case ERepairType.WEAPONS:
				num += LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.GetBrokenWeaponsCount();
				num += LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.GetBrokenWeaponsCount();
				foreach (Character character2 in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					if (character2 != null && character2.Equipment.Equipment.HasBrokenWeapons())
					{
						num += character2.Equipment.Equipment.GetBrokenWeaponsCount();
					}
				}
				break;
			case ERepairType.ARMOR_AND_SHIELD:
				num += LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.GetBrokenArmorsCount();
				num += LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.GetBrokenArmorsCount();
				foreach (Character character3 in LegacyLogic.Instance.WorldManager.Party.Members)
				{
					if (character3 != null && character3.Equipment.Equipment.HasBrokenArmors())
					{
						num += character3.Equipment.Equipment.GetBrokenArmorsCount();
					}
				}
				break;
			}
			m_inventory = new Inventory(num);
			m_inventory.isOffer = true;
		}

		public void StartRepair()
		{
			m_inventory.Clear();
			m_isRepairing = true;
			List<Equipment> brokenItems = LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.GetBrokenItems();
			foreach (Equipment equipment in brokenItems)
			{
				if (IsAbleToRepair(equipment))
				{
					m_inventory.AddItem(equipment);
				}
			}
			brokenItems = LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.GetBrokenItems();
			foreach (Equipment equipment2 in brokenItems)
			{
				if (IsAbleToRepair(equipment2))
				{
					m_inventory.AddItem(equipment2);
				}
			}
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				if (character != null && character.Equipment.Equipment.HasBrokenItems())
				{
					foreach (Equipment equipment3 in character.Equipment.Equipment.GetBrokenItems())
					{
						if (IsAbleToRepair(equipment3))
						{
							m_inventory.AddItem(equipment3);
						}
					}
				}
			}
			LegacyLogic.Instance.ConversationManager._HideNPCs();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_REPAIR_START, EventArgs.Empty);
		}

		public void StopRepair()
		{
			m_isRepairing = false;
			LegacyLogic.Instance.ConversationManager._ShowNPCs();
			if (UpdateDialog != null)
			{
				UpdateDialog(this, EventArgs.Empty);
			}
			foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
			{
				character.CalculateCurrentAttributes();
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_REPAIR_STOP, EventArgs.Empty);
		}

		private Boolean IsAbleToRepair(Equipment p_equipment)
		{
			return m_repairType == ERepairType.ALL || (m_repairType == ERepairType.ARMOR_AND_SHIELD && (p_equipment.GetItemType() == EDataType.ARMOR || p_equipment.GetItemType() == EDataType.SHIELD)) || (m_repairType == ERepairType.WEAPONS && p_equipment.GetItemType() == EDataType.MELEE_WEAPON);
		}

		public Int32 GetCurrentItemCount()
		{
			return m_inventory.GetCurrentItemCount();
		}

		public Int32 GetMaximumItemCount()
		{
			return m_inventory.GetMaximumItemCount();
		}

		public Boolean IsFull()
		{
			return m_inventory.IsFull();
		}

		public Boolean CanAddItem(BaseItem p_item)
		{
			return false;
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
			if (!IsFull())
			{
				for (Int32 i = 0; i < m_inventory.GetMaximumItemCount(); i++)
				{
					if (m_inventory.GetItemAt(i) == null)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public void AddItem(BaseItem p_item, Int32 p_slot)
		{
			m_inventory.AddItem(p_item, p_slot);
		}

		public void RemoveItemAt(Int32 p_slot)
		{
			BaseItem itemAt = GetItemAt(p_slot);
			m_inventory.RemoveItemAt(p_slot);
			foreach (TradingItemOffer tradingItemOffer in m_repairOffers)
			{
				foreach (BaseItem baseItem in tradingItemOffer.Items)
				{
					if (baseItem == itemAt)
					{
						tradingItemOffer.Items.Remove(baseItem);
						return;
					}
				}
			}
		}

		public BaseItem GetItemAt(Int32 p_slot)
		{
			return m_inventory.GetItemAt(p_slot);
		}

		public void ChangeItemPosition(Int32 p_slot1, Int32 p_slot2)
		{
			m_inventory.ChangeItemPosition(p_slot1, p_slot2);
		}

		public Boolean IsItemPlaceableAt(BaseItem p_item, Int32 p_slot)
		{
			return false;
		}

		public Boolean IsSlotOccupied(Int32 p_slot)
		{
			return m_inventory.IsSlotOccupied(p_slot);
		}

		public void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count)
		{
		}

		public IInventory GetEventSource()
		{
			return m_inventory;
		}
	}
}
