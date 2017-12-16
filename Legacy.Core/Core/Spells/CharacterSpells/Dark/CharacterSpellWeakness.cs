using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Dark
{
	public class CharacterSpellWeakness : CharacterSpell
	{
		public CharacterSpellWeakness() : base(ECharacterSpell.SPELL_DARK_WEAKNESS)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, ((Int32)(staticData.GetBuffValues(1)[0] * 100f)).ToString() + "%");
			SetDescriptionValue(1, (Int32)(staticData.GetBuffValues(1)[1] * p_magicFactor + 0.5f));
			SetDescriptionValue(2, staticData.GetDuration(1));
		}
	}
}
