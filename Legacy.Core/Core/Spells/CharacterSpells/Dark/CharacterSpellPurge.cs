using System;
using System.Collections.Generic;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Dark
{
	public class CharacterSpellPurge : CharacterSpell
	{
		public CharacterSpellPurge() : base(ECharacterSpell.SPELL_DARK_PURGE)
		{
		}

		protected override void HandleMonsters(Character p_attacker, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			if (p_targets.Count > 0)
			{
				((Monster)p_targets[0]).BuffHandler.RemoveAllBuffs();
				p_result.SpellTargets.Add(new SpellTarget((Monster)p_targets[0]));
			}
		}
	}
}
