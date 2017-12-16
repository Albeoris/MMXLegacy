using System;
using System.Collections.Generic;
using Legacy.Audio;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.NpcInteraction.Functions;
using Legacy.Core.PartyManagement;
using Legacy.Game.MMGUI;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/IdentifyScreen")]
	public class IdentifyScreen : MonoBehaviour, IScrollingListener
	{
		[SerializeField]
		private IdentifyItemContainer m_itemContainer;

		[SerializeField]
		private UIScrollBar m_scrollBar;

		[SerializeField]
		private Single m_scrollDuration = 0.1f;

		[SerializeField]
		private UIButton m_allItemsButton;

		[SerializeField]
		private UILabel m_allItemsButtonLabel;

		[SerializeField]
		private UILabel m_singleItemButtonLabel;

		[SerializeField]
		private UILabel m_headlineTextLabel;

		[SerializeField]
		private UISprite m_headLineIcon;

		[SerializeField]
		private String m_identifyIconName;

		[SerializeField]
		private String m_repairIconName;

		protected List<ItemSlotIdentify> m_itemSlots;

		protected IdentifyInventoryController m_identifyInventory;

		protected RepairInventoryController m_repairInventory;

		protected ItemSlotIdentify m_selectedItem;

		private Vector3 m_itemContainerOrigin;

		private Boolean m_didDoAnyAction;

		private Boolean m_fromSpell;

		private Int32 m_costs;

		private Boolean m_active;

		private Single m_listHeight;

		public Boolean IsActive => m_active;

	    public void Init()
		{
			m_itemContainer.Init(this);
			m_itemContainerOrigin = m_itemContainer.transform.localPosition;
			m_listHeight = m_itemContainer.Grid.cellHeight * 7f;
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			ScrollingHelper.InitScrollListeners(this, m_scrollBar.gameObject);
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_IDENTIFY_START, new EventHandler(OnStartIdentify));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_IDENTIFY_STOP, new EventHandler(OnEndIdentify));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_REPAIR_START, new EventHandler(OnStartRepair));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_REPAIR_STOP, new EventHandler(OnEndRepair));
		}

		public void CleanUp()
		{
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			m_itemContainer.CleanUp();
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_IDENTIFY_START, new EventHandler(OnStartIdentify));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_IDENTIFY_STOP, new EventHandler(OnEndIdentify));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_REPAIR_START, new EventHandler(OnStartRepair));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_REPAIR_STOP, new EventHandler(OnEndRepair));
		}

		private void HotkeyNullifier(Object p_sender, EventArgs p_args)
		{
		}

		private void OnStartIdentify(Object p_sender, EventArgs p_args)
		{
			m_repairInventory = null;
			m_identifyInventory = (IdentifyInventoryController)p_sender;
			if (m_identifyInventory == null)
			{
				Debug.LogError("Could not find the inventory controller necessary for displaying items!");
				return;
			}
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnAllItemsButtonClickedDelegate));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_1, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_2, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_3, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_4, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.RegisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_5, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			if (m_identifyInventory.FromScroll || m_identifyInventory.FromSpell)
			{
				InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CloseDelegate));
			}
			m_itemContainer.SetIdentifyInventory(m_identifyInventory);
			m_fromSpell = m_identifyInventory.FromSpell;
			m_costs = m_identifyInventory.IdentifyPrice;
			m_allItemsButtonLabel.text = LocaManager.GetText("IDENTIFY_SCREEN_BUTTON_IDENTIFY_ALL");
			m_singleItemButtonLabel.text = LocaManager.GetText("IDENTIFY_SCREEN_BUTTON_IDENTIFY");
			m_headlineTextLabel.text = LocaManager.GetText("IDENTIFY_SCREEN_HEADLINE");
			m_headLineIcon.spriteName = m_identifyIconName;
			NGUITools.SetActiveSelf(gameObject, true);
			m_active = true;
			SetScrollBarPosition(0);
			UpdateButtons();
			ScrollingHelper.InitScrollListeners(this, m_itemContainer.gameObject);
		}

		private void OnEndIdentify(Object p_sender, EventArgs p_args)
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnAllItemsButtonClickedDelegate));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CloseDelegate));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_1, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_2, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_3, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_4, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.QUICK_ACTION_SLOT_5, new EventHandler<HotkeyEventArgs>(HotkeyNullifier));
			m_active = false;
			NGUITools.SetActiveSelf(gameObject, false);
		}

		private void OnStartRepair(Object p_sender, EventArgs p_args)
		{
			m_identifyInventory = null;
			m_repairInventory = (RepairInventoryController)p_sender;
			if (m_repairInventory == null)
			{
				Debug.LogError("Could not find the inventory controller necessary for displaying items!");
				return;
			}
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnAllItemsButtonClickedDelegate));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CloseDelegate));
			m_itemContainer.SetRepairInventory(m_repairInventory);
			m_costs = m_repairInventory.RepairPrice;
			m_allItemsButtonLabel.text = LocaManager.GetText("REPAIR_SCREEN_BUTTON_REPAIR_ALL");
			m_singleItemButtonLabel.text = LocaManager.GetText("REPAIR_SCREEN_BUTTON_REPAIR");
			m_headlineTextLabel.text = LocaManager.GetText("REPAIR_SCREEN_HEADLINE");
			m_headLineIcon.spriteName = m_repairIconName;
			NGUITools.SetActiveSelf(gameObject, true);
			m_active = true;
			SetScrollBarPosition(0);
			UpdateButtons();
			ScrollingHelper.InitScrollListeners(this, m_itemContainer.gameObject);
		}

		private void OnEndRepair(Object p_sender, EventArgs p_args)
		{
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(OnAllItemsButtonClickedDelegate));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CloseDelegate));
			m_active = false;
			NGUITools.SetActiveSelf(gameObject, false);
		}

		private void CloseDelegate(Object p_sender, EventArgs p_args)
		{
			Close(null);
		}

		public void Close(GameObject p_sender)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (m_identifyInventory != null)
			{
				if (party.Inventory.Inventory.GetUnidentifiedItems().Count == 0 && party.MuleInventory.Inventory.GetUnidentifiedItems().Count == 0 && m_didDoAnyAction)
				{
					LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, new GameMessageEventArgs("DIALOG_TEXT_ALL_ITEMS_IDENTIFIED", 0f));
				}
				m_didDoAnyAction = false;
				m_identifyInventory.StopIdentify();
			}
			else
			{
				if (party.Inventory.Inventory.GetBrokenItems().Count == 0 && party.MuleInventory.Inventory.GetBrokenItems().Count == 0 && m_didDoAnyAction)
				{
					String text = String.Empty;
					switch (m_repairInventory.RepairType)
					{
					case ERepairType.ALL:
						text = "DIALOG_TEXT_ALL_ITEMS_REPAIRED";
						break;
					case ERepairType.WEAPONS:
						text = "GAME_MESSAGE_WEAPONS_REPAIRED";
						break;
					case ERepairType.ARMOR_AND_SHIELD:
						text = "GAME_MESSAGE_ARMOR_REPAIRED";
						break;
					}
					if (!String.IsNullOrEmpty(text))
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(null, EEventType.GAME_MESSAGE, new GameMessageEventArgs(text, 0f));
					}
				}
				m_didDoAnyAction = false;
				m_repairInventory.StopRepair();
			}
		}

		private void OnAllItemsButtonClickedDelegate(Object p_sender, EventArgs p_args)
		{
			OnAllItemsButtonClicked(null);
		}

		public void OnSingleItemButtonClicked(GameObject p_sender)
		{
			if (m_itemContainer.SelectedItem != null)
			{
				((ItemSlotIdentify)m_itemContainer.SelectedItem).PlayFX(m_itemContainer.Mode);
				BaseItem item = m_itemContainer.SelectedItem.Item;
				PartyInventoryController partyInventoryController = LegacyLogic.Instance.WorldManager.Party.Inventory;
				Int32 slotIndexForItem = partyInventoryController.GetSlotIndexForItem(item);
				if (slotIndexForItem == -1)
				{
					partyInventoryController = (PartyInventoryController)LegacyLogic.Instance.WorldManager.Party.GetOtherInventory(partyInventoryController);
					slotIndexForItem = partyInventoryController.GetSlotIndexForItem(item);
				}
				Equipment equipment = (Equipment)item;
				if (m_identifyInventory != null)
				{
					if (equipment.Identified)
					{
						return;
					}
					equipment.Identified = true;
					LegacyLogic.Instance.EventManager.InvokeEvent(partyInventoryController, EEventType.INVENTORY_ITEM_CHANGED, new InventoryItemEventArgs(new InventorySlotRef(partyInventoryController.Inventory, slotIndexForItem)));
					if (m_identifyInventory.FromScroll)
					{
						m_identifyInventory.RemoveOneScroll();
					}
					else if (!m_fromSpell)
					{
						LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_costs);
					}
					else if (m_costs > 0)
					{
						LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.ChangeMP(-m_costs);
					}
					AudioManager.Instance.RequestPlayAudioID("IdentifyGUI", 0);
				}
				else
				{
					if (!equipment.Broken)
					{
						return;
					}
					equipment.Broken = false;
					InventoryItemEventArgs p_eventArgs = null;
					switch (m_itemContainer.RepairInventory.RepairType)
					{
					case ERepairType.ALL:
						p_eventArgs = new InventoryItemEventArgs(InventoryItemEventArgs.ERepairType.ALL);
						break;
					case ERepairType.WEAPONS:
						p_eventArgs = new InventoryItemEventArgs(InventoryItemEventArgs.ERepairType.WEAPONS);
						break;
					case ERepairType.ARMOR_AND_SHIELD:
						p_eventArgs = new InventoryItemEventArgs(InventoryItemEventArgs.ERepairType.ARMOR);
						break;
					}
					if (slotIndexForItem == -1)
					{
						CharacterInventoryController characterInventoryController = null;
						foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
						{
							slotIndexForItem = character.Equipment.GetSlotIndexForItem(item);
							if (slotIndexForItem > -1)
							{
								characterInventoryController = character.Equipment;
								break;
							}
						}
						if (characterInventoryController != null && slotIndexForItem > -1)
						{
							LegacyLogic.Instance.EventManager.InvokeEvent(characterInventoryController.Equipment, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, p_eventArgs);
						}
					}
					else
					{
						LegacyLogic.Instance.EventManager.InvokeEvent(partyInventoryController, EEventType.INVENTORY_ITEM_REPAIR_STATUS_CHANGED, p_eventArgs);
					}
					LegacyLogic.Instance.WorldManager.Party.ChangeGold(-m_costs);
				}
				m_didDoAnyAction = true;
				m_itemContainer.UpdateItems();
				m_itemContainer.SelectItemSlot(null);
				UpdateScrollBar();
				UpdateButtons();
			}
		}

		public void OnAllItemsButtonClicked(GameObject p_sender)
		{
			m_itemContainer.PlayAllFX(m_listHeight);
			if (m_identifyInventory != null)
			{
				if (m_itemContainer.IdentifyInventory.GetCurrentItemCount() == 0 || !m_allItemsButton.isEnabled)
				{
					return;
				}
				Int32 num = 0;
				for (Int32 i = 0; i < m_itemContainer.IdentifyInventory.GetMaximumItemCount(); i++)
				{
					Equipment equipment = m_itemContainer.IdentifyInventory.GetItemAt(i) as Equipment;
					if (equipment != null && !equipment.Identified)
					{
						num++;
					}
				}
				m_didDoAnyAction = true;
				Int32 num2 = num * m_costs;
				if (m_identifyInventory.FromScroll)
				{
					for (Int32 j = 0; j < num; j++)
					{
						m_identifyInventory.RemoveOneScroll();
					}
				}
				else if (!m_fromSpell)
				{
					LegacyLogic.Instance.WorldManager.Party.ChangeGold(-num2);
				}
				else if (m_costs > 0)
				{
					LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.ChangeMP(-num2);
				}
				LegacyLogic.Instance.WorldManager.Party.Inventory.IdentifyAllItems();
				LegacyLogic.Instance.WorldManager.Party.MuleInventory.IdentifyAllItems();
				AudioManager.Instance.RequestPlayAudioID("IdentifyGUI", 0);
			}
			else
			{
				if (m_itemContainer.RepairInventory.GetCurrentItemCount() == 0 || !m_allItemsButton.isEnabled)
				{
					return;
				}
				Int32 num3 = 0;
				for (Int32 k = 0; k < m_itemContainer.RepairInventory.GetMaximumItemCount(); k++)
				{
					Equipment equipment2 = m_itemContainer.RepairInventory.GetItemAt(k) as Equipment;
					if (equipment2 != null && equipment2.Broken)
					{
						num3++;
					}
				}
				m_didDoAnyAction = true;
				Int32 num4 = num3 * m_costs;
				LegacyLogic.Instance.WorldManager.Party.ChangeGold(-num4);
				switch (m_itemContainer.RepairInventory.RepairType)
				{
				case ERepairType.ALL:
					LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.RepairAllItems();
					LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.RepairAllItems();
					foreach (Character character in LegacyLogic.Instance.WorldManager.Party.Members)
					{
						if (character != null && character.Equipment.Equipment.HasBrokenItems())
						{
							character.Equipment.Equipment.RepairAllItems();
						}
					}
					break;
				case ERepairType.WEAPONS:
					LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.RepairWeapons();
					LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.RepairWeapons();
					foreach (Character character2 in LegacyLogic.Instance.WorldManager.Party.Members)
					{
						if (character2 != null && character2.Equipment.Equipment.HasBrokenWeapons())
						{
							character2.Equipment.Equipment.RepairWeapons();
						}
					}
					break;
				case ERepairType.ARMOR_AND_SHIELD:
					LegacyLogic.Instance.WorldManager.Party.Inventory.Inventory.RepairArmorAndShields();
					LegacyLogic.Instance.WorldManager.Party.MuleInventory.Inventory.RepairArmorAndShields();
					foreach (Character character3 in LegacyLogic.Instance.WorldManager.Party.Members)
					{
						if (character3 != null && character3.Equipment.Equipment.HasBrokenArmors())
						{
							character3.Equipment.Equipment.RepairArmorAndShields();
						}
					}
					break;
				}
			}
			m_itemContainer.UpdateItems();
			m_itemContainer.SelectItemSlot(null);
			UpdateScrollBar();
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			Int32 num = 0;
			Boolean flag;
			if (m_identifyInventory != null)
			{
				for (Int32 i = 0; i < m_itemContainer.IdentifyInventory.GetMaximumItemCount(); i++)
				{
					Equipment equipment = m_itemContainer.IdentifyInventory.GetItemAt(i) as Equipment;
					if (equipment != null && !equipment.Identified)
					{
						num++;
					}
				}
				if (m_identifyInventory.FromSpell)
				{
					Int32 num2 = num * m_costs;
					flag = (LegacyLogic.Instance.WorldManager.Party.SelectedCharacter.ManaPoints >= num2);
				}
				else if (m_identifyInventory.FromScroll)
				{
					Boolean flag2 = m_itemContainer.IdentifyInventory.HasScrollResourcesForAmount(num);
					flag = flag2;
				}
				else
				{
					Int32 num2 = num * m_costs;
					flag = (LegacyLogic.Instance.WorldManager.Party.Gold >= num2);
				}
			}
			else
			{
				for (Int32 j = 0; j < m_itemContainer.RepairInventory.GetMaximumItemCount(); j++)
				{
					Equipment equipment2 = m_itemContainer.RepairInventory.GetItemAt(j) as Equipment;
					if (equipment2 != null && equipment2.Broken)
					{
						num++;
					}
				}
				Int32 num2 = num * m_costs;
				flag = (LegacyLogic.Instance.WorldManager.Party.Gold >= num2);
			}
			flag &= (num > 0);
			m_allItemsButton.isEnabled = flag;
			m_allItemsButtonLabel.color = ((!flag) ? Color.gray : Color.white);
		}

		private void SetScrollBarPosition(Int32 p_position)
		{
			if (m_itemContainer.NeedsScrollBar())
			{
				NGUITools.SetActive(m_scrollBar.gameObject, true);
				Int32 num = 8;
				Int32 num2 = (m_itemContainer.Mode != IdentifyItemContainer.EMode.IDENTIFY) ? m_itemContainer.RepairInventory.GetCurrentItemCount() : m_itemContainer.IdentifyInventory.GetCurrentItemCount();
				Int32 num3 = num2 - num;
				if (p_position < 0)
				{
					p_position = 0;
				}
				else if (p_position > num3)
				{
					p_position = num3;
				}
				m_scrollBar.barSize = num / (Single)num2;
				if (num3 > 0)
				{
					m_scrollBar.scrollValue = p_position / (Single)num3;
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

		private void UpdateScrollBar()
		{
			Single num = 8f;
			Single num2 = (m_itemContainer.Mode != IdentifyItemContainer.EMode.IDENTIFY) ? m_itemContainer.RepairInventory.GetCurrentItemCount() : m_itemContainer.IdentifyInventory.GetCurrentItemCount();
			Single num3 = num2 - num + 1f;
			SetScrollBarPosition((Int32)(num3 * m_scrollBar.scrollValue));
		}

		private void OnScrollBarChange(UIScrollBar p_sb)
		{
			Single num = 8f;
			Single num2 = (m_itemContainer.Mode != IdentifyItemContainer.EMode.IDENTIFY) ? m_itemContainer.RepairInventory.GetCurrentItemCount() : m_itemContainer.IdentifyInventory.GetCurrentItemCount();
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
			Single num2 = (m_itemContainer.Mode != IdentifyItemContainer.EMode.IDENTIFY) ? m_itemContainer.RepairInventory.GetCurrentItemCount() : m_itemContainer.IdentifyInventory.GetCurrentItemCount();
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
