using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.EventManagement;
using Legacy.Core.NpcInteraction;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;
using UnityEngine;
using Object = System.Object;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SpellTradingScreen")]
	public class SpellTradingScreen : MonoBehaviour, IScrollingListener
	{
		public const Int32 SPELL_VISIBLE_AMOUNT = 8;

		[SerializeField]
		private UIScrollBar m_scrollBar;

		[SerializeField]
		private Single m_scrollDuration = 0.1f;

		[SerializeField]
		private UIGrid m_grid;

		[SerializeField]
		private GameObject m_spellSlotPrefab;

		[SerializeField]
		private UIButton m_buyButton;

		[SerializeField]
		private UILabel m_buyButtonText;

		protected List<SpellSlot> m_spellSlots;

		private Vector3 m_itemGridOrigin;

		private TradingSpellController m_tradingSpellController;

		private SpellSlot m_selectedSpell;

		private Int32 m_spellCounter;

		public void Init()
		{
			m_itemGridOrigin = m_grid.transform.localPosition;
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Combine(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
			m_spellSlots = new List<SpellSlot>();
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_SPELL_TRADE_START, new EventHandler(OnSpellTradingStart));
			LegacyLogic.Instance.EventManager.RegisterEvent(EEventType.NPC_SPELL_TRADE_STOP, new EventHandler(OnSpellTradingEnd));
		}

		public void CleanUp()
		{
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_SPELL_TRADE_START, new EventHandler(OnSpellTradingStart));
			LegacyLogic.Instance.EventManager.UnregisterEvent(EEventType.NPC_SPELL_TRADE_STOP, new EventHandler(OnSpellTradingEnd));
		}

		private void Start()
		{
			ScrollingHelper.InitScrollListeners(this, m_scrollBar.gameObject);
		}

		private void OnDestroy()
		{
			UIScrollBar scrollBar = m_scrollBar;
			scrollBar.onChange = (UIScrollBar.OnScrollBarChange)Delegate.Remove(scrollBar.onChange, new UIScrollBar.OnScrollBarChange(OnScrollBarChange));
		}

		private void OnSpellTradingStart(Object p_sender, EventArgs p_args)
		{
			m_tradingSpellController = (TradingSpellController)p_sender;
			NGUITools.SetActiveSelf(gameObject, true);
			UpdateSpells();
			SelectSpellSlot(null);
			SetScrollBarPosition(0);
		}

		private void OnSpellTradingEnd(Object p_sender, EventArgs p_args)
		{
			NGUITools.SetActiveSelf(gameObject, false);
			m_tradingSpellController = null;
		}

		private void Close()
		{
			m_tradingSpellController.StopTrading();
		}

		private void UpdateSpells()
		{
			Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
			m_spellCounter = 0;
			foreach (CharacterSpell characterSpell in m_tradingSpellController.Spells)
			{
				if (selectedCharacter.SpellHandler.CouldLearnSpell(characterSpell))
				{
					Boolean flag = selectedCharacter.SpellHandler.CanLearnSpell(characterSpell);
					if (flag)
					{
						AddSpell(characterSpell, true);
					}
				}
			}
			foreach (CharacterSpell characterSpell2 in m_tradingSpellController.Spells)
			{
				if (selectedCharacter.SpellHandler.CouldLearnSpell(characterSpell2) && !selectedCharacter.SpellHandler.CanLearnSpell(characterSpell2))
				{
					AddSpell(characterSpell2, false);
				}
			}
			for (Int32 i = m_spellCounter; i < m_spellSlots.Count; i++)
			{
				m_spellSlots[i].SetSpell(null, false);
				NGUITools.SetActiveSelf(m_spellSlots[i].gameObject, false);
			}
			m_grid.Reposition();
		}

		private void AddSpell(CharacterSpell spell, Boolean p_canLearn)
		{
			if (m_spellSlots.Count <= m_spellCounter)
			{
				AddSpellSlot();
			}
			m_spellSlots[m_spellCounter].SetSpell(spell, p_canLearn);
			NGUITools.SetActiveSelf(m_spellSlots[m_spellCounter].gameObject, true);
			m_spellCounter++;
		}

		private void AddSpellSlot()
		{
			GameObject gameObject = NGUITools.AddChild(m_grid.gameObject, m_spellSlotPrefab);
			SpellSlot component = gameObject.GetComponent<SpellSlot>();
			m_spellSlots.Add(component);
			component.Index = m_spellSlots.Count - 1;
			component.Parent = this;
			ScrollingHelper.InitScrollListeners(this, gameObject);
		}

		private void SetScrollBarPosition(Int32 p_position)
		{
			if (m_spellCounter > 8)
			{
				NGUITools.SetActive(m_scrollBar.gameObject, true);
				Int32 spellCounter = m_spellCounter;
				Int32 num = spellCounter - 8;
				if (p_position < 0)
				{
					p_position = 0;
				}
				else if (p_position > num)
				{
					p_position = num;
				}
				m_scrollBar.barSize = 8f / spellCounter;
				if (num > 0)
				{
					m_scrollBar.scrollValue = p_position / (Single)num;
				}
				else
				{
					m_scrollBar.scrollValue = 0f;
				}
				TweenPosition.Begin(m_grid.gameObject, m_scrollDuration, m_itemGridOrigin + new Vector3(0f, p_position * m_grid.cellHeight, 0f));
			}
			else
			{
				NGUITools.SetActive(m_scrollBar.gameObject, false);
				TweenPosition.Begin(m_grid.gameObject, m_scrollDuration, m_itemGridOrigin);
			}
		}

		public void SelectSpellSlot(SpellSlot p_slot)
		{
			if (m_selectedSpell != null)
			{
				m_selectedSpell.SetSelected(false);
			}
			if (p_slot != null)
			{
				p_slot.SetSelected(true);
				m_selectedSpell = p_slot;
				Boolean flag = p_slot.Spell.GetCalculatedCosts() <= LegacyLogic.Instance.WorldManager.Party.Gold;
				Boolean flag2 = flag && p_slot.CanLearn;
				m_buyButton.isEnabled = flag2;
				m_buyButtonText.color = ((!flag2) ? Color.gray : Color.white);
			}
			else
			{
				m_selectedSpell = null;
				m_buyButton.isEnabled = false;
				m_buyButtonText.color = Color.gray;
			}
		}

		public void BuySpell()
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			CharacterSpell spell = m_selectedSpell.Spell;
			if (party.Gold >= spell.GetCalculatedCosts())
			{
				Character selectedCharacter = LegacyLogic.Instance.WorldManager.Party.SelectedCharacter;
				selectedCharacter.SpellHandler.AddSpell((ECharacterSpell)spell.StaticID);
				party.ChangeGold(-spell.GetCalculatedCosts());
				UpdateSpells();
				SelectSpellSlot(null);
				UpdateScrollBar();
			}
		}

		private void OnScrollBarChange(UIScrollBar p_sb)
		{
			Single num = m_spellCounter;
			Single num2 = num - 8f;
			if (num2 > 0f)
			{
				SetScrollBarPosition(Mathf.RoundToInt(p_sb.scrollValue * num2));
			}
			else
			{
				SetScrollBarPosition(0);
			}
		}

		private void UpdateScrollBar()
		{
			Single num = m_spellCounter - 8 + 1;
			SetScrollBarPosition((Int32)(num * m_scrollBar.scrollValue));
		}

		public void OnScroll(Single p_delta)
		{
			Single num = m_spellCounter;
			Single num2 = num - 8f;
			if (num2 > 0f)
			{
				Single num3 = -1f / num2 * p_delta * 10f;
				SetScrollBarPosition(Mathf.RoundToInt((m_scrollBar.scrollValue + num3) * num2));
			}
			else
			{
				SetScrollBarPosition(0);
			}
		}
	}
}
