using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellHeroicDestiny : CharacterSpell
	{
		public CharacterSpellHeroicDestiny() : base(ECharacterSpell.SPELL_PRIME_HEROIC_DESTINY)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.Destiny * p_magicFactor + 0.5f));
			SetDescriptionValue(1, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
