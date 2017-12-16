using System;
using Legacy.Core.Buffs;
using Legacy.Core.Entities;
using Legacy.Core.EventManagement;
using Legacy.Core.PartyManagement;

namespace Legacy.Core.Spells.MonsterSpells
{
	public class MonsterSpellCrystalShell : MonsterSpell
	{
		public MonsterSpellCrystalShell(String p_effectAnimationClip, Int32 p_castProbability) : base(EMonsterSpell.CRYSTAL_SHELL, p_effectAnimationClip, p_castProbability)
		{
		}

		public override void DoEffect(Monster p_monster, Character p_target, SpellEventArgs p_spellArgs)
		{
			p_monster.AddBuff(BuffFactory.CreateMonsterBuff(EMonsterBuffType.CRYSTAL_SHELL, p_monster.MagicPower, m_level));
			m_cooldown = 3;
		}

		public override Boolean CheckSpellPreconditions(Monster p_monster)
		{
			return m_cooldown <= 0 && p_monster.BuffHandler.HasBuff(EMonsterBuffType.CRYSTAL_SHELL);
		}
	}
}
