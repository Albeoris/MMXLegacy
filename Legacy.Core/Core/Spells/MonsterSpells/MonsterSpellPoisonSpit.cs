using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellPoisonSpit : MonsterSpell
	{
		public MonsterSpellPoisonSpit(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.POISON_SPIT, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.SpellRanges);
			SetDescriptionValue(1, GetDamageAsString());
			return Localization.Instance.GetText("MONSTER_SPELL_POISON_SPIT_INFO", m_descriptionValues);
		}
	}
}
