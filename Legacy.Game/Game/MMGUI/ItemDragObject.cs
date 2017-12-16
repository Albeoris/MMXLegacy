using System;
using Legacy.Core.Entities.Items;

namespace Legacy.Game.MMGUI
{
	public class ItemDragObject : BaseDragObject
	{
		private ItemSlot m_itemSlot;

		private BaseItem m_item;

		public ItemDragObject(ItemSlot p_slot)
		{
			m_itemSlot = p_slot;
			m_item = p_slot.Item;
		}

		public BaseItem Item => m_item;

	    public ItemSlot ItemSlot => m_itemSlot;

	    public override void SetActive(Boolean p_active)
		{
			base.SetActive(p_active);
			if (p_active)
			{
				m_sprite.atlas = m_itemSlot.ItemTexture.atlas;
				m_sprite.spriteName = m_itemSlot.ItemTexture.spriteName;
				m_itemSlot.Parent.Inventory.RemoveItemAt(m_itemSlot.Index);
			}
			if (m_item is Equipment)
			{
				Equipment equipment = (Equipment)m_item;
				if (p_active)
				{
					ItemSlot.UpdateItemBackground(m_itemBackground, m_item);
				}
				NGUITools.SetActiveSelf(m_brokenSprite.gameObject, equipment.Broken && p_active);
			}
			else if (m_item is Consumable)
			{
				Consumable consumable = (Consumable)m_item;
				if (m_item is Scroll)
				{
					m_sprite.spriteName = "ITM_consumable_scroll";
					m_scrollSprite.spriteName = m_item.Icon;
					NGUITools.SetActiveSelf(m_scrollSprite.gameObject, p_active);
				}
				NGUITools.SetActiveSelf(m_itemCounter.gameObject, p_active);
				m_itemCounter.text = consumable.Counter.ToString();
			}
		}

		public override void CancelDragAction()
		{
			m_itemSlot.Parent.Inventory.AddItem(m_item, m_itemSlot.Index);
		}
	}
}
