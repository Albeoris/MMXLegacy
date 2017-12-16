using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellHourOfPower : CharacterSpell
	{
		public CharacterSpellHourOfPower() : base(ECharacterSpell.SPELL_PRIME_HOUR_OF_POWER)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.Might * p_magicFactor + 0.5f));
			SetDescriptionValue(1, staticData.Duration);
		}
	}
}
