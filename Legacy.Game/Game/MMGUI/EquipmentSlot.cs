using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/EquipmentSlot")]
	public class EquipmentSlot : ItemSlot
	{
		[SerializeField]
		private Color m_ghostColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private Color m_equippableColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private Color m_unequippableColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private Color m_normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

		[SerializeField]
		private UISprite m_slotTypeTexture;

		[SerializeField]
		private String m_tooltipText = String.Empty;

		private Boolean m_isGhost;

		public Boolean IsGhost => m_isGhost;

	    public override void OnDragHover(DragHoverEventArgs p_eventArgs)
		{
			if (IsTwoHandSlot() && DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				Equipment equipment = ((ItemDragObject)DragDropManager.Instance.DraggedItem).Item as Equipment;
				if (equipment != null && equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND)
				{
					OnTwoHandSlotHover(p_eventArgs);
				}
			}
			base.OnDragHover(p_eventArgs);
		}

		private Boolean IsTwoHandSlot()
		{
			return Index == 0 || Index == 1;
		}

		private void OnTwoHandSlotHover(DragHoverEventArgs p_eventArgs)
		{
			EquipmentItemContainer equipmentItemContainer = Parent as EquipmentItemContainer;
			if (equipmentItemContainer != null)
			{
				equipmentItemContainer.UpdateTwoHandedHighlight(this, p_eventArgs);
			}
		}

		public void ForceDragHover(DragHoverEventArgs p_eventArgs)
		{
			base.OnDragHover(p_eventArgs);
		}

		public override void SetItem(BaseItem p_item)
		{
			Equipment equipment = p_item as Equipment;
			if (equipment != null && equipment.ItemSlot == EItemSlot.ITEM_SLOT_2_HAND && m_index == 1)
			{
				SetAsGhostItem(true);
			}
			else if (IsGhost)
			{
				SetAsGhostItem(false);
			}
			NGUITools.SetActiveSelf(m_slotTypeTexture.gameObject, equipment == null);
			base.SetItem(p_item);
		}

		public void SetAsGhostItem(Boolean p_isGhost)
		{
			m_isGhost = p_isGhost;
			if (m_isGhost)
			{
				m_itemTexture.color = m_ghostColor;
			}
			else
			{
				m_itemTexture.color = m_normalColor;
			}
		}

		public void ShopEquipmentDragStarted(Equipment p_equipment)
		{
			if (!m_initialised)
			{
				return;
			}
			if (Parent.Inventory.IsItemPlaceableAt(p_equipment, m_index))
			{
				TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_equippableColor);
			}
			else if (((CharacterInventoryController)Parent.Inventory).IsCorrectSlot(p_equipment, m_index))
			{
				TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_unequippableColor);
			}
		}

		public void EquipmentDragStarted(Equipment p_equipment)
		{
			if (!m_initialised)
			{
				return;
			}
			if (Parent.Inventory.IsItemPlaceableAt(p_equipment, m_index))
			{
				TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_equippableColor);
			}
			else if (((CharacterInventoryController)Parent.Inventory).IsCorrectSlot(p_equipment, m_index))
			{
				TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_unequippableColor);
			}
		}

		public void EquipmentDragStopped()
		{
			if (!m_initialised)
			{
				return;
			}
			TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_originColor);
		}

		public void ShopEquipmentDragStopped()
		{
			if (!m_initialised)
			{
				return;
			}
			TweenColor.Begin(m_tweenTarget, m_hoverDuration, m_originColor);
		}

		public override void OnTooltip(Boolean p_isOver)
		{
			if (m_item != null)
			{
				if (p_isOver)
				{
					TooltipManager.Instance.Show(this, m_item, Parent.Inventory, gameObject.transform.position, m_itemTexture.gameObject.transform.localScale * 0.5f);
				}
				else
				{
					TooltipManager.Instance.Hide(this);
				}
			}
			else if (p_isOver)
			{
				Vector3 position = m_slotTypeTexture.transform.position;
				Vector3 p_offset = m_slotTypeTexture.transform.localScale * 0.5f;
				TooltipManager.Instance.Show(this, LocaManager.GetText(m_tooltipText), position, p_offset);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}
	}
}
