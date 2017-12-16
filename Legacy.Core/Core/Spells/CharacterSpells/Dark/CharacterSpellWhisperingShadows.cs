using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Dark
{
	public class CharacterSpellWhisperingShadows : CharacterSpell
	{
		public CharacterSpellWhisperingShadows() : base(ECharacterSpell.SPELL_DARK_WHISPERING_SHADOWS)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
