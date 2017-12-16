using System;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ItemSlot")]
	public class ItemSlot : MonoBehaviour
	{
		public const String ITEM_MISSING_TEXTURE = "ITM_missing";

		public static readonly Color ITEM_BACKGROUND_COLOR_NORMAL = Color.white;

		public static readonly Color ITEM_BACKGROUND_COLOR_SINGLE_ENCHANTMENT = Color.green;

		public static readonly Color ITEM_BACKGROUND_COLOR_DOUBLE_ENCHANTMENT = new Color(0f, 0.75f, 1f);

		public static readonly Color ITEM_BACKGROUND_COLOR_RELIC = new Color(1f, 0.75f, 0f);

		public static readonly Color ITEM_BACKGROUND_COLOR_UNIDENTIFIED = Color.red;

		public static Boolean m_isDragging;

		[SerializeField]
		protected UISprite m_itemTexture;

		[SerializeField]
		protected GameObject m_tweenTarget;

		[SerializeField]
		private UISprite m_itemBackground;

		[SerializeField]
		private UISprite m_brokenIcon;

		[SerializeField]
		protected UISprite m_hoverTexture;

		[SerializeField]
		protected Single m_hoverDuration = 0.1f;

		protected BaseItem m_item;

		protected Int32 m_index;

		protected ItemContainer m_parent;

		protected Color m_originColor;

		protected Boolean m_initialised;

		private Boolean m_moving;

		public ItemContainer Parent
		{
			get => m_parent;
		    set => m_parent = value;
		}

		public UISprite ItemTexture => m_itemTexture;

	    public BaseItem Item => m_item;

	    public Int32 Index
		{
			get => m_index;
	        set => m_index = value;
	    }

		private void Awake()
		{
			Init();
			NGUITools.SetActiveSelf(m_itemTexture.gameObject, false);
		}

		protected virtual void Init()
		{
			m_initialised = true;
			if (m_tweenTarget != null)
			{
				UIWidget component = m_tweenTarget.GetComponent<UIWidget>();
				if (component != null)
				{
					m_originColor = component.color;
				}
				else
				{
					Renderer renderer = m_tweenTarget.renderer;
					if (renderer != null)
					{
						m_originColor = renderer.material.color;
					}
					else
					{
						Light light = m_tweenTarget.light;
						if (light != null)
						{
							m_originColor = light.color;
						}
						else
						{
							m_originColor = new Color(1f, 1f, 1f, 1f);
						}
					}
				}
			}
		}

		private void OnAfterEnable()
		{
			EnableSlot();
		}

		protected virtual void EnableSlot()
		{
			if (m_itemTexture != null)
			{
				NGUITools.SetActiveSelf(m_itemTexture.gameObject, m_item != null);
				NGUITools.SetActive(m_hoverTexture.gameObject, false);
			}
		}

		private void OnDisable()
		{
			if (m_tweenTarget != null)
			{
				TweenColor component = m_tweenTarget.GetComponent<TweenColor>();
				if (component != null)
				{
					component.color = m_originColor;
					component.enabled = false;
				}
			}
			TooltipManager.Instance.Hide(this);
			NGUITools.SetActiveSelf(m_hoverTexture.gameObject, false);
		}

		public virtual void SetItem(BaseItem p_item)
		{
			m_item = p_item;
			if (m_itemTexture != null)
			{
				if (p_item != null)
				{
					if (m_item.Icon != null && m_item.Icon != String.Empty)
					{
						m_itemTexture.spriteName = m_item.Icon;
					}
					else
					{
						m_itemTexture.spriteName = "ITM_missing";
					}
					UpdateSlotRepresentation(true);
					UpdateItemBackground(m_itemBackground, p_item);
					UpdateItemBrokenState();
					if (gameObject.activeSelf)
					{
						NGUITools.SetActiveSelf(m_itemTexture.gameObject, true);
					}
				}
				else
				{
					UpdateSlotRepresentation(false);
					if (gameObject.activeSelf)
					{
						NGUITools.SetActiveSelf(m_brokenIcon.gameObject, false);
						NGUITools.SetActiveSelf(m_itemTexture.gameObject, false);
						NGUITools.SetActiveSelf(m_itemBackground.gameObject, false);
					}
				}
			}
		}

		public static void UpdateItemBackground(UISprite p_itemBackground, BaseItem p_item)
		{
			if (p_item is Equipment)
			{
				Equipment equipment = (Equipment)p_item;
				NGUITools.SetActiveSelf(p_itemBackground.gameObject, true);
				if (!equipment.Identified)
				{
					p_itemBackground.color = ITEM_BACKGROUND_COLOR_UNIDENTIFIED;
				}
				else if (equipment.IsRelic())
				{
					p_itemBackground.color = ITEM_BACKGROUND_COLOR_RELIC;
				}
				else if (equipment.Prefixes.Count > 0 && equipment.Suffixes.Count > 0)
				{
					p_itemBackground.color = ITEM_BACKGROUND_COLOR_DOUBLE_ENCHANTMENT;
				}
				else if (equipment.Prefixes.Count > 0 || equipment.Suffixes.Count > 0)
				{
					p_itemBackground.color = ITEM_BACKGROUND_COLOR_SINGLE_ENCHANTMENT;
				}
				else
				{
					p_itemBackground.color = ITEM_BACKGROUND_COLOR_NORMAL;
				}
			}
			else
			{
				NGUITools.SetActiveSelf(p_itemBackground.gameObject, false);
			}
		}

		public void UpdateItemBrokenState()
		{
			if (m_item is Equipment)
			{
				Equipment equipment = (Equipment)m_item;
				NGUITools.SetActiveSelf(m_brokenIcon.gameObject, equipment.Broken);
			}
			else
			{
				NGUITools.SetActiveSelf(m_brokenIcon.gameObject, false);
			}
		}

		protected virtual void UpdateSlotRepresentation(Boolean p_isActive)
		{
		}

		private void OnDrop(GameObject m_drag)
		{
			if (UICamera.currentTouchID == -1)
			{
				m_parent.DropItem(this);
			}
		}

		public virtual void OnTooltip(Boolean show)
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

		public virtual void OnDragHover(DragHoverEventArgs p_eventArgs)
		{
			if (DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				BaseItem item = ((ItemDragObject)DragDropManager.Instance.DraggedItem).Item;
				if (item != null)
				{
					NGUITools.SetActive(m_hoverTexture.gameObject, p_eventArgs.IsHovered);
					if (p_eventArgs.IsHovered)
					{
						DragDropManager.Instance.SetHoveredSlot(this);
					}
					else
					{
						DragDropManager.Instance.SetHoveredSlot(null);
					}
				}
			}
		}

		private void OnHover(Boolean p_isHovered)
		{
			NGUITools.SetActiveSelf(m_hoverTexture.gameObject, p_isHovered);
		}

		private void OnDrag(Vector2 p_delta)
		{
			if (m_moving)
			{
				DragDropManager.Instance.StartDrag(new ItemDragObject(this));
				m_moving = false;
			}
		}

		protected virtual void OnPress(Boolean p_isDown)
		{
			if (UICamera.currentTouchID == -1)
			{
				if (p_isDown)
				{
					if (m_item != null)
					{
						m_moving = true;
					}
				}
				else if (DragDropManager.Instance.DraggedItem is ItemDragObject && ((ItemDragObject)DragDropManager.Instance.DraggedItem).ItemSlot == this)
				{
					DragDropManager.Instance.StopDrag();
					m_moving = false;
				}
			}
			else if (UICamera.currentTouchID == -2 && !p_isDown)
			{
				m_parent.ItemRightClick(this);
			}
		}
	}
}
