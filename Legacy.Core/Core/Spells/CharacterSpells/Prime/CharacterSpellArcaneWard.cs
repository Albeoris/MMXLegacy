using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellArcaneWard : CharacterSpell
	{
		public CharacterSpellArcaneWard() : base(ECharacterSpell.SPELL_PRIME_ARCANE_WARD)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.ResistanceAir * p_magicFactor + 0.5f));
			SetDescriptionValue(1, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
