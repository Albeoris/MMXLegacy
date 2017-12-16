using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellSunRay : MonsterSpell
	{
		public MonsterSpellSunRay(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.SUN_RAY, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_SUN_RAY_INFO", m_descriptionValues);
		}
	}
}
