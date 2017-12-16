using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ItemSlotTrading")]
	public class ItemSlotTrading : MonoBehaviour
	{
		public const String ITEM_MISSING_TEXTURE = "ITM_missing";

		[SerializeField]
		protected UISprite m_itemTexture;

		[SerializeField]
		protected UISprite m_itemBackground;

		[SerializeField]
		protected UISprite m_scrollTexture;

		[SerializeField]
		protected UILabel m_itemName;

		[SerializeField]
		protected UILabel m_itemCost;

		[SerializeField]
		protected UILabel m_itemCounter;

		[SerializeField]
		protected UISprite m_hoverTexture;

		[SerializeField]
		protected UISprite m_selectTexture;

		[SerializeField]
		protected UISprite m_goldIcon;

		[SerializeField]
		protected Color m_insufficientGold = new Color(1f, 0f, 0f, 1f);

		[SerializeField]
		private Single m_selectedAlpha = 0.5f;

		[SerializeField]
		private Single m_hoverAlpha = 0.25f;

		[SerializeField]
		private Single m_selectedHoverAlpha = 0.6f;

		[SerializeField]
		private Single m_alphaTweenDuration = 0.05f;

		protected BaseItem m_item;

		protected Int32 m_index;

		protected TradingItemContainer m_parent;

		protected Boolean m_selected;

		protected Int32 m_originSlotIndex;

		protected Color m_originColorCost;

		protected Boolean m_hovered;

		protected Int32 m_itemPrice;

		protected Boolean m_canDrag;

		public TradingItemContainer Parent
		{
			get => m_parent;
		    set => m_parent = value;
		}

		public UISprite ItemTexture => m_itemTexture;

	    public BaseItem Item => m_item;

	    public Int32 OriginSlotIndex => m_originSlotIndex;

	    public Int32 Index
		{
			get => m_index;
	        set => m_index = value;
	    }

		private void Awake()
		{
			m_canDrag = true;
			m_originColorCost = GetElementColor(m_itemCost.gameObject);
			NGUITools.SetActiveSelf(m_hoverTexture.gameObject, false);
			SetSelected(false);
		}

		private Color GetElementColor(GameObject p_object)
		{
			if (p_object != null)
			{
				UIWidget component = p_object.GetComponent<UIWidget>();
				if (component != null)
				{
					return component.color;
				}
				Renderer renderer = p_object.renderer;
				if (renderer != null)
				{
					return renderer.material.color;
				}
				Light light = p_object.light;
				if (light != null)
				{
					return light.color;
				}
			}
			return new Color(1f, 1f, 1f, 1f);
		}

		private void OnDisable()
		{
			NGUITools.SetActiveSelf(m_hoverTexture.gameObject, false);
			TooltipManager.Instance.Hide(this);
		}

		public virtual void SetItem(BaseItem p_item, Int32 p_originSlotIndex)
		{
			m_item = p_item;
			m_originSlotIndex = p_originSlotIndex;
			if (m_itemTexture != null)
			{
				if (p_item != null)
				{
					m_itemPrice = p_item.Price;
					m_itemName.text = p_item.Name;
					NGUITools.SetActive(m_itemCost.gameObject, true);
					NGUITools.SetActive(m_goldIcon.gameObject, true);
					m_itemCost.text = m_itemPrice.ToString();
					UpdateItemCostColor();
					ItemSlot.UpdateItemBackground(m_itemBackground, m_item);
					if (m_item.Icon != null && m_item.Icon != String.Empty)
					{
						if (m_item is Scroll)
						{
							m_itemTexture.spriteName = "ITM_consumable_scroll";
							m_scrollTexture.spriteName = m_item.Icon;
							NGUITools.SetActiveSelf(m_scrollTexture.gameObject, true);
						}
						else
						{
							NGUITools.SetActiveSelf(m_scrollTexture.gameObject, false);
							m_itemTexture.spriteName = m_item.Icon;
						}
					}
					else
					{
						m_itemTexture.spriteName = "ITM_missing";
					}
				}
				else
				{
					NGUITools.SetActiveSelf(m_scrollTexture.gameObject, false);
					NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
				}
			}
			UpdateItemCounter(p_item);
		}

		public virtual void UpdateItemCostColor()
		{
			if (m_itemPrice <= LegacyLogic.Instance.WorldManager.Party.Gold)
			{
				m_itemCost.color = m_originColorCost;
			}
			else
			{
				m_itemCost.color = m_insufficientGold;
			}
		}

		public void SetSelected(Boolean p_selected)
		{
			m_selected = p_selected;
			if (m_hovered)
			{
				TweenAlpha.Begin(m_selectTexture.gameObject, m_alphaTweenDuration, (!m_selected) ? m_hoverAlpha : m_selectedHoverAlpha);
				NGUITools.SetActiveSelf(m_selectTexture.gameObject, true);
			}
			else
			{
				TweenAlpha.Begin(m_selectTexture.gameObject, m_alphaTweenDuration, (!m_selected) ? 0f : m_selectedAlpha);
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

		private void OnDrop(GameObject m_drag)
		{
			if (UICamera.currentTouchID == -1)
			{
				m_parent.OnDrop(m_drag);
			}
		}

		private void OnTooltip(Boolean show)
		{
			if (show && m_item != null)
			{
				Equipment equipment = m_item as Equipment;
				if (equipment != null && equipment.Identified)
				{
					CharacterInventoryController equipment2 = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.Equipment;
					EEquipSlots autoSlotEquipped = equipment2.GetAutoSlotEquipped(equipment);
					if (autoSlotEquipped >= EEquipSlots.MAIN_HAND)
					{
						Equipment equipment3 = (Equipment)equipment2.GetItemAt(autoSlotEquipped);
						Equipment equipment4 = null;
						if (autoSlotEquipped == EEquipSlots.MAIN_HAND)
						{
							equipment4 = (Equipment)equipment2.GetItemAt(EEquipSlots.OFF_HAND);
							if (equipment4 == equipment3)
							{
								equipment4 = null;
							}
						}
						if (autoSlotEquipped == EEquipSlots.FINGER1)
						{
							equipment4 = (Equipment)equipment2.GetItemAt(EEquipSlots.FINGER2);
						}
						if (equipment3 is Shield != equipment is Shield)
						{
							equipment3 = null;
						}
						if (equipment4 is Shield != equipment is Shield)
						{
							equipment4 = null;
						}
						if (equipment3 != null)
						{
							TooltipManager.Instance.Show(this, equipment, equipment3, equipment4, Parent.Inventory, gameObject.transform.position, m_itemTexture.gameObject.transform.localScale * 0.5f);
							return;
						}
					}
				}
				TooltipManager.Instance.Show(this, m_item, Parent.Inventory, gameObject.transform.position, m_itemTexture.gameObject.transform.localScale * 0.5f);
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		protected virtual void OnHover(Boolean p_isHovered)
		{
			m_hovered = p_isHovered;
			if (p_isHovered)
			{
				TweenAlpha.Begin(m_selectTexture.gameObject, m_alphaTweenDuration, (!m_selected) ? m_hoverAlpha : m_selectedHoverAlpha);
			}
			else
			{
				TweenAlpha.Begin(m_selectTexture.gameObject, m_alphaTweenDuration, (!m_selected) ? 0f : m_selectedAlpha);
			}
			NGUITools.SetActiveSelf(m_hoverTexture.gameObject, p_isHovered);
		}

		protected virtual void OnPress(Boolean p_isDown)
		{
			if (UICamera.currentTouchID == -1)
			{
				if (p_isDown)
				{
					if (!m_selected)
					{
						m_parent.SelectItemSlot(this);
					}
				}
				else if (DragDropManager.Instance.DraggedItem is ShopDragObject && ((ShopDragObject)DragDropManager.Instance.DraggedItem).ItemSlot == this)
				{
					DragDropManager.Instance.StopDrag();
				}
			}
			else if (UICamera.currentTouchID == -2 && !p_isDown)
			{
				DragDropManager.Instance.StopDrag();
				m_parent.ItemRightClick(this);
			}
		}

		private void OnDrag(Vector2 p_delta)
		{
			if (!m_canDrag)
			{
				return;
			}
			if (DragDropManager.Instance.DraggedItem == null && UICamera.currentTouchID == -1)
			{
				DragDropManager.Instance.StartDrag(new ShopDragObject(this));
			}
		}
	}
}
