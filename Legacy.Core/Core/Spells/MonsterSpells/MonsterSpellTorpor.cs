using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellTorpor : MonsterSpell
	{
		public MonsterSpellTorpor(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.TORPOR, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_TORPOR_INFO", m_descriptionValues);
		}
	}
}
