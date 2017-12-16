using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Warfare
{
	public class CharacterWarfareChallenge : CharacterSpell
	{
		public CharacterWarfareChallenge() : base(ECharacterSpell.WARFARE_CHALLENGE)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, staticData.GetDuration(1));
		}
	}
}
