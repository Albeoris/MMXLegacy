using System;
using System.Collections.Generic;
using Legacy.Core.Entities.InteractiveObjects;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/LootItemContainer")]
	public class LootItemContainer : MonoBehaviour
	{
		public const Int32 ITEM_VISIBLE_AMOUNT = 8;

		[SerializeField]
		private GameObject m_itemSlotPrefab;

		[SerializeField]
		private UIButton m_lootButton;

		[SerializeField]
		private UIButton m_lootAllButton;

		[SerializeField]
		private UILabel m_lootButtonText;

		[SerializeField]
		private UILabel m_lootAllButtonText;

		protected List<ItemSlotLoot> m_itemSlots;

		protected Container m_container;

		protected ItemSlotLoot m_selectedItem;

		protected UIGrid m_grid;

		protected LootScreen m_lootScreen;

		public Container Container => m_container;

	    public UIGrid Grid => m_grid;

	    public ItemSlotLoot SelectedItem => m_selectedItem;

	    public Boolean LootAllButtonAvailable => m_lootAllButton.isEnabled;

	    private void Start()
		{
			m_lootButton.isEnabled = false;
			m_lootButtonText.color = Color.gray;
		}

		private void OnEnable()
		{
			m_grid.Reposition();
		}

		public void Init(LootScreen p_lootScreen)
		{
			m_lootScreen = p_lootScreen;
			m_itemSlots = new List<ItemSlotLoot>();
			m_grid = GetComponent<UIGrid>();
		}

		public void SetInventory(Container p_container)
		{
			m_container = p_container;
			UpdateItems();
			if (m_selectedItem != null)
			{
				SelectItemSlot(null);
			}
		}

		public void UpdateItems()
		{
			Int32 num = 0;
			for (Int32 i = 0; i < m_container.Content.GetMaximumItemCount(); i++)
			{
				BaseItem itemAt = m_container.Content.GetItemAt(i);
				if (itemAt != null)
				{
					AddItem(itemAt, num, i);
					num++;
				}
			}
			m_lootAllButton.isEnabled = (num > 0);
			m_lootAllButtonText.color = ((num <= 0) ? Color.gray : Color.white);
			for (Int32 j = num; j < m_itemSlots.Count; j++)
			{
				m_itemSlots[j].SetItem(null, 0);
				NGUITools.SetActiveSelf(m_itemSlots[j].gameObject, false);
			}
			m_grid.Reposition();
		}

		protected virtual void AddItem(BaseItem p_item, Int32 p_slotIndex, Int32 p_inventorySlot)
		{
			while (p_slotIndex >= m_itemSlots.Count)
			{
				AddItemSlot();
			}
			m_itemSlots[p_slotIndex].SetItem(p_item, p_inventorySlot);
			NGUITools.SetActiveSelf(m_itemSlots[p_slotIndex].gameObject, true);
		}

		public void AddItemSlot()
		{
			GameObject gameObject = NGUITools.AddChild(this.gameObject, m_itemSlotPrefab);
			ItemSlotLoot component = gameObject.GetComponent<ItemSlotLoot>();
			m_itemSlots.Add(component);
			component.Index = m_itemSlots.Count - 1;
			component.Parent = this;
		}

		public void SelectItemSlot(ItemSlotLoot p_slot)
		{
			if (m_selectedItem != null)
			{
				m_selectedItem.SetSelected(false);
			}
			m_lootButton.isEnabled = (p_slot != null);
			m_lootButtonText.color = ((!(p_slot != null)) ? Color.gray : Color.white);
			if (p_slot != null)
			{
				p_slot.SetSelected(true);
			}
			m_selectedItem = p_slot;
		}

		public Boolean NeedsScrollBar()
		{
			return Container.Content.GetCurrentItemCount() > 8;
		}

		public void ItemRightClick(ItemSlotLoot p_slot)
		{
			SelectItemSlot(p_slot);
			m_lootScreen.OnLootButtonClicked();
		}

		public void DropCallback(IInventory p_inventory, Int32 p_targetSlot)
		{
			m_lootScreen.LootItem(p_inventory, p_targetSlot);
		}
	}
}
