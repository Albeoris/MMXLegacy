using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Air
{
	public class CharacterSpellWindShield : CharacterSpell
	{
		public CharacterSpellWindShield() : base(ECharacterSpell.SPELL_AIR_WIND_SHIELD)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)Math.Round(staticData.SpecificValue[0] * p_magicFactor, MidpointRounding.AwayFromZero));
			SetDescriptionValue(1, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
