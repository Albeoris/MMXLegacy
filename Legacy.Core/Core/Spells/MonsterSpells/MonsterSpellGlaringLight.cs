using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellGlaringLight : MonsterSpell
	{
		public MonsterSpellGlaringLight(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.GLARING_LIGHT, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_GLARING_LIGHT_INFO", m_descriptionValues);
		}
	}
}
