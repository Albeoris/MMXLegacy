using System;
using Legacy.Core.Api;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Game.IngameManagement;
using Legacy.Game.MMGUI;
using UnityEngine;

namespace Legacy.Game
{
	[AddComponentMenu("MM Legacy/MMGUI/PartyScreen")]
	public class PartyScreen : BaseScreen
	{
		[SerializeField]
		private PartyInventoryItemContainer m_inventory;

		[SerializeField]
		private PartyInventoryItemContainer m_muleInventory;

		[SerializeField]
		private RessourceView m_ressourceView;

		[SerializeField]
		private AwardContainer m_awards;

		[SerializeField]
		private TradingCompareContainer m_compareContainer;

		[SerializeField]
		private InventoryTab m_inventoryTab;

		[SerializeField]
		private InventoryTab m_muleInventoryTab;

		private Boolean m_needCompareScreen;

		public Boolean NeedCompareScreen
		{
			get => m_needCompareScreen;
		    set
			{
				NGUITools.SetActive(m_awards.gameObject, !value);
				NGUITools.SetActive(m_compareContainer.gameObject, value);
				m_needCompareScreen = value;
			}
		}

		public void Init(Party p_party)
		{
			if (m_inventory != null)
			{
				m_inventory.Init(p_party.Inventory);
			}
			if (m_muleInventory != null)
			{
				m_muleInventory.Init(p_party.MuleInventory);
			}
			if (m_awards != null)
			{
				m_awards.Init(p_party);
			}
			if (m_compareContainer != null)
			{
				m_compareContainer.Init(p_party);
			}
			m_inventoryTab.Init(m_inventory, this, 0);
			m_muleInventoryTab.Init(m_muleInventory, this, 1);
		}

		public void CleanUp()
		{
			if (m_inventory != null)
			{
				m_inventory.CleanUp();
				m_muleInventory.CleanUp();
			}
			if (m_awards != null)
			{
				m_awards.CleanUp();
			}
			if (m_compareContainer != null)
			{
				m_compareContainer.CleanUp();
			}
		}

		public override void ToggleOpenClose()
		{
			ConversationManager conversationManager = LegacyLogic.Instance.ConversationManager;
			if (conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.TradingInventory.IsTrading)
			{
				NeedCompareScreen = true;
			}
			else
			{
				NeedCompareScreen = false;
			}
			base.ToggleOpenClose();
		}

		protected override void SetElementsActiveState(Boolean p_enabled)
		{
			base.SetElementsActiveState(p_enabled);
			if (p_enabled)
			{
				Boolean state = LegacyLogic.Instance.WorldManager.Party.HirelingHandler.HasEffect(ETargetCondition.HIRE_MULE);
				NGUITools.SetActiveSelf(m_tabController.Tabs[1].gameObject, state);
			}
			m_compareContainer.UpdateSlots();
		}

		public void OpenInventoryTab()
		{
			m_tabController.OnTabClicked(m_tabController.Tabs[0].TabID, false);
			m_ressourceView.OnResourceChanged(LegacyLogic.Instance.WorldManager.Party, EventArgs.Empty);
		}

		public void ToggleInventory()
		{
			ConversationManager conversationManager = LegacyLogic.Instance.ConversationManager;
			if (IsOpen && conversationManager.CurrentNpc != null && conversationManager.CurrentNpc.TradingInventory.IsTrading)
			{
				conversationManager.CurrentNpc.TradingInventory.StopTrading();
			}
			else
			{
				IngameController.Instance.ToggleInventory(this, EventArgs.Empty);
			}
			if (IsOpen)
			{
				m_compareContainer.UpdateSlots();
			}
		}

		internal void ChangeParty(Party p_party)
		{
			if (m_inventory != null)
			{
				m_inventory.CleanUp();
				m_inventory.Init(p_party.Inventory);
			}
			if (m_muleInventory != null)
			{
				m_muleInventory.CleanUp();
				m_muleInventory.Init(p_party.MuleInventory);
			}
			if (m_awards != null)
			{
				m_awards.CleanUp();
				m_awards.Init(p_party);
			}
			if (m_compareContainer != null)
			{
				m_compareContainer.CleanUp();
				m_compareContainer.Init(p_party);
			}
		}
	}
}
