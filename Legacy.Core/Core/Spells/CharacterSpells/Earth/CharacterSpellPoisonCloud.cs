using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Earth
{
	public class CharacterSpellPoisonCloud : CharacterSpell
	{
		public CharacterSpellPoisonCloud() : base(ECharacterSpell.SPELL_EARTH_POISON_CLOUD)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, staticData.GetBuffValues(1)[0]);
			SetDescriptionValue(1, GetDamageAsString(0, p_magicFactor));
			SetDescriptionValue(2, (Int32)(staticData.GetBuffValues(1)[1] * p_magicFactor + 0.5f));
			SetDescriptionValue(3, (Int32)(staticData.GetBuffValues(1)[0] * p_magicFactor + 0.5f));
		}
	}
}
