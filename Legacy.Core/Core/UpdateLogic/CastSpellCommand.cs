using System;
using Legacy.Core.Entities.Items;
using Legacy.Core.PartyManagement;
using Legacy.Core.Spells.CharacterSpells;

namespace Legacy.Core.UpdateLogic
{
	public class CastSpellCommand : Command
	{
		private CharacterSpell m_spell;

		private Scroll m_scroll;

		private Character m_targetCharacter;

		public CastSpellCommand(CharacterSpell p_spell, Character p_targetCharacter, Scroll p_scroll) : base(ECommandTypes.CAST_SPELL)
		{
			m_spell = p_spell;
			m_targetCharacter = p_targetCharacter;
			m_scroll = p_scroll;
		}

		public CharacterSpell Spell => m_spell;

	    public Character TargetCharacter => m_targetCharacter;

	    public Scroll Scroll => m_scroll;
	}
}
