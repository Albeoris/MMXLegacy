using System;

namespace Legacy.Core.Spells.CharacterSpells.Fire
{
	public class CharacterSpellFireBall : CharacterSpell
	{
		public CharacterSpellFireBall() : base(ECharacterSpell.SPELL_FIRE_FIREBALL)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, StaticData.Range);
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
		}
	}
}
