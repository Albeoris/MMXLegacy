using System;
using Legacy.Core.Internationalization;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellChainLightning : MonsterSpell
	{
		public MonsterSpellChainLightning(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.CHAIN_LIGHTNING, p_effectAnimationClip, p_castProbability)
		{
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_CHAIN_LIGHTNING_INFO", m_descriptionValues);
		}
	}
}
