using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellSundering : CharacterSpell
	{
		public CharacterSpellSundering() : base(ECharacterSpell.SPELL_PRIME_SUNDERING)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, StaticData.Range);
			SetDescriptionValue(1, (Int32)(staticData.GetBuffValues(1)[0] * p_magicFactor + 0.5f));
		}
	}
}
