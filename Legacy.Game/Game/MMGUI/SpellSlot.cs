using System;
using Legacy.Core.Api;
using Legacy.Core.Spells.CharacterSpells;
using Legacy.Game.MMGUI.Tooltip;
using UnityEngine;

namespace Legacy.Game.MMGUI
{
	[AddComponentMenu("MM Legacy/MMGUI/SpellSlot")]
	public class SpellSlot : MonoBehaviour
	{
		[SerializeField]
		protected UISprite m_spellTexture;

		[SerializeField]
		protected UILabel m_spellName;

		[SerializeField]
		protected UILabel m_spellCost;

		[SerializeField]
		protected UISprite m_hoverTexture;

		[SerializeField]
		protected UISprite m_selectTexture;

		[SerializeField]
		private Color m_insufficientGold = new Color(1f, 0f, 0f, 1f);

		[SerializeField]
		private Single m_selectedAlpha = 0.5f;

		[SerializeField]
		private Single m_hoverAlpha = 0.25f;

		[SerializeField]
		private Single m_selectedHoverAlpha = 0.6f;

		[SerializeField]
		private Single m_alphaTweenDuration = 0.05f;

		[SerializeField]
		private Color m_notLearnableColor = new Color(0.25f, 0.25f, 0.25f);

		protected CharacterSpell m_spell;

		protected Int32 m_index;

		protected SpellTradingScreen m_parent;

		protected Boolean m_selected;

		protected Color m_originColorCost;

		protected Boolean m_hovered;

		protected Boolean m_canLearn;

		public SpellTradingScreen Parent
		{
			get => m_parent;
		    set => m_parent = value;
		}

		public UISprite SpellTexture => m_spellTexture;

	    public CharacterSpell Spell => m_spell;

	    public Int32 Index
		{
			get => m_index;
	        set => m_index = value;
	    }

		public Boolean CanLearn => m_canLearn;

	    private void Awake()
		{
			m_originColorCost = m_spellCost.color;
			NGUITools.SetActiveSelf(m_hoverTexture.gameObject, false);
			SetSelected(false);
		}

		private void OnDisable()
		{
			NGUITools.SetActiveSelf(m_hoverTexture.gameObject, false);
			TooltipManager.Instance.Hide(this);
		}

		public virtual void SetSpell(CharacterSpell p_spell, Boolean p_canLearn)
		{
			m_spell = p_spell;
			m_canLearn = p_canLearn;
			if (m_spellTexture != null && m_spell != null)
			{
				m_spellName.text = LocaManager.GetText(m_spell.NameKey);
				m_spellName.color = ((!p_canLearn) ? m_notLearnableColor : Color.white);
				m_spellCost.text = m_spell.GetCalculatedCosts().ToString();
				m_spellTexture.spriteName = m_spell.StaticData.Icon;
				m_spellTexture.color = ((!p_canLearn) ? new Color(0.5f, 0.5f, 0.5f, 0f) : new Color(0.5f, 0.5f, 0.5f, 0.5f));
				UpdateSpellCostColor();
			}
		}

		public void UpdateSpellCostColor()
		{
			if (!m_canLearn)
			{
				m_spellCost.color = m_notLearnableColor;
			}
			else if (m_spell.GetCalculatedCosts() <= LegacyLogic.Instance.WorldManager.Party.Gold)
			{
				m_spellCost.color = m_originColorCost;
			}
			else
			{
				m_spellCost.color = m_insufficientGold;
			}
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

		private void OnTooltip(Boolean show)
		{
			if (show && m_spell != null)
			{
				TooltipManager.Instance.Show(this, m_spell, gameObject.transform.position, m_selectTexture.gameObject.transform.localScale * 0.5f);
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
				if (p_isDown && !m_selected)
				{
					m_parent.SelectSpellSlot(this);
				}
			}
			else if (UICamera.currentTouchID == -2 && !p_isDown)
			{
				if (!m_selected)
				{
					m_parent.SelectSpellSlot(this);
				}
				if (m_canLearn)
				{
					m_parent.BuySpell();
				}
			}
		}
	}
}
