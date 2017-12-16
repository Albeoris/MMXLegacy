using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellDeepFreeze : MonsterSpell
	{
		public MonsterSpellDeepFreeze(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.DEEP_FREEZE, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_DEEP_FREEZE_INFO", m_descriptionValues);
		}
	}
}
