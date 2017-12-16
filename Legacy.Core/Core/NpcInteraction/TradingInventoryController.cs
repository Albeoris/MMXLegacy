using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class TradingInventoryController : ISaveGameObject, IInventory
	{
		private List<TradingItemOffer> m_offers;

		protected Inventory m_inventory;

		private Npc m_npc;

		private Boolean m_isTrading;

		public TradingInventoryController(NpcConversationStaticData p_data, Npc p_npc)
		{
			m_offers = new List<TradingItemOffer>();
			m_inventory = new Inventory(ConfigManager.Instance.Game.TradingInventorySize);
			m_inventory.isOffer = true;
			m_npc = p_npc;
			if (p_data.m_offers != null)
			{
				foreach (NpcConversationStaticData.Offer offer in p_data.m_offers)
				{
					TradingItemOffer item = new TradingItemOffer(offer.m_id, offer.m_conditions);
					m_offers.Add(item);
				}
			}
		}

		public Boolean IsTrading => m_isTrading;

	    public Npc Npc => m_npc;

	    public void UpdateOffers()
		{
			foreach (TradingItemOffer tradingItemOffer in m_offers)
			{
				tradingItemOffer.UpdateItems();
			}
		}

		public void StartTrade()
		{
			m_inventory.Clear();
			m_isTrading = true;
			foreach (TradingItemOffer tradingItemOffer in m_offers)
			{
				if (tradingItemOffer.CheckConditions(m_npc) == EDialogState.NORMAL)
				{
					tradingItemOffer.AddItemsToInventory(this);
				}
			}
			LegacyLogic.Instance.ConversationManager._HideNPCs();
			NPCTradeEventArgs p_eventArgs = new NPCTradeEventArgs(this);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_TRADE_START, p_eventArgs);
		}

		public void StopTrading()
		{
			m_isTrading = false;
			LegacyLogic.Instance.ConversationManager._ShowNPCs();
			NPCTradeEventArgs p_eventArgs = new NPCTradeEventArgs(this);
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_TRADE_STOP, p_eventArgs);
		}

		private TradingItemOffer GetOfferById(Int32 p_staticID)
		{
			foreach (TradingItemOffer tradingItemOffer in m_offers)
			{
				if (tradingItemOffer.OfferData.StaticID == p_staticID)
				{
					return tradingItemOffer;
				}
			}
			return null;
		}

		public void SellItem(BaseItem p_item, Int32 p_itemSlot, Int32 p_count)
		{
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
			foreach (TradingItemOffer tradingItemOffer in m_offers)
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

		public void Load(SaveGameData p_data)
		{
			Int32 num = p_data.Get<Int32>("OfferCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				Int32 p_staticID = p_data.Get<Int32>("OfferID" + i, 1);
				TradingItemOffer offerById = GetOfferById(p_staticID);
				if (offerById != null)
				{
					SaveGameData saveGameData = p_data.Get<SaveGameData>("Offer" + i, null);
					if (saveGameData != null)
					{
						offerById.Load(saveGameData);
					}
				}
			}
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("OfferCount", m_offers.Count);
			Int32 num = 0;
			foreach (TradingItemOffer tradingItemOffer in m_offers)
			{
				p_data.Set<Int32>("OfferID" + num, tradingItemOffer.OfferData.StaticID);
				SaveGameData saveGameData = new SaveGameData("Offer" + num);
				tradingItemOffer.Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
				num++;
			}
		}
	}
}
