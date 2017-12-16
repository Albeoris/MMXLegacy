using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellPacification : MonsterSpell
	{
		public MonsterSpellPacification(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.PACIFICATION, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_PACIFICATION_INFO", m_descriptionValues);
		}
	}
}
