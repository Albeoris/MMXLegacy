using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Dark
{
	public class CharacterSpellTerror : CharacterSpell
	{
		public CharacterSpellTerror() : base(ECharacterSpell.SPELL_DARK_TERROR)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, StaticData.Range);
			SetDescriptionValue(1, staticData.GetDuration(1));
		}
	}
}
