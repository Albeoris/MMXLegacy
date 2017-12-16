using System;
using Legacy.Core.Entities.Items;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/InventorySlot")]
	public class InventorySlot : ItemSlot
	{
		[SerializeField]
		private Color m_hoverColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		protected UILabel m_itemCounter;

		[SerializeField]
		protected UISprite m_scrollTexture;

		public override void SetItem(BaseItem p_item)
		{
			base.SetItem(p_item);
			UpdateItemCounter(p_item);
			if (m_item is Scroll)
			{
				NGUITools.SetActiveSelf(m_scrollTexture.gameObject, true);
				m_itemTexture.spriteName = "ITM_consumable_scroll";
				m_scrollTexture.spriteName = m_item.Icon;
			}
			else
			{
				NGUITools.SetActiveSelf(m_scrollTexture.gameObject, false);
			}
		}

		private void UpdateItemCounter(BaseItem p_item)
		{
			if (p_item is Consumable)
			{
				Consumable consumable = (Consumable)p_item;
				NGUITools.SetActiveSelf(m_itemCounter.gameObject, true);
				m_itemCounter.text = consumable.Counter.ToString();
			}
			else
			{
				NGUITools.SetActiveSelf(m_itemCounter.gameObject, false);
			}
		}

		protected override void EnableSlot()
		{
			base.EnableSlot();
			NGUITools.SetActiveSelf(m_itemCounter.gameObject, m_item is Consumable);
		}

		public override void OnDragHover(DragHoverEventArgs p_eventArgs)
		{
			if (DragDropManager.Instance.DraggedItem != null && DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				BaseItem item = ((ItemDragObject)DragDropManager.Instance.DraggedItem).Item;
				if (item != null)
				{
					NGUITools.SetActive(m_hoverTexture.gameObject, p_eventArgs.IsHovered);
					if (p_eventArgs.IsHovered)
					{
						DragDropManager.Instance.SetHoveredSlot(this);
						TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_hoverColor);
					}
					else
					{
						DragDropManager.Instance.SetHoveredSlot(null);
						TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_originColor);
					}
				}
			}
		}
	}
}
