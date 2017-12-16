using System;

namespace Legacy.Core.Spells.CharacterSpells.Air
{
	public class CharacterSpellSparks : CharacterSpell
	{
		public CharacterSpellSparks() : base(ECharacterSpell.SPELL_AIR_SPARKS)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
		}
	}
}
