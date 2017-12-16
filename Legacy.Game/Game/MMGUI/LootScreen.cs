using System;
using System.Collections.Generic;
using Legacy.Core;
using Legacy.Core.Api;
using Legacy.Core.Entities.Items;
using Legacy.Core.Hints;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Game.IngameManagement;
using Legacy.MMInput;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/LootScreen")]
	public class LootScreen : BaseScreen, IScrollingListener
	{
		[SerializeField]
		private LootItemContainer m_itemContainer;

		[SerializeField]
		private UIScrollBar m_scrollBar;

		[SerializeField]
		private Single m_scrollDuration = 0.1f;

		[SerializeField]
		private Single m_delayedCloseTime = 0.2f;

		private Single m_delayedClose;

		private Vector3 m_itemContainerOrigin;

		public LootItemContainer ItemContainer
		{
			get => m_itemContainer;
		    set => m_itemContainer = value;
		}

		public void Init()
		{
			m_itemContainer.Init(this);
			m_itemContainerOrigin = m_itemContainer.transform.localPosition;
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			InputManager.RegisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(ConfirmKeyPressed));
			InputManager.RegisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CloseKeyPressed));
			ScrollingHelper.InitScrollListeners(this, m_scrollBar.gameObject);
		}

		private void OnDestroy()
		{
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.INTERACT, new EventHandler<HotkeyEventArgs>(ConfirmKeyPressed));
			InputManager.UnregisterHotkeyEvent(EHotkeyType.OPEN_CLOSE_MENU, new EventHandler<HotkeyEventArgs>(CloseKeyPressed));
		}

		protected override void OpenDefaultTab()
		{
			SetScrollBarPosition(0);
			ScrollingHelper.InitScrollListeners(this, m_itemContainer.gameObject);
		}

		protected override void OpenTab(Int32 p_tabIndex)
		{
		}

		public void Update()
		{
			if (m_delayedClose > 0f)
			{
				m_delayedClose -= Time.deltaTime;
				if (m_delayedClose <= 0f && IngameController.Instance.CurrentIngameContext == IngameController.Instance.LootContainerView)
				{
					m_bilaterialScreen.CloseScreen();
				}
			}
		}

		public override void Close()
		{
			m_delayedClose = 0f;
			base.Close();
		}

		private void SetScrollBarPosition(Int32 p_position)
		{
			if (m_itemContainer.NeedsScrollBar())
			{
				NGUITools.SetActive(m_scrollBar.gameObject, true);
				Int32 num = 8;
				Int32 currentItemCount = m_itemContainer.Container.Content.GetCurrentItemCount();
				Int32 num2 = currentItemCount - num;
				if (p_position < 0)
				{
					p_position = 0;
				}
				else if (p_position > num2)
				{
					p_position = num2;
				}
				m_scrollBar.barSize = num / (Single)currentItemCount;
				if (num2 > 0)
				{
					m_scrollBar.scrollValue = p_position / (Single)num2;
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

		public void OnLootButtonClicked()
		{
			LootItem(LegacyLogic.Instance.WorldManager.Party.ActiveInventory, -1);
		}

		public void OnLootAllButtonClicked()
		{
			if (m_itemContainer.Container.ContainsEquipment())
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.EQUIPMENT);
			}
			m_itemContainer.Container.LootAll();
			m_itemContainer.SelectItemSlot(null);
			m_itemContainer.UpdateItems();
			UpdateScrollBar();
			if (m_itemContainer.Container.Content.GetCurrentItemCount() == 0)
			{
				m_delayedClose = m_delayedCloseTime;
			}
		}

		public void ConfirmKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (IsOpen && p_args.KeyDown && m_itemContainer.LootAllButtonAvailable)
			{
				OnLootAllButtonClicked();
			}
		}

		public void CloseKeyPressed(Object p_sender, HotkeyEventArgs p_args)
		{
			if (IsOpen && p_args.KeyDown)
			{
				m_bilaterialScreen.CloseScreen();
			}
		}

		public void LootItem(IInventory p_targetInvenotry, Int32 p_targetSlot)
		{
			BaseItem baseItem = m_itemContainer.SelectedItem.Item;
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (!p_targetInvenotry.CanAddItem(baseItem))
			{
				p_targetInvenotry = party.GetOtherInventory(p_targetInvenotry);
			}
			if (!p_targetInvenotry.CanAddItem(baseItem))
			{
				LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.INVENTORY_FULL);
				return;
			}
			if (baseItem is Equipment)
			{
				LegacyLogic.Instance.WorldManager.HintManager.TriggerHint(EHintType.EQUIPMENT);
			}
			m_itemContainer.Container.Content.RemoveItemAt(m_itemContainer.SelectedItem.OriginSlotIndex);
			NpcEffect npcEffect;
			if (baseItem is GoldStack && party.HirelingHandler.HasEffect(ETargetCondition.HIRE_BONUSGF, out npcEffect) && npcEffect.TargetEffect != ETargetCondition.NONE)
			{
				Int32 p_amount = (Int32)Math.Round(((GoldStack)baseItem).Amount * (1f + npcEffect.EffectValue), MidpointRounding.AwayFromZero);
				baseItem = new GoldStack(p_amount);
			}
			if (p_targetSlot == -1)
			{
				p_targetInvenotry.AddItem(baseItem);
				if (baseItem is Equipment && !((Equipment)baseItem).Identified && !((Equipment)baseItem).IsRelic())
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.UNIDENTIFIED_ITEM);
				}
				if (baseItem is Equipment && ((Equipment)baseItem).IsRelic())
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.RELIC);
				}
			}
			else
			{
				p_targetInvenotry.AddItem(baseItem, p_targetSlot);
				if (baseItem is Equipment && !((Equipment)baseItem).Identified && !((Equipment)baseItem).IsRelic())
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.UNIDENTIFIED_ITEM);
				}
				if (baseItem is Equipment && ((Equipment)baseItem).IsRelic())
				{
					LegacyLogic.Instance.CharacterBarkHandler.RandomPartyMemberBark(EBarks.RELIC);
				}
			}
			m_itemContainer.SelectItemSlot(null);
			m_itemContainer.UpdateItems();
			UpdateScrollBar();
			if (m_itemContainer.Container.Content.GetCurrentItemCount() == 0)
			{
				m_delayedClose = m_delayedCloseTime;
			}
			List<BaseItem> list = new List<BaseItem>();
			Int32 num = 0;
			if (baseItem is GoldStack)
			{
				num += ((GoldStack)baseItem).Amount;
			}
			else
			{
				list.Add(baseItem);
			}
			m_itemContainer.Container.NotifyHUDLoot(list, num);
			m_itemContainer.Container.DestroyContainerCheck();
		}

		private void UpdateScrollBar()
		{
			Single num = 8f;
			Single num2 = m_itemContainer.Container.Content.GetCurrentItemCount();
			Single num3 = num2 - num + 1f;
			SetScrollBarPosition((Int32)(num3 * m_scrollBar.scrollValue));
		}

		private void OnScrollBarChange(UIScrollBar p_sb)
		{
			Single num = 8f;
			Single num2 = m_itemContainer.Container.Content.GetCurrentItemCount();
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
			Single num2 = m_itemContainer.Container.Content.GetCurrentItemCount();
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
