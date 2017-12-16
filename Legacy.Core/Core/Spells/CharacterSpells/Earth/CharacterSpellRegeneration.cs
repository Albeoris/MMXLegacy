using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Earth
{
	public class CharacterSpellRegeneration : CharacterSpell
	{
		public CharacterSpellRegeneration() : base(ECharacterSpell.SPELL_EARTH_REGENERATION)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(1, staticData.Duration);
			Int32 num = (Int32)(staticData.SpecificValue[0] * p_magicFactor + 0.5f);
			Int32 num2 = (Int32)(staticData.SpecificValue[0] * p_magicFactor + 0.5f);
			String p_value;
			if (num == num2)
			{
				p_value = num.ToString();
			}
			else
			{
				p_value = num + " - " + num2;
			}
			SetDescriptionValue(2, p_value);
		}
	}
}
