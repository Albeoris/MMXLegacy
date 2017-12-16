using System;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.CharacterSpells.Prime
{
	public class CharacterSpellManaSurge : CharacterSpell
	{
		public CharacterSpellManaSurge() : base(ECharacterSpell.SPELL_PRIME_MANA_SURGE)
		{
		}

		public override void FillDescriptionValues(Single p_magicFactor)
		{
			MonsterBuffStaticData staticData = StaticDataHandler.GetStaticData<MonsterBuffStaticData>(EDataType.MONSTER_BUFFS, (Int32)StaticData.MonsterBuffs[0]);
			SetDescriptionValue(0, StaticData.Range);
			Int32 num = (Int32)(staticData.GetBuffValues(1)[0] * p_magicFactor + 0.5f);
			Int32 num2 = (Int32)(staticData.GetBuffValues(1)[1] * p_magicFactor + 0.5f);
			String p_value;
			if (num == num2)
			{
				p_value = num.ToString();
			}
			else
			{
				p_value = num + " - " + num2;
			}
			SetDescriptionValue(1, p_value);
		}
	}
}
