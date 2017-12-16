using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Fire
{
	public class CharacterSpellBurningDetermination : CharacterSpell
	{
		public CharacterSpellBurningDetermination() : base(ECharacterSpell.SPELL_FIRE_BURNING_DETERMINATION)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
