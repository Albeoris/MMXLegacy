using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Water
{
	public class CharacterSpellIceBolt : CharacterSpell
	{
		public CharacterSpellIceBolt() : base(ECharacterSpell.SPELL_WATER_ICE_BOLT)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, StaticData.Range);
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(2, staticData.GetBuffValues(1)[0]);
			SetDescriptionValue(3, staticData.GetDuration(1));
		}
	}
}
