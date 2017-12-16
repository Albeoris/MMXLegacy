using System;

namespace Legacy.Core.Spells.CharacterSpells.Light
{
	public class CharacterSpellHeal : CharacterSpell
	{
		public CharacterSpellHeal() : base(ECharacterSpell.SPELL_LIGHT_HEAL)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
		}
	}
}
