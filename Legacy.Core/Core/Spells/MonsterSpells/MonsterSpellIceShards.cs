using System;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellIceShards : MonsterSpell
	{
		public MonsterSpellIceShards(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.ICE_SHARDS, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			if (p_target.FightHandler.CurrentGeneralBlockAttempts > 0)
			{
				p_target.FightHandler.CurrentGeneralBlockAttempts--;
			}
			else if (p_target.FightHandler.CurrentMeleeBlockAttempts > 0)
			{
				p_target.FightHandler.CurrentMeleeBlockAttempts--;
			}
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_ICE_SHARDS_INFO", m_descriptionValues);
		}
	}
}
