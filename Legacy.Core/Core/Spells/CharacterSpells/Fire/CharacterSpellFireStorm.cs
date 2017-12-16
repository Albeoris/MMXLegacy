using System;

namespace Legacy.Core.Spells.CharacterSpells.Fire
{
	public class CharacterSpellFireStorm : CharacterSpell
	{
		public CharacterSpellFireStorm() : base(ECharacterSpell.SPELL_FIRE_STORM)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
		}
	}
}
