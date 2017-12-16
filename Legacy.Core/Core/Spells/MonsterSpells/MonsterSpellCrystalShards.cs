using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellCrystalShards : MonsterSpell
	{
		public MonsterSpellCrystalShards(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.CRYSTAL_SHARDS, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
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
