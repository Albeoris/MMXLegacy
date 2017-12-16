using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareSkullCrack : CharacterSpell
	{
		public CharacterWarfareSkullCrack() : base(ECharacterSpell.WARFARE_SKULL_CRACK)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, staticData.GetDuration(1));
		}
	}
}
