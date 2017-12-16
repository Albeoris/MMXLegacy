using System;
using Legacy.Core.Api;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.Internationalization;
using Legacy.Core.PartyManagement;
using Legacy.Core.StaticData;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellIceStorm : MonsterSpell
	{
		public MonsterSpellIceStorm(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.ICE_STORM, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			Party party = LegacyLogic.Instance.WorldManager.Party;
			Character[] members = party.Members;
			for (Int32 i = 0; i < members.Length; i++)
			{
				if (members[i] != null)
				{
					if (members[i].FightHandler.CurrentGeneralBlockAttempts > 0)
					{
						members[i].FightHandler.CurrentGeneralBlockAttempts--;
					}
					else if (members[i].FightHandler.CurrentMeleeBlockAttempts > 0)
					{
						members[i].FightHandler.CurrentMeleeBlockAttempts--;
					}
				}
			}
		}

		public override String GetDescriptionForCaster(MonsterStaticData p_monster)
		{
			SetDescriptionValue(0, GetDamageAsString());
			SetDescriptionValue(1, p_monster.SpellRanges);
			return Localization.Instance.GetText("MONSTER_SPELL_ICE_STORM_INFO", m_descriptionValues);
		}
	}
}
