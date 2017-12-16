using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI;
using UnityEngine;

namespace Legacy.Game
{
	public class IdentifyItemContainer : TradingItemContainer
	{
		private EMode m_currentMode;

		private IdentifyInventoryController m_identifyInventory;

		private RepairInventoryController m_repairInventory;

		private IInventory m_currentInventory;

		private IdentifyScreen m_identifyScreen;

		private Single m_originListPosition;

		public IdentifyInventoryController IdentifyInventory => m_identifyInventory;

	    public RepairInventoryController RepairInventory => m_repairInventory;

	    public EMode Mode => m_currentMode;

	    public void Init(IdentifyScreen p_identifyScreen)
		{
			m_identifyScreen = p_identifyScreen;
			m_tradingScreen = null;
			m_itemSlots = new List<ItemSlotTrading>();
			m_grid = GetComponent<UIGrid>();
			m_originListPosition = transform.localPosition.y;
		}

		public void SetIdentifyInventory(IdentifyInventoryController p_identifyInventory)
		{
			m_currentMode = EMode.IDENTIFY;
			m_identifyInventory = p_identifyInventory;
			m_repairInventory = null;
			m_currentInventory = m_identifyInventory;
			UpdateItems();
			SelectItemSlot(null);
		}

		public void SetRepairInventory(RepairInventoryController p_repairInventory)
		{
			m_currentMode = EMode.REPAIR;
			m_repairInventory = p_repairInventory;
			m_identifyInventory = null;
			m_currentInventory = m_repairInventory;
			UpdateItems();
			SelectItemSlot(null);
		}

		public override void ItemRightClick(ItemSlotTrading p_slot)
		{
			SelectItemSlot(p_slot);
			if (m_identifyScreen != null)
			{
				if (m_currentMode == EMode.IDENTIFY && ((Equipment)p_slot.Item).Identified)
				{
					return;
				}
				if (m_currentMode == EMode.REPAIR && !((Equipment)p_slot.Item).Broken)
				{
					return;
				}
				m_identifyScreen.OnSingleItemButtonClicked(null);
			}
		}

		public override void AddItemSlot()
		{
			GameObject gameObject = NGUITools.AddChild(this.gameObject, m_itemSlotPrefab);
			ItemSlotIdentify component = gameObject.GetComponent<ItemSlotIdentify>();
			m_itemSlots.Add(component);
			Vector3 localPosition = component.transform.localPosition;
			localPosition.z = -26f;
			component.transform.localPosition = localPosition;
			component.Index = m_itemSlots.Count - 1;
			component.Parent = this;
		}

		public override void UpdateItems()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_currentInventory.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_currentInventory.GetItemAt(i);
				if (itemAt != null)
				{
					AddItem(itemAt, num, i);
					num++;
				}
			}
			for (Int32 j = num; j < m_itemSlots.Count; j++)
			{
				m_itemSlots[j].SetItem(null, 0);
				NGUITools.SetActiveSelf(m_itemSlots[j].gameObject, false);
			}
			m_grid.Reposition();
		}

		public void PlayAllFX(Single p_listHeight)
		{
			Single num = transform.localPosition.y - m_originListPosition;
			Single num2 = p_listHeight + num;
			for (Int32 i = 0; i < m_itemSlots.Count; i++)
			{
				if (m_itemSlots[i].transform.localPosition.y <= -num && m_itemSlots[i].transform.localPosition.y >= -num2)
				{
					Debug.Log("PlayAllFX " + i);
					((ItemSlotIdentify)m_itemSlots[i]).PlayFX(m_currentMode);
				}
			}
		}

		public override Boolean NeedsScrollBar()
		{
			return m_currentInventory.GetCurrentItemCount() > 8;
		}

		public override Boolean CanBuyConditionsMet(Int32 p_need, Int32 p_own)
		{
			if (m_currentMode == EMode.IDENTIFY)
			{
				if (m_identifyInventory.GetCurrentItemCount() == 0)
				{
					return false;
				}
				if (m_identifyInventory.FromScroll && m_identifyInventory.HasScrollResourcesForAmount(1))
				{
					return true;
				}
				if (m_identifyInventory.FromSpell && LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.ManaPoints >= m_identifyInventory.IdentifyPrice)
				{
					return true;
				}
				if (!m_identifyInventory.FromSpell && !m_identifyInventory.FromScroll && LegacyLogic.Instance.WorldManager.Party.Gold >= m_identifyInventory.IdentifyPrice)
				{
					return true;
				}
			}
			else
			{
				if (m_repairInventory.GetCurrentItemCount() == 0)
				{
					return false;
				}
				if (LegacyLogic.Instance.WorldManager.Party.Gold >= m_repairInventory.RepairPrice)
				{
					return true;
				}
			}
			return false;
		}

		public override void SelectItemSlot(ItemSlotTrading p_slot)
		{
			if (m_selectedItem != null)
			{
				m_selectedItem.SetSelected(false);
			}
			if (p_slot != null)
			{
				p_slot.SetSelected(true);
				m_selectedItem = p_slot;
				Boolean flag = CanBuyConditionsMet(p_slot.Item.Price, LegacyLogic.Instance.WorldManager.Party.Gold);
				Equipment equipment = (Equipment)p_slot.Item;
				if (m_buyButton != null)
				{
					if (m_currentMode == EMode.IDENTIFY)
					{
						m_buyButton.isEnabled = (flag && !equipment.Identified);
						m_buyButtonText.color = ((!flag || equipment.Identified) ? Color.gray : Color.white);
					}
					else
					{
						m_buyButton.isEnabled = (flag && equipment.Broken);
						m_buyButtonText.color = ((!flag || !equipment.Broken) ? Color.gray : Color.white);
					}
				}
			}
			else
			{
				m_selectedItem = null;
				if (m_buyButton != null)
				{
					m_buyButton.isEnabled = false;
					m_buyButtonText.color = Color.gray;
				}
			}
		}

		public enum EMode
		{
			IDENTIFY,
			REPAIR
		}
	}
}
