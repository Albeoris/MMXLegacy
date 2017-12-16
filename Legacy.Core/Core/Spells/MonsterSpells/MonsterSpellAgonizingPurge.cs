using System;
using System.Collections.Generic;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellAgonizingPurge : MonsterSpell
	{
		public MonsterSpellAgonizingPurge(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.AGONIZING_PURGE, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			if (party.Buffs.Buffs.Count > 0)
			{
				List<PartyBuff> buffs = party.Buffs.Buffs;
				Int32 index = Random.Range(0, buffs.Count - 1);
				party.Buffs.RemoveBuff(buffs[index].Type);
			}
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_AGONIZING_PURGE_INFO", m_descriptionValues);
		}

		public override Boolean CheckSpellPreconditions(Monster p_monster)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			return party.Buffs.Buffs.Count > 0;
		}
	}
}
