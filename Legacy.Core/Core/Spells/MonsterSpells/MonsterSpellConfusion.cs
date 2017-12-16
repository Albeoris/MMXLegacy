using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellConfusion : MonsterSpell
	{
		public MonsterSpellConfusion(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.CONFUSION, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_CONFUSION_INFO", m_descriptionValues);
		}
	}
}
