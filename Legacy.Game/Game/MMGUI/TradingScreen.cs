using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/TradingScreen")]
	public class TradingScreen : BaseScreen, IScrollingListener
	{
		[SerializeField]
		private TradingItemContainer m_itemContainer;

		[SerializeField]
		private UIScrollBar m_scrollBar;

		[SerializeField]
		private Single m_scrollDuration = 0.1f;

		private Vector3 m_itemContainerOrigin;

		public TradingItemContainer ItemContainer
		{
			get => m_itemContainer;
		    set => m_itemContainer = value;
		}

		public void Init()
		{
			m_itemContainer.Init(this);
			m_itemContainerOrigin = m_itemContainer.transform.localPosition;
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			ScrollingHelper.InitScrollListeners(this, m_scrollBar.gameObject);
		}

		public void CleanUp()
		{
			m_itemContainer.CleanUp();
		}

		private void OnDestroy()
		{
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
		}

		protected override void OpenDefaultTab()
		{
			SetScrollBarPosition(0);
			ScrollingHelper.InitScrollListeners(this, m_itemContainer.gameObject);
		}

		protected override void OpenTab(Int32 p_tabIndex)
		{
		}

		private void SetScrollBarPosition(Int32 p_position)
		{
			if (m_itemContainer.NeedsScrollBar())
			{
				NGUITools.SetActive(m_scrollBar.gameObject, true);
				Int32 num = 8;
				Int32 currentItemCount = m_itemContainer.Inventory.GetCurrentItemCount();
				Int32 num2 = currentItemCount - num;
				if (p_position < 0)
				{
					p_position = 0;
				}
				else if (p_position > num2)
				{
					p_position = num2;
				}
				m_scrollBar.barSize = num / (Single)currentItemCount;
				if (num2 > 0)
				{
					m_scrollBar.scrollValue = p_position / (Single)num2;
				}
				else
				{
					m_scrollBar.scrollValue = 0f;
				}
				TweenPosition.Begin(m_itemContainer.gameObject, m_scrollDuration, m_itemContainerOrigin + new Vector3(0f, p_position * m_itemContainer.Grid.cellHeight, 0f));
			}
			else
			{
				NGUITools.SetActive(m_scrollBar.gameObject, false);
				TweenPosition.Begin(m_itemContainer.gameObject, m_scrollDuration, m_itemContainerOrigin);
			}
		}

		public void OnBuyButtonClicked()
		{
			BuyItem(LegacyLogic.Instance.WorldManager.Party.ActiveInventory, -1);
		}

		public void BuyItem(IInventory p_targetInventory, Int32 p_targetSlot)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			BaseItem item = m_itemContainer.SelectedItem.Item;
			if (party.Gold >= item.Price)
			{
				if (!p_targetInventory.CanAddItem(item))
				{
					p_targetInventory = party.GetOtherInventory(p_targetInventory);
				}
				if (!p_targetInventory.CanAddItem(item))
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.INVENTORY_FULL);
					return;
				}
				if (item is Consumable)
				{
					Consumable consumable = (Consumable)item;
					Int32 num = consumable.Counter;
					if (num * consumable.Price > party.Gold)
					{
						num = party.Gold / consumable.Price;
					}
					if (num > 1)
					{
						PopupRequest.Instance.OpenRequest(PopupRequest.ERequestType.SPLIT_ITEMS, String.Empty, LocaManager.GetText("POPUP_REQUEST_ITEMS_BUY_LABEL"), new PopupRequest.RequestCallback(ItemSellSplitterCallback));
						PopupRequest.Instance.ItemSplitter.Open(PopupItemSplitter.Mode.BUY, num, consumable, m_itemContainer.SelectedItem.OriginSlotIndex, p_targetInventory, p_targetSlot);
					}
					else
					{
						Consumable consumable2 = (Consumable)ItemFactory.CreateItem(consumable.GetItemType(), consumable.StaticId);
						consumable2.PriceMultiplicator = consumable.PriceMultiplicator;
						m_itemContainer.Inventory.RemoveAmountOfItemAt(m_itemContainer.SelectedItem.OriginSlotIndex, 1);
						CompleteBuyItem(p_targetInventory, consumable2, 1, p_targetSlot);
						if (consumable.Counter <= 0)
						{
							m_itemContainer.SelectItemSlot(null);
						}
						m_itemContainer.UpdateItems();
						UpdateScrollBar();
					}
				}
				else
				{
					m_itemContainer.Inventory.RemoveItemAt(m_itemContainer.SelectedItem.OriginSlotIndex);
					CompleteBuyItem(p_targetInventory, item, 1, p_targetSlot);
					m_itemContainer.SelectItemSlot(null);
					m_itemContainer.UpdateItems();
					UpdateScrollBar();
					if (item is Equipment && ((Equipment)item).IsRelic())
					{
						LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.RELIC);
					}
				}
			}
		}

		public void CompleteBuyItem(IInventory p_inventory, BaseItem p_item, Int32 p_count, Int32 p_targetIndex)
		{
			Consumable consumable = p_item as Consumable;
			if (consumable != null)
			{
				consumable.Counter = p_count;
			}
			Int32 p_delta = -p_item.Price * p_count;
			LegacyLogic.Instance.WorldManager.Party.ChangeGold(p_delta);
			if (p_inventory is CharacterInventoryController)
			{
				Party party = LegacyLogic.Instance.WorldManager.Party;
				Equipment equipment = p_inventory.GetItemAt(p_targetIndex) as Equipment;
				party.AddItem(equipment);
				Equipment equipment2 = p_item as Equipment;
				if (equipment2 != null && equipment2.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND && equipment != null && equipment.ItemSlot != EItemSlot.ITEM_SLOT_2_HAND)
				{
					if (p_targetIndex == 0 && p_inventory.GetItemAt(1) != null)
					{
						party.AddItem(p_inventory.GetItemAt(1));
					}
					else if (p_targetIndex == 1 && p_inventory.GetItemAt(0) != null)
					{
						party.AddItem(p_inventory.GetItemAt(0));
					}
				}
			}
			if (p_targetIndex == -1)
			{
				p_inventory.AddItem(p_item);
			}
			else
			{
				p_inventory.AddItem(p_item, p_targetIndex);
			}
			AudioController.Play("BuyItem");
		}

		private void ItemSellSplitterCallback(PopupRequest.EResultType p_result, String p_inputString)
		{
			if (p_result == PopupRequest.EResultType.CONFIRMED)
			{
				Consumable consumable = (Consumable)PopupRequest.Instance.ItemSplitter.Item;
				Consumable consumable2 = (Consumable)ItemFactory.CreateItem(consumable.GetItemType(), consumable.StaticId);
				consumable2.PriceMultiplicator = consumable.PriceMultiplicator;
				Int32 count = PopupRequest.Instance.ItemSplitter.Count;
				m_itemContainer.Inventory.RemoveAmountOfItemAt(PopupRequest.Instance.ItemSplitter.ItemSlotIndex, count);
				CompleteBuyItem(PopupRequest.Instance.ItemSplitter.TargetInventory, consumable2, count, PopupRequest.Instance.ItemSplitter.TargetSlotIndex);
				if (consumable.Counter <= 0)
				{
					m_itemContainer.SelectItemSlot(null);
				}
				m_itemContainer.UpdateItems();
				UpdateScrollBar();
			}
			PopupRequest.Instance.ItemSplitter.Finish();
		}

		private void UpdateScrollBar()
		{
			Single num = 8f;
			Single num2 = m_itemContainer.Inventory.GetCurrentItemCount();
			Single num3 = num2 - num + 1f;
			SetScrollBarPosition((Int32)(num3 * m_scrollBar.scrollValue));
		}

		private void OnScrollBarChange(UIScrollBar p_sb)
		{
			Single num = 8f;
			Single num2 = m_itemContainer.Inventory.GetCurrentItemCount();
			Single num3 = num2 - num;
			if (num3 > 0f)
			{
				SetScrollBarPosition(Mathf.RoundToInt(p_sb.scrollValue * num3));
			}
			else
			{
				SetScrollBarPosition(0);
			}
		}

		public void OnScroll(Single p_delta)
		{
			Single num = 8f;
			Single num2 = m_itemContainer.Inventory.GetCurrentItemCount();
			Single num3 = num2 - num;
			if (num3 > 0f)
			{
				Single num4 = -1f / num3 * p_delta * 10f;
				SetScrollBarPosition(Mathf.RoundToInt((m_scrollBar.scrollValue + num4) * num3));
			}
			else
			{
				SetScrollBarPosition(0);
			}
		}
	}
}
