using System;

namespace Legacy.Core.Spells.CharacterSpells.Fire
{
	public class CharacterSpellFireBlast : CharacterSpell
	{
		public CharacterSpellFireBlast() : base(ECharacterSpell.SPELL_FIRE_BLAST)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(1, StaticData.Range);
		}
	}
}
