using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellPetrification : MonsterSpell
	{
		public MonsterSpellPetrification(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.PETRIFACTION, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_PETRIFACTION_INFO", m_descriptionValues);
		}
	}
}
