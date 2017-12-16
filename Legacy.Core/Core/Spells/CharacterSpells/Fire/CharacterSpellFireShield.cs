using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Fire
{
	public class CharacterSpellFireShield : CharacterSpell
	{
		public CharacterSpellFireShield() : base(ECharacterSpell.SPELL_FIRE_FIRE_SHIELD)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			Int32 num = (Int32)(staticData.SpecificValue[0] * p_magicFactor + 0.5f);
			Int32 num2 = (Int32)(staticData.SpecificValue[1] * p_magicFactor + 0.5f);
			String p_value;
			if (num == num2)
			{
				p_value = num.ToString();
			}
			else
			{
				p_value = num + " - " + num2;
			}
			SetDescriptionValue(0, p_value);
			SetDescriptionValue(1, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
