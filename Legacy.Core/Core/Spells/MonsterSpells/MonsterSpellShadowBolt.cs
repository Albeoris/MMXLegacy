using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellShadowBolt : MonsterSpell
	{
		public MonsterSpellShadowBolt(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.SHADOW_BOLT, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_SHADOW_BOLT_INFO", m_descriptionValues);
		}
	}
}
