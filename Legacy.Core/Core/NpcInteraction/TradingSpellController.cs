using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.Spells;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Core.StaticData;

namespace Legacy.Core.NpcInteraction
{
	public class TradingSpellController
	{
		private List<TradingSpellOffer> m_offers;

		private List<CharacterSpell> m_spells;

		private Npc m_npc;

		private Boolean m_isTrading;

		private Int32 m_followUpDialog;

		public TradingSpellController(NpcConversationStaticData p_data, Npc p_npc)
		{
			m_offers = new List<TradingSpellOffer>();
			m_npc = p_npc;
			m_spells = new List<CharacterSpell>();
			if (p_data.m_spellOffers != null)
			{
				foreach (NpcConversationStaticData.SpellOffer spellOffer in p_data.m_spellOffers)
				{
					TradingSpellOffer item = new TradingSpellOffer(spellOffer.m_id, spellOffer.m_conditions);
					m_offers.Add(item);
				}
			}
		}

		public Boolean IsTrading => m_isTrading;

	    public List<CharacterSpell> Spells => m_spells;

	    public void StartTrade(Int32 p_followUpDialog)
		{
			m_isTrading = true;
			m_followUpDialog = p_followUpDialog;
			LegacyLogic.Instance.ConversationManager._HideNPCs();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_SPELL_TRADE_START, EventArgs.Empty);
		}

		public void UpdateSpells()
		{
			m_spells.Clear();
			foreach (TradingSpellOffer tradingSpellOffer in m_offers)
			{
				if (tradingSpellOffer.CheckConditions(m_npc) == EDialogState.NORMAL)
				{
					foreach (Int32 p_spellType in tradingSpellOffer.OfferData.SpellIDs)
					{
						CharacterSpell item = SpellFactory.CreateCharacterSpell((ECharacterSpell)p_spellType);
						m_spells.Add(item);
					}
				}
			}
		}

		public void StopTrading()
		{
			m_isTrading = false;
			LegacyLogic.Instance.ConversationManager._ShowNPCs();
			LegacyLogic.Instance.EventManager.InvokeEvent(this, EEventType.NPC_SPELL_TRADE_STOP, EventArgs.Empty);
			LegacyLogic.Instance.ConversationManager._ChangeDialog(m_npc.StaticID, m_followUpDialog);
		}
	}
}
