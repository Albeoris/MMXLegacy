using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Light
{
	public class CharacterSpellClairvoyance : CharacterSpell
	{
		public CharacterSpellClairvoyance() : base(ECharacterSpell.SPELL_LIGHT_CLAIRVOYANCE)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
