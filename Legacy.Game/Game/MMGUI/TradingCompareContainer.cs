using System;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/TradingCompareContainer")]
	public class TradingCompareContainer : MonoBehaviour
	{
		private Party m_party;

		[SerializeField]
		private TradingCompareCharacter[] m_chars;

		[SerializeField]
		private EquipmentItemContainer m_itemContainer;

		public void Init(Party p_party)
		{
			m_party = p_party;
			m_itemContainer.Init(p_party.SelectedCharacter.Equipment);
			for (Int32 i = 0; i < m_chars.Length; i++)
			{
				m_chars[i].Init(m_party.GetMemberByOrder(i));
				m_chars[i].OnPortraitClicked += OnCharacterClicked;
				m_chars[i].SetSelected(m_party.SelectedCharacter.Index == i);
			}
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.PARTY_ORDER_CHANGED, new EventHandler(OnPartyOrderChanged));
		}

		public void CleanUp()
		{
			foreach (TradingCompareCharacter tradingCompareCharacter in m_chars)
			{
				tradingCompareCharacter.OnPortraitClicked -= OnCharacterClicked;
			}
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.PARTY_ORDER_CHANGED, new EventHandler(OnPartyOrderChanged));
		}

		private void OnPartyOrderChanged(Object p_sender, EventArgs p_args)
		{
			for (Int32 i = 0; i < m_chars.Length; i++)
			{
				m_chars[i].Init(m_party.GetMemberByOrder(i));
				m_chars[i].SetSelected(m_party.SelectedCharacter.Index == i);
			}
		}

		public void UpdateSlots()
		{
			OnCharacterClicked(m_chars[m_party.CurrentCharacterIndexByOrder], EventArgs.Empty);
		}

		private void OnCharacterClicked(Object p_sender, EventArgs p_args)
		{
			TradingCompareCharacter tradingCompareCharacter = (TradingCompareCharacter)p_sender;
			foreach (TradingCompareCharacter tradingCompareCharacter2 in m_chars)
			{
				tradingCompareCharacter2.SetSelected(tradingCompareCharacter.Character);
			}
			if (m_party.SelectedCharacter != tradingCompareCharacter.Character)
			{
				m_party.SelectCharacter(tradingCompareCharacter.Character.Index);
			}
			AudioController.Play("PortraitSelect");
			m_itemContainer.ChangeInventory(tradingCompareCharacter.Character.Equipment);
			m_itemContainer.InitStartItems();
		}
	}
}
