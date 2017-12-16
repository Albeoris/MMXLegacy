using System;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellFlickeringLight : MonsterSpell
	{
		public MonsterSpellFlickeringLight(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.FLICKERING_LIGHT, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			if (p_monster != null && !p_monster.BuffHandler.HasBuff(EMonsterBuffType.FLICKERING_LIGHT))
			{
				p_monster.AddBuff(BuffFactory.CreateMonsterBuff(EMonsterBuffType.FLICKERING_LIGHT, p_monster.MagicPower, m_level));
				m_cooldown = 3;
			}
		}

		public override Boolean CheckSpellPreconditions(Monster p_monster)
		{
			return m_cooldown <= 0 && !p_monster.BuffHandler.HasBuff(EMonsterBuffType.FLICKERING_LIGHT);
		}
	}
}
