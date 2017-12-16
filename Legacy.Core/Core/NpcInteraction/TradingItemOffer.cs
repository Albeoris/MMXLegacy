using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.NpcInteraction.Conditions;
using Legacy.Core.PartyManagement;
using Legacy.Core.SaveGameManagement;
using Legacy.Core.StaticData;
using Legacy.Utilities;

namespace Legacy.Core.NpcInteraction
{
	public class TradingItemOffer : ISaveGameObject
	{
		private List<BaseItem> m_items = new List<BaseItem>();

		private ItemOfferStaticData m_offerData;

		private MMTime m_updateTime;

		private DialogCondition[] m_conditions;

		private Boolean m_initialItemUpdateDone;

		public TradingItemOffer(Int32 p_offerId, DialogCondition[] p_conditions)
		{
			m_offerData = StaticDataHandler.GetStaticData<ItemOfferStaticData>(EDataType.ITEM_OFFERS, p_offerId);
			m_conditions = p_conditions;
			m_initialItemUpdateDone = false;
		}

		public List<BaseItem> Items => m_items;

	    public ItemOfferStaticData OfferData => m_offerData;

	    public void UpdateItems()
		{
			Boolean flag = !m_initialItemUpdateDone;
			m_initialItemUpdateDone = true;
			if (m_offerData.RefreshType == EOfferRefreshType.DAYBREAK && m_updateTime.Days != LegacyLogic.Instance.GameTime.Time.Days)
			{
				flag = true;
			}
			if (flag)
			{
				m_items.Clear();
				m_updateTime = LegacyLogic.Instance.GameTime.Time;
				foreach (ItemOffer itemOffer in m_offerData.ItemOffers)
				{
					for (Int32 j = 0; j < itemOffer.ItemQuantity; j++)
					{
						BaseItem baseItem = ItemFactory.CreateItem(itemOffer.ItemType, itemOffer.ItemID);
						Boolean flag2 = false;
						foreach (BaseItem baseItem2 in m_items)
						{
							flag2 = Consumable.AreSameConsumables(baseItem2, baseItem);
							if (flag2)
							{
								Consumable consumable = (Consumable)baseItem2;
								consumable.Counter++;
								break;
							}
						}
						if (baseItem != null && !flag2)
						{
							m_items.Add(baseItem);
						}
					}
				}
			}
		}

		public EDialogState CheckConditions(Npc p_npc)
		{
			EDialogState edialogState = EDialogState.NORMAL;
			if (m_conditions != null)
			{
				for (Int32 i = 0; i < m_conditions.Length; i++)
				{
					EDialogState edialogState2 = m_conditions[i].CheckCondition(p_npc);
					if (edialogState2 > edialogState)
					{
						edialogState = edialogState2;
						if (edialogState == EDialogState.HIDDEN)
						{
							break;
						}
					}
				}
			}
			return edialogState;
		}

		public void AddItemsToInventory(IInventory p_inventory)
		{
			Boolean flag = false;
			NpcEffect npcEffect;
			if (LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HasEffect(ETargetCondition.HIRE_BONUSTRADEDISC, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
			{
				flag = true;
			}
			foreach (BaseItem baseItem in m_items)
			{
				if (flag)
				{
					baseItem.PriceMultiplicator = 1f - npcEffect.EffectValue;
				}
				else
				{
					baseItem.PriceMultiplicator = 1f;
				}
				p_inventory.AddItem(baseItem);
			}
		}

		public void Load(SaveGameData p_data)
		{
			m_items.Clear();
			Int32 num = p_data.Get<Int32>("ItemCount", 0);
			for (Int32 i = 0; i < num; i++)
			{
				EDataType p_type = (EDataType)p_data.Get<Int32>("Type" + i, 0);
				SaveGameData saveGameData = p_data.Get<SaveGameData>("Item" + i, null);
				if (saveGameData != null)
				{
					try
					{
						BaseItem baseItem = ItemFactory.CreateItem(p_type);
						baseItem.Load(saveGameData);
						m_items.Add(baseItem);
					}
					catch (Exception ex)
					{
						LegacyLogger.Log(ex.ToString());
					}
				}
			}
			SaveGameData saveGameData2 = p_data.Get<SaveGameData>("UpdateTime", null);
			if (saveGameData2 != null)
			{
				m_updateTime.Load(saveGameData2);
			}
			m_initialItemUpdateDone = p_data.Get<Boolean>("InitialItemUpdateDone", false);
		}

		public void Save(SaveGameData p_data)
		{
			p_data.Set<Int32>("ItemCount", m_items.Count);
			for (Int32 i = 0; i < m_items.Count; i++)
			{
				p_data.Set<Int32>("Type" + i, (Int32)m_items[i].GetItemType());
				SaveGameData saveGameData = new SaveGameData("Item" + i);
				m_items[i].Save(saveGameData);
				p_data.Set<SaveGameData>(saveGameData.ID, saveGameData);
			}
			SaveGameData saveGameData2 = new SaveGameData("UpdateTime");
			m_updateTime.Save(saveGameData2);
			p_data.Set<SaveGameData>(saveGameData2.ID, saveGameData2);
			p_data.Set<Boolean>("InitialItemUpdateDone", m_initialItemUpdateDone);
		}
	}
}
