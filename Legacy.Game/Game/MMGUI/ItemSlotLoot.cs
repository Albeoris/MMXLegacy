using System;
using Legacy.Core.Entities.Items;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ItemSlotLoot")]
	public class ItemSlotLoot : MonoBehaviour
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
		protected UILabel m_itemCounter;

		[SerializeField]
		protected UISprite m_hoverTexture;

		[SerializeField]
		protected UISprite m_selectTexture;

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

		protected LootItemContainer m_parent;

		protected Boolean m_selected;

		protected Int32 m_originSlotIndex;

		protected Boolean m_hovered;

		public LootItemContainer Parent
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
					m_itemName.text = p_item.Name;
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
					NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
					NGUITools.SetActiveSelf(m_scrollTexture.gameObject, false);
				}
			}
			UpdateItemCounter(p_item);
		}

		public void SetSelected(Boolean p_selected)
		{
			m_selected = p_selected;
			if (m_hovered)
			{
				TweenAlpha.Begin(m_selectTexture.gameObject, m_alphaTweenDuration, (!m_selected) ? m_hoverAlpha : m_selectedHoverAlpha);
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
			else if (p_item is GoldStack)
			{
				GoldStack goldStack = (GoldStack)p_item;
				NGUITools.SetActiveSelf(m_itemCounter.gameObject, true);
				m_itemCounter.text = goldStack.Amount.ToString();
			}
			else
			{
				NGUITools.SetActiveSelf(m_itemCounter.gameObject, false);
			}
		}

		private void OnTooltip(Boolean show)
		{
			if (show && m_item != null)
			{
				TooltipManager.Instance.Show(this, m_item, Parent.Container.Content, gameObject.transform.position, m_selectTexture.gameObject.transform.localScale * 0.5f);
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
				else if (DragDropManager.Instance.DraggedItem is LootDragObject && ((LootDragObject)DragDropManager.Instance.DraggedItem).ItemSlot == this)
				{
					DragDropManager.Instance.StopDrag();
				}
			}
			else if (UICamera.currentTouchID == -2 && !p_isDown)
			{
				m_parent.ItemRightClick(this);
			}
		}

		private void OnDrag(Vector2 p_delta)
		{
			if (DragDropManager.Instance.DraggedItem == null && UICamera.currentTouchID == -1)
			{
				DragDropManager.Instance.StartDrag(new LootDragObject(this));
			}
		}
	}
}
