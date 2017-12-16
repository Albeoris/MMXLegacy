using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Air
{
	public class CharacterSpellEagleEye : CharacterSpell
	{
		public CharacterSpellEagleEye() : base(ECharacterSpell.SPELL_AIR_EAGLE_EYE)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.Perception * p_magicFactor + 0.5f));
			SetDescriptionValue(1, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
