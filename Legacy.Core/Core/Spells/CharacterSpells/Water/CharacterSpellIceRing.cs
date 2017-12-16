using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Water
{
	public class CharacterSpellIceRing : CharacterSpell
	{
		public CharacterSpellIceRing() : base(ECharacterSpell.SPELL_WATER_ICE_RING)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			SetDescriptionValue(0, GetDamageAsString(0, p_magicFactor));
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(1, staticData.GetBuffValues(1)[0]);
			SetDescriptionValue(2, staticData.GetDuration(1));
		}
	}
}
