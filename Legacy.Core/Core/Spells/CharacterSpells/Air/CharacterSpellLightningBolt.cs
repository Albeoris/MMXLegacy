using System;

namespace Legacy.Core.Spells.CharacterSpells.Air
{
	public class CharacterSpellLightningBolt : CharacterSpell
	{
		public CharacterSpellLightningBolt() : base(ECharacterSpell.SPELL_AIR_LIGHTNING_BOLT)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, StaticData.Range);
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
		}
	}
}
