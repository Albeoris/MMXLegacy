using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellFireBolt : MonsterSpell
	{
		public MonsterSpellFireBolt(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.FIREBOLT, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_FIREBOLT_INFO", m_descriptionValues);
		}
	}
}
