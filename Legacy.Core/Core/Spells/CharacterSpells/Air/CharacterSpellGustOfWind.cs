using System;

namespace Legacy.Core.Spells.CharacterSpells.Air
{
	public class CharacterSpellGustOfWind : CharacterSpellPushBackAble
	{
		public CharacterSpellGustOfWind() : base(ECharacterSpell.SPELL_AIR_GUST_OF_WIND)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(1, StaticData.Range);
			SetDescriptionValue(2, GetDamageAsString(1, p_magicFactor));
		}
	}
}
