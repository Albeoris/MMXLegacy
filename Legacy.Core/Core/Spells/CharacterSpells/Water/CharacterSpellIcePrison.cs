using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Water
{
	public class CharacterSpellIcePrison : CharacterSpell
	{
		public CharacterSpellIcePrison() : base(ECharacterSpell.SPELL_WATER_ICE_PRISON)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, staticData.GetDuration(1));
		}
	}
}
