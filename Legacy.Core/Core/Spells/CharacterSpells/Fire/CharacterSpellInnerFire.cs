using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Fire
{
	public class CharacterSpellInnerFire : CharacterSpell
	{
		public CharacterSpellInnerFire() : base(ECharacterSpell.SPELL_FIRE_INNER_FIRE)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			PartyBuffStaticData staticData = StaticDataHandler.GetStaticData<PartyBuffStaticData>(EDataType.PARTY_BUFFS, (Int32)StaticData.PartyBuff);
			SetDescriptionValue(0, (Int32)(staticData.Might * p_magicFactor + 0.5f));
			SetDescriptionValue(1, (Int32)(staticData.Duration * p_magicFactor + 0.5f));
		}
	}
}
