using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Water
{
	public class CharacterSpellLiquidMembrane : CharacterSpell
	{
		public CharacterSpellLiquidMembrane() : base(ECharacterSpell.SPELL_WATER_LIQUID_MEMBRANE)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			Int32 num = (Int32)(staticData.SpecificValue[0] * p_magicFactor * 100f + 0.5f);
			Single p_value = num;
			SetDescriptionValue(0, p_value);
			SetDescriptionValue(1, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
