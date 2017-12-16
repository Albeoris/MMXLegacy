using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.EventManagement
{
	public class OpenSpellCharacterSelectionEventArgs : EventArgs
	{
		private Boolean m_isScroll;

		private CharacterSpell m_spell;

		private Scroll m_scroll;

		public OpenSpellCharacterSelectionEventArgs(CharacterSpell p_spell)
		{
			m_isScroll = false;
			m_spell = p_spell;
		}

		public OpenSpellCharacterSelectionEventArgs(CharacterSpell p_spell, Scroll p_scroll)
		{
			m_isScroll = true;
			m_scroll = p_scroll;
			m_spell = p_spell;
		}

		public Scroll Scroll => m_scroll;

	    public CharacterSpell Spell => m_spell;

	    public Boolean IsScroll => m_isScroll;
	}
}
