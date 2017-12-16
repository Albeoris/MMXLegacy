using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Earth
{
	public class CharacterSpellAcidSplash : CharacterSpell
	{
		public CharacterSpellAcidSplash() : base(ECharacterSpell.SPELL_EARTH_ACID_SPLASH)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(1, (Int32)(staticData.GetBuffValues(0)[0] * 100f + 0.5f));
			SetDescriptionValue(2, (Int32)(staticData.GetBuffValues(0)[1] * 100f + 0.5f));
		}
	}
}
