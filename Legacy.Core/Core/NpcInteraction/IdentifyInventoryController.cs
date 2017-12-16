using System;
using System.Collections.Generic;
using System.Threading;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.NpcInteraction
{
	public class IdentifyInventoryController : IInventory
	{
		private List<TradingItemOffer> m_identifyOffers;

		protected Inventory m_inventory;

		protected Boolean m_isIdentifying;

		protected Int32 m_identifyPrice;

		private CharacterSpell m_sourceSpell;

		private Scroll m_sourceScroll;

		private Int32 m_scrollInventorySlot;

		public IdentifyInventoryController() : this(null)
		{
		}

		public IdentifyInventoryController(Object p_caller)
		{
			Init(p_caller);
		}

		public event EventHandler UpdateDialog;

		public Boolean IsIdentifying => m_isIdentifying;

	    public Int32 IdentifyPrice
		{
			get => m_identifyPrice;
	        set => m_identifyPrice = value;
	    }

		public Boolean FromSpell => m_sourceSpell != null;

	    public Boolean FromScroll => m_sourceScroll != null;

	    public void Init(Object p_caller)
		{
			m_identifyOffers = new List<TradingItemOffer>();
			m_isIdentifying = false;
			m_sourceSpell = null;
			m_sourceScroll = null;
			if (p_caller is Scroll)
			{
				Scroll scroll = (Scroll)p_caller;
				m_identifyPrice = 0;
				m_sourceScroll = scroll;
				m_scrollInventorySlot = LegacyLogic.Instance.WorldManager.Party.Inventory.GetSlotIndexForItem(scroll);
				if (m_scrollInventorySlot < 0)
				{
					LegacyLogic.Instance.WorldManager.Party.MuleInventory.GetSlotIndexForItem(scroll);
				}
			}
			else if (p_caller is CharacterSpell)
			{
				CharacterSpell characterSpell = (CharacterSpell)p_caller;
				m_identifyPrice = characterSpell.StaticData.ManaCost;
				m_sourceSpell = characterSpell;
			}
			else if (p_caller is Npc)
			{
				m_scrollInventorySlot = -1;
				m_sourceSpell = null;
				m_sourceScroll = null;
			}
			else
			{
				m_identifyPrice = 0;
				m_scrollInventorySlot = -1;
			}
			Int32 num = LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.GetUnidentifiedItems().Count;
			num += LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.GetUnidentifiedItems().Count;
			m_inventory = new Inventory(num);
			m_inventory.isOffer = true;
		}

		public void StartIdentify()
		{
			m_inventory.Clear();
			m_isIdentifying = true;
			List<Equipment> unidentifiedItems = LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.GetUnidentifiedItems();
			foreach (Equipment p_item in unidentifiedItems)
			{
				m_inventory.AddItem(p_item);
			}
			unidentifiedItems = LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.GetUnidentifiedItems();
			foreach (Equipment p_item2 in unidentifiedItems)
			{
				m_inventory.AddItem(p_item2);
			}
			LegacyLogic.Instance.ConversationManager._HideNPCs();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_IDENTIFY_START, EventArgs.Empty);
		}

		public void StopIdentify()
		{
			m_isIdentifying = false;
			LegacyLogic.Instance.ConversationManager._ShowNPCs();
			if (UpdateDialog != null && !FromScroll && !FromSpell)
			{
				UpdateDialog(this, EventArgs.Empty);
			}
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_IDENTIFY_STOP, EventArgs.Empty);
		}

		public Boolean HasScrollResourcesForAmount(Int32 p_amount)
		{
			if (m_sourceScroll == null)
			{
				return false;
			}
			PartyInventoryController partyInventoryController = LegacyLogic.Instance.WorldManager.Party.Inventory;
			BaseItem itemAt = partyInventoryController.GetItemAt(m_scrollInventorySlot);
			if (itemAt != m_sourceScroll)
			{
				partyInventoryController = LegacyLogic.Instance.WorldManager.Party.MuleInventory;
				itemAt = partyInventoryController.GetItemAt(m_scrollInventorySlot);
			}
			if (itemAt == null)
			{
				return false;
			}
			Scroll scroll = (Scroll)itemAt;
			return scroll != null && scroll.Counter >= p_amount;
		}

		public void RemoveOneScroll()
		{
			PartyInventoryController partyInventoryController = LegacyLogic.Instance.WorldManager.Party.Inventory;
			BaseItem itemAt = partyInventoryController.GetItemAt(m_scrollInventorySlot);
			if (itemAt != m_sourceScroll)
			{
				partyInventoryController = LegacyLogic.Instance.WorldManager.Party.MuleInventory;
				itemAt = partyInventoryController.GetItemAt(m_scrollInventorySlot);
			}
			if ((Scroll)itemAt == null)
			{
				return;
			}
			partyInventoryController.ConsumeSuccess(m_sourceScroll);
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

		public void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count)
		{
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
			if (m_sourceScroll != null && m_scrollInventorySlot >= 0)
			{
				RemoveOneScroll();
			}
			foreach (TradingItemOffer tradingItemOffer in m_identifyOffers)
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

		public IInventory GetEventSource()
		{
			return m_inventory;
		}
	}
}
