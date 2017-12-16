using System;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Configuration;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.UpdateLogic;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/ActionButtonView")]
	public class ActionButtonView : MonoBehaviour
	{
		private const String BEST_HEALTH_POTION_DEFAULT = "ITM_consumable_potion_health_1";

		private const String BEST_MANA_POTION_DEFAULT = "ITM_consumable_potion_mana_1";

		private const Single COLOR_TWEEN_TIME = 0.2f;

		private EQuickActionType m_type;

		private CharacterSpell m_spell;

		private Consumable m_item;

		private Boolean m_disableHighlights = true;

		private Int32 m_index;

		private Boolean m_usable = true;

		private Boolean m_available = true;

		[SerializeField]
		private UISprite m_icon;

		[SerializeField]
		private UISprite m_iconItem;

		[SerializeField]
		private UISprite m_iconScroll;

		[SerializeField]
		private UILabel m_itemCounter;

		[SerializeField]
		private UILabel m_buttonNumber;

		[SerializeField]
		private UISprite m_hoverHighlight;

		[SerializeField]
		private UISprite m_clickedHighlight;

		[SerializeField]
		private Color m_notUsableColor = Color.white;

		[SerializeField]
		private Color m_notAvailableColor = Color.white;

		private Color m_iconColor;

		private static ActionButtonView s_lastDraggedItem;

		public EQuickActionType Type
		{
			get => m_type;
		    set
			{
				m_type = value;
				SelectDefaultIcons();
			}
		}

		public String ButtonHotkey
		{
			set => m_buttonNumber.text = value;
		}

		public CharacterSpell Spell
		{
			get => m_spell;
		    set
			{
				m_spell = value;
				if (value != null && m_type == EQuickActionType.CAST_SPELL)
				{
					Icon = m_spell.StaticData.Icon;
				}
			}
		}

		public String Icon
		{
			get => m_icon.spriteName;
		    set => m_icon.spriteName = value;
		}

		public Consumable Item
		{
			get => m_item;
		    set
			{
				m_item = value;
				UpdateItem(m_item);
			}
		}

		private void Awake()
		{
			m_iconColor = m_icon.color;
		}

		public void Init(Int32 p_index, EQuickActionType p_type, Consumable p_item, CharacterSpell p_spell)
		{
			m_index = p_index;
			Type = p_type;
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_CHANGED, new EventHandler(OnItemChanged));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_SOLD, new EventHandler(OnItemSold));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_DUMPED, new EventHandler(OnItemDumped));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnItemAdded));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnFoodChanged));
			switch (p_type)
			{
			case EQuickActionType.CAST_SPELL:
				Spell = p_spell;
				break;
			case EQuickActionType.USE_ITEM:
				m_item = p_item;
				if (m_item is Scroll)
				{
					m_iconItem.spriteName = "ITM_consumable_scroll";
					m_iconScroll.spriteName = m_item.Icon;
				}
				else if (m_item != null)
				{
					m_iconItem.spriteName = m_item.Icon;
				}
				else
				{
					Clear(false);
				}
				break;
			case EQuickActionType.USE_BEST_MANAPOTION:
				GetBestPotion(EPotionType.MANA_POTION);
				break;
			case EQuickActionType.USE_BEST_HEALTHPOTION:
				GetBestPotion(EPotionType.HEALTH_POTION);
				break;
			}
			UpdateVisibility();
			UpdateItemCounter();
		}

		public void CleanUp()
		{
			s_lastDraggedItem = null;
			m_spell = null;
			m_item = null;
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_CHANGED, new EventHandler(OnItemChanged));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_SOLD, new EventHandler(OnItemSold));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_DUMPED, new EventHandler(OnItemDumped));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.INVENTORY_ITEM_ADDED, new EventHandler(OnItemAdded));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_RESOURCES_CHANGED, new EventHandler(OnFoodChanged));
		}

		public void UpdateVisibility()
		{
			switch (Type)
			{
			case EQuickActionType.CAST_SPELL:
			case EQuickActionType.ATTACK:
			case EQuickActionType.ATTACKRANGED:
			case EQuickActionType.DEFEND:
				NGUITools.SetActive(m_icon.gameObject, true);
				NGUITools.SetActive(m_iconItem.gameObject, false);
				NGUITools.SetActive(m_iconScroll.gameObject, false);
				break;
			case EQuickActionType.USE_ITEM:
				NGUITools.SetActive(m_icon.gameObject, false);
				NGUITools.SetActive(m_iconItem.gameObject, true);
				NGUITools.SetActive(m_iconScroll.gameObject, m_item is Scroll);
				break;
			case EQuickActionType.USE_BEST_MANAPOTION:
				NGUITools.SetActive(m_icon.gameObject, true);
				NGUITools.SetActive(m_iconItem.gameObject, true);
				NGUITools.SetActive(m_iconScroll.gameObject, false);
				break;
			case EQuickActionType.USE_BEST_HEALTHPOTION:
				NGUITools.SetActive(m_icon.gameObject, true);
				NGUITools.SetActive(m_iconItem.gameObject, true);
				NGUITools.SetActive(m_iconScroll.gameObject, false);
				break;
			default:
				NGUITools.SetActive(m_icon.gameObject, false);
				NGUITools.SetActive(m_iconItem.gameObject, false);
				NGUITools.SetActive(m_iconScroll.gameObject, false);
				break;
			}
		}

		private void SelectDefaultIcons()
		{
			switch (m_type)
			{
			case EQuickActionType.ATTACK:
				Icon = "SPL_action_melee";
				break;
			case EQuickActionType.ATTACKRANGED:
				Icon = "SPL_action_ranged";
				break;
			case EQuickActionType.USE_BEST_MANAPOTION:
			case EQuickActionType.USE_BEST_HEALTHPOTION:
				Icon = "SPL_action_bestpotion";
				break;
			case EQuickActionType.DEFEND:
				Icon = "SPL_action_defend";
				break;
			}
		}

		private void OnDisable()
		{
			TooltipManager.Instance.Hide(this);
		}

		private void OnEnable()
		{
			m_disableHighlights = true;
		}

		private void Update()
		{
			if (m_disableHighlights && m_hoverHighlight.gameObject.activeSelf)
			{
				NGUITools.SetActive(m_hoverHighlight.gameObject, false);
				NGUITools.SetActive(m_clickedHighlight.gameObject, false);
				m_disableHighlights = false;
			}
		}

		private void OnFoodChanged(Object p_sender, EventArgs p_args)
		{
			UpdateItemCounter();
		}

		private void OnItemAdded(Object sender, EventArgs p_args)
		{
			InventoryItemEventArgs inventoryItemEventArgs = (InventoryItemEventArgs)p_args;
			BaseItem item = inventoryItemEventArgs.Slot.GetItem();
			if (item != null)
			{
				UpdateItem(item);
			}
		}

		private void UpdateItem(BaseItem p_item)
		{
			if (m_type == EQuickActionType.USE_ITEM)
			{
				if (Consumable.AreSameConsumables(p_item, m_item))
				{
					m_item.Counter = LegacyLogic.Instance.WorldManager.Party.GetTotalAmountOfConsumable(m_item);
				}
			}
			else if (m_type == EQuickActionType.USE_BEST_HEALTHPOTION)
			{
				Potion potion = p_item as Potion;
				if (potion != null && potion.PotionType == EPotionType.HEALTH_POTION)
				{
					GetBestPotion(EPotionType.HEALTH_POTION);
					if (LegacyLogic.Instance.WorldManager.Party.SelectedCharacter != null)
					{
						LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(m_type, m_item, ECharacterSpell.SPELL_FIRE_WARD);
					}
				}
			}
			else if (m_type == EQuickActionType.USE_BEST_MANAPOTION)
			{
				Potion potion2 = p_item as Potion;
				if (potion2 != null && potion2.PotionType == EPotionType.MANA_POTION)
				{
					GetBestPotion(EPotionType.MANA_POTION);
					if (LegacyLogic.Instance.WorldManager.Party.SelectedCharacter != null)
					{
						LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(m_type, m_item, ECharacterSpell.SPELL_FIRE_WARD);
					}
				}
			}
			if (m_item is Scroll)
			{
				m_iconItem.spriteName = "ITM_consumable_scroll";
				m_iconScroll.spriteName = m_item.Icon;
			}
			else if (m_item != null)
			{
				m_iconItem.spriteName = m_item.Icon;
			}
			UpdateItemCounter();
		}

		public void PressedHotkey()
		{
			OnClickButton(gameObject);
		}

		public void OnClickButton(GameObject p_button)
		{
			if (m_type == EQuickActionType.CAST_SPELL && !m_usable)
			{
				Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
				selectedCharacter.BarkHandler.TriggerBark(EBarks.LOW_MANA, selectedCharacter);
			}
			if (!m_available)
			{
				return;
			}
			switch (m_type)
			{
			case EQuickActionType.CAST_SPELL:
				CastSpell();
				break;
			case EQuickActionType.USE_ITEM:
			case EQuickActionType.USE_BEST_MANAPOTION:
			case EQuickActionType.USE_BEST_HEALTHPOTION:
				UseItem();
				break;
			case EQuickActionType.ATTACK:
				Attack();
				break;
			case EQuickActionType.ATTACKRANGED:
				AttackRanged();
				break;
			case EQuickActionType.DEFEND:
				Defend();
				break;
			}
		}

		public void Clear(Boolean p_clearAllStates)
		{
			NGUITools.SetActive(m_icon.gameObject, false);
			NGUITools.SetActive(m_iconItem.gameObject, false);
			NGUITools.SetActive(m_iconScroll.gameObject, false);
			Reset();
			if (p_clearAllStates || (m_type != EQuickActionType.USE_BEST_HEALTHPOTION && m_type != EQuickActionType.USE_BEST_MANAPOTION))
			{
				Type = EQuickActionType.NONE;
				LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(EQuickActionType.NONE, null, ECharacterSpell.SPELL_FIRE_WARD);
			}
			UpdateItemCounter();
		}

		public void UpdateItemCounter()
		{
			if (m_itemCounter == null)
			{
				return;
			}
			if (m_item == null)
			{
				NGUITools.SetActive(m_itemCounter.gameObject, false);
			}
			else
			{
				NGUITools.SetActive(m_icon.gameObject, true);
				NGUITools.SetActive(m_itemCounter.gameObject, true);
				if (m_item.Counter > 0)
				{
					m_itemCounter.text = m_item.Counter.ToString();
				}
				else
				{
					m_itemCounter.text = String.Empty;
				}
				UpdateVisibility();
			}
			CheckUsability();
		}

		public void CheckUsability()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			PartyTurnActor partyTurnActor = LegacyLogic.Instance.UpdateManager.PartyTurnActor;
			Character selectedCharacter = party.SelectedCharacter;
			Int32 currentCharacter = party.CurrentCharacter;
			switch (m_type)
			{
			case EQuickActionType.NONE:
				SetAvailable(false, false);
				break;
			case EQuickActionType.CAST_SPELL:
				if (m_spell != null)
				{
					SetAvailable(partyTurnActor.CanDoCommand(new CastSpellCommand(m_spell, null, null), currentCharacter), m_spell.HasResources(selectedCharacter));
				}
				else
				{
					SetAvailable(false, false);
				}
				break;
			case EQuickActionType.USE_ITEM:
			{
				InventorySlotRef consumableSlot = party.GetConsumableSlot(m_item);
				if (m_item != null)
				{
					SetAvailable(m_item.Counter > 0 && partyTurnActor.CanDoCommand(new ConsumeCommand(consumableSlot, currentCharacter), currentCharacter), m_item.Counter > 0);
				}
				else
				{
					SetAvailable(false, false);
				}
				break;
			}
			case EQuickActionType.ATTACK:
				if (selectedCharacter != null)
				{
					if (selectedCharacter.Equipment != null)
					{
						SetAvailable(partyTurnActor.CanDoCommand(MeleeAttackCommand.Instance, currentCharacter), selectedCharacter.Equipment.IsMeleeAttackWeaponEquiped());
					}
					else
					{
						SetAvailable(false, false);
					}
				}
				else
				{
					SetAvailable(false, false);
				}
				break;
			case EQuickActionType.ATTACKRANGED:
				if (selectedCharacter != null)
				{
					if (selectedCharacter.Equipment != null)
					{
						SetAvailable(partyTurnActor.CanDoCommand(RangeAttackCommand.Instance, currentCharacter), selectedCharacter.Equipment.IsRangedAttackWeaponEquiped());
					}
					else
					{
						SetAvailable(false, false);
					}
				}
				else
				{
					SetAvailable(false, false);
				}
				break;
			case EQuickActionType.USE_BEST_MANAPOTION:
			{
				InventorySlotRef consumableSlot2 = party.GetConsumableSlot(m_item);
				if (consumableSlot2.Slot != -1)
				{
					SetAvailable(partyTurnActor.CanDoCommand(new ConsumeCommand(consumableSlot2, currentCharacter), currentCharacter), m_item.Counter > 0);
				}
				else
				{
					SetAvailable(false, false);
				}
				break;
			}
			case EQuickActionType.USE_BEST_HEALTHPOTION:
			{
				InventorySlotRef consumableSlot3 = party.GetConsumableSlot(m_item);
				if (consumableSlot3.Slot != -1)
				{
					SetAvailable(partyTurnActor.CanDoCommand(new ConsumeCommand(consumableSlot3, currentCharacter), currentCharacter), m_item.Counter > 0);
				}
				else
				{
					SetAvailable(false, false);
				}
				break;
			}
			case EQuickActionType.DEFEND:
				SetAvailable(partyTurnActor.CanDoCommand(DefendCommand.Instance, currentCharacter), true);
				break;
			}
		}

		private void SetAvailable(Boolean p_available, Boolean p_usable)
		{
			Single duration = 0.2f;
			if (m_usable != p_usable)
			{
				duration = 0f;
				m_usable = p_usable;
			}
			m_available = p_available;
			if (!m_usable)
			{
				TweenColor.Begin(m_icon.gameObject, duration, m_notUsableColor);
				TweenColor.Begin(m_iconItem.gameObject, duration, m_notUsableColor);
				TweenColor.Begin(m_iconScroll.gameObject, duration, m_notUsableColor);
			}
			else if (p_available)
			{
				TweenColor.Begin(m_icon.gameObject, duration, m_iconColor);
				TweenColor.Begin(m_iconItem.gameObject, duration, m_iconColor);
				TweenColor.Begin(m_iconScroll.gameObject, duration, m_iconColor);
			}
			else
			{
				TweenColor.Begin(m_icon.gameObject, duration, m_notAvailableColor);
				TweenColor.Begin(m_iconItem.gameObject, duration, m_notAvailableColor);
				TweenColor.Begin(m_iconScroll.gameObject, duration, m_notAvailableColor);
			}
		}

		private void CastSpell()
		{
			if (m_spell != null)
			{
				if (m_spell.TargetType == ETargetType.SINGLE_PARTY_MEMBER)
				{
					OpenSpellCharacterSelectionEventArgs p_eventArgs = new OpenSpellCharacterSelectionEventArgs(m_spell);
					LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.CHARACTER_CAST_SPELL_WITH_CHARACTER_SELECTION, p_eventArgs);
					return;
				}
				if (m_spell.SpellType == ECharacterSpell.SPELL_PRIME_SPIRIT_BEACON)
				{
					SpiritBeaconEventArgs p_eventArgs2 = new SpiritBeaconEventArgs(null);
					LegacyLogic.Instance.EventManager.InvokeEvent(m_spell, EEventType.CHARACTER_CAST_SPIRIT_BEACON, p_eventArgs2);
					return;
				}
				if (m_spell.SpellType == ECharacterSpell.SPELL_PRIME_IDENTIFY)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(m_spell, EEventType.CHARACTER_CAST_IDENTIFY, EventArgs.Empty);
					return;
				}
			}
			LegacyLogic.Instance.CommandManager.AddCommand(new CastSpellCommand(m_spell, null, null));
		}

		private void UseItem()
		{
			if (m_item != null && m_item.Counter > 0)
			{
				InventorySlotRef consumableSlot = LegacyLogic.Instance.WorldManager.Party.GetConsumableSlot(m_item);
				Int32 currentCharacter = LegacyLogic.Instance.WorldManager.Party.CurrentCharacter;
				LegacyLogic.Instance.CommandManager.AddCommand(new ConsumeCommand(consumableSlot, currentCharacter));
			}
		}

		private void Attack()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(MeleeAttackCommand.Instance);
		}

		private void AttackRanged()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(RangeAttackCommand.Instance);
		}

		private void Defend()
		{
			LegacyLogic.Instance.CommandManager.AddCommand(DefendCommand.Instance);
		}

		private void OnItemChanged(Object p_sender, EventArgs p_e)
		{
			InventoryItemEventArgs inventoryItemEventArgs = (InventoryItemEventArgs)p_e;
			BaseItem item = inventoryItemEventArgs.Slot.GetItem();
			if (Consumable.AreSameConsumables(m_item, item))
			{
				UpdateItem(item);
			}
			UpdateVisibility();
		}

		private void OnItemSold(Object p_sender, EventArgs p_e)
		{
			InventoryItemEventArgs inventoryItemEventArgs = (InventoryItemEventArgs)p_e;
			BaseItem itemAt = inventoryItemEventArgs.Slot.Inventory.GetItemAt(inventoryItemEventArgs.Slot.Slot);
			if (Consumable.AreSameConsumables(m_item, itemAt))
			{
				UpdateItem(itemAt);
			}
			UpdateVisibility();
		}

		private void OnItemDumped(Object p_sender, EventArgs p_e)
		{
			BaseItem baseItem = p_sender as BaseItem;
			if (Consumable.AreSameConsumables(m_item, baseItem))
			{
				UpdateItem(baseItem);
			}
		}

		public void GetBestPotion(EPotionType type)
		{
			if (m_item != null)
			{
				m_item.Counter = LegacyLogic.Instance.WorldManager.Party.GetTotalAmountOfConsumable(m_item);
			}
			Potion bestPotion = LegacyLogic.Instance.WorldManager.Party.GetBestPotion(type, m_item as Potion);
			if (m_item != bestPotion && bestPotion != null)
			{
				m_item = bestPotion;
				m_iconItem.spriteName = m_item.Icon;
				UpdateItemCounter();
			}
			if (m_item == null || m_item.Counter == 0)
			{
				if (type == EPotionType.HEALTH_POTION)
				{
					m_iconItem.spriteName = "ITM_consumable_potion_health_1";
				}
				else
				{
					m_iconItem.spriteName = "ITM_consumable_potion_mana_1";
				}
			}
		}

		public void Reset()
		{
			m_type = EQuickActionType.NONE;
			m_spell = null;
			m_item = null;
			m_disableHighlights = true;
		}

		private void OnDrop(GameObject go)
		{
			if (s_lastDraggedItem != null)
			{
				AddActionToOldSlot();
				s_lastDraggedItem = null;
			}
			if (DragDropManager.Instance.DraggedItem is ItemDragObject)
			{
				ItemDragObject itemDragObject = (ItemDragObject)DragDropManager.Instance.DraggedItem;
				BaseItem item = itemDragObject.Item;
				if (item is Consumable)
				{
					NGUITools.SetActive(m_icon.gameObject, false);
					NGUITools.SetActive(m_iconItem.gameObject, true);
					NGUITools.SetActive(m_iconScroll.gameObject, true);
					m_type = EQuickActionType.USE_ITEM;
					if (itemDragObject.Item is Scroll)
					{
						m_item = ItemFactory.CreateItem<Scroll>(item.StaticId);
					}
					else
					{
						m_item = ItemFactory.CreateItem<Potion>(item.StaticId);
					}
					UpdateItem(item);
					LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(EQuickActionType.USE_ITEM, m_item, ECharacterSpell.SPELL_FIRE_WARD);
				}
			}
			else if (DragDropManager.Instance.DraggedItem is SpellDragObject)
			{
				NGUITools.SetActive(m_icon.gameObject, true);
				NGUITools.SetActive(m_iconItem.gameObject, false);
				NGUITools.SetActive(m_iconScroll.gameObject, false);
				m_item = null;
				SpellDragObject spellDragObject = (SpellDragObject)DragDropManager.Instance.DraggedItem;
				CharacterSpell spell = spellDragObject.Spell;
				Type = EQuickActionType.CAST_SPELL;
				m_spell = spell;
				UpdateItemCounter();
				Icon = m_spell.StaticData.Icon;
				LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(EQuickActionType.CAST_SPELL, null, (ECharacterSpell)m_spell.StaticID);
			}
			else if (DragDropManager.Instance.DraggedItem is BasicActionDragObject)
			{
				BasicActionDragObject basicActionDragObject = (BasicActionDragObject)DragDropManager.Instance.DraggedItem;
				EQuickActionType actionType = basicActionDragObject.View.ActionType;
				Boolean state = actionType == EQuickActionType.USE_BEST_HEALTHPOTION || actionType == EQuickActionType.USE_BEST_MANAPOTION;
				NGUITools.SetActive(m_icon.gameObject, true);
				NGUITools.SetActive(m_iconItem.gameObject, state);
				NGUITools.SetActive(m_iconScroll.gameObject, false);
				m_item = null;
				Type = actionType;
				Icon = basicActionDragObject.View.Icon.spriteName;
				if (actionType == EQuickActionType.USE_BEST_HEALTHPOTION)
				{
					GetBestPotion(EPotionType.HEALTH_POTION);
				}
				else if (actionType == EQuickActionType.USE_BEST_MANAPOTION)
				{
					GetBestPotion(EPotionType.MANA_POTION);
				}
				UpdateItemCounter();
				LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(actionType, null, ECharacterSpell.SPELL_FIRE_WARD);
			}
			else if (DragDropManager.Instance.DraggedItem is QuickActionDragObject)
			{
				Reset();
				QuickActionDragObject quickActionDragObject = (QuickActionDragObject)DragDropManager.Instance.DraggedItem;
				EQuickActionType type = quickActionDragObject.Type;
				Boolean flag = type == EQuickActionType.USE_BEST_HEALTHPOTION || type == EQuickActionType.USE_BEST_MANAPOTION || type == EQuickActionType.USE_ITEM;
				NGUITools.SetActive(m_icon.gameObject, type != EQuickActionType.USE_ITEM);
				NGUITools.SetActive(m_iconItem.gameObject, flag);
				NGUITools.SetActive(m_iconScroll.gameObject, quickActionDragObject.Item is Scroll);
				m_type = type;
				ECharacterSpell p_spell = ECharacterSpell.NONE;
				if (quickActionDragObject.Spell != null)
				{
					m_spell = quickActionDragObject.Spell;
					p_spell = (ECharacterSpell)m_spell.StaticID;
				}
				if (quickActionDragObject.Item != null)
				{
					Consumable item2 = quickActionDragObject.Item;
					m_item = item2;
				}
				UpdateItemCounter();
				Icon = quickActionDragObject.Spritename;
				if (flag)
				{
					if (quickActionDragObject.Item != null)
					{
						if (quickActionDragObject.Item is Scroll)
						{
							m_iconItem.spriteName = "ITM_consumable_scroll";
							m_iconScroll.spriteName = quickActionDragObject.Item.Icon;
						}
						else
						{
							m_iconItem.spriteName = quickActionDragObject.Item.Icon;
						}
					}
					else if (quickActionDragObject.Type == EQuickActionType.USE_BEST_HEALTHPOTION)
					{
						m_iconItem.spriteName = "ITM_consumable_potion_health_1";
					}
					else if (quickActionDragObject.Type == EQuickActionType.USE_BEST_MANAPOTION)
					{
						m_iconItem.spriteName = "ITM_consumable_potion_mana_1";
					}
				}
				if (type == EQuickActionType.CAST_SPELL)
				{
					LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(type, null, p_spell);
				}
				else if (type == EQuickActionType.USE_ITEM)
				{
					LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(type, m_item, p_spell);
				}
				else
				{
					LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[m_index] = new CharacterQuickActions.Action(type, null, p_spell);
				}
			}
			UpdateVisibility();
		}

		private void AddActionToOldSlot()
		{
			if (s_lastDraggedItem != this)
			{
				s_lastDraggedItem.Clear(true);
				ECharacterSpell p_spell = ECharacterSpell.NONE;
				if (m_spell != null)
				{
					p_spell = (ECharacterSpell)m_spell.StaticID;
				}
				LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.QuickActions[s_lastDraggedItem.m_index] = new CharacterQuickActions.Action(m_type, m_item, p_spell);
				s_lastDraggedItem.Init(s_lastDraggedItem.m_index, m_type, m_item, m_spell);
			}
		}

		private void OnDrag()
		{
			if (ConfigManager.Instance.Options.LockActionBar)
			{
				return;
			}
			if (DragDropManager.Instance.DraggedItem == null && m_type != EQuickActionType.NONE && UICamera.currentTouchID == -1)
			{
				DragDropManager.Instance.StartDrag(new QuickActionDragObject(this));
				s_lastDraggedItem = this;
				Clear(true);
			}
		}

		private void OnPress(Boolean p_isDown)
		{
			if (p_isDown)
			{
				NGUITools.SetActive(m_clickedHighlight.gameObject, true);
			}
			else
			{
				NGUITools.SetActive(m_clickedHighlight.gameObject, false);
			}
			if (UICamera.currentTouchID == -1 && !p_isDown && DragDropManager.Instance.DraggedItem is QuickActionDragObject)
			{
				QuickActionDragObject quickActionDragObject = (QuickActionDragObject)DragDropManager.Instance.DraggedItem;
				if (quickActionDragObject.View == this && UICamera.hoveredObject != gameObject)
				{
					Clear(true);
					DragDropManager.Instance.StopDrag();
				}
				else if (quickActionDragObject.View == this && UICamera.hoveredObject == gameObject)
				{
					OnDrop(UICamera.hoveredObject);
					DragDropManager.Instance.StopDrag();
				}
			}
		}

		private void OnTooltip(Boolean p_show)
		{
			if (p_show)
			{
				Vector3 position = gameObject.transform.position;
				Vector3 p_offset = m_icon.gameObject.transform.localScale * 0.5f;
				switch (m_type)
				{
				case EQuickActionType.CAST_SPELL:
					TooltipManager.Instance.Show(this, m_spell, position, p_offset);
					break;
				case EQuickActionType.USE_ITEM:
				case EQuickActionType.USE_BEST_MANAPOTION:
				case EQuickActionType.USE_BEST_HEALTHPOTION:
					if (m_item != null)
					{
						TooltipManager.Instance.Show(this, m_item, null, position, p_offset);
					}
					break;
				case EQuickActionType.ATTACK:
				case EQuickActionType.ATTACKRANGED:
				case EQuickActionType.DEFEND:
					TooltipManager.Instance.Show(this, m_type, position, p_offset);
					break;
				}
			}
			else
			{
				TooltipManager.Instance.Hide(this);
			}
		}

		private void OnHover(Boolean p_isHover)
		{
			if (p_isHover)
			{
				NGUITools.SetActive(m_hoverHighlight.gameObject, true);
				m_disableHighlights = false;
			}
			else
			{
				NGUITools.SetActive(m_hoverHighlight.gameObject, false);
				NGUITools.SetActive(m_clickedHighlight.gameObject, false);
				m_disableHighlights = true;
			}
		}
	}
}
