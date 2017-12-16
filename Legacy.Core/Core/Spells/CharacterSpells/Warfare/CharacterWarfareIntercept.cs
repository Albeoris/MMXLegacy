using System;
using System.Collections.Generic;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareIntercept : CharacterSpell
	{
		public CharacterWarfareIntercept() : base(ECharacterSpell.WARFARE_INTERCEPT)
		{
		}

		protected override void HandlePartyCharacters(Character p_caster, SpellEventArgs p_result, List<Object> p_targets, Single p_magicFactor)
		{
			for (Int32 i = 0; i < p_targets.Count; i++)
			{
				Character character = p_targets[i] as Character;
				if (character != null)
				{
					character.FightHandler.InterceptingCharacter = p_caster;
				}
			}
		}
	}
}
