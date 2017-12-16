using System;

namespace Legacy.Core.Spells.CharacterSpells.Water
{
	public class CharacterSpellTsunami : CharacterSpellPushBackAble
	{
		public CharacterSpellTsunami() : base(ECharacterSpell.SPELL_WATER_TSUNAMI)
		{
			m_PushCount = 2;
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
		}
	}
}
